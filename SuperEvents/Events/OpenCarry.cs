#region

using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;

#endregion

namespace SuperEvents.Events
{
    public class OpenCarry : AmbientEvent
    {
        private Ped _bad1;
        private Blip _cBlip;
        private LHandle _pursuit;
        private bool _letsRun;
        private bool _onScene;
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
            var eventBooter = new OpenCarry();
            eventBooter.StartEvent();
        }

        protected override void StartEvent()
        {
            var bad = Game.LocalPlayer.Character.GetNearbyPeds(15);
            if (bad == null || bad.Length == 0)
            {
                base.Failed();
                return;
            }

            foreach (var badguy in bad)
            {
                if (!badguy.Exists()) break;
                _bad1 = badguy;
            }

            if (_bad1 == Game.LocalPlayer.Character || !_bad1.IsHuman || _bad1.IsInAnyVehicle(true) || _bad1.IsDead ||
                _bad1.RelationshipGroup == "COP" || _bad1.RelationshipGroup == "MEDIC " ||
                _bad1.RelationshipGroup == "FIREMAN")
            {
                base.Failed();
                return;
            }

            _bad1.IsPersistent = true;
            _bad1.Inventory.GiveNewWeapon(WeaponHash.AdvancedRifle, -1, true);
            _bad1.Metadata.stpAlcoholDetected = true;
            _bad1.Metadata.hasGunPermit = false;
            _bad1.Metadata.searchPed = "~r~assault rifle~s~, ~y~pocket knife~s~, ~g~wallet~s~";
            _bad1.Tasks.Wander();
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
            if (!Settings.ShowBlips)
            {
                base.StartEvent();
                return;
            }

            _cBlip = _bad1.AttachBlip();
            _cBlip.Color = Color.Red;
            _cBlip.Scale = .5f;
            base.StartEvent();
        }

        protected override void MainLogic()
        {
            GameFiber.StartNew(delegate
            {
                while (EventsActive)
                    try
                    {
                        GameFiber.Yield();
                        if (Game.IsKeyDown(Settings.EndEvent)) End();

                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_bad1) < 10f)
                        {
                            _onScene = true;
                            _questioning.Enabled = true;
                            if (Settings.ShowHints)
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~Open Carry", "Investigate the person.");
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                        }

                        if (Game.IsKeyDown(Settings.Interact))
                        {
                            _mainMenu.Visible = !_mainMenu.Visible;
                            _convoMenu.Visible = false;
                        }

                        if (_bad1.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(_bad1) > 200f) End();
                            if (_bad1.IsDead || _bad1.IsCuffed) End();
                            if (_letsRun && !Functions.IsPursuitStillRunning(_pursuit)) End();
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
            });
            base.MainLogic();
        }

        protected override void End()
        {
            if (_bad1.Exists()) _bad1.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
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
                GameFiber.StartNew(delegate
                {
                    NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character,
                        -1);
                    Game.DisplaySubtitle("~g~You: ~s~Why are you carring a that gun around?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 +
                                         ": ~s~Its.. It's my right.. I'll leave im sorry! Please leave me alone!''");
                    GameFiber.Wait(1000);
                    _pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(_pursuit, _bad1);
                    Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                    _letsRun = true;
                });
        }
    }
}