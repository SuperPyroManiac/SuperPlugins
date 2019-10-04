using System;
using System.Drawing;
using Rage;
using SuperEvents.SimpleFunctions;

namespace SuperEvents.Events
{
    public class CarFire : AmbientEvent
    {
        private Ped _victim;
        private Blip _cBlip;
        private Vehicle _cVehicle;
        private bool _onScene;
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        internal static void Launch()
        {
            var eventBooter = new CarFire();
            eventBooter.StartEvent();
        }
        protected override void StartEvent()
        {
            EFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
            if (_spawnPoint.DistanceTo(Game.LocalPlayer.Character) < 35f) {base.Failed(); return;}
            EFunctions.SpawnAnyCar(out _cVehicle, _spawnPoint);
            EFunctions.Damage(_cVehicle, 200, 200);
            for (var i = 0; i < 5; i++) EFunctions.FireControl(_spawnPoint.Around2D(1f,5f), 24, true);
            _victim = _cVehicle.CreateRandomDriver();
            _victim.IsPersistent = true;
            _victim.Kill();
            if (!Settings.ShowBlips) {base.StartEvent(); return;}
            _cBlip = _cVehicle.AttachBlip();
            _cBlip.Color = Color.Red;
            _cBlip.Scale = .5f;
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
                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 30f)
                        {
                            _onScene = true;
                            _cVehicle.IsOnFire = true;
                            for (var i = 0; i < 10; i++)
                                EFunctions.FireControl(_spawnPoint.Around2D(1f, 5f), 24, false);
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~Car Fire", "Clear the scene.");
                        }

                        if (_cVehicle.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(_cVehicle) > 200) End();
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
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_victim.Exists()) _victim.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            base.End();
        }

    }
}
