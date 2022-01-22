using System;
using System.Drawing;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts
{
    [CalloutInfo("CarAccident2", CalloutProbability.Medium)]
    internal class CarAccident2 : Callout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(45f, 320f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~b~Dispatch:~s~ Reports of a motor vehicle accident.";
            CalloutAdvisory = "Caller reports their is multiple vehicles involved.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "CITIZENS_REPORT_04 CRIME_HIT_AND_RUN_03 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: car accident callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~MVA",
                "Reports of a car accident, respond ~r~CODE-3");
            //cVehicle1
            CFunctions.SpawnNormalCar(out _cVehicle1, _spawnPoint);
            _cVehicle1.EngineHealth = 0;
            _spawnPointoffset = _cVehicle1.GetOffsetPosition(new Vector3(0, 7.0f, 0));
            CFunctions.Damage(_cVehicle1, 200, 200);
            //cVehicle2
            CFunctions.SpawnNormalCar(out _cVehicle2, _spawnPointoffset);
            _cVehicle2.EngineHealth = 0;
            _cVehicle2.Rotation = new Rotator(0f, 0f, 180f);
            CFunctions.Damage(_cVehicle2, 200, 200);
            _cVehicle2.Metadata.searchDriver =
                "~r~half full hard liqure bottle~s~, ~y~pack of lighters~s~, ~g~coke cans~s~, ~g~cigarettes~s~";
            //Victim1
            _victim1 = _cVehicle1.CreateRandomDriver();
            _victim1.IsPersistent = true;
            _victim1.BlockPermanentEvents = true;
            _victim1.Tasks.LeaveVehicle(_cVehicle1, LeaveVehicleFlags.LeaveDoorOpen);
            CFunctions.SetAnimation(_victim1, "move_injured_ground");
            //Victim2
            _victim2 = _cVehicle2.CreateRandomDriver();
            _victim2.IsPersistent = true;
            _victim2.BlockPermanentEvents = true;
            _victim2.Tasks.LeaveVehicle(_cVehicle2, LeaveVehicleFlags.LeaveDoorOpen);
            CFunctions.SetDrunk(_victim2, true);
            _victim2.Metadata.searchPed = "~r~crushed beer can~s~, ~g~wallet~s~";
            _victim2.Metadata.stpAlcoholDetected = true;
            _name1 = Functions.GetPersonaForPed(_victim2).FullName;
            //Start UI
            _mainMenu.MouseControlsEnabled = false;
            _mainMenu.AllowCameraMovement = true;
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
            _callFd.LeftBadge = UIMenuItem.BadgeStyle.Alert;
            _convoMenu.ParentMenu = _mainMenu;
            _callFd.Enabled = false;
            _questioning.Enabled = false;
            //Blips
            _cBlip1 = _victim1.AttachBlip();
            _cBlip1.Color = Color.Red;
            _cBlip1.EnableRoute(Color.Red);
            _cBlip2 = _victim2.AttachBlip();
            _cBlip2.Color = Color.Red;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            try
            {
                //GamePlay
                if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle1) < 25f)
                {
                    _onScene = true;
                    _cBlip1.DisableRoute();
                    _questioning.Enabled = true;
                    _callFd.Enabled = true;
                    NativeFunction.Natives.xCDDC2B77CE54AC6E(_victim1, _victim2, -1, 1000); //TASK_WRITHE
                    NativeFunction.Natives.x5AD23D40115353AC(_victim2, Game.LocalPlayer.Character, -1);
                    _victim1.BlockPermanentEvents = false;
                    _victim2.BlockPermanentEvents = false;
                    Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                }

                //Keybinds
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
                _interaction.ProcessMenus();
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
                Game.LogTrivial("SuperCallouts Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperCallouts Error Report End");
                End();
            }

            base.Process();
        }

        public override void End()
        {
            if (_victim1.Exists()) _victim1.Dismiss();
            if (_cVehicle1.Exists()) _cVehicle1.Dismiss();
            if (_victim2.Exists()) _victim2.Dismiss();
            if (_cVehicle2.Exists()) _cVehicle2.Dismiss();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            _mainMenu.Visible = false;
            CFunctions.Code4Message();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }

        //UI Items
        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _callFd)
            {
                Game.DisplaySubtitle("~g~You~s~: Dispatch, we have an MVA. One person is seriously injured.");
                if (Main.UsingUb)
                {
                    Wrapper.CallEms();
                    Wrapper.CallFd();
                }
                else
                {
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                        EBackupUnitType.Ambulance);
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                        EBackupUnitType.Firetruck);
                }

                _callFd.Enabled = false;
            }
            else if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }

        private void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _speakSuspect)
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: What happened? Are you ok?", 5000);
                    NativeFunction.Natives.x5AD23D40115353AC(_victim2,
                        Game.LocalPlayer.Character, -1);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name1 + "~s~: Who are you? I don't have to talk to you!'", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~g~You~s~: I'm a police officer, I need you to tell me what happened, someone is really hurt!'",
                        5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: No!", 5000);
                    _victim2.Tasks.EnterVehicle(_cVehicle2, -1);
                    _victim2.BlockPermanentEvents = true;
                });
        }

        #region Variables

        private Ped _victim1;
        private Ped _victim2;
        private Vehicle _cVehicle1;
        private Vehicle _cVehicle2;
        private Blip _cBlip1;
        private Blip _cBlip2;
        private Vector3 _spawnPoint;
        private Vector3 _spawnPointoffset;
        private bool _onScene;

        private string _name1;

        //UI Items
        private readonly MenuPool _interaction = new();
        private readonly UIMenu _mainMenu = new("SuperCallouts", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new("SuperCallouts", "~y~Choose a subject to speak with.");

        private readonly UIMenuItem _callFd = new("~r~ Call Fire Department", "Calls for ambulance and firetruck.");

        private readonly UIMenuItem _questioning = new("Speak With Subjects");
        private readonly UIMenuItem _endCall = new("~y~End Callout", "Ends the event early.");
        private UIMenuItem _speakSuspect;

        #endregion
    }
}