using System;
using System.Collections.Generic;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents2.SimpleFunctions;

namespace SuperEvents2.Events
{
    internal class CarAccident : AmbientEvent
    {
        private Vehicle _eVehicle;
        private Vehicle _eVehicle2;
        private Ped _ePed;
        private Ped _ePed2;
        private readonly int _choice = new Random().Next(0,5);
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        private string _name1;
        private string _name2;
        //UI Items
        private UIMenuItem _speakSuspect;
        private UIMenuItem _speakSuspect2;
        
        internal override void StartEvent(Vector3 s, float f)
        {
            //Setup
            EFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
            if (_spawnPoint.DistanceTo(Player) < 35f) {End(true); return;}
            //Vehicles
            EFunctions.SpawnNormalCar(out _eVehicle, _spawnPoint);
            _eVehicle.Heading = _spawnPointH;
            EFunctions.Damage(_eVehicle, 200, 200);
            EFunctions.SpawnNormalCar(out _eVehicle2, _eVehicle.GetOffsetPositionFront(7f));
            _eVehicle2.Rotation = new Rotator(0f, 0f, 90f);
            EFunctions.Damage(_eVehicle2, 200, 200);
            EntitiesToClear.Add(_eVehicle);
            EntitiesToClear.Add(_eVehicle2);
            //Peds
            _ePed = _eVehicle.CreateRandomDriver();
            _ePed.IsPersistent = true;
            _ePed.BlockPermanentEvents = true;
            _ePed2 = _eVehicle2.CreateRandomDriver();
            _ePed2.IsPersistent = true;
            _ePed2.BlockPermanentEvents = true;
            _name1 = Functions.GetPersonaForPed(_ePed).FullName;
            _name2 = Functions.GetPersonaForPed(_ePed2).FullName;
            EntitiesToClear.Add(_ePed);
            EntitiesToClear.Add(_ePed2);
            //Randomize
            Game.LogTrivial("SuperEvents: Scenorio #" + _choice);
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
                    End(true);
                    break;
            }
            //UI Items
            _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
            _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name2);
            ConvoMenu.AddItem(_speakSuspect);
            ConvoMenu.AddItem(_speakSuspect2);
            
            base.StartEvent(_spawnPoint, _spawnPointH);
        }

        protected override void Process()
        {
            try
            {
                switch (_tasks)
                {
                    case Tasks.CheckDistance:
                        if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 25f)
                        {
                            if (Settings.ShowHints)
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~Car Accident", "Investigate the scene.");
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                            Questioning.Enabled = true;
                            _tasks = Tasks.OnScene;
                        }
                        break;
                    case Tasks.OnScene:
                        _ePed.BlockPermanentEvents = false;
                        _ePed2.BlockPermanentEvents = false;
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
                                EFunctions.FireControl(_spawnPoint.Around2D(7f), 24, true);
                                break;
                            default:
                                End(true);
                                break;
                        }

                        _tasks = Tasks.End;
                        break;
                    case Tasks.End:
                        break;
                    default:
                        End(true);
                        break;
                }
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
                Game.LogTrivial("SuperEvents Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperEvents Error Report End");
                End(true);
            }
            base.Process();
        }

        protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        { 
            if (selItem == _speakSuspect)
            {
                Game.DisplaySubtitle("Not implimented yet.");
            }
            if (selItem == _speakSuspect2)
            {
                Game.DisplaySubtitle("Not implimented yet.");
            }
         
            base.Conversations(sender, selItem, index);
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