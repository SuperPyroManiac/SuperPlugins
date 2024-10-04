using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.API.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Fire", CalloutProbability.Medium)]
internal class Fire : SuperCallout
{
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private int _partHandleBigFire;
    private int _partHandleMistySmoke;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 15f;
    internal override string CalloutName { get; set; } = "Fire";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a car fire";
        CalloutAdvisory = "Caller reports large flames coming from the vehicle.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION",
            SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Fire",
            "Reports of a car fire, respond ~r~CODE-3");

        _cVehicle = PyroFunctions.SpawnCar(SpawnPoint);
        EntitiesToClear.Add(_cVehicle);
        
        _partHandleBigFire = Particles.StartLoopedParticlesOnEntity("scr_trevor3", "scr_trev3_trailer_plume", _cVehicle,
            new Vector3(0, 1f, 0), Vector3.Zero, 0.7f);
        
        _partHandleMistySmoke = Particles.StartLoopedParticlesOnEntity("scr_agencyheistb", "scr_env_agency3b_smoke", _cVehicle,
            new Vector3(0, 1f, 0), Vector3.Zero, .7f);

        _cBlip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Red, true, true, 40f);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.Position = SpawnPoint.Position;
        _cBlip.Scale = 20;
        _cBlip.DisableRoute();
        
        GameFiber.Wait(5000);
        _cVehicle.StartFire(true);
        
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