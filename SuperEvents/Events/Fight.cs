#region

using System;
using System.Drawing;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
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
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperEvents", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new UIMenu("SuperEvents", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem _stopFight = new UIMenuItem("~r~Stop Fighting", "Breaks up the fight.");
        private readonly UIMenuItem _questioning = new UIMenuItem("Speak With Subjects");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Call", "Ends the callout early.");
        private readonly UIMenuItem _speakSuspect = new UIMenuItem("Speak with the ~r~Suspect");
        private readonly UIMenuItem _speakSuspect2 = new UIMenuItem("Speak with the ~b~Victim");
        private readonly UIMenuItem _goBack = new UIMenuItem("Back", "Returns to main menu.");

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
            //Start UI
            _interaction.Add(_mainMenu);
            _interaction.Add(_convoMenu);
            _mainMenu.AddItem(_stopFight);
            _mainMenu.AddItem(_questioning);
            _mainMenu.AddItem(_endCall);
            _convoMenu.AddItem(_speakSuspect);
            _convoMenu.AddItem(_speakSuspect2);
            _convoMenu.AddItem(_goBack);
            _mainMenu.RefreshIndex();
            _convoMenu.RefreshIndex();
            _mainMenu.BindMenuToItem(_convoMenu, _questioning);
            _convoMenu.BindMenuToItem(_mainMenu, _goBack);
            _mainMenu.OnItemSelect += Interactions;
            _convoMenu.OnItemSelect += Conversations;
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
                            Game.DisplayHelp("Press " + Settings.Interact + " to open interaction menu.");
                        }
                        if (Game.IsKeyDown(Settings.Interact))
                        {
                            _mainMenu.Visible = !_mainMenu.Visible;
                            _convoMenu.Visible = false;
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
            _interaction.CloseAllMenus();
            base.End();
        }
        
        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _stopFight)
            {
                Game.DisplaySubtitle("~g~You~s~: Police! Stop fighting now!");
                _bad1.Tasks.ClearImmediately();
                _bad2.Tasks.ClearImmediately();
                NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad2, Game.LocalPlayer.Character, -1);
            }
            else if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Event Ended.");
                End();
            }
        }
        private void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _speakSuspect)
            {
                Game.DisplaySubtitle("~r~Suspect:~s~ Wowee");
            }
            if (selItem == _speakSuspect2)
            {
                Game.DisplaySubtitle("~b~Victim:~s~ Wowee");
                _speakSuspect.SetRightLabel("Suspect is dead.");
                _speakSuspect.SetLeftBadge(UIMenuItem.BadgeStyle.Alert);
                _speakSuspect.Enabled = false;
            }
        }
    }
}