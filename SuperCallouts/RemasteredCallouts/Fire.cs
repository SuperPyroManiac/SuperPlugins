using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Types.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Fire", CalloutProbability.Medium)]
internal class Fire : SuperCallout
{
    private Blip _blip;
    private Vehicle _vehicle;
    private int _partHandleBigFire;
    private int _partHandleMistySmoke;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 15f;
    internal override string CalloutName { get; set; } = "Fire";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a car fire";
        CalloutAdvisory = "Caller reports large flames coming from the vehicle.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE PYRO_CAR_FIRE IN_OR_ON_POSITION",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Fire",
            "Reports of a car fire, respond ~r~CODE-3"
        );

        SpawnVehicle();
        CreateFireEffects();
        CreateBlip();
    }

    private void SpawnVehicle()
    {
        _vehicle = PyroFunctions.SpawnCar(SpawnPoint);
        EntitiesToClear.Add(_vehicle);
    }

    private void CreateFireEffects()
    {
        _partHandleBigFire = Particles.StartLoopedParticlesOnEntity(
            "scr_trevor3",
            "scr_trev3_trailer_plume",
            _vehicle,
            new Vector3(0, 1f, 0),
            Vector3.Zero,
            0.7f
        );
        _partHandleMistySmoke = Particles.StartLoopedParticlesOnEntity(
            "scr_agencyheistb",
            "scr_env_agency3b_smoke",
            _vehicle,
            new Vector3(0, 1f, 0),
            Vector3.Zero,
            .7f
        );
    }

    private void CreateBlip()
    {
        _blip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Red, true, true, 40f);
        BlipsToClear.Add(_blip);
    }

    internal override void CalloutOnScene()
    {
        if (_blip)
        {
            _blip.Position = SpawnPoint.Position;
            _blip.Scale = 20;
            _blip.DisableRoute();
        }

        GameFiber.Wait(5000);
        _vehicle?.StartFire(true);

        GameFiber.Wait(12000);
        Particles.StopLoopedParticles(_partHandleBigFire);
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        Particles.StopLoopedParticles(_partHandleBigFire);
        Particles.StopLoopedParticles(_partHandleMistySmoke);
        base.CalloutEnd(forceCleanup);
    }
}
