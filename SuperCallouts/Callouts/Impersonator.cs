using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Types.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Police Impersonator", CalloutProbability.Medium)]
internal class Impersonator : SuperCallout
{
    private Ped _bad;
    private Blip _cBlip;
    private Vehicle _cVehicle1;
    private Vehicle _cVehicle2;
    private string _name1;
    private UIMenuItem _speakSuspect;
    private Ped _victim;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Impersonator";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Officer impersonator.";
        CalloutAdvisory = "Caller says they have been stopped by someone that does not look like an officer.";
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Suspicious Pullover",
            Settings.EmergencyNumber
                + " call of someone being pulled over by a non uniformed officer. Description does not match our department for undercover cops. Respond ~r~CODE-3"
        );

        PyroFunctions.SpawnNormalCar(out _cVehicle1, SpawnPoint.Position);
        _cVehicle1.Heading = SpawnPoint.Heading;
        EntitiesToClear.Add(_cVehicle1);

        _cVehicle2 = new Vehicle("DILETTANTE2", _cVehicle1.GetOffsetPositionFront(-9f))
        {
            Heading = SpawnPoint.Heading,
            IsPersistent = true,
        };
        _cVehicle2.Metadata.searchDriver = "~y~police radio scanner~s~, ~y~handcuffs~s~, ~g~parking ticket~s~, ~g~cigarettes~s~";
        EntitiesToClear.Add(_cVehicle2);

        _bad = _cVehicle2.CreateRandomDriver();
        _bad.IsPersistent = true;
        _bad.Inventory.Weapons.Add(WeaponHash.Pistol);
        _bad.Metadata.searchPed = "~r~kids plastic police badge~s~, ~r~loaded pistol~s~, ~g~wallet~s~";
        _name1 = Functions.GetPersonaForPed(_bad).FullName;
        EntitiesToClear.Add(_bad);

        _victim = _cVehicle1.CreateRandomDriver();
        _victim.IsPersistent = true;
        EntitiesToClear.Add(_victim);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        ConvoMenu.AddItem(_speakSuspect);

        _cBlip = _bad.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        if (!_victim || !_bad)
        {
            CalloutEnd(true);
            return;
        }

        _cBlip?.DisableRoute();
        _victim.Tasks.CruiseWithVehicle(10f, VehicleDrivingFlags.Normal);
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Suspicious Pullover",
            "Be advised, caller has been instructed to leave scene by the dispatcher."
        );
        Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
        var rNd = new Random(DateTime.Now.Millisecond);
        var choices = rNd.Next(1, 4);
        switch (choices)
        {
            case 1:
                Game.DisplayHelp("Suspect is fleeing!");
                var pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, _bad);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;
            case 2:
                _bad.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(1500);
                _bad.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
                GameFiber.Wait(3000);
                _bad.Tasks.FightAgainst(Game.LocalPlayer.Character, -1);
                break;
            case 3:
                GameFiber.Wait(2000);
                Questioning.Enabled = true;
                break;
            default:
                CalloutEnd(true);
                break;
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
            GameFiber.StartNew(
                delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: What's going on? Why did you have that person stopped?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: I'm off duty, that person was driving really dangerously.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~g~You~s~: Alright, even if you are off duty you can't be doing that. What department do you work with?",
                        5000
                    );
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name1 + "~s~: I'm with a secret department in Los Santos. I can't disclose it to you.",
                        5000
                    );
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~g~You~s~: If that's the case you may want to call your supervisor. Do you have any identification or a badge?",
                        5000
                    );
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name1 + "~s~: I'll have you fired for this officer. I'm not going to talk to you anymore.",
                        5000
                    );
                }
            );
        base.Conversations(sender, selItem, index);
    }
}
