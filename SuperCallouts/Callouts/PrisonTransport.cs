#region
using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Transport Escape", CalloutProbability.Medium, "Prisoner escaped transport vehicle - high priority",
    "Code 3")]
internal class PrisonTransport : SuperCallout
{
    internal override Vector3 SpawnPoint { get; set; } = World.GetNextPositionOnStreet(Player.Position.Around(500f));
    internal override float OnSceneDistance { get; set; } = 90;
    internal override string CalloutName { get; set; } = "Transport Escape";
    private readonly Random _rNd = new();
    private Ped _badguy;
    private Blip _cBlip1;
    private Ped _cop;
    private Vehicle _cVehicle;

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Prisoner escaped transport.";
        CalloutAdvisory = "Officers report a suspect has jumped out of a moving transport vehicle.";
        Functions.PlayScannerAudioUsingPosition("OFFICERS_REPORT_01 CRIME_SUSPECT_ON_THE_RUN_01 IN_OR_ON_POSITION",
            SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Escaped Prisoner",
            "DOC reports a prisoner has unlocked the transport vehicle and is on the run. ~r~CODE-3");

        _cVehicle = new Vehicle("POLICET", SpawnPoint) { IsPersistent = true };
        EntitiesToClear.Add(_cVehicle);

        _cop = new Ped("csb_cop", SpawnPoint, 0f);
        _cop.IsPersistent = true;
        _cop.WarpIntoVehicle(_cVehicle, -1);
        _cop.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_cop);

        _badguy = new Ped("s_m_y_prisoner_01", SpawnPoint, 0f);
        _badguy.IsPersistent = true;
        _badguy.WarpIntoVehicle(_cVehicle, 1);
        _badguy.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_badguy);

        _cBlip1 = _cVehicle.AttachBlip();
        _cBlip1.EnableRoute(Color.Red);
        _cBlip1.Color = Color.Red;
        BlipsToClear.Add(_cBlip1);
    }

    internal override void CalloutOnScene()
    {
        _cBlip1.DisableRoute();
        var pursuit = Functions.CreatePursuit();
        var choices = _rNd.Next(1, 3);
        switch (choices)
        {
            case 1:
                _badguy.Inventory.Weapons.Add(WeaponHash.MicroSMG).Ammo = -1;
                _badguy.Tasks.FightAgainst(_cop);
                _badguy.Health = 250;
                GameFiber.Wait(6000);
                if (_badguy.IsAlive)
                {
                    Functions.AddPedToPursuit(pursuit, _badguy);
                    Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                    if (_cop.IsAlive) _cop.Kill();
                }
                break;
            case 2:
                Functions.AddPedToPursuit(pursuit, _badguy);
                Functions.AddCopToPursuit(pursuit, _cop);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;
            default:
                CalloutEnd(true);
                break;
        }
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        if (_cVehicle.Exists() && _cop.Exists() && _cop.IsAlive) _cop.Tasks.EnterVehicle(_cVehicle, -1);
        base.CalloutEnd(forceCleanup);
    }
}