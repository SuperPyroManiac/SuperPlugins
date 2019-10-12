#region

using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;

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
        private string _name1;
        private string _name2;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperEvents", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new UIMenu("SuperEvents", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem _callCode2 =
            new UIMenuItem("~r~ Code 2 Backup", "Calls another officer to help out.");
        private readonly UIMenuItem _questioning = new UIMenuItem("Speak With Subjects");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Call", "Ends the callout early.");
        private UIMenuItem _speakSuspect;
        private UIMenuItem _speakSuspect2;

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
            if (_bad1.IsDead || _bad1.RelationshipGroup == "COP" || _bad2.IsDead || _bad2.RelationshipGroup == "COP")
            {
                base.Failed();
                return;
            }

            _bad1.Tasks.PutHandsUp(-1, _bad2);
            _bad2.Tasks.PutHandsUp(-1, _bad1);
            _name1 = Functions.GetPersonaForPed(_bad1).FullName;
            _name2 = Functions.GetPersonaForPed(_bad2).FullName;
            //Start UI
            _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
            _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name2);
            _interaction.Add(_mainMenu);
            _interaction.Add(_convoMenu);
            _mainMenu.AddItem(_callCode2);
            _mainMenu.AddItem(_questioning);
            _mainMenu.AddItem(_endCall);
            _convoMenu.AddItem(_speakSuspect);
            _convoMenu.AddItem(_speakSuspect2);
            _mainMenu.RefreshIndex();
            _convoMenu.RefreshIndex();
            _mainMenu.BindMenuToItem(_convoMenu, _questioning);
            _mainMenu.OnItemSelect += Interactions;
            _convoMenu.OnItemSelect += Conversations;
            _callCode2.SetLeftBadge(UIMenuItem.BadgeStyle.Alert);
            _convoMenu.ParentMenu = _mainMenu;
            _callCode2.Enabled = false;
            _questioning.Enabled = false;
            _speakSuspect2.Enabled = false;
            //Blips
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
                    try
                    {
                        GameFiber.Yield();
                        if (Game.IsKeyDown(Settings.EndEvent)) End();

                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 10f)
                        {
                            _onScene = true;
                            _questioning.Enabled = true;
                            _callCode2.Enabled = true;
                            if (Settings.ShowHints)
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~People in Road", "Investigate the people.");
                            Game.DisplaySubtitle(
                                "~r~Stangers: ~s~Run us over! We do not want to live on this world anymore!");
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                        }

                        if (Game.IsKeyDown(Settings.Interact))
                        {
                            _mainMenu.Visible = !_mainMenu.Visible;
                            _convoMenu.Visible = false;
                        }

                        if (_bad1.Exists() && _bad2.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(_bad1) > 200f ||
                                Game.LocalPlayer.Character.DistanceTo(_bad2) > 200f) End();
                            if (_bad1.IsDead || _bad2.IsDead) End();
                        }
                        else
                        {
                            End();
                        }

                        _interaction.ProcessMenus();
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

        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _callCode2)
            {
                Game.DisplaySubtitle("~g~You~s~: Dispatch, can I get some asistance here?");
                _bad1.Tasks.ClearImmediately();
                _bad2.Tasks.ClearImmediately();
                NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad2, Game.LocalPlayer.Character, -1);
                try
                {
                    UltimateBackup.API.Functions.callCode2Backup();
                }
                catch (Exception e)
                {
                    Game.LogTrivial(
                        "SuperEvents Warning: Ultimate Backup is not installed! Backup was not automatically called!");
                    Game.DisplayHelp("~r~Ultimate Backup is not installed! Backup was not automatically called!", 8000);
                }

                _callCode2.Enabled = false;
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
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle(
                        "~g~You~s~: Why are you guys standing in the middle of the road? I need you to move to the sidewalk.",
                        5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: We don't want to live anymore. Please just shoot me!'",
                        5000);
                    _speakSuspect2.Enabled = true;
                });
            }
            else if (selItem == _speakSuspect2)
            {
                Game.DisplaySubtitle("~g~You~s~: Sir, can you explain what's going on here?'", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle("~r~" + _name2 + "~s~: I CANT TAKE IT ANYMORE!", 5000);
                GameFiber.Wait(1000);
                _bad2.Tasks.FightAgainst(Game.LocalPlayer.Character);
            }
        }
    }
}