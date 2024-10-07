using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Suspicious Vehicle", CalloutProbability.Medium)]
internal class WeirdCar : SuperCallout
{
    private readonly Random _rNd = new();
    private Ped _bad1;
    private Blip _cBlip;
    private Vehicle _cVehicle1;
    private string _name;
    private UIMenuItem _speakSuspect;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 40;
    internal override string CalloutName { get; set; } = "Suspicious Vehicle";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Suspicious vehicle.";
        CalloutAdvisory = "Suspicious vehicle was found on the side of the road. Approach with caution.";
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Suspicious Vehicle",
            "Report of a suspicious vehicle on the side of the road. Respond ~y~CODE-2");

        _cVehicle1 = PyroFunctions.SpawnCar(SpawnPoint);
        EntitiesToClear.Add(_cVehicle1);

        _cBlip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Yellow, true, true, 40f);
        BlipsToClear.Add(_cBlip);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name);
        ConvoMenu.AddItem(_speakSuspect);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.Position = SpawnPoint.Position;
        _cBlip.Scale = 20;
        _cBlip.DisableRoute();
        Game.DisplayNotification("Investigate the vehicle.");
        var choices = _rNd.Next(1, 4);
        Log.Info("Suspicious vehicle scene: " + choices);
        switch (choices)
        {
            case 1:
                Log.Info("Callout Scene 1");
                _cVehicle1.ApplyDamage(500, 500);
                _cVehicle1.IsStolen = true;
                break;
            case 2:
                Log.Info("Callout Scene 2");
                GameFiber.StartNew(delegate
                {
                    _cVehicle1.IsStolen = true;
                    _bad1 = _cVehicle1.CreateRandomDriver();
                    _bad1.IsPersistent = true;
                    _bad1.BlockPermanentEvents = true;
                    _bad1.SetDrunk(Enums.DrunkState.Sloshed);
                    _bad1.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion();
                    _bad1.Tasks.FaceEntity(Player);
                    Game.DisplaySubtitle("~r~Driver:~s~ The world will end with fire!");
                    GameFiber.Wait(3000);
                    _cVehicle1.Explode();
                });
                break;
            case 3:
                Log.Info("Callout Scene 3");
                _bad1 = _cVehicle1.CreateRandomDriver();
                _bad1.IsPersistent = true;
                _bad1.BlockPermanentEvents = true;
                _bad1.SetWanted(true);
                _name = Functions.GetPersonaForPed(_bad1).FullName;
                Questioning.Enabled = true;
                _cVehicle1.IsStolen = true;
                break;
            default:
                Game.DisplayNotification(
                    "An error has been detected! Ending callout early to prevent LSPDFR crash!");
                End();
                break;
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
            GameFiber.StartNew(delegate
            {
                _speakSuspect.Enabled = false;
                Game.DisplaySubtitle("~g~You~s~: Hey there! We have reports of suspicious activity here, what's going on?", 5000);
                _bad1.Tasks.LeaveVehicle(_cVehicle1, LeaveVehicleFlags.LeaveDoorOpen);
                GameFiber.Wait(5000);
                _bad1.Tasks.FaceEntity(Player);
                _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                Game.DisplaySubtitle("~r~" + _name + "~s~: Nothing is wrong sir, I don't know why you got that idea.", 5000);
            });
        base.Conversations(sender, selItem, index);
    }
}