#region

using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.RemasteredCallouts;

[CalloutInterface("[SC] Stolen PD Vehicle", CalloutProbability.Medium, "Suspect has stolen a police vehicle", "Code 3")]
internal class StolenCopVehicle : SuperCallout
{
    private Ped _bad;
    private Blip _cBlip;
    private Vehicle _cVehicle;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)), 0);//TODO: MAKE THIS LIVE UPDATE
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Stolen Police Vehicle";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Stolen police vehicle.";
        CalloutAdvisory = "A suspect stole an officers vehicle during arrest.";
        Functions.PlayScannerAudioUsingPosition(
            "WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_RESIST_ARREST IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Stolen Police Vehicle",
            "A suspect has stolen a police vehicle during his arrest. Respond ~r~CODE-3");

        Model[] vehicleModels = { "POLICE", "POLICE2", "POLICE3", "SHERIFF", "SHERIFF2" };
        _cVehicle = new Vehicle(vehicleModels[new Random(DateTime.Now.Millisecond).Next(vehicleModels.Length)], SpawnPoint.Position)
            { IsPersistent = true, IsStolen = true, IsSirenOn = true, IsSirenSilent = true };
        EntitiesToClear.Add(_cVehicle);

        _bad = new Ped(SpawnPoint.Position.Around(15f));
        _bad.WarpIntoVehicle(_cVehicle, -1);
        _bad.IsPersistent = true;
        _bad.BlockPermanentEvents = true;
        _bad.SetWanted(true);
        _bad.SetDrunk(Enums.DrunkState.VeryDrunk);
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
        if (_cBlip.Exists()) _cBlip.Delete();
        PyroFunctions.StartPursuit(_bad);
        
        if (PyroCommon.Main.UsingUb)
            Wrapper.CallPursuit();
        else
            Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit,
                EBackupUnitType.LocalUnit);//TODO: REDO backup
    }
}