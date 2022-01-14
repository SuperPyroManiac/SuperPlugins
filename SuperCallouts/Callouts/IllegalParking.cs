using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts
{
    
    [CalloutInfo("IllegalParking", CalloutProbability.Medium)]
    internal class IllegalParking : Callout
    {
        //private List<Tuple<Vector3, float>> _spawnPoints = new List<Tuple<Vector3, float>>
        //{
        //    Tuple.Create(new Vector3(5f, 5f, 5f), 360f)//TODO
        //};

        //private Tuple<Vector3, float> _chosenSpawnData;
        private Vector3 _spawnPoint;
        private Vehicle _cVehicle;
        private Blip _cBlip;
        private bool _onScene;
        private float _heading;
        //UI Items
        private MenuPool _interaction;
        private UIMenu _mainMenu;
        private UIMenu _convoMenu;
        private UIMenuItem _questioning;
        private UIMenuItem _endCall;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            //Spawnpoint Setup
            /*foreach (var tupe in _spawnPoints)
            {
                _chosenSpawnData = _spawnPoints.OrderBy(x => x.Item1.DistanceTo(Game.LocalPlayer.Character.Position))
                    .FirstOrDefault();
            }*/
            //_spawnPoint = _chosenSpawnData.Item1;
            //_heading = _chosenSpawnData.Item2; TODO
            //Setup
            CFunctions.FindSideOfRoad(750, 280, out _spawnPoint, out _heading);
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Reports of a vehicle parked illegally.";
            CalloutAdvisory = "Caller says a vehicle is parked on their property without permission.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: illegally parked car callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~y~Traffic",
                "Reports of an empty vehicle on private property, respond ~g~CODE-1");
            //cVehicle
            CFunctions.SpawnNormalCar(out _cVehicle, _spawnPoint, _heading);
            //Blip
            _cBlip = _cVehicle.AttachBlip();
            _cBlip.Color = Color.DodgerBlue;
            _cBlip.EnableRoute(Color.DodgerBlue);
            //UI Items
            CFunctions.BuildUi(out _interaction, out _mainMenu, out _convoMenu, out _questioning, out _endCall);
            _mainMenu.OnItemSelect += InteractionProcess;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            //GamePlay
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle) < 25f)
            {
                _onScene = true;
                _cBlip.DisableRoute();
                Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Investigate The Vehicle", "~y~Traffic",
                    "The vehicle appears abandoned. Decide how to deal with it.");
            }
            //Keybinds
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (Game.IsKeyDown(Settings.Interact))
            {
                _mainMenu.Visible = !_mainMenu.Visible;
            }
            _interaction.ProcessMenus();
            base.Process();
        }
        
        public override void End()
        {
            if (_cBlip) _cBlip.Delete();
            if (_cVehicle) _cVehicle.Dismiss();
                        BigMessageThread bigMessage = new BigMessageThread();
            bigMessage.MessageInstance.ShowColoredShard("Code 4", "Callout Ended", HudColor.Green, HudColor.Black,
                2);
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
        
        private void InteractionProcess(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }
    }
}