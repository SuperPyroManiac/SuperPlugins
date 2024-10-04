using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.API.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Stolen Construction Vehicle", CalloutProbability.Low)]
internal class StolenDumptruck : SuperCallout
{
    private Ped _bad;
    private Blip _cBlip;
    private Vehicle _cVehicle;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Stolen Construction Vehicle";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Stolen construction vehicle.";
        CalloutAdvisory = "A very large vehicle was stolen from a construction site.";
        Functions.PlayScannerAudioUsingPosition(
            "WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_ROBBERY_01 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Stolen Construction Vehicle",
            "A suspect has stolen a very large construction vehicle. Respond ~r~CODE-3");

        _cVehicle = new Vehicle("dump", SpawnPoint.Position)
            { IsPersistent = true, IsStolen = true };
        EntitiesToClear.Add(_cVehicle);

        _bad = new Ped(SpawnPoint.Position.Around(15f));
        _bad.WarpIntoVehicle(_cVehicle, -1);
        _bad.IsPersistent = true;
        _bad.BlockPermanentEvents = true;
        _bad.Metadata.stpDrugsDetected = true;
        _bad.Metadata.stpAlcoholDetected = true;
        PyroFunctions.SetDrunkOld(_bad, true);
        EntitiesToClear.Add(_bad);

        _cBlip = _bad.AttachBlip();
        _cBlip.EnableRoute(Color.Red);
        _cBlip.Color = Color.Red;
        _cBlip.Scale = .5f;
        BlipsToClear.Add(_cBlip);

        _bad.Tasks.CruiseWithVehicle(_cVehicle, 100f, VehicleDrivingFlags.Emergency);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.Delete();
        var pursuit = Functions.CreatePursuit();
        Functions.AddPedToPursuit(pursuit, _bad);
        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
        Backup.Request(Enums.BackupType.Pursuit);
    }
}