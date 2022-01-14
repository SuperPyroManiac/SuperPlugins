using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts
{
    [CalloutInfo("BlockingTraffic", CalloutProbability.Medium)]
    class BlockingTraffic : Callout
    {
        
        #region Variables
        private Vehicle _cVehicle;
        private Blip _cBlip;
        private Vector3 _spawnPoint;
        private bool _onScene;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperCallouts", "~y~Choose an option.");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Callout", "Ends the callout.");
        #endregion

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(450f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~b~Dispatch:~s~ Reports of a vehicle blocking traffic.";
            CalloutAdvisory = "Caller says the vehicle is abandoned in the middle of the road.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_11_351_01 IN_OR_ON_POSITION",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: car blocking traffic callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Blocking Traffic",
                "Reports of a car blocking the road, respond ~y~CODE-2");
            //cVehicle
            CFunctions.SpawnNormalCar(out _cVehicle, _spawnPoint);
            //Start UI
            _mainMenu.MouseControlsEnabled = false;
            _mainMenu.AllowCameraMovement = true;
            _interaction.Add(_mainMenu);
            _mainMenu.AddItem(_endCall);
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
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
                    Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
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
                Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
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
            _cVehicle?.Dismiss();
            _cBlip?.Delete();
            _mainMenu.Visible = false;
            CFunctions.Code4Message();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
        
        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }
    }
}