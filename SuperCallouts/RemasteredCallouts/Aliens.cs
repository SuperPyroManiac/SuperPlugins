using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
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
    private Blip _blip;
    private Vehicle _vehicle;
    internal override Location SpawnPoint { get; set; } =
        new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 40;
    internal override string CalloutName { get; set; } = "Aliens";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of strange people.";
        CalloutAdvisory = "Caller says they're not human. Possibly a prank call.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 PYRO_ALIEN IN_OR_ON_POSITION",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Alien Sighting",
            "Caller claims that the subjects are aliens. Low priority, respond ~y~CODE-2"
        );

        SpawnVehicle();
        SpawnAliens();
        CreateBlip();
    }

    private void SpawnVehicle()
    {
        _vehicle = new Vehicle("DUNE2", SpawnPoint.Position);
        _vehicle.IsPersistent = true;
        _vehicle.IsEngineOn = true;
        EntitiesToClear.Add(_vehicle);
    }

    private void SpawnAliens()
    {
        _alien1 = CreateAlien(_vehicle.Position.Around(5f));
        _alien2 = CreateAlien(_vehicle.Position.Around(5f));
        _alien3 = CreateAlien(_vehicle.Position.Around(5f));
    }

    private Ped CreateAlien(Vector3 position)
    {
        var alien = new Ped("S_M_M_MOVALIEN_01", position, 0f);
        alien.SetVariation(0, 0, 0);
        alien.SetVariation(3, 0, 0);
        alien.SetVariation(4, 0, 0);
        alien.SetVariation(5, 0, 0);
        alien.IsPersistent = true;
        EntitiesToClear.Add(alien);
        return alien;
    }

    private void CreateBlip()
    {
        _blip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Yellow, true, true);
        BlipsToClear.Add(_blip);
    }

    internal override void CalloutOnScene()
    {
        if (!_alien1 || !_alien2 || !_alien3 || !_vehicle)
        {
            CalloutEnd(true);
            return;
        }

        MakeAliensApproachPlayer();
        _blip?.DisableRoute();
        LaunchAliensAndVehicle();
        CleanupEntities();
        CalloutEnd();
    }

    private void MakeAliensApproachPlayer()
    {
        _alien1.Tasks.GoToEntity(Player);
        _alien2.Tasks.GoToEntity(Player);
        _alien3.Tasks.GoToEntity(Player);
        GameFiber.Wait(4000);
    }

    private void LaunchAliensAndVehicle()
    {
        _alien1.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
        _alien2.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
        _alien3.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
        _vehicle.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
    }

    private void CleanupEntities()
    {
        foreach (var entity in EntitiesToClear.Where(entity => entity))
            entity.Delete();
        EntitiesToClear.Clear();
    }
}
