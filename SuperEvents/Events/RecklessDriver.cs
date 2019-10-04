#region

using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using Rage;
using SuperEvents.SimpleFunctions;

#endregion

namespace SuperEvents.Events
{
    public class RecklessDriver : AmbientEvent
    {
        private Ped _bad1;
        private Vehicle _cVehicle;
        private LHandle _pursuit;
        private Blip _cBlip;
        private bool _pursuitActive;
        private bool _onScene;

        internal static void Launch()
        {
            var eventBooter = new RecklessDriver();
            eventBooter.StartEvent();
        }

        protected override void StartEvent()
        {
            var car = Game.LocalPlayer.Character.GetNearbyVehicles(15);
            if (car == null || car.Length == 0) {base.Failed(); return;}
            foreach (var cars in car)
            {
                if (!cars.Exists()) {base.Failed(); return;}
                if (cars.HasDriver) _cVehicle = cars;
            }
            if (!_cVehicle.Exists()) {base.Failed(); return;}
            _bad1 = _cVehicle.Driver;
            if (!_bad1.Exists()) {base.Failed(); return;}
            if (_bad1 == Game.LocalPlayer.Character || !_bad1.IsHuman || !_bad1.IsInAnyVehicle(true) || _bad1.IsDead || _bad1.RelationshipGroup == "COP") {base.Failed(); return;}
            _bad1.IsPersistent = true;
            _cVehicle.IsPersistent = true;
            _bad1.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, -1, true);
            _bad1.Tasks.CruiseWithVehicle(_cVehicle, 20f, VehicleDrivingFlags.Reverse);
            EFunctions.SetWanted(_bad1, true);
            _cVehicle.IsStolen = true;
            _bad1.Metadata.searchPed = "~r~baggy of marijuana~s~";
            if (Settings.ShowBlips)
            {
                _cBlip = _bad1.AttachBlip();
                _cBlip.Color = Color.Red;
                _cBlip.Scale = .5f;
            }
            base.StartEvent();
        }

        protected override void MainLogic()
        {
            GameFiber.StartNew(delegate
            {
                while (EventsActive)
                {
                    try
                    {
                        GameFiber.Yield();
                        if (Game.IsKeyDown(Settings.EndEvent)) End();

                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_bad1) < 30f)
                        {
                            _onScene = true;
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~Reckless Driving", "Stop the vehicle.");
                        }

                        if (_onScene && !_pursuitActive && Functions.IsPlayerPerformingPullover())
                        {
                            Functions.ForceEndCurrentPullover();
                            _pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(_pursuit, _bad1);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            _pursuitActive = true;
                        }

                        if (!_pursuitActive && !_bad1.IsInAnyVehicle(true)) End();

                        if (_pursuitActive && !Functions.IsPursuitStillRunning(_pursuit)) End();

                        if (_bad1.Exists())
                        {
                            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_bad1) > 300f) End();
                            if (_onScene && !_pursuitActive &&
                                Game.LocalPlayer.Character.DistanceTo(_bad1) > 300f) End();
                            if (_bad1.IsDead) End();
                            if (_bad1.IsCuffed) End();
                        }
                        else
                        {
                            End();
                        }
                    }
                    catch (Exception e)
                    {
                        Game.LogTrivial("Oops there was an error here. Please send this log to SuperPyroManiac!");
                        Game.LogTrivial("SuperEvents Error Report Start");
                        Game.LogTrivial("======================================================");
                        Game.LogTrivial(e.ToString());
                        Game.LogTrivial("======================================================");
                        Game.LogTrivial("SuperEvents Error Report End");
                        End();
                    }
                }
            });
            base.MainLogic();
        }

        protected override void End()
        {
            if(_bad1.Exists()) _bad1.Dismiss();
            if(_cVehicle.Exists()) _cVehicle.Dismiss();
            if(_cBlip.Exists()) _cBlip.Delete();
            base.End();
        }
    }
}