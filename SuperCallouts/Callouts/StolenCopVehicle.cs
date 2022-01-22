using System;
using System.Drawing;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts
{
    [CalloutInfo("StolenCopVehicle", CalloutProbability.Medium)]
    internal class StolenCopVehicle : Callout
    {
        private Ped _bad;
        private Vehicle _cVehicle;
        private Blip _cBlip;
        private LHandle _pursuit;
        private Vector3 _spawnPoint;
        private CState _state = CState.CheckDistance;
        //UI Items
        private MenuPool _interaction;
        private UIMenu _mainMenu;
        private UIMenu _convoMenu;
        private UIMenuItem _questioning;
        private UIMenuItem _endCall;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
            CalloutMessage = "~b~Dispatch:~s~ Stolen police vehicle.";
            CalloutAdvisory = "A suspect stole an officers vehicle during arrest.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_RESIST_ARREST IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: StolenCopCar callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Stolen Police Vehicle",
                "A suspect has stolen a police vehicle during his arrest. Respond ~r~CODE-3");
            //cVehicle
            Model[] vehicleModels = {"POLICE", "POLICE2", "POLICE3", "SHERIFF", "SHERIFF2"};
            _cVehicle = new Vehicle(vehicleModels[new Random().Next(vehicleModels.Length)], _spawnPoint) {IsPersistent = true, IsStolen = true, IsSirenOn = true, IsSirenSilent = true};
            //Bad
            _bad = new Ped(_spawnPoint.Around(15f));
            _bad.WarpIntoVehicle(_cVehicle, -1);
            _bad.IsPersistent = true;
            _bad.BlockPermanentEvents = true;
            _bad.Metadata.stpDrugsDetected = true;
            _bad.Metadata.stpAlcoholDetected = true;
            CFunctions.SetWanted(_bad, true);
            CFunctions.SetDrunk(_bad, true);
            //Blip
            _cBlip = _bad.AttachBlip();
            _cBlip.EnableRoute(Color.Red);
            _cBlip.Color = Color.Red;
            _cBlip.Scale = .5f;
            //Task
            _bad.Tasks.CruiseWithVehicle(_cVehicle, 100f, VehicleDrivingFlags.Emergency);
            //UI
            CFunctions.BuildUi(out _interaction, out _mainMenu, out _convoMenu, out _questioning, out _endCall);
            _mainMenu.OnItemSelect += Interactions;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            try
            {
                switch (_state)
                {
                    case CState.CheckDistance:
                        if (Game.LocalPlayer.Character.DistanceTo(_bad) < 30f)
                        {
                            _cBlip.Delete();
                            _pursuit = Functions.CreatePursuit();
                            Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                            _state = CState.OnScene;
                        }
                        break;
                    case CState.OnScene:
                        Game.DisplayHelp("Suspect is fleeing!");
                        Functions.AddPedToPursuit(_pursuit, _bad);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        if (Main.UsingUb)
                        {
                            Wrapper.CallPursuit();
                        }
                        else
                        {
                            Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit, EBackupUnitType.LocalUnit);
                        }
                        _state = CState.End;
                        break;
                }
                
                //Keybinds
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.IsKeyDown(Settings.Interact))
                {
                    _mainMenu.Visible = !_mainMenu.Visible;
                    _convoMenu.Visible = false;
                }
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
            CFunctions.Code4Message();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            if (_cBlip) _cBlip.Delete();
            if (_cVehicle) _cVehicle.Dismiss();
            if (_bad) _bad.Dismiss();
            _interaction.CloseAllMenus();
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
        
        private enum CState
        {
            CheckDistance,
            OnScene,
            End
        }

    }
}