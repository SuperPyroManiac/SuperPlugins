#region

using System;
using System.Drawing;
using LSPD_First_Response.Engine;
using Rage;

#endregion

namespace SuperEvents.Events
{
    public class SuicidalPed : AmbientEvent
    {
        private Ped _bad1;
        private Ped _bad2;
        private Vector3 _spawnPoint;
        private Blip _cBlip1;
        private Blip _cBlip2;
        private bool _onScene;

        internal static void Launch()
        {
            var eventBooter = new SuicidalPed();
            eventBooter.StartEvent();
        }

        protected override void StartEvent()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(45f, 100f));
            _bad1 = new Ped(_spawnPoint) {IsPersistent = true, BlockPermanentEvents = true};
            _bad2 = new Ped(_bad1.GetOffsetPositionFront(2f)) {IsPersistent = true, BlockPermanentEvents = true};
            if (_bad1.IsDead || _bad1.RelationshipGroup == "COP" || _bad2.IsDead || _bad2.RelationshipGroup == "COP") {base.Failed(); return;}
            _bad1.Tasks.PutHandsUp(-1, _bad2);
            _bad2.Tasks.PutHandsUp(-1, _bad1);
            if (Settings.ShowBlips)
            {
                _cBlip1 = _bad1.AttachBlip();
                _cBlip1.Color = Color.Red;
                _cBlip1.Scale = .5f;
                _cBlip2 = _bad2.AttachBlip();
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

                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 10f)
                        {
                            _onScene = true;
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~People in Road", "Investigate the people.");
                            Game.DisplaySubtitle(
                                "~r~Stangers: ~s~Run us over! We do not want to live on this world anymore!");
                        }

                        if (_bad1.Exists() && _bad2.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(_bad1) > 200f ||
                                Game.LocalPlayer.Character.DistanceTo(_bad2) > 200f) End();
                            if (_bad1.IsDead || _bad2.IsDead || _bad1.IsCuffed || _bad2.IsCuffed) End();
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
            if(_bad2.Exists()) _bad2.Dismiss();
            if(_cBlip1.Exists()) _cBlip1.Delete();
            if(_cBlip2.Exists()) _cBlip2.Delete();
            base.End();
        }
    }
}