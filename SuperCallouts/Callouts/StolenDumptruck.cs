using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Stolen Construction Vehicle", CalloutProbability.Low)]
internal class StolenDumptruck : SuperCallout
{
    private Ped _suspect;
    private Blip _suspectBlip;
    private Vehicle _dumpTruck;

    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Stolen Construction Vehicle";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Stolen construction vehicle.";
        CalloutAdvisory = "A very large vehicle was stolen from a construction site.";
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_ROBBERY_01 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Stolen Construction Vehicle",
            "A suspect has stolen a very large construction vehicle. Respond ~r~CODE-3"
        );

        SpawnVehicle();
        SpawnSuspect();
        CreateBlip();
        InitiateDriving();
    }

    private void SpawnVehicle()
    {
        _dumpTruck = new Vehicle("dump", SpawnPoint.Position) { IsPersistent = true, IsStolen = true };
        EntitiesToClear.Add(_dumpTruck);
    }

    private void SpawnSuspect()
    {
        _suspect = new Ped(SpawnPoint.Position.Around(15f));
        _suspect.WarpIntoVehicle(_dumpTruck, -1);
        _suspect.IsPersistent = true;
        _suspect.BlockPermanentEvents = true;
        _suspect.Metadata.stpDrugsDetected = true;
        _suspect.Metadata.stpAlcoholDetected = true;
        PyroFunctions.SetDrunkOld(_suspect, true);
        EntitiesToClear.Add(_suspect);
    }

    private void CreateBlip()
    {
        _suspectBlip = _suspect.AttachBlip();
        _suspectBlip.EnableRoute(Color.Red);
        _suspectBlip.Color = Color.Red;
        _suspectBlip.Scale = .5f;
        BlipsToClear.Add(_suspectBlip);
    }

    private void InitiateDriving()
    {
        _suspect.Tasks.CruiseWithVehicle(_dumpTruck, 100f, VehicleDrivingFlags.Emergency);
    }

    internal override void CalloutOnScene()
    {
        _suspectBlip?.Delete();
        StartPursuit();
        RequestBackup();
    }

    private void StartPursuit()
    {
        var pursuit = Functions.CreatePursuit();
        Functions.AddPedToPursuit(pursuit, _suspect);
        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
    }

    private void RequestBackup()
    {
        PyroFunctions.RequestBackup(Enums.BackupType.Pursuit);
    }
}
