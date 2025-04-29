using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using PyroCommon.Types;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Types.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Open Carry", CalloutProbability.Low)]
internal class OpenCarry : SuperCallout
{
    private Ped _suspect;
    private Blip _blip;
    private string _suspectName;
    private bool _blipHelper;
    private UIMenuItem _speakSuspect;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 20;
    internal override string CalloutName { get; set; } = "Open Carry";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a person with a firearm.";
        CalloutAdvisory = "Caller reports the person is walking around with a firearm out but has not caused any trouble.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE PYRO_OPEN_CARRY IN_OR_ON_POSITION",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Person With Gun",
            "Reports of a person walking around with an assault rifle. Respond ~y~CODE-2"
        );

        SpawnSuspect();
        CreateBlip();
        CreateConversationOptions();
    }

    private void SpawnSuspect()
    {
        _suspect = PyroFunctions.SpawnPed(SpawnPoint);
        _suspectName = Functions.GetPersonaForPed(_suspect).FullName;
        _suspect.SetDrunk(Enums.DrunkState.ModeratelyDrunk);
        _suspect.SetLicenseStatus(Enums.Permits.Guns, Enums.PermitStatus.None);
        PyroFunctions.AddFirearmItem("~r~Assault Rifle", "weapon_assaultrifle", true, false, true, _suspect);
        PyroFunctions.AddWeaponItem("~r~Knife", "weapon_knife", _suspect);
        _suspect.Tasks.Wander();
        EntitiesToClear.Add(_suspect);
    }

    private void CreateBlip()
    {
        _blip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Red, true, true, 50f);
        BlipsToClear.Add(_blip);
    }

    private void CreateConversationOptions()
    {
        _speakSuspect = new UIMenuItem($"Speak with ~y~{_suspectName}");
        ConvoMenu.AddItem(_speakSuspect);
    }

    internal override void CalloutRunning()
    {
        if (!_suspect)
        {
            CalloutEnd(true);
            return;
        }

        UpdateSuspectStatus();
        UpdateBlipIfNeeded();
    }

    private void UpdateSuspectStatus()
    {
        if (_suspect.IsDead)
        {
            _speakSuspect.Enabled = false;
            _speakSuspect.RightLabel = "~r~Dead";
        }
    }

    private void UpdateBlipIfNeeded()
    {
        if (!OnScene && !_blipHelper)
        {
            GameFiber.StartNew(
                delegate
                {
                    _blipHelper = true;
                    SpawnPoint = new Location(_suspect.Position);
                    if (_blip)
                    {
                        _blip.DisableRoute();
                        _blip.Position = SpawnPoint.Position.Around2D(25, 45);
                        _blip.EnableRoute(Color.Red);
                    }
                    GameFiber.Sleep(5000);
                    _blipHelper = false;
                }
            );
        }
    }

    internal override void CalloutOnScene()
    {
        UpdateBlip();

        if (!_suspect)
        {
            CalloutEnd(true);
            return;
        }

        InitiateEncounter();
        HandleScenario();
    }

    private void UpdateBlip()
    {
        if (_blip)
        {
            _blip.Position = SpawnPoint.Position;
            _blip.Scale = 20;
            _blip.DisableRoute();
        }
    }

    private void InitiateEncounter()
    {
        Game.DisplaySubtitle("~g~You~s~: Hey, stop for a second.");
        _suspect.Tasks.ClearImmediately();
        _suspect.Tasks.FaceEntity(Player);
        GameFiber.Wait(3000);
    }

    private void HandleScenario()
    {
        switch (new Random(DateTime.Now.Millisecond).Next(1, 6))
        {
            case 1: // Fleeing suspect
                Log.Info("Callout Scene 1");
                Questioning.Enabled = true;
                Game.DisplaySubtitle("~r~Suspect: ~s~I know my rights, leave me alone!", 5000);
                _suspect.SetResistance(Enums.ResistanceAction.Flee);
                PyroFunctions.StartPursuit(false, false, _suspect);
                break;

            case 2: // Uncooperative suspect
                Log.Info("Callout Scene 2");
                Questioning.Enabled = true;
                Game.DisplayNotification("Investigate the person.");
                _suspect.SetResistance(Enums.ResistanceAction.Uncooperative, true);
                _suspect.Inventory.Weapons.Clear();
                break;

            case 3: // Aggressive suspect
                Log.Info("Callout Scene 3");
                Game.DisplaySubtitle("~r~Suspect: ~s~REEEEEE", 5000);
                _suspect.SetResistance(Enums.ResistanceAction.Attack);
                _suspect.Tasks.AimWeaponAt(Player, -1);
                break;

            case 4: // Compliant suspect with valid license
                Log.Info("Callout Scene 4");
                Questioning.Enabled = true;
                Game.DisplayNotification("Investigate the person.");
                _suspect.Tasks.ClearImmediately();
                _suspect.Inventory.Weapons.Clear();
                _suspect.SetResistance(Enums.ResistanceAction.None);
                _suspect.SetLicenseStatus(Enums.Permits.Guns, Enums.PermitStatus.Valid);
                break;

            case 5: // Attacking suspect
                Log.Info("Callout Scene 5");
                _suspect.SetResistance(Enums.ResistanceAction.Attack, false, 100);
                _suspect.Tasks.FireWeaponAt(Player, -1, FiringPattern.FullAutomatic);
                break;
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (!_suspect)
        {
            CalloutEnd(true);
            return;
        }

        if (selItem == _speakSuspect)
        {
            GameFiber.StartNew(
                delegate
                {
                    _speakSuspect.Enabled = false;
                    Game.DisplaySubtitle(
                        "~g~You~s~: I'm with the police. What is the reason for carrying your weapon out?",
                        5000
                    );
                    _suspect.Tasks.FaceEntity(Player);
                    GameFiber.Wait(5000);
                    _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    Game.DisplaySubtitle(
                        $"~r~{_suspectName}~s~: It's my right officer. Nobody can tell me I can't have my gun.''",
                        5000
                    );
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~g~You~s~: Alright, I understand your rights to open carry, but carrying a rifle in this way does cause concern to people. It's also illegal to be loaded while you carry.",
                        5000
                    );
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: I don't see why not!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~g~You~s~: It's the law, as well as it scares people to see someone walking around with a rifle in their hands. There's no reason to. Do you have a license for it?",
                        5000
                    );
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: Check for yourself.", 5000);
                }
            );
        }
        base.Conversations(sender, selItem, index);
    }
}
