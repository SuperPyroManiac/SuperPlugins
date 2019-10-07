#region

using System;
using System.Drawing;
using Rage;
using Rage.Native;
using SuperEvents.SimpleFunctions;

#endregion

namespace SuperEvents.Events
{
    internal class Fight : AmbientEvent
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
            var eventBooter = new Fight();
            eventBooter.StartEvent();
        }

        protected override void StartEvent()
        {
            EFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
            if (_spawnPoint.DistanceTo(Game.LocalPlayer.Character) < 35f) {base.Failed(); return;}
            _bad1 = new Ped(_spawnPoint) {Heading = _spawnPointH, IsPersistent = true, Health = 400};
            _bad2 = new Ped(_bad1.GetOffsetPositionFront(2)) {IsPersistent = true, Health = 400};
            if (!_bad1.Exists() || !_bad2.Exists()) {base.Failed(); return;}
            _bad1.Tasks.PlayAnimation("misstrevor2ig_3", "point", 2f, AnimationFlags.SecondaryTask);
            _bad2.Metadata.searchPed = "~r~50 dollar bill~s~, ~y~empty baggy with white powder~s~, ~g~wallet~s~";
            _bad2.Metadata.stpDrugsDetected = true;
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
                        if (!_onScene && !_bad1.IsAnySpeechPlaying) _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                        if (!_onScene && !_bad2.IsAnySpeechPlaying) _bad2.PlayAmbientSpeech("GENERIC_CURSE_MED");
                        if (Game.IsKeyDown(Settings.EndEvent)) End();
                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 20f)
                        {
                            _onScene = true;
                            _bad1.Tasks.FightAgainst(_bad2);
                            _bad2.Tasks.FightAgainst(_bad1);
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~A Fight", "Stop the fight, and make sure everyone is ok.");
                            Game.DisplayHelp("Press " + Settings.Interact + " to speak with the suspects.");
                        }
                        if (_onScene && !_letsChat && Game.IsKeyDown(Settings.Interact))
                        {
                            _letsChat = true;
                            _bad1.Tasks.Clear();
                            _bad2.Tasks.Clear();
                            Game.DisplaySubtitle("~g~Me: ~s~Stop fighting now!", 5000);
                            GameFiber.Wait(5000);
                            NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                            NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad2, Game.LocalPlayer.Character, -1);
                            Game.DisplaySubtitle("~g~Me: ~s~What is going on here?", 5000);
                            GameFiber.Wait(5000);
                            NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, _bad2, -1);
                            _bad1.Tasks.PlayAnimation("misstrevor2ig_3", "point", 2f, AnimationFlags.SecondaryTask);
                            Game.DisplaySubtitle("~r~Suspect 1: ~s~I dropped ~r~$50~s~ dollars and they took it!");
                            GameFiber.Wait(5000);
                            Game.DisplaySubtitle("~r~Suspect 2: ~s~I did not! I don't have time for this.'");
                            _bad2.Tasks.Wander();
                            NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                        }
                        if (!_bad1.IsAlive || !_bad2.IsAlive || _bad1.IsCuffed || _bad2.IsCuffed || Game.LocalPlayer.Character.DistanceTo(_spawnPoint) > 200) End();
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