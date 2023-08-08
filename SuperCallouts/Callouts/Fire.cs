#region
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Fire", CalloutProbability.Medium, "Reports of a vehicle fire", "Code 3")]
internal class Fire : SuperCallout
{
    internal override Vector3 SpawnPoint { get; set; }
    internal override float OnSceneDistance { get; set; } = 35;
    internal override string CalloutName { get; set; } = "Fire";
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private float _spawnPointH;
    
    internal override void CalloutPrep()
    {
        PyroFunctions.FindSideOfRoad(750, 280, out var tempSpawnPoint, out _spawnPointH);
        SpawnPoint = tempSpawnPoint;
        CalloutMessage = "~b~Dispatch:~s~ Reports of a car fire";
        CalloutAdvisory = "Caller reports large flames coming from the vehicle.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION",
            SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Fire",
            "Reports of a car fire, respond ~r~CODE-3");

        PyroFunctions.SpawnAnyCar(out _cVehicle, SpawnPoint);
        _cVehicle.Heading = _spawnPointH;
        EntitiesToClear.Add(_cVehicle);
        
        _cBlip = _cVehicle.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.DisableRoute();
        for (var i = 0; i < 5; i++) PyroFunctions.FireControl(SpawnPoint.Around2D(1f, 5f), 24, true);
        for (var i = 0; i < 10; i++) PyroFunctions.FireControl(SpawnPoint.Around2D(1f, 5f), 24, false);
    }
}