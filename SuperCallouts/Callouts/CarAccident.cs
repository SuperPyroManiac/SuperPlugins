#region

using System;
using System.Drawing;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("CarAccident", CalloutProbability.Medium)]
    internal class CarAccident : Callout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CFunctions.FindSideOfRoad(750, 280, out _spawnPoint, out _spawnPointH);
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~b~Dispatch:~s~ Reports of a motor vehicle accident.";
            CalloutAdvisory = "Caller reports possible hit and run.";
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
            if (Main.UsingCi) Wrapper.StartCi(this, "10-50");
            //cVehicle
            CFunctions.SpawnAnyCar(out _cVehicle, _spawnPoint);
            _cVehicle.Heading = _spawnPointH;
            CFunctions.Damage(_cVehicle, 200, 200);
            //cVictim
            _cVictim = _cVehicle.CreateRandomDriver();
            _cVictim.IsPersistent = true;
            _cVictim.Kill();
            //Start UI
            _mainMenu.MouseControlsEnabled = false;
            _mainMenu.AllowCameraMovement = true;
            _interaction.Add(_mainMenu);
            _mainMenu.AddItem(_callEms);
            _mainMenu.AddItem(_endCall);
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
            _callEms.LeftBadge = UIMenuItem.BadgeStyle.Alert;
            _callEms.Enabled = false;
            //cBlip
            _cBlip = _cVehicle.AttachBlip();
            _cBlip.Color = Color.Red;
            _cBlip.EnableRoute(Color.Red);
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            try
            {
                //GamePlay
                if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle) < 25f)
                {
                    _onScene = true;
                    if (Main.UsingCi) Wrapper.CiSendMessage(this, "Arriving on scene. 10-23");
                    _cBlip.DisableRoute();
                    _callEms.Enabled = true;
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
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_cVictim.Exists()) _cVictim.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            _mainMenu.Visible = false;
            CFunctions.Code4Message();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            if (Main.UsingCi) Wrapper.CiSendMessage(this, "Scene clear, Code4");
            base.End();
        }

        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _callEms)
            {
                Game.DisplaySubtitle(
                    "~g~You~s~: Dispatch, we have a vehicle accident, possible hit and run. Looks like someone is inside and injured! I need EMS out here.");
                if (Main.UsingCi)
                    Wrapper.CiSendMessage(this, "EMS has been notified and is on route. 11-78");
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

                _callEms.Enabled = false;
            }
            else if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }

        #region Variables

        private Ped _cVictim;
        private Vehicle _cVehicle;
        private Blip _cBlip;
        private Vector3 _spawnPoint;
        private float _spawnPointH;

        private bool _onScene;

        //UI Items
        private readonly MenuPool _interaction = new();
        private readonly UIMenu _mainMenu = new("SuperCallouts", "~y~Choose an option.");

        private readonly UIMenuItem _callEms = new("~r~ Call EMS", "Calls for an ambulance.");

        private readonly UIMenuItem _endCall = new("~y~End Callout", "Ends the callout early.");

        #endregion
    }
}