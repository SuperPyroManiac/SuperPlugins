using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
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
        private string _name1;
        private string _name2;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperEvents", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new UIMenu("SuperEvents", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem _callEms = new UIMenuItem("~r~ Call EMS", "Calls in an ambulance.");
        private readonly UIMenuItem _questioning = new UIMenuItem("Speak With Subjects");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Call", "Ends the callout early.");
        private readonly UIMenuItem _goBack = new UIMenuItem("Back", "Returns to main menu.");
        private UIMenuItem _speakSuspect;

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
            _name1 = Functions.GetPersonaForPed(_bad1).FullName;
            _name2 = Functions.GetPersonaForPed(_bad2).FullName;
            _bad2.Metadata.searchPed = "~r~empty bag with powder on it~s~, ~r~used meth pipe~s~, ~g~picture of son~s~";
            //Start UI
            _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
            _interaction.Add(_mainMenu);
            _interaction.Add(_convoMenu);
            _mainMenu.AddItem(_callEms);
            _mainMenu.AddItem(_questioning);
            _mainMenu.AddItem(_endCall);
            _convoMenu.AddItem(_speakSuspect);
            _convoMenu.AddItem(_goBack);
            _mainMenu.RefreshIndex();
            _convoMenu.RefreshIndex();
            _mainMenu.BindMenuToItem(_convoMenu, _questioning);
            _convoMenu.BindMenuToItem(_mainMenu, _goBack);
            _mainMenu.OnItemSelect += Interactions;
            _convoMenu.OnItemSelect += Conversations;
            _callEms.SetLeftBadge(UIMenuItem.BadgeStyle.Alert);
            _convoMenu.ParentMenu = _mainMenu;
            _callEms.Enabled = false;
            _questioning.Enabled = false;
            //Blips
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
                            _questioning.Enabled = true;
                            _callEms.Enabled = true;
                            _cBlip2.Delete();
                            _bad2.IsRagdoll = false;
                            _bad2.Kill();
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~A Medical Emergency", "Help the person. Call EMS or perform CPR.");
                            Game.DisplayHelp("Press " + Settings.Interact + " to open interaction menu.");
                        }
                        if (Game.IsKeyDown(Settings.Interact))
                        {
                            _mainMenu.Visible = !_mainMenu.Visible;
                            _convoMenu.Visible = false;
                        }
                        if (_onScene && !_bad2.Exists())
                        {
                            _bad1.Tasks.Clear();
                            End();
                        }
                        if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) > 120) End();
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
            if (selItem == _callEms)
            {
                Game.DisplaySubtitle("~g~You~s~: Dispatch, we have a person unconsious on the ground. Send me medical!");
                _bad1.Tasks.ClearImmediately();
                NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                try
                {
                    UltimateBackup.API.Functions.callAmbulance();
                }
                catch (Exception e)
                {
                    Game.LogTrivial("SuperEvents Warning: Ultimate Backup is not installed! Backup was not automatically called!");
                    Game.DisplayHelp("~r~Ultimate Backup is not installed! Backup was not automatically called!", 8000);
                }
                _callEms.Enabled = false;
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
                    Game.DisplaySubtitle("~g~You~s~: What happened here?", 5000);
                    NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, _bad2, -1);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~y~" + _name1 + "~s~: I don't know! we are friends, we were walking and they just fell over! Their name is ~y~" + _name2, 5000);
                });
            }
        }
    }
}