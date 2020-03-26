using System;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using LSPD_First_Response;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts2.SimpleFunctions;

namespace SuperCallouts2.Callouts
{
    [CalloutInfo("Fire", CalloutProbability.Medium)]
    internal class Fire : Callout
    {
        #region Variables
        private Blip _cBlip;
        private Vehicle _cVehicle;
        private bool _onScene;
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperCallouts", "~y~Choose an option.");
        private readonly UIMenuItem _callFd =
            new UIMenuItem("~r~ Call Fire Department", "Calls for a firetruck.");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Callout", "Ends the callout early.");
        #endregion
        
        public override bool OnBeforeCalloutDisplayed()
        {
            CFunctions.FindSideOfRoad(750, 280, out _spawnPoint, out _spawnPointH);
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~b~Dispatch:~s~ Reports of a car fire";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: fire callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Fire",
                "Reports of a car fire, respond ~r~CODE-3");
            //cVehicle
            CFunctions.SpawnAnyCar(out _cVehicle, _spawnPoint);
            _cVehicle.Heading = _spawnPointH;
            //Start UI
            _interaction.Add(_mainMenu);
            _mainMenu.AddItem(_callFd);
            _mainMenu.AddItem(_endCall);
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
            _callFd.SetLeftBadge(UIMenuItem.BadgeStyle.Alert);
            _callFd.Enabled = false;
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
                    _cBlip.DisableRoute();
                    _callFd.Enabled = true;
                    for (var i = 0; i < 5; i++) CFunctions.FireControl(_spawnPoint.Around2D(1f, 5f), 24, true);
                    for (var i = 0; i < 10; i++) CFunctions.FireControl(_spawnPoint.Around2D(1f, 5f), 24, false);
                    Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                }
                //Keybinds
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.IsKeyDown(Settings.Interact))
                {
                    _mainMenu.Visible = !_mainMenu.Visible;
                }
                _interaction.ProcessMenus();

            }
            catch (Exception e)
            {
                        Game.LogTrivial("Oops there was an error here. Please send this log to SuperPyroManiac!");
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
            if (_cBlip.Exists()) _cBlip.Delete();
            _mainMenu.Visible = false;
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _callFd)
            {
                Game.DisplaySubtitle(
                    "~g~You~s~: Dispatch, we have a large vehicle fire that's spreading. We need a fire crew here ASAP!");
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3, EBackupUnitType.Firetruck);
                _callFd.Enabled = false;
            }
            else if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }
    }
}