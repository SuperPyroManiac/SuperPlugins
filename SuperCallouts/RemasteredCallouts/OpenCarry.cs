using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInterface("[SC] Open Carry", CalloutProbability.Low,
    "Person walking around with an assault rifle", "Code 2")]
internal class OpenCarry : SuperCallout
{
    private Ped _suspect;
    private Blip _cBlip;
    private string _name1;
    private bool _blipHelper;
    private UIMenuItem _speakSuspect;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 20;
    internal override string CalloutName { get; set; } = "Open Carry";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a person with a firearm.";
        CalloutAdvisory = "Caller reports the person is walking around with a firearm out but has not caused any trouble.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch","~r~Person With Gun","Reports of a person walking around with an assault rifle. Respond ~y~CODE-2");
        _suspect = PyroFunctions.SpawnPed(SpawnPoint);
        _name1 = Functions.GetPersonaForPed(_suspect).FullName;
        _suspect.SetDrunk(Enums.DrunkState.ModeratelyDrunk);
        _suspect.SetLicenseStatus(Enums.Permits.Guns, Enums.PermitStatus.None);
        PyroFunctions.AddFirearmItem("~r~Assault Rifle", "weapon_assaultrifle", true, false, true, _suspect);
        PyroFunctions.AddWeaponItem("~r~Knife", "weapon_knife", _suspect);
        _suspect.Tasks.Wander();
        EntitiesToClear.Add(_suspect);

        _cBlip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Red, true, true, 50f);
        BlipsToClear.Add(_cBlip);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        ConvoMenu.AddItem(_speakSuspect);
        Questioning.Enabled = true;
        _speakSuspect.Enabled = false;
    }

    internal override void CalloutRunning()
    {
        if ( _suspect.IsDead )
        {
            _speakSuspect.Enabled = false;
            _speakSuspect.RightLabel = "~r~Dead";
        }

        if ( !OnScene && !_blipHelper )
        {
            GameFiber.StartNew(delegate
            {
                _blipHelper = true;
                SpawnPoint = new Location(_suspect.Position);
                _cBlip.Position = SpawnPoint.Position.Around2D(25, 45);
                GameFiber.Sleep(5000);
                _blipHelper = false;
            });
        }
    }

    internal override void CalloutOnScene()
    {
        _cBlip.Position = SpawnPoint.Position;
        _cBlip.Scale = 20;
        _cBlip.DisableRoute();
        Game.DisplaySubtitle("~g~You~s~: Hey, stop for a second.");
        _suspect.Tasks.ClearImmediately();
        _suspect.Tasks.FaceEntity(Player);
        GameFiber.Wait(3000);
        switch ( new Random(DateTime.Now.Millisecond).Next(1, 6) )
        {
            case 1:
                Log.Info("Callout Scene 1");
                _speakSuspect.Enabled = true;
                Game.DisplaySubtitle("~r~Suspect: ~s~I know my rights, leave me alone!", 5000);
                _suspect.SetResistance(Enums.ResistanceAction.Flee);
                PyroFunctions.StartPursuit(false, false, _suspect);
                break;
            case 2:
                Log.Info("Callout Scene 2");
                _speakSuspect.Enabled = true;
                Game.DisplayNotification("Investigate the person.");
                _suspect.SetResistance(Enums.ResistanceAction.Uncooperative, true);
                _suspect.Inventory.Weapons.Clear();
                break;
            case 3:
                Log.Info("Callout Scene 3");
                Game.DisplaySubtitle("~r~Suspect: ~s~REEEEEE", 5000);
                _suspect.SetResistance(Enums.ResistanceAction.Attack);
                _suspect.Tasks.AimWeaponAt(Player, -1);
                break;
            case 4:
                Log.Info("Callout Scene 4");
                _speakSuspect.Enabled = true;
                Game.DisplayNotification("Investigate the person.");
                _suspect.Tasks.ClearImmediately();
                _suspect.Inventory.Weapons.Clear();
                _suspect.SetResistance(Enums.ResistanceAction.None);
                _suspect.SetLicenseStatus(Enums.Permits.Guns, Enums.PermitStatus.Valid);
                break;
            case 5:
                Log.Info("Callout Scene 5");
                _suspect.SetResistance(Enums.ResistanceAction.Attack, false, 100);
                _suspect.Tasks.FireWeaponAt(Player, -1, FiringPattern.FullAutomatic);
                break;
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if ( selItem == _speakSuspect )
            GameFiber.StartNew(delegate
            {
                _speakSuspect.Enabled = false;
                Game.DisplaySubtitle(
                    "~g~You~s~: I'm with the police. What is the reason for carrying your weapon out?",
                    5000);
                _suspect.Tasks.FaceEntity(Player);
                GameFiber.Wait(5000);
                _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                Game.DisplaySubtitle(
                    "~r~" + _name1 +
                    "~s~: It's my right officer. Nobody can tell me I can't have my gun.''", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~g~You~s~: Alright, I understand your rights to open carry, but carrying a rifle in this way does cause concern to people. It's also illegal to be loaded while you carry.",
                    5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle("~r~" + _name1 + "~s~: I don't see why not!", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~g~You~s~: It's the law, as well as it scares people to see someone walking around with a rifle in their hands. There's no reason to. Do you have a license for it?",
                    5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle("~r~" + _name1 + "~s~: Check for yourself.", 5000);
            });
        base.Conversations(sender, selItem, index);
    }
}