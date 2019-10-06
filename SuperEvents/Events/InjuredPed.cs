using System;
using System.Drawing;
using Rage;
using Rage.Native;
using SuperEvents.SimpleFunctions;

namespace SuperEvents.Events
{
    public class InjuredPed : AmbientEvent
    {
        private Ped _bad1;
        private Ped _bad2;
        private Blip _cBlip1;
        private Blip _cBlip2;
        private bool _onScene;
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        private bool _letsChat;

        internal static void Launch()
        {
            var eventBooter = new InjuredPed();
            eventBooter.StartEvent();
        }

        protected override void StartEvent()
        {
            EFunctions.FindSideOfRoad(100, 45, out _spawnPoint, out _spawnPointH);
            if (_spawnPoint.DistanceTo(Game.LocalPlayer.Character) < 35f) {base.Failed(); return;}
            _bad1 = new Ped(_spawnPoint) {Heading = _spawnPointH, IsPersistent = true, Health = 400};
            _bad2 = new Ped(_bad1.GetOffsetPositionFront(2)) {IsPersistent = true, Health = 400};
            if (!_bad1.Exists() || !_bad2.Exists()) {base.Failed(); return;}
            _bad1.Tasks.Cower(-1);
            EFunctions.SetAnimation(_bad2, "move_injured_ground");
            _bad2.IsRagdoll = true;
            if (!Settings.ShowBlips) {base.StartEvent(); return;}
            _cBlip1 = _bad1.AttachBlip();
            _cBlip1.Color = Color.Red;
            _cBlip1.Scale = .5f;
            _cBlip2 = _bad2.AttachBlip();
            _cBlip2.Color = Color.Red;
            _cBlip2.Scale = .5f;
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
                        if (!_onScene && !_bad1.IsAnySpeechPlaying) _bad1.PlayAmbientSpeech("GENERIC_FRIGHTENED_MED");
                        if (!_onScene && !_bad2.IsAnySpeechPlaying) _bad2.PlayAmbientSpeech("GENERIC_WAR_CRY");
                        if (Game.IsKeyDown(Settings.EndEvent)) End();
                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 20f)
                        {
                            _onScene = true;
                            _cBlip2.Delete();
                            _bad2.IsRagdoll = false;
                            _bad2.Kill();
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~A Medical Emergency", "Help the person. Call EMS or perform CPR.");
                            Game.DisplayHelp("Press " + Settings.Interact + " to speak with the bystander.");
                        }
                        if (_onScene && !_letsChat && Game.IsKeyDown(Settings.Interact))
                        {
                            _letsChat = true;
                            NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                            Game.DisplaySubtitle("~g~Me: ~s~What happened here?", 5000);
                            GameFiber.Wait(5000);
                            Game.DisplaySubtitle("~y~Bystander: ~s~We were just walking and they just fell to the ground! Please help!");
                        }
                        if (_onScene && !_bad2.Exists())
                        {
                            _bad1.Tasks.Clear();
                            End();
                        }
                        if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) > 120) End();
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
            if (_bad1.Exists()) _bad1.Dismiss();
            if (_bad2.Exists()) _bad2.Dismiss();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            base.End();
        }
    }
}