using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Car Accident3", CalloutProbability.Medium)]
internal class CarAccident3 : SuperCallout
{
    private readonly int _scenarioType = new Random(DateTime.Now.Millisecond).Next(0, 4);
    private Blip _sceneBlip;
    private Ped _driver1;
    private Ped _driver2;
    private Vehicle _vehicle1;
    private Vehicle _vehicle2;

    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Car Accident (3)";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a motor vehicle accident.";
        CalloutAdvisory = "Caller reports the drivers are violently arguing.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_HIT_AND_RUN_03 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~b~Dispatch", "~r~MVA", "Reports of a car accident, respond ~r~CODE-3");

        SpawnVehicles();
        SpawnDrivers();
        CreateBlip();
        SetupScenario();
    }

    private void SpawnVehicles()
    {
        PyroFunctions.SpawnNormalCar(out _vehicle1, SpawnPoint.Position, SpawnPoint.Heading);
        _vehicle1.IsPersistent = true;
        PyroFunctions.DamageVehicle(_vehicle1, 200, 200);
        EntitiesToClear.Add(_vehicle1);

        PyroFunctions.SpawnNormalCar(out _vehicle2, _vehicle1.GetOffsetPositionFront(7f));
        _vehicle2.IsPersistent = true;
        _vehicle2.Rotation = new Rotator(0f, 0f, 90f);
        PyroFunctions.DamageVehicle(_vehicle2, 200, 200);
        EntitiesToClear.Add(_vehicle2);
    }

    private void SpawnDrivers()
    {
        _driver1 = _vehicle1.CreateRandomDriver();
        _driver1.IsPersistent = true;
        _driver1.BlockPermanentEvents = true;
        EntitiesToClear.Add(_driver1);

        _driver2 = _vehicle2.CreateRandomDriver();
        _driver2.IsPersistent = true;
        _driver2.BlockPermanentEvents = true;
        EntitiesToClear.Add(_driver2);
    }

    private void CreateBlip()
    {
        _sceneBlip = new Blip(SpawnPoint.Position, 15f)
        {
            Color = Color.Red,
            Alpha = .5f,
            Name = "Callout",
        };
        _sceneBlip.Flash(500, 8000);
        _sceneBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_sceneBlip);
    }

    private void SetupScenario()
    {
        Log.Info($"Car Accident Scenario #{_scenarioType}");

        if (!_vehicle1.Exists() || !_vehicle2.Exists())
        {
            CalloutEnd(true);
            return;
        }

        switch (_scenarioType)
        {
            case 0: // Peds fight
                _driver1.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                _driver2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                break;

            case 1: // Ped Dies, other flees
                _driver1.Kill();
                _driver2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                break;

            case 2: // Hit and run
                _driver2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                break;

            case 3: // Fire + dead ped
                _driver1.Kill();
                _driver2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                break;

            default:
                CalloutEnd(true);
                break;
        }
    }

    internal override void CalloutOnScene()
    {
        if (!_driver1 || !_driver2)
        {
            CalloutEnd(true);
            return;
        }

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~y~On Scene", "~r~Car Accident", "Investigate the scene.");
        _sceneBlip?.DisableRoute();

        _driver1.BlockPermanentEvents = false;
        _driver2.BlockPermanentEvents = false;

        ExecuteScenario();
    }

    private void ExecuteScenario()
    {
        switch (_scenarioType)
        {
            case 0: // Peds fight
                _driver1.Tasks.FightAgainst(_driver2);
                _driver2.Tasks.FightAgainst(_driver1);
                break;

            case 1: // Ped Dies, other flees
                var pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, _driver2);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;

            case 2: // Hit and run
                var pursuit2 = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit2, _driver1);
                Functions.SetPursuitIsActiveForPlayer(pursuit2, true);
                break;

            case 3: // Fire + dead ped
                _driver2.Tasks.Cower(-1);
                PyroFunctions.FireControl(SpawnPoint.Position.Around2D(7f), 24, true);
                break;

            default:
                End();
                break;
        }
    }
}
