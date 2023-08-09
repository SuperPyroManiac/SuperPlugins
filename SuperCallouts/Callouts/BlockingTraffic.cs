#region

using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Blocking Traffic", CalloutProbability.Medium, "Vehicle parked in the road", "Code 3")]
internal class BlockingTraffic : SuperCallout
{
    private Blip _cBlip;
    private Vehicle _cVehicle;
    internal override Vector3 SpawnPoint { get; set; } = World.GetNextPositionOnStreet(Player.Position.Around(450f));
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Blocking Traffic";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a vehicle blocking traffic.";
        CalloutAdvisory = "Caller says the vehicle is abandoned in the middle of the road.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_11_351_01 IN_OR_ON_POSITION",
            SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Blocking Traffic",
            "Reports of a car blocking the road, respond ~y~CODE-2");

        PyroFunctions.SpawnNormalCar(out _cVehicle, SpawnPoint);
        EntitiesToClear.Add(_cVehicle);

        _cBlip = _cVehicle.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.DisableRoute();
    }
}