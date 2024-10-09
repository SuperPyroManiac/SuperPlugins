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
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private bool _blipHelper;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 50f;
    internal override string CalloutName { get; set; } = "Stolen Police Vehicle";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Stolen police vehicle.";
        CalloutAdvisory = "A suspect stole an officers vehicle during arrest.";
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_RESIST_ARREST IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch","~r~Stolen Police Vehicle",
            "A suspect has stolen a police vehicle during his arrest. Respond ~r~CODE-3");

        Model[] vehicleModels = ["POLICE", "POLICE2", "POLICE3", "SHERIFF", "SHERIFF2"];
        _cVehicle = new Vehicle(vehicleModels[new Random(DateTime.Now.Millisecond).Next(vehicleModels.Length)], SpawnPoint.Position)
                { IsPersistent = true, IsStolen = true, IsSirenOn = true, IsSirenSilent = true };
        EntitiesToClear.Add(_cVehicle);

        _suspect = new Ped(SpawnPoint.Position.Around(15f));
        _suspect.WarpIntoVehicle(_cVehicle, -1);
        _suspect.IsPersistent = true;
        _suspect.BlockPermanentEvents = true;
        _suspect.SetWanted(true);
        _suspect.SetDrunk(Enums.DrunkState.VeryDrunk);
        EntitiesToClear.Add(_suspect);

        _cBlip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Red, true, false, 15);
        BlipsToClear.Add(_cBlip);

        _suspect.Tasks.CruiseWithVehicle(_cVehicle, 100f, VehicleDrivingFlags.Emergency);
    }

    internal override void CalloutRunning()
    {
        if ( !_suspect )
        {
            CalloutEnd(true);
            return;
        }
        
        if ( !OnScene && !_blipHelper )
        {
            GameFiber.StartNew(delegate
            {
                _blipHelper = true;
                SpawnPoint = new Location(_suspect.Position);
                if ( _cBlip )
                {
                    _cBlip.DisableRoute();
                    _cBlip.Position = SpawnPoint.Position;
                    _cBlip.EnableRoute(Color.Red);
                }

                GameFiber.Sleep(2500);
                _blipHelper = false;
            });
        }
    }

    internal override void CalloutOnScene()
    {
        if ( !_suspect )
        {
            CalloutEnd(true);
            return;
        }
        
        _cBlip?.Delete();
        PyroFunctions.StartPursuit(false, true, _suspect);
        PyroFunctions.RequestBackup(Enums.BackupType.Pursuit);
    }
}