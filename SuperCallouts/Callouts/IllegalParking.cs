#region

using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("[SC] Illegal Parking", CalloutProbability.Medium, "Reports of a vehicle parked illegally", "LOW")]
internal class IllegalParking : SuperCallout
{
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private float _heading;
    internal override Vector3 SpawnPoint { get; set; }
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Illegal Parking";

    internal override void CalloutPrep()
    {
        PyroFunctions.FindSideOfRoad(750, 280, out var tempSpawnPoint, out _heading);
        SpawnPoint = tempSpawnPoint;
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Reports of a vehicle parked illegally.";
        CalloutAdvisory = "Caller says a vehicle is parked on their property without permission.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION",
            SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~y~Traffic",
            "Reports of an empty vehicle on private property, respond ~g~CODE-1");

        PyroFunctions.SpawnNormalCar(out _cVehicle, SpawnPoint, _heading);
        EntitiesToClear.Add(_cVehicle);

        _cBlip = _cVehicle.AttachBlip();
        _cBlip.Color = Color.DodgerBlue;
        _cBlip.EnableRoute(Color.DodgerBlue);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.DisableRoute();
    }
}