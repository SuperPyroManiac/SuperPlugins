#region

using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Rage.Native;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("[SC] Fire", CalloutProbability.Medium, "Reports of a vehicle fire", "Code 3")]
internal class Fire : SuperCallout
{
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private int CarSmokePTFX;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 85;
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

        _cBlip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Red, true, true, 40f);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.Position = SpawnPoint.Position;
        _cBlip.Scale = 20;
        _cBlip.DisableRoute();
        
        _cVehicle.StartFire(true);
        // for (var i = 0; i < 5; i++) PyroFunctions.FireControl(SpawnPoint.Position.Around2D(1f, 5f), 24, true);
        // for (var i = 0; i < 10; i++) PyroFunctions.FireControl(SpawnPoint.Position.Around2D(1f, 5f), 24, false);
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        base.CalloutEnd(forceCleanup);
    }
}