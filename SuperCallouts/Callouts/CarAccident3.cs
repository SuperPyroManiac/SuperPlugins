using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.API.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Car Accident3", CalloutProbability.Medium)]
internal class CarAccident3 : SuperCallout
{
    private readonly int _choice = new Random(DateTime.Now.Millisecond).Next(0, 4);
    private Blip _eBlip;
    private Ped _ePed;
    private Ped _ePed2;
    private Vehicle _eVehicle;
    private Vehicle _eVehicle2;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Car Accident (3)";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a motor vehicle accident.";
        CalloutAdvisory = "Caller reports the drivers are violently arguing.";
        Functions.PlayScannerAudioUsingPosition(
            "CITIZENS_REPORT_04 CRIME_HIT_AND_RUN_03 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~b~Dispatch", "~r~MVA",
            "Reports of a car accident, respond ~r~CODE-3");

        PyroFunctions.SpawnNormalCar(out _eVehicle, SpawnPoint.Position, SpawnPoint.Heading);
        _eVehicle.IsPersistent = true;
        PyroFunctions.DamageVehicle(_eVehicle, 200, 200);
        EntitiesToClear.Add(_eVehicle);

        PyroFunctions.SpawnNormalCar(out _eVehicle2, _eVehicle.GetOffsetPositionFront(7f));
        _eVehicle2.IsPersistent = true;
        _eVehicle2.Rotation = new Rotator(0f, 0f, 90f);
        PyroFunctions.DamageVehicle(_eVehicle2, 200, 200);
        EntitiesToClear.Add(_eVehicle2);

        _ePed = _eVehicle.CreateRandomDriver();
        _ePed.IsPersistent = true;
        _ePed.BlockPermanentEvents = true;
        EntitiesToClear.Add(_ePed);

        _ePed2 = _eVehicle2.CreateRandomDriver();
        _ePed2.IsPersistent = true;
        _ePed2.BlockPermanentEvents = true;
        EntitiesToClear.Add(_ePed2);

        _eBlip = new Blip(SpawnPoint.Position, 15f);
        _eBlip.Color = Color.Red;
        _eBlip.Alpha /= 2;
        _eBlip.Name = "Callout";
        _eBlip.Flash(500, 8000);
        _eBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_eBlip);

        Log.Info("Car Accident Scenario #" + _choice);
        if (!_eVehicle.Exists() || !_eVehicle2.Exists())
        { CalloutEnd(true); return; }
        switch (_choice)
        {
            case 0: //Peds fight
                _ePed.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                break;
            case 1: //Ped Dies, other flees
                _ePed.Kill();
                _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                break;
            case 2: //Hit and run
                _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                break;
            case 3: //Fire + dead ped.
                _ePed.Kill();
                _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                break;
            default:
                CalloutEnd(true);
                break;
        }
    }

    internal override void CalloutOnScene()
    {
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept",
            "~y~On Scene",
            "~r~Car Accident", "Investigate the scene.");
        _eBlip.DisableRoute();
        _ePed.BlockPermanentEvents = false;
        _ePed2.BlockPermanentEvents = false;
        switch (_choice)
        {
            case 0: //Peds fight
                _ePed.Tasks.FightAgainst(_ePed2);
                _ePed2.Tasks.FightAgainst(_ePed);
                break;
            case 1: //Ped Dies, other flees
                var pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, _ePed2);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;
            case 2: //Hit and run
                var pursuit2 = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit2, _ePed);
                Functions.SetPursuitIsActiveForPlayer(pursuit2, true);
                break;
            case 3: //Fire + dead ped.
                _ePed2.Tasks.Cower(-1);
                PyroFunctions.FireControl(SpawnPoint.Position.Around2D(7f), 24, true);
                break;
            default:
                End();
                break;
        }
    }
}