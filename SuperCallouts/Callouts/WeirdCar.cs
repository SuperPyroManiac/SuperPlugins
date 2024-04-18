#region

using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("[SC] Suspicious Vehicle", CalloutProbability.Medium, "Reports of a suspicious vehicle, limited details",
    "Code 2")]
internal class WeirdCar : SuperCallout
{
    private readonly Random _rNd = new();
    private Ped _bad1;
    private Blip _cBlip1;
    private Vehicle _cVehicle1;
    private string _name;
    private UIMenuItem _speakSuspect;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 30;
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

        PyroFunctions.SpawnNormalCar(out _cVehicle1, SpawnPoint.Position);
        _cVehicle1.Heading = SpawnPoint.Heading;
        _cVehicle1.IsPersistent = true;
        EntitiesToClear.Add(_cVehicle1);

        _cBlip1 = _cVehicle1.AttachBlip();
        _cBlip1.EnableRoute(Color.Yellow);
        _cBlip1.Color = Color.Yellow;
        BlipsToClear.Add(_cBlip1);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name);
        ConvoMenu.AddItem(_speakSuspect);
    }

    internal override void CalloutOnScene()
    {
        _cBlip1.DisableRoute();
        Game.DisplayNotification("Investigate the vehicle.");
        var choices = _rNd.Next(1, 4);
        switch (choices)
        {
            case 1:
                PyroFunctions.DamageVehicle(_cVehicle1, 500, 500);
                _cVehicle1.IsStolen = true;
                CalloutInterfaceAPI.Functions.SendMessage(this, "Officer on scene.");
                break;
            case 2:
                GameFiber.StartNew(delegate
                {
                    _cVehicle1.IsStolen = true;
                    _bad1 = _cVehicle1.CreateRandomDriver();
                    _bad1.IsPersistent = true;
                    _bad1.BlockPermanentEvents = true;
                    _bad1.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                    Game.DisplaySubtitle("~r~Driver:~s~ The world will end with fire!");
                    GameFiber.Wait(3000);
                    _cVehicle1.Explode();
                    CalloutInterfaceAPI.Functions.SendMessage(this, "Officer on scene.");
                    CalloutInterfaceAPI.Functions.SendMessage(this, "Vehicle explosion.");
                });
                break;
            case 3:
                _bad1 = _cVehicle1.CreateRandomDriver();
                _bad1.IsPersistent = true;
                _bad1.BlockPermanentEvents = true;
                _name = Functions.GetPersonaForPed(_bad1).FullName;
                PyroFunctions.SetWanted(_bad1, true);
                _cVehicle1.IsStolen = true;
                CalloutInterfaceAPI.Functions.SendMessage(this, "Officer on scene.");
                _speakSuspect.Enabled = true;
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
                Game.DisplaySubtitle("~g~You~s~: We have reports of suspicious activity here, what's going on?",
                    5000);
                _bad1.Tasks.LeaveVehicle(_cVehicle1, LeaveVehicleFlags.LeaveDoorOpen);
                GameFiber.Wait(5000);
                NativeFunction.Natives.x5AD23D40115353AC(_bad1, Game.LocalPlayer.Character, -1);
                _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                Game.DisplaySubtitle(
                    "~r~" + _name + "~s~: Nothing is wrong sir, I don't know why you got that idea.", 5000);
            });
        base.Conversations(sender, selItem, index);
    }
}