using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts2.SimpleFunctions;

namespace SuperCallouts2.Callouts
{
    [CalloutInfo("CarAccident3", CalloutProbability.Medium)]
    internal class CarAccident3 : Callout
    {
        private Vehicle _eVehicle;
        private Vehicle _eVehicle2;
        private Ped _ePed;
        private Ped _ePed2;
        private readonly int _choice = new Random().Next(0,4);
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        private Blip _eBlip;
        //UI Items
        private MenuPool _interaction;
        private UIMenu _mainMenu;
        private UIMenu _convoMenu;
        private UIMenuItem _questioning;
        private UIMenuItem _endCall;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            //Setup
            CFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~b~Dispatch:~s~ Reports of a motor vehicle accident.";
            CalloutAdvisory = "Caller reports the drivers are violently arguing.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_HIT_AND_RUN_03 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: car accident callout accepted...");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~b~Dispatch", "~r~MVA",
                "Reports of a car accident, respond ~r~CODE-3");
            //Vehicles
            CFunctions.SpawnNormalCar(out _eVehicle, _spawnPoint, _spawnPointH);
            CFunctions.Damage(_eVehicle, 200, 200);
            CFunctions.SpawnNormalCar(out _eVehicle2, _eVehicle.GetOffsetPositionFront(7f));
            _eVehicle2.Rotation = new Rotator(0f, 0f, 90f);
            CFunctions.Damage(_eVehicle2, 200, 200);
            //Peds
            _ePed = _eVehicle.CreateRandomDriver();
            _ePed.IsPersistent = true;
            _ePed.BlockPermanentEvents = true;
            _ePed2 = _eVehicle2.CreateRandomDriver();
            _ePed2.IsPersistent = true;
            _ePed2.BlockPermanentEvents = true;
            //Blip
            _eBlip = new Blip(_spawnPoint, 15f);
            _eBlip.Color = Color.Red;
            _eBlip.Alpha /= 2;
            _eBlip.Name = "Callout";
            _eBlip.Flash(500, 8000);
            _eBlip.EnableRoute(Color.Red);
            //Randomize
            Game.LogTrivial("PragmaticCallouts: Car Accident Scenorio #" + _choice);
            switch (_choice)
            {
                case 0://Peds fight
                    _ePed.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                    _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                    break;
                case 1://Ped Dies, other flees
                    _ePed.Kill();
                    _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                    break;
                case 2://Hit and run
                    _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                    break;
                case 3://Fire + dead ped.
                    _ePed.Kill();
                    _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                    break;
                default:
                    End();
                    break;
            }
            
            //UI Items
            CFunctions.BuildUi(out _interaction, out _mainMenu, out _convoMenu, out _questioning, out _endCall);
            _mainMenu.OnItemSelect += InteractionProcess;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        { 
            try
            {
                switch (_tasks)
                {
                    case Tasks.CheckDistance:
                        if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 25f)
                        {
                            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~y~On Scene",
                                "~r~Car Accident", "Investigate the scene.");
                            _tasks = Tasks.OnScene;
                        }
                        break;
                    case Tasks.OnScene:
                        _eBlip.DisableRoute();
                        _ePed.BlockPermanentEvents = false;
                        _ePed2.BlockPermanentEvents = false;
                        Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                        switch (_choice)
                        {
                            case 0://Peds fight
                                _ePed.Tasks.FightAgainst(_ePed2);
                                _ePed2.Tasks.FightAgainst(_ePed);
                                break;
                            case 1://Ped Dies, other flees
                                var pursuit = Functions.CreatePursuit();
                                Functions.AddPedToPursuit(pursuit, _ePed2);
                                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                                break;
                            case 2://Hit and run
                                var pursuit2 = Functions.CreatePursuit();
                                Functions.AddPedToPursuit(pursuit2, _ePed);
                                Functions.SetPursuitIsActiveForPlayer(pursuit2, true);
                                _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                                break;
                            case 3://Fire + dead ped.
                                _ePed2.Tasks.Cower(-1);
                                CFunctions.FireControl(_spawnPoint.Around2D(7f), 24, true);
                                break;
                            default:
                                End();
                                break;
                        }

                        _tasks = Tasks.End;
                        break;
                    case Tasks.End:
                        break;
                    default:
                        End();
                        break;
                }
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
            if (_ePed) _ePed.Dismiss();
            if (_ePed2) _ePed2.Dismiss();
            if (_eVehicle) _eVehicle.Dismiss();
            if (_eVehicle2) _eVehicle2.Dismiss();
            if (_eBlip) _eBlip.Delete();
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
        
        private Tasks _tasks = Tasks.CheckDistance;
        private enum Tasks
        {
            CheckDistance,
            OnScene,
            End
        }
    }
}
