using System;
using System.Drawing;
using Rage;
using Rage.Native;
using SuperEvents.SimpleFunctions;

namespace SuperEvents.Events
{
    public class CarAccident : AmbientEvent
    {
        private Blip _cBlip1;
        private Blip _cBlip2;
        private Vehicle _cVehicle1;
        private Vehicle _cVehicle2;
        private bool _onScene;
        private Vector3 _spawnPoint;
        private Vector3 _spawnPointoffset;
        private Ped _victim1;
        private Ped _victim2;
        
        internal static void Launch()
        {
            var eventBooter = new CarAccident();
            eventBooter.StartEvent();
        }
        protected override void StartEvent()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(45f, 120f));
            if (_spawnPoint.DistanceTo(Game.LocalPlayer.Character) < 35f) {base.Failed(); return;}
            EFunctions.SpawnNormalCar(out _cVehicle1, _spawnPoint);
            _cVehicle1.EngineHealth = 0;
            _spawnPointoffset = _cVehicle1.GetOffsetPosition(new Vector3(0, 7.0f, 0));
            EFunctions.SpawnNormalCar(out _cVehicle2, _spawnPointoffset);
            _cVehicle2.EngineHealth = 0;
            _cVehicle2.Rotation = new Rotator(0f, 0f, 180f);
            EFunctions.Damage(_cVehicle1, 200, 200);
            EFunctions.Damage(_cVehicle2, 200, 200);
            _victim1 = _cVehicle1.CreateRandomDriver();
            _victim1.IsPersistent = true;
            _victim1.BlockPermanentEvents = true;
            _victim2 = _cVehicle2.CreateRandomDriver();
            _victim2.IsPersistent = true;
            _victim2.BlockPermanentEvents = true;
            _victim1.Tasks.LeaveVehicle(_cVehicle1, LeaveVehicleFlags.LeaveDoorOpen);
            _victim2.Tasks.LeaveVehicle(_cVehicle2, LeaveVehicleFlags.LeaveDoorOpen);
            EFunctions.SetAnimation(_victim1, "move_injured_ground");
            EFunctions.SetDrunk(_victim2, true);
            _victim1.Health = 200;
            _victim2.Health = 200;
            if (Settings.ShowBlips)
            {
                _cBlip1 = _victim1.AttachBlip();
                _cBlip1.Color = Color.Red;
                _cBlip1.Scale = .5f;
                _cBlip2 = _victim2.AttachBlip();
                _cBlip2.Color = Color.Red;
                _cBlip2.Scale = .5f;
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
                        if (!_onScene && !_victim1.IsAnySpeechPlaying) _victim1.PlayAmbientSpeech("GENERIC_WAR_CRY");
                        if (!_onScene && !_victim2.IsAnySpeechPlaying)
                            _victim2.PlayAmbientSpeech("GENERIC_FRIGHTENED_MED");
                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 30f)
                        {
                            NativeFunction.CallByName<uint>("TASK_WRITHE", _victim1, _victim2, -1, 1000);
                            _onScene = true;
                            _victim1.BlockPermanentEvents = false;
                            _victim2.BlockPermanentEvents = false;
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~Car Accident", "Investigate the scene.");
                        }

                        if (_victim2.IsCuffed || _victim2.IsDead) End();
                        if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) > 200) End();
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
            if (_victim1.Exists()) _victim1.Dismiss();
            if (_cVehicle1.Exists()) _cVehicle1.Dismiss();
            if (_victim2.Exists()) _victim2.Dismiss();
            if (_cVehicle2.Exists()) _cVehicle2.Dismiss();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            base.End();
        }
        
    }
}