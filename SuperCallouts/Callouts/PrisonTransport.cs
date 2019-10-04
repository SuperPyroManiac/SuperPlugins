#region

using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("PrisonTransport", CalloutProbability.High)]
    internal class PrisonTransport : Callout
    {
        private Ped _badguy;
        private Blip _cBlip1;
        private Blip _cBlip2;
        private Ped _cop;
        private Vehicle _cVehicle;
        private bool _onScene;
        private LHandle _pursuit;
        private readonly Random _rNd = new Random();
        private Vector3 _spawnPoint;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 50f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~b~Dispatch:~s~ Prisoner escaped transport.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("OFFICERS_REPORT_01 CRIME_SUSPECT_ON_THE_RUN_01 IN_OR_ON_POSITION",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Prison Truck callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Escaped Prisoner",
                "DOC reports a prisoner has unlocked the transport vehicle and is on the run. Respond ~r~CODE-3");
            _cVehicle = new Vehicle("POLICET", _spawnPoint);
            _cVehicle.IsPersistent = true;
            _cop = new Ped("csb_cop", _spawnPoint, 0f);
            _cop.WarpIntoVehicle(_cVehicle, -1);
            _badguy = new Ped("s_m_y_prisoner_01", _spawnPoint, 0f);
            _badguy.WarpIntoVehicle(_cVehicle, 1);
            _cBlip1 = _cVehicle.AttachBlip();
            _cBlip1.EnableRoute(Color.Red);
            _cBlip1.Color = Color.Red;
            _badguy.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
            _cop.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 90f)
            {
                _onScene = true;
                _cBlip1.Delete();
                _cBlip2 = _badguy.AttachBlip();
                _cBlip2.Color = Color.Red;
                _pursuit = Functions.CreatePursuit();
                var choices = _rNd.Next(1, 3);
                switch (choices)
                {
                    case 1:
                        GameFiber.StartNew(delegate
                        {
                            _badguy.Inventory.Weapons.Add(WeaponHash.MicroSMG).Ammo = -1;
                            _badguy.Tasks.FightAgainst(_cop);
                            _badguy.Health = 250;
                            GameFiber.Wait(6000);
                            if (_badguy.IsAlive)
                            {
                                Functions.AddPedToPursuit(_pursuit, _badguy);
                                Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                                if (_cop.IsAlive) _cop.Kill();
                            }
                        });
                        break;
                    case 2:
                        Functions.AddPedToPursuit(_pursuit, _badguy);
                        Functions.AddCopToPursuit(_pursuit, _cop);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        break;
                    default:
                        Game.DisplayNotification(
                            "An error has been detected! Ending callout early to prevent LSPDFR crash!");
                        End();
                        break;
                }
            }

            if (_onScene && !Functions.IsPursuitStillRunning(_pursuit)) End();
            base.Process();
        }

        public override void End()
        {
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_badguy.Exists()) _badguy.Dismiss();
            if (_cop.Exists()) _cop.Delete();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            base.End();
        }
    }
}