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
    private readonly Random _rnd = new();
    private Ped _cPed;
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private string _name;
    private UIMenuItem _speakSuspect;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 40;
    internal override string CalloutName { get; set; } = "Suspicious Vehicle";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Suspicious vehicle.";
        CalloutAdvisory = "Suspicious vehicle was found on the side of the road. Approach with caution.";
        Functions.PlayScannerAudioUsingPosition("WE_HAVE PYRO_SUS_VEHICLE IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Suspicious Vehicle",
            "Report of a suspicious vehicle on the side of the road. Respond ~y~CODE-2"
        );

        _cVehicle = PyroFunctions.SpawnCar(SpawnPoint);
        EntitiesToClear.Add(_cVehicle);

        _cBlip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Yellow, true, true, 40f);
        BlipsToClear.Add(_cBlip);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name);
        ConvoMenu.AddItem(_speakSuspect);
    }

    internal override void CalloutOnScene()
    {
        if (!_cVehicle)
        {
            CalloutEnd(true);
            return;
        }

        UpdateBlip();
        Game.DisplayNotification("Investigate the vehicle.");

        var sceneType = _rnd.Next(1, 4);
        Log.Info($"Suspicious vehicle scene: {sceneType}");

        switch (sceneType)
        {
            case 1: // Damaged stolen vehicle
                SetupDamagedStolenVehicle();
                break;
            case 2: // Exploding vehicle with drunk driver
                SetupExplodingVehicle();
                break;
            case 3: // Wanted driver
                SetupWantedDriver();
                break;
            default:
                Game.DisplayNotification("An error has been detected! Ending callout early to prevent LSPDFR crash!");
                End();
                break;
        }
    }

    private void UpdateBlip()
    {
        if (_cBlip)
        {
            _cBlip.Position = SpawnPoint.Position;
            _cBlip.Scale = 20;
            _cBlip.DisableRoute();
        }
    }

    private void SetupDamagedStolenVehicle()
    {
        Log.Info("Callout Scene 1");
        _cVehicle.ApplyDamage(500, 500);
        _cVehicle.IsStolen = true;
    }

    private void SetupExplodingVehicle()
    {
        Log.Info("Callout Scene 2");
        GameFiber.StartNew(
            delegate
            {
                _cVehicle.IsStolen = true;
                _cPed = _cVehicle.CreateRandomDriver();
                _cPed.IsPersistent = true;
                _cPed.BlockPermanentEvents = true;
                _cPed.SetDrunk(Enums.DrunkState.Sloshed);
                _cPed.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion();
                _cPed.Tasks.FaceEntity(Player);
                Game.DisplaySubtitle("~r~Driver:~s~ The world will end with fire!");
                GameFiber.Wait(3000);
                _cVehicle.Explode();
            }
        );
    }

    private void SetupWantedDriver()
    {
        Log.Info("Callout Scene 3");
        _cPed = _cVehicle.CreateRandomDriver();
        _cPed.IsPersistent = true;
        _cPed.BlockPermanentEvents = true;
        _cPed.SetWanted(true);
        _name = Functions.GetPersonaForPed(_cPed).FullName;
        Questioning.Enabled = true;
        _cVehicle.IsStolen = true;
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (!_cPed)
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
                        "~g~You~s~: Hey there! We have reports of suspicious activity here, what's going on?",
                        5000
                    );
                    _cPed.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.LeaveDoorOpen);
                    GameFiber.Wait(5000);
                    _cPed.Tasks.FaceEntity(Player);
                    _cPed.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    Game.DisplaySubtitle($"~r~{_name}~s~: Nothing is wrong sir, I don't know why you got that idea.", 5000);
                }
            );
        }
        base.Conversations(sender, selItem, index);
    }
}
