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
    internal class CarFire : AmbientEvent
    {
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        private Ped _victim;
        private Vehicle _eVehicle;

        internal override void StartEvent(Vector3 s, float f)
        {
            //Setup
            EFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
            if (_spawnPoint.DistanceTo(Player) < 35f) {End(true); return;}
            //eVehicle
            EFunctions.SpawnNormalCar(out _eVehicle, _spawnPoint);
            EntitiesToClear.Add(_eVehicle);
            
            base.StartEvent(_spawnPoint, _spawnPointH);
        }

        protected override void Process()
        {
            try
            {
                switch (_tasks)
                {
                    case Tasks.CheckDistance:
                        if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 15f)
                        {
                            if (Settings.ShowHints)
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~A Fire", "Call the Fire Department and clear the scene!");
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                            _tasks = Tasks.OnScene;
                        }
                        break;
                    case Tasks.OnScene:
                        var choice = new Random().Next(1,4);
                        Game.LogTrivial("SuperEvents: Fire event picked scenerio #" + choice);
                        switch (choice)
                        {
                            case 1:
                                EFunctions.FireControl(_spawnPoint.Around2D(7f), 25, true);
                                EFunctions.FireControl(_spawnPoint.Around2D(7f), 25, false);
                                break;
                            case 2:
                                _eVehicle.Explode();
                                EFunctions.FireControl(_spawnPoint.Around2D(7f), 10, true);
                                break;
                            case 3:
                                _victim = _eVehicle.CreateRandomDriver();
                                _victim.IsPersistent = true;
                                EntitiesToClear.Add(_victim);
                                EFunctions.FireControl(_spawnPoint.Around2D(7f), 25, true);
                                EFunctions.FireControl(_spawnPoint.Around2D(7f), 25, false);
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
                base.Process();
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