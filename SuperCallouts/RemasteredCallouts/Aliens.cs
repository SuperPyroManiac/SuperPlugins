using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Aliens", CalloutProbability.VeryLow)]
internal class Aliens : SuperCallout
{
    private Ped _alien1;
    private Ped _alien2;
    private Ped _alien3;
    private Blip _cBlip1;
    private Vehicle _cVehicle1;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 40;
    internal override string CalloutName { get; set; } = "Aliens";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of strange people.";
        CalloutAdvisory = "Caller says they're not human. Possibly a prank call.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 SUSPECTS_LAST_SEEN_02 IN_OR_ON_POSITION",
            SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Alien Sighting",
            "Caller claims that the subjects are aliens. Low priority, respond ~y~CODE-2");

        _cVehicle1 = new Vehicle("DUNE2", SpawnPoint.Position);
        _cVehicle1.IsPersistent = true;
        _cVehicle1.IsEngineOn = true;
        EntitiesToClear.Add(_cVehicle1);

        _alien1 = new Ped("S_M_M_MOVALIEN_01", _cVehicle1.Position.Around(5f), 0f);
        _alien1.SetVariation(0, 0, 0);
        _alien1.SetVariation(3, 0, 0);
        _alien1.SetVariation(4, 0, 0);
        _alien1.SetVariation(5, 0, 0);
        _alien1.IsPersistent = true;
        EntitiesToClear.Add(_alien1);

        _alien2 = new Ped("S_M_M_MOVALIEN_01", _cVehicle1.Position.Around(5f), 0f);
        _alien2.SetVariation(0, 0, 0);
        _alien2.SetVariation(3, 0, 0);
        _alien2.SetVariation(4, 0, 0);
        _alien2.SetVariation(5, 0, 0);
        _alien2.IsPersistent = true;
        EntitiesToClear.Add(_alien2);

        _alien3 = new Ped("S_M_M_MOVALIEN_01", _cVehicle1.Position.Around(5f), 0f);
        _alien3.SetVariation(0, 0, 0);
        _alien3.SetVariation(3, 0, 0);
        _alien3.SetVariation(4, 0, 0);
        _alien3.SetVariation(5, 0, 0);
        _alien3.IsPersistent = true;
        EntitiesToClear.Add(_alien3);
        
        _cBlip1 = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Yellow, true, true);
        BlipsToClear.Add(_cBlip1);
    }

    internal override void CalloutOnScene()
    {
        _alien1.Tasks.GoToEntity(Player);
        _alien2.Tasks.GoToEntity(Player);
        _alien3.Tasks.GoToEntity(Player);

        _cBlip1.DisableRoute();
        GameFiber.Wait(4000);
        _alien1.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
        _alien2.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
        _alien3.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
        _cVehicle1.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
        CalloutEnd(true);
    }
}