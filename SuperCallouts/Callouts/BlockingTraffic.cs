using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Blocking Traffic", CalloutProbability.Medium)]
internal class BlockingTraffic : SuperCallout
{
    private Blip _vehicleBlip;
    private Vehicle _vehicle;

    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(450f)));
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Blocking Traffic";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a vehicle blocking traffic.";
        CalloutAdvisory = "Caller says the vehicle is abandoned in the middle of the road.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_11_351_01 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Blocking Traffic", "Reports of a car blocking the road, respond ~y~CODE-2");

        SpawnVehicle();
        CreateBlip();
    }

    private void SpawnVehicle()
    {
        PyroFunctions.SpawnNormalCar(out _vehicle, SpawnPoint.Position);
        EntitiesToClear.Add(_vehicle);
    }

    private void CreateBlip()
    {
        _vehicleBlip = _vehicle.AttachBlip();
        _vehicleBlip.Color = Color.Red;
        _vehicleBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_vehicleBlip);
    }

    internal override void CalloutOnScene()
    {
        _vehicleBlip?.DisableRoute();
    }
}
