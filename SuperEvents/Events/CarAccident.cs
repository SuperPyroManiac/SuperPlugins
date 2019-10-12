using System;
using System.Drawing;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
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
        private string _name1;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperEvents", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new UIMenu("SuperEvents", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem _callFd = new UIMenuItem("~r~ Call Fire Department", "Calls for ambulance and firetruck.");
        private readonly UIMenuItem _questioning = new UIMenuItem("Speak With Subjects");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Call", "Ends the callout early.");
        
        private UIMenuItem _speakSuspect;

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
            _cVehicle2.Metadata.searchDriver = "~r~empty beer cans~s~, ~y~pocket knife~s~, ~g~a bucket full of wet socks~s~";
            _victim2.Metadata.searchPed = "~r~crushed beer can~s~, ~g~wallet~s~";
            _victim2.Metadata.stpAlcoholDetected = true;
            _name1 = Functions.GetPersonaForPed(_victim2).FullName;
            //Start UI
            _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
            _interaction.Add(_mainMenu);
            _interaction.Add(_convoMenu);
            _mainMenu.AddItem(_callFd);
            _mainMenu.AddItem(_questioning);
            _mainMenu.AddItem(_endCall);
            _convoMenu.AddItem(_speakSuspect);
            
            _mainMenu.RefreshIndex();
            _convoMenu.RefreshIndex();
            _mainMenu.BindMenuToItem(_convoMenu, _questioning);
            
            _mainMenu.OnItemSelect += Interactions;
            _convoMenu.OnItemSelect += Conversations;
            _callFd.SetLeftBadge(UIMenuItem.BadgeStyle.Alert);
            _convoMenu.ParentMenu = _mainMenu;
            _callFd.Enabled = false;
            _questioning.Enabled = false;
            //Blips
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
                        if (!_onScene && !_victim2.IsAnySpeechPlaying) _victim2.PlayAmbientSpeech("GENERIC_FRIGHTENED_MED");
                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 30f)
                        {
                            NativeFunction.CallByName<uint>("TASK_WRITHE", _victim1, _victim2, -1, 1000);
                            _onScene = true;
                            _questioning.Enabled = true;
                            _callFd.Enabled = true;
                            _victim1.BlockPermanentEvents = false;
                            _victim2.BlockPermanentEvents = false;
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~Car Accident", "Investigate the scene.");
                            NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _victim2, Game.LocalPlayer.Character, -1);
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");                        }
                        if (Game.IsKeyDown(Settings.Interact))
                        {
                            _mainMenu.Visible = !_mainMenu.Visible;
                            _convoMenu.Visible = false;
                        }
                        if (_victim2.IsCuffed || _victim2.IsDead || Game.LocalPlayer.Character.DistanceTo(_spawnPoint) > 200) End();
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
            if (_victim1.Exists()) _victim1.Dismiss();
            if (_cVehicle1.Exists()) _cVehicle1.Dismiss();
            if (_victim2.Exists()) _victim2.Dismiss();
            if (_cVehicle2.Exists()) _cVehicle2.Dismiss();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            base.End();
        }
        
                private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _callFd)
            {
                Game.DisplaySubtitle("~g~You~s~: Dispatch, we have an MVA. One person is seriously injured.");
                try
                {
                    UltimateBackup.API.Functions.callAmbulance();
                    UltimateBackup.API.Functions.callFireDepartment();
                }
                catch (Exception e)
                {
                    Game.LogTrivial("SuperEvents Warning: Ultimate Backup is not installed! Backup was not automatically called!");
                    Game.DisplayHelp("~r~Ultimate Backup is not installed! Backup was not automatically called!", 8000);
                }
                _callFd.Enabled = false;
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
                    Game.DisplaySubtitle("~g~You~s~: What happened? Are you ok?", 5000);
                    NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _victim2, Game.LocalPlayer.Character, -1);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: I can't remember, all I know is I was driving and now im here..", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: I need you to remember, someone is seriously hurt! Can you tell me anything?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: I don't like you.. I'm going home.", 5000);
                    _victim2.Tasks.EnterVehicle(_cVehicle2, -1);
                    _victim2.BlockPermanentEvents = true;
                });
            }
        }
    }
}