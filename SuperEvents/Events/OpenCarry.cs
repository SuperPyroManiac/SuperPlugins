#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SuperEvents.Events
{
    public class OpenCarry : AmbientEvent
    {
        private Ped _bad1;
        private Blip _cBlip;
        private bool _onScene;

        internal static void Launch()
        {
            var eventBooter = new OpenCarry();
            eventBooter.StartEvent();
        }

        protected override void StartEvent()
        {
            var bad = Game.LocalPlayer.Character.GetNearbyPeds(15);
            if (bad == null || bad.Length == 0) {base.Failed(); return;}
            foreach (var badguy in bad)
            {
                if (!badguy.Exists()) break;
                _bad1 = badguy;
            }

            if (_bad1 == Game.LocalPlayer.Character || !_bad1.IsHuman || _bad1.IsInAnyVehicle(true) || _bad1.IsDead || _bad1.RelationshipGroup == "COP") {base.Failed(); return;}
            _bad1.IsPersistent = true;
            _bad1.Inventory.GiveNewWeapon(WeaponHash.AdvancedRifle, -1, true);
            _bad1.Metadata.stpAlcoholDetected = true;
            _bad1.Metadata.hasGunPermit = false;
            _bad1.Metadata.searchPed = "~r~assault rifle~s~, ~y~pocket knife~s~, ~g~wallet~s~";
            _bad1.Tasks.Wander();
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

                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_bad1) < 10f)
                        {
                            _onScene = true;
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~Open Carry", "Investigate the person.");
                        }

                        if (_bad1.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(_bad1) > 200f) End();
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
            if(_cBlip.Exists()) _cBlip.Delete();
            base.End();
        }
    }
}