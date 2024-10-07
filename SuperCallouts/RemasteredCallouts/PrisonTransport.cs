using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Transport Escape", CalloutProbability.Medium)]
internal class PrisonTransport : SuperCallout
{
    private Ped _suspect;
    private Blip _cBlip;
    private Ped _cop;
    private Vehicle _cVehicle;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(500f)));
    internal override float OnSceneDistance { get; set; } = 90;
    internal override string CalloutName { get; set; } = "Transport Escape";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Prisoner escaped transport.";
        CalloutAdvisory = "Officers report a suspect has jumped out of a moving transport vehicle.";
        Functions.PlayScannerAudioUsingPosition("OFFICERS_REPORT_01 CRIME_SUSPECT_ON_THE_RUN_01 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Escaped Prisoner",
            "DOC reports a prisoner has unlocked the transport vehicle and is on the run. ~r~CODE-3");

        _cVehicle = new Vehicle("POLICET", SpawnPoint.Position) { IsPersistent = true };
        EntitiesToClear.Add(_cVehicle);

        _cop = new Ped("csb_cop", SpawnPoint.Position, 0f);
        _cop.IsPersistent = true;
        _cop.WarpIntoVehicle(_cVehicle, -1);
        _cop.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_cop);

        _suspect = new Ped("s_m_y_prisoner_01", SpawnPoint.Position, 0f);
        _suspect.IsPersistent = true;
        _suspect.SetWanted(false);
        _suspect.WarpIntoVehicle(_cVehicle, 1);
        _suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
        PyroFunctions.ClearSearchItems(_suspect);
        EntitiesToClear.Add(_suspect);

        _cBlip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Red, true, false, 20f);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.DisableRoute();
        switch (new Random(DateTime.Now.Millisecond).Next(1, 3))
        {
            case 1:
              Log.Info("Callout Scene 1");
                PyroFunctions.AddFirearmItem("Pistol", "weapon_pistol_mk2", true, true, true, _suspect);
                _suspect.Inventory.EquippedWeapon = "weapon_pistol_mk2";
                _suspect.Tasks.FightAgainst(_cop);
                _suspect.Health = 250;
                GameFiber.Wait(6000);
                if (_suspect.IsAlive)
                {
                    if (_cop.IsAlive) _cop.Kill();
                    PyroFunctions.StartPursuit(false, false, _suspect);
                }
                break;
            case 2:
              Log.Info("Callout Scene 2");
                var pursuit = PyroFunctions.StartPursuit(false, false, _suspect);
                Functions.AddCopToPursuit(pursuit, _cop);
                break;
        }
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        if (_cVehicle.Exists() && _cop.Exists() && _cop.IsAlive) _cop.Tasks.EnterVehicle(_cVehicle, -1);
        base.CalloutEnd(forceCleanup);
    }
}