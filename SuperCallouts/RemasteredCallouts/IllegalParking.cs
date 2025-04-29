using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Extensions;
using PyroCommon.Models;
using PyroCommon.Utils;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Models.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Illegal Parking", CalloutProbability.Medium)]
internal class IllegalParking : SuperCallout
{
    private Blip _blip;
    private Vehicle _vehicle;
    private Ped _suspect;
    private readonly int _sceneType = new Random(DateTime.Now.Millisecond).Next(1, 5);
    private int _partHandleBigFire;
    private int _partHandleMistySmoke;
    internal override Location SpawnPoint { get; set; } = CommonUtils.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 25f;
    internal override string CalloutName { get; set; } = "Illegal Parking";

    internal override void CalloutPrep()
    {
        CalloutMessage = $"~r~{Settings.EmergencyNumber} Report:~s~ Reports of a vehicle parked illegally.";
        CalloutAdvisory = "Caller says a vehicle is parked on their property without permission.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE PYRO_ILLEGAL_PARKING IN_OR_ON_POSITION",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~y~Traffic",
            "Reports of an empty vehicle on private property, respond ~g~CODE-1"
        );

        SpawnVehicle();
        CreateBlip();
    }

    private void SpawnVehicle()
    {
        _vehicle = CommonUtils.SpawnCar(SpawnPoint);
        EntitiesToClear.Add(_vehicle);

        if (_sceneType == 1)
        {
            _vehicle.ApplyDamage(100, 100);
            _partHandleBigFire = ParticleUtils.StartLoopedParticlesOnEntity(
                "scr_trevor3",
                "scr_trev3_trailer_plume",
                _vehicle,
                new Vector3(0, 1f, 0),
                Vector3.Zero,
                0.7f
            );
            _partHandleMistySmoke = ParticleUtils.StartLoopedParticlesOnEntity(
                "scr_agencyheistb",
                "scr_env_agency3b_smoke",
                _vehicle,
                new Vector3(0, 1f, 0),
                Vector3.Zero,
                .7f
            );
        }
    }

    private void CreateBlip()
    {
        _blip = CommonUtils.CreateSearchBlip(SpawnPoint, Color.Yellow, true, true, 40f);
        BlipsToClear.Add(_blip);
    }

    internal override void CalloutOnScene()
    {
        UpdateBlip();

        if (!_vehicle)
        {
            CalloutEnd(true);
            return;
        }

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

    private void HandleScenario()
    {
        switch (_sceneType)
        {
            case 1: // Burning vehicle
                LogUtils.Info("Callout Scene 1");
                GameFiber.Wait(5000);
                _vehicle.StartFire(false);
                GameFiber.Wait(12000);
                ParticleUtils.StopLoopedParticles(_partHandleBigFire);
                break;

            case 2: // Stolen vehicle with bomb
                LogUtils.Info("Callout Scene 2");
                _vehicle.IsStolen = true;
                CommonUtils.AddSearchItem("~r~Bomb", null, _vehicle);
                break;

            case 3: // Suicide scene
                LogUtils.Info("Callout Scene 3");
                _suspect = _vehicle.CreateRandomDriver();
                _suspect.IsPersistent = true;
                EntitiesToClear.Add(_suspect);
                _suspect.Kill();
                CommonUtils.AddDrugItem("~r~Large pile of white powder", Enums.DrugType.Methamphetamine, _suspect);
                CommonUtils.AddSearchItem("~r~Empty used needles", _suspect);
                CommonUtils.AddSearchItem("~y~Suicide letter", null, _vehicle);
                break;

            case 4: // Normal illegally parked vehicle
                break;
        }
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        ParticleUtils.StopLoopedParticles(_partHandleBigFire);
        ParticleUtils.StopLoopedParticles(_partHandleMistySmoke);
        base.CalloutEnd(forceCleanup);
    }
}
