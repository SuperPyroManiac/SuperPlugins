using System;
using LSPD_First_Response.Mod.API;
using Rage;
using SuperEvents.SimpleFunctions;

namespace SuperEvents.Events
{
    internal class PulloverShooting : AmbientEvent
    {
        private Ped _cPed;
        private Vehicle _cVehicle;
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        private Ped _sPed;
        private Vehicle _sVehicle;

        private Tasks _tasks = Tasks.CheckDistance;

        internal override void StartEvent(Vector3 s)
        {
            //Setup
            EFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
            if (_spawnPoint.DistanceTo(Player) < 35f)
            {
                End(true);
                return;
            }

            //Vehicles
            _cVehicle = new Vehicle("POLICE2", _spawnPoint) {Heading = _spawnPointH, IsPersistent = true};
            EFunctions.SpawnNormalCar(out _sVehicle, _cVehicle.GetOffsetPositionFront(8));
            _sVehicle.Metadata.searchDriver = "~r~baggy of meth~s~, ~g~a pair of shoes~s~";
            _sVehicle.Metadata.searchTrunk = "~y~stacks of past due medical bills~s~";
            EntitiesToClear.Add(_cVehicle);
            EntitiesToClear.Add(_sVehicle);
            //Peds
            _cPed = new Ped("s_m_y_cop_01", Vector3.Zero, 0f) {IsPersistent = true, BlockPermanentEvents = true};
            _cPed.WarpIntoVehicle(_cVehicle, -1);
            _cPed.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
            _sPed = new Ped {IsPersistent = true, Health = 400, BlockPermanentEvents = true};
            _sPed.WarpIntoVehicle(_sVehicle, -1);
            _sPed.Inventory.Weapons.Add(WeaponHash.BullpupShotgun).Ammo = -1;
            EFunctions.SetWanted(_sPed, true);
            _sPed.Metadata.stpAlcoholDetected = true;
            _sPed.Metadata.hasGunPermit = false;
            _sPed.Metadata.searchPed = "~r~assault rifle~s~, ~r~pistol~s~, ~r~used meth pipe~s~, ~y~suicide letter~s~";
            EntitiesToClear.Add(_cPed);
            EntitiesToClear.Add(_sPed);

            base.StartEvent(_spawnPoint);
        }

        protected override void Process()
        {
            try
            {
                switch (_tasks)
                {
                    case Tasks.CheckDistance:
                        if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 30f)
                        {
                            if (Settings.ShowHints)
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~Officer Under Fire", "Help the other officer!");
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                            _tasks = Tasks.OnScene;
                        }

                        break;
                    case Tasks.OnScene:
                        var choice = new Random().Next(1, 4);
                        Game.LogTrivial("SuperEvents: PulloverShooting event picked scenerio #" + choice);
                        switch (choice)
                        {
                            case 1:
                                _sPed.BlockPermanentEvents = false;
                                _sPed.Tasks.FightAgainst(_cPed);
                                _cPed.BlockPermanentEvents = false;
                                _cPed.Tasks.FightAgainst(_sPed);
                                break;
                            case 2:
                                _sPed.BlockPermanentEvents = false;
                                _sPed.Tasks.FightAgainst(Player);
                                _cPed.Kill();
                                break;
                            case 3:
                                _sPed.BlockPermanentEvents = false;
                                var pursuit = Functions.CreatePursuit();
                                Functions.AddPedToPursuit(pursuit, _sPed);
                                Functions.SetPursuitIsActiveForPlayer(pursuit, Player);
                                _cPed.Kill();
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
                Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
                Game.LogTrivial("SuperEvents Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperEvents Error Report End");
                End(true);
            }
        }

        private enum Tasks
        {
            CheckDistance,
            OnScene,
            End
        }
    }
}