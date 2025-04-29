using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Extensions;
using PyroCommon.Utils;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Models.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Transport Escape", CalloutProbability.Medium)]
internal class PrisonTransport : SuperCallout
{
    private Ped _suspect;
    private Blip _blip;
    private Ped _officer;
    private Vehicle _vehicle;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(500f)));
    internal override float OnSceneDistance { get; set; } = 90;
    internal override string CalloutName { get; set; } = "Transport Escape";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Prisoner escaped transport.";
        CalloutAdvisory = "Officers report a suspect has jumped out of a moving transport vehicle.";
        Functions.PlayScannerAudioUsingPosition("OFFICERS_REPORT_01 PYRO_PRISONTRANS_ESCAPE IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Escaped Prisoner",
            "DOC reports a prisoner has unlocked the transport vehicle and is on the run. ~r~CODE-3"
        );

        SpawnVehicle();
        SpawnOfficer();
        SpawnSuspect();
        CreateBlip();
    }

    private void SpawnVehicle()
    {
        _vehicle = new Vehicle("POLICET", SpawnPoint.Position) { IsPersistent = true };
        EntitiesToClear.Add(_vehicle);
    }

    private void SpawnOfficer()
    {
        _officer = new Ped("csb_cop", SpawnPoint.Position, 0f);
        _officer.IsPersistent = true;
        _officer.WarpIntoVehicle(_vehicle, -1);
        _officer.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_officer);
    }

    private void SpawnSuspect()
    {
        _suspect = new Ped("s_m_y_prisoner_01", SpawnPoint.Position, 0f);
        _suspect.IsPersistent = true;
        _suspect.SetWanted(false);
        _suspect.WarpIntoVehicle(_vehicle, 1);
        _suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
        CommonUtils.ClearSearchItems(_suspect);
        EntitiesToClear.Add(_suspect);
    }

    private void CreateBlip()
    {
        _blip = CommonUtils.CreateSearchBlip(SpawnPoint, Color.Red, true, false, 20f);
        BlipsToClear.Add(_blip);
    }

    internal override void CalloutOnScene()
    {
        _blip?.DisableRoute();

        if (!_officer || !_suspect)
        {
            CalloutEnd(true);
            return;
        }

        HandleScenario();
    }

    private void HandleScenario()
    {
        switch (new Random(DateTime.Now.Millisecond).Next(1, 3))
        {
            case 1: // Armed prisoner attacks officer
                LogUtils.Info("Callout Scene 1");
                CommonUtils.AddFirearmItem("Pistol", "weapon_pistol_mk2", true, true, true, _suspect);
                _suspect.Inventory.EquippedWeapon = "weapon_pistol_mk2";
                _suspect.Tasks.FightAgainst(_officer);
                _suspect.Health = 250;
                GameFiber.Wait(6000);
                if (_suspect.IsAlive)
                {
                    if (_officer.IsAlive)
                        _officer.Kill();
                    CommonUtils.StartPursuit(false, false, _suspect);
                }
                break;

            case 2: // Fleeing prisoner with officer in pursuit
                LogUtils.Info("Callout Scene 2");
                var pursuit = CommonUtils.StartPursuit(false, false, _suspect);
                Functions.AddCopToPursuit(pursuit, _officer);
                break;
        }
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        if (_vehicle && _officer && _officer.IsAlive)
            _officer.Tasks.EnterVehicle(_vehicle, -1);
        base.CalloutEnd(forceCleanup);
    }
}
