using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Stolen PD Vehicle", CalloutProbability.Medium)]
internal class StolenCopVehicle : SuperCallout
{
    private Ped _suspect;
    private Blip _blip;
    private Vehicle _vehicle;
    private bool _blipHelper;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 50f;
    internal override string CalloutName { get; set; } = "Stolen Police Vehicle";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Stolen police vehicle.";
        CalloutAdvisory = "A suspect stole an officers vehicle during arrest.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE PYRO_STOLEN_PDCAR IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Stolen Police Vehicle",
            "A suspect has stolen a police vehicle during his arrest. Respond ~r~CODE-3"
        );

        SpawnVehicle();
        SpawnSuspect();
        CreateBlip();
    }

    private void SpawnVehicle()
    {
        Model[] vehicleModels = ["POLICE", "POLICE2", "POLICE3", "SHERIFF", "SHERIFF2"];
        _vehicle = new Vehicle(vehicleModels[new Random(DateTime.Now.Millisecond).Next(vehicleModels.Length)], SpawnPoint.Position)
        {
            IsPersistent = true,
            IsStolen = true,
            IsSirenOn = true,
            IsSirenSilent = true,
        };
        EntitiesToClear.Add(_vehicle);
    }

    private void SpawnSuspect()
    {
        _suspect = new Ped(SpawnPoint.Position.Around(15f));
        _suspect.WarpIntoVehicle(_vehicle, -1);
        _suspect.IsPersistent = true;
        _suspect.BlockPermanentEvents = true;
        _suspect.SetWanted(true);
        _suspect.SetDrunk(Enums.DrunkState.VeryDrunk);
        EntitiesToClear.Add(_suspect);

        _suspect.Tasks.CruiseWithVehicle(_vehicle, 100f, VehicleDrivingFlags.Emergency);
    }

    private void CreateBlip()
    {
        _blip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Red, true, false, 15);
        BlipsToClear.Add(_blip);
    }

    internal override void CalloutRunning()
    {
        if (!_suspect)
        {
            CalloutEnd(true);
            return;
        }

        UpdateBlipIfNeeded();
    }

    private void UpdateBlipIfNeeded()
    {
        if (!OnScene && !_blipHelper)
        {
            GameFiber.StartNew(
                delegate
                {
                    _blipHelper = true;
                    SpawnPoint = new Location(_suspect.Position);
                    if (_blip)
                    {
                        _blip.DisableRoute();
                        _blip.Position = SpawnPoint.Position;
                        _blip.EnableRoute(Color.Red);
                    }

                    GameFiber.Sleep(2500);
                    _blipHelper = false;
                }
            );
        }
    }

    internal override void CalloutOnScene()
    {
        if (!_suspect)
        {
            CalloutEnd(true);
            return;
        }

        _blip?.Delete();
        PyroFunctions.StartPursuit(false, true, _suspect);
        PyroFunctions.RequestBackup(Enums.BackupType.Pursuit);
    }
}
