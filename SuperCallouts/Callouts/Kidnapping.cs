#region

using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using SuperCallouts.CustomScenes;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("Kidnapping", CalloutProbability.Medium)]
    internal class Kidnapping : Callout
    {
        private Ped _bad;
        private Blip _cBlip;
        private Blip _cBlip2;
        private Vehicle _cVehicle;
        private bool _onScene;
        private LHandle _pursuit;
        private readonly Random _rNd = new Random();
        private Vector3 _spawnPoint;
        private Ped _victim;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(450f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~o~Traffic ANPR Report:~s~ Vehicle involved in kidnapping spotted.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_03 CRIME_2_42_01 IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Kidnapping callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch",
                "~r~Possible Missing Person Found",
                "ANPR has spotted a vehicle that was involved in a kidnapping last month. Respond ~r~CODE-3");
            SimpleFunctions.SpawnNormalCar(out _cVehicle, _spawnPoint);
            _cVehicle.IsPersistent = true;
            _bad = _cVehicle.CreateRandomDriver();
            _bad.IsPersistent = true;
            _bad.Inventory.Weapons.Add(WeaponHash.APPistol).Ammo = -1;
            _victim = new Ped();
            _victim.IsPersistent = true;
            _victim.WarpIntoVehicle(_cVehicle, 0);
            _bad.Tasks.CruiseWithVehicle(30f, VehicleDrivingFlags.Emergency);
            _cBlip = _bad.AttachBlip();
            _cBlip.EnableRoute(Color.Red);
            _cBlip.Color = Color.Red;
            _cBlip.Scale = .75f;
            Game.DisplaySubtitle("Get to the ~r~scene~w~!", 10000);
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_bad.Position) <= 30f)
            {
                _onScene = true;
                _cBlip.Delete();
                _pursuit = Functions.CreatePursuit();
                Game.DisplayHelp("Stop the driver!", 5000);
                var choices = _rNd.Next(1, 5);
                switch (choices)
                {
                    case 1:
                        GameFiber.StartNew(delegate
                        {
                            GameFiber.Wait(10000);
                            _victim.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.BailOut);
                            _cBlip2 = _victim.AttachBlip();
                            _cBlip2.Color = Color.DeepSkyBlue;
                            _cBlip2.Scale = .75f;
                            GameFiber.Wait(4000);
                            _victim.Tasks.Cower(-1);
                            Functions.AddPedToPursuit(_pursuit, _bad);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        });
                        break;
                    case 2:
                        GameFiber.StartNew(delegate
                        {
                            Functions.AddPedToPursuit(_pursuit, _bad);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            GameFiber.Wait(60000);
                            _victim.Kill();
                            _bad.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.BailOut);
                            GameFiber.Wait(4000);
                            _bad.Tasks.ReactAndFlee(Game.LocalPlayer.Character);
                        });
                        break;
                    case 3:
                        GameFiber.StartNew(delegate
                        {
                            Functions.AddPedToPursuit(_pursuit, _bad);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            GameFiber.Wait(70000);
                            _bad.Tasks.PerformDrivingManeuver(VehicleManeuver.GoForwardStraightBraking);
                            GameFiber.Wait(1000);
                            _bad.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                            GameFiber.Wait(4000);
                            Game.DisplaySubtitle("~r~Suspect: ~w~I refuse to go to prison!");
                            _bad.Tasks.FireWeaponAt(Game.LocalPlayer.Character, -1, FiringPattern.BurstFirePistol);
                            _victim.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                            _victim.Tasks.Cower(-1);
                        });
                        break;
                    case 4:
                        GameFiber.StartNew(delegate
                        {
                            Functions.AddPedToPursuit(_pursuit, _bad);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        });
                        break;
                    default:
                        Game.DisplayNotification(
                            "An error has been detected! Ending callout early to prevent LSPDFR crash!");
                        End();
                        break;
                }
            }

            if (_onScene && !Functions.IsPursuitStillRunning(_pursuit) || _onScene && _bad.IsDead) End();
            base.Process();
        }

        public override void End()
        {
            if (_bad.Exists()) _bad.Dismiss();
            if (_victim.Exists()) _victim.Dismiss();
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}