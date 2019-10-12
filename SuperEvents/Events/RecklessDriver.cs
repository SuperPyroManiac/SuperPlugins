#region

using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents.SimpleFunctions;

#endregion

namespace SuperEvents.Events
{
    public class RecklessDriver : AmbientEvent
    {
        private Ped _bad1;
        private Vehicle _cVehicle;
        private LHandle _pursuit;
        private Blip _cBlip;
        private bool _pursuitActive;
        private bool _onScene;
        private bool _noDouble;
        private string _name1;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperEvents", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new UIMenu("SuperEvents", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem _questioning = new UIMenuItem("Speak With Subjects");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Call", "Ends the callout early.");
        
        private UIMenuItem _speakSuspect;

        internal static void Launch()
        {
            var eventBooter = new RecklessDriver();
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
            _bad1.Tasks.CruiseWithVehicle(_cVehicle, 20f, VehicleDrivingFlags.Reverse);
            EFunctions.SetWanted(_bad1, true);
            _cVehicle.IsStolen = true;
            _bad1.Metadata.searchPed = "~r~baggy of meth~s~, ~r~combat pistol~s~";
            _bad1.Metadata.stpDrugsDetected = true;
            _cVehicle.Metadata.searchDriver = "~r~sealed bag of meth~s~, ~r~ripped open empty bag~s~, ~y~machete~s~, ~g~a pair of shoes~s~";
            _name1 = Functions.GetPersonaForPed(_bad1).FullName;
            //Start UI
            _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
            _interaction.Add(_mainMenu);
            _interaction.Add(_convoMenu);
            _mainMenu.AddItem(_questioning);
            _mainMenu.AddItem(_endCall);
            _convoMenu.AddItem(_speakSuspect);
            
            _mainMenu.RefreshIndex();
            _convoMenu.RefreshIndex();
            _mainMenu.BindMenuToItem(_convoMenu, _questioning);
            
            _mainMenu.OnItemSelect += Interactions;
            _convoMenu.OnItemSelect += Conversations;
            _convoMenu.ParentMenu = _mainMenu;
            _questioning.Enabled = false;
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
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~Reckless Driving", "Stop the vehicle.");
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                        }
                        
                        if (Game.IsKeyDown(Settings.Interact))
                        {
                            _mainMenu.Visible = !_mainMenu.Visible;
                            _convoMenu.Visible = false;
                        }

                        if (_onScene && !_pursuitActive && Functions.IsPlayerPerformingPullover() && !_noDouble)
                        {
                            Functions.ForceEndCurrentPullover();
                            _pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(_pursuit, _bad1);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            _noDouble = true;
                            _pursuitActive = true;
                        }

                        if (_pursuitActive && !Functions.IsPursuitStillRunning(_pursuit))
                        {
                            _questioning.Enabled = true;
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                            _pursuitActive = false;
                        }

                        if (_bad1.Exists())
                        {
                            if (!_pursuitActive && Game.LocalPlayer.Character.DistanceTo(_bad1) > 100f) End();
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
        private void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _speakSuspect)
            {
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: Why in the world were you driving backwards! You could have killed someone!", 5000);
                    NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: Man i'm not feeling very good...", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: What do you mean?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: Oh man I had something I wanted a hide but I ate it to hide it, I feel dizzy.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: What did you eat? I'll get an ambulance out here.'", 5000);
                    GameFiber.Wait(2000);
                    _bad1.Kill();
                });
            }
        }
    }
}