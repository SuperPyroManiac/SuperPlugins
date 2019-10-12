#region

using System;
using System.Drawing;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents.SimpleFunctions;

#endregion

namespace SuperEvents.Events
{
    public class PulloverShooting : AmbientEvent
    {
        private Ped _bad;
        private Blip _cBlip1;
        private Blip _cBlip2;
        private Ped _cop;
        private Vehicle _cVehicle1;
        private Vehicle _cVehicle2;
        private bool _onScene;
        private Vector3 _spawnPoint;
        private Vector3 _spawnPoint2;
        private float _spawnPointH;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperEvents", "~y~Choose an option.");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Call", "Ends the callout early.");

        internal static void Launch()
        {
            var eventBooter = new PulloverShooting();
            eventBooter.StartEvent();
        }

        protected override void StartEvent()
        {
            EFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
            if (_spawnPoint.DistanceTo(Game.LocalPlayer.Character) < 35f) {base.Failed(); return;}
            EFunctions.SpawnNormalCar(out _cVehicle1, _spawnPoint);
            _cVehicle1.Heading = _spawnPointH;
            _spawnPoint2 = _cVehicle1.GetOffsetPositionFront(-9f);
            _cVehicle2 = new Vehicle("POLICE", _spawnPoint2)
            {
                IsPersistent = true, Heading = _spawnPointH, IsSirenOn = true, IsSirenSilent = true
            };
            _bad = new Ped {IsPersistent = true, Health = 400, BlockPermanentEvents = true};
            _cop = new Ped("s_m_y_cop_01", _spawnPoint, 0f) {IsPersistent = true, BlockPermanentEvents = true};
            if (!_bad.Exists() || !_cop.Exists()) {base.Failed(); return;}
            _bad.Inventory.Weapons.Add(WeaponHash.BullpupShotgun).Ammo = -1;
            _cop.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
            _bad.WarpIntoVehicle(_cVehicle1, -1);
            _cop.WarpIntoVehicle(_cVehicle2, -1);
            EFunctions.SetWanted(_bad, true);
            _bad.RelationshipGroup = new RelationshipGroup("BADGANG");
            Game.SetRelationshipBetweenRelationshipGroups("BADGANG", "COP", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("BADGANG", "PLAYER", Relationship.Hate);
            _bad.Tasks.LeaveVehicle(_cVehicle1, LeaveVehicleFlags.LeaveDoorOpen);
            _cop.Tasks.LeaveVehicle(_cVehicle2, LeaveVehicleFlags.LeaveDoorOpen);
            _bad.Metadata.stpAlcoholDetected = true;
            _bad.Metadata.hasGunPermit = false;
            _bad.Metadata.searchPed = "~r~assault rifle~s~, ~r~pistol~s~, ~r~used meth pipe~s~, ~y~suicide letter~s~";
            _cVehicle1.Metadata.searchDriver = "~r~baggy of meth~s~, ~g~a pair of shoes~s~";
            _cVehicle1.Metadata.searchTrunk = "~y~stacks of past due medical bills~s~";
            //Start UI
            _interaction.Add(_mainMenu);
            _mainMenu.AddItem(_endCall);
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
            //Blips
            if (Settings.ShowBlips)
            {
                _cBlip1 = _bad.AttachBlip();
                _cBlip1.Color = Color.Red;
                _cBlip1.Scale = .5f;
                _cBlip2 = _cop.AttachBlip();
                _cBlip2.Color = Color.Red;
                _cBlip2.Scale = .5f;
            }
            base.StartEvent();
        }

        protected override void MainLogic()
        {
            GameFiber.StartNew(delegate
            {
                while (EventsActive)
                {
                    try
                    {
                        GameFiber.Yield();
                        if (Game.IsKeyDown(Settings.EndEvent)) End();
                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 30f)
                        {
                            if (!_bad.Exists() || !_cop.Exists())
                            {
                                End();
                                return;
                            }

                            _onScene = true;
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                            _bad.BlockPermanentEvents = false;
                            _cop.BlockPermanentEvents = false;
                            _bad.Tasks.FightAgainst(_cop);
                            _cop.Tasks.FightAgainst(_bad);
                            if (Settings.ShowHints)
                            {
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~Officer Under Fire", "Help the other officer!");
                            }
                        }
                        
                        if (Game.IsKeyDown(Settings.Interact))
                        {
                            _mainMenu.Visible = !_mainMenu.Visible;
                        }

                        if (_bad.IsCuffed || _bad.IsDead) End();
                        if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) > 200) End();
                        _interaction.ProcessMenus();
                    }
                    catch (Exception e)
                    {
                        Game.LogTrivial("Oops there was an error here. Please send this log to SuperPyroManiac!");
                        Game.LogTrivial("SuperEvents Error Report Start");
                        Game.LogTrivial("======================================================");
                        Game.LogTrivial(e.ToString());
                        Game.LogTrivial("======================================================");
                        Game.LogTrivial("SuperEvents Error Report End");
                        End();
                    }
                }
            });
            base.MainLogic();
        }

        protected override void End()
        {
            if (_bad.Exists()) _bad.Dismiss();
            if (_cop.Exists()) _cop.Dismiss();
            if (_cVehicle1.Exists()) _cVehicle1.Dismiss();
            if (_cVehicle2.Exists()) _cVehicle2.Dismiss();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            base.End();
        }
        
        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Event Ended.");
                End();
            }
        }
    }
}