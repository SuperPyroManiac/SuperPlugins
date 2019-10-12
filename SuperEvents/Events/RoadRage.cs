using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents.SimpleFunctions;

namespace SuperEvents.Events
{
    public class RoadRage : AmbientEvent
    {
        private Ped _bad1;
        private Vehicle _cVehicle;
        private LHandle _pursuit;
        private Blip _cBlip;
        private bool _pursuitActive;
        private bool _onScene;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperEvents", "~y~Choose an option.");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Call", "Ends the callout early.");

        internal static void Launch()
        {
            var eventBooter = new RoadRage();
            eventBooter.StartEvent();
        }

        protected override void StartEvent()
        {
            var car = Game.LocalPlayer.Character.GetNearbyVehicles(15);
            if (car == null || car.Length == 0) {base.Failed(); return;}
            foreach (var cars in car)
            {
                if (!cars.Exists()) {base.Failed(); return;}
                if (cars.HasDriver) _cVehicle = cars;
            }
            if (!_cVehicle.Exists()) {base.Failed(); return;}
            _bad1 = _cVehicle.Driver;
            if (!_bad1.Exists()) {base.Failed(); return;}
            if (_bad1 == Game.LocalPlayer.Character || !_bad1.IsHuman || _bad1.IsInAnyVehicle(true) || _bad1.IsDead || _bad1.RelationshipGroup == "COP" || _bad1.RelationshipGroup == "MEDIC " || _bad1.RelationshipGroup == "FIREMAN" || _cVehicle.HasSiren) {base.Failed(); return;}
            _bad1.IsPersistent = true;
            _cVehicle.IsPersistent = true;
            _bad1.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, -1, true);
            _bad1.Tasks.CruiseWithVehicle(_cVehicle, 20f, VehicleDrivingFlags.Emergency);
            EFunctions.SetWanted(_bad1, true);
            //Start UI
            _interaction.Add(_mainMenu);
            _mainMenu.AddItem(_endCall);
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
            //Blips
            if (Settings.ShowBlips)
            {
                _cBlip = _bad1.AttachBlip();
                _cBlip.Color = Color.Red;
                _cBlip.Scale = .5f;
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

                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_bad1) < 30f)
                        {
                            _onScene = true;
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                            if (Settings.ShowHints)
                            {
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~Road Rage", "Stop the vehicle.");
                            }
                        }

                        if (_onScene && !_pursuitActive && Functions.IsPlayerPerformingPullover())
                        {
                            var rNd = new Random();
                            var choices = rNd.Next(1, 6);
                            switch (choices)
                            {
                                case 1:
                                    Functions.ForceEndCurrentPullover();
                                    _pursuit = Functions.CreatePursuit();
                                    Functions.AddPedToPursuit(_pursuit, _bad1);
                                    Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                                    _pursuitActive = true;
                                    break;
                                default:
                                    End();
                                    break;
                            }
                        }

                        if (!_pursuitActive && !_bad1.IsInAnyVehicle(true)) End();

                        if (_pursuitActive && !Functions.IsPursuitStillRunning(_pursuit)) End();

                        if (_bad1.Exists())
                        {
                            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_bad1) > 300f) End();
                            if (_onScene && !_pursuitActive &&
                                Game.LocalPlayer.Character.DistanceTo(_bad1) > 300f) End();
                            if (_bad1.IsDead || _bad1.IsCuffed) End();
                        }
                        else
                        {
                            End();
                        }
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
            if(_bad1.Exists()) _bad1.Dismiss();
            if(_cVehicle.Exists()) _cVehicle.Dismiss();
            if(_cBlip.Exists()) _cBlip.Delete();
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