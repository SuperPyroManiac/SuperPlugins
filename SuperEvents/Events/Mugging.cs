#region

using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents.SimpleFunctions;

#endregion

namespace SuperEvents.Events
{
    public class Mugging : AmbientEvent
    {
        private Ped _bad1;
        private Ped _victim;
        private Blip _cBlip1;
        private Blip _cBlip2;
        private bool _onScene;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperEvents", "~y~Choose an option.");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Event", "Ends the event early.");

        internal static void Launch()
        {
            var eventBooter = new Mugging();
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
                _bad1.RelationshipGroup == "COP")
            {
                base.Failed();
                return;
            }

            _bad1.IsPersistent = true;
            _bad1.Inventory.GiveNewWeapon(WeaponHash.Pistol, -1, true);
            _victim = new Ped(_bad1.GetOffsetPositionFront(3f)) {IsPersistent = true};
            EFunctions.SetWanted(_bad1, true);
            _victim.Tasks.PutHandsUp(-1, _bad1);
            _bad1.Tasks.AimWeaponAt(_victim, -1);
            //Start UI
            _interaction.Add(_mainMenu);
            _mainMenu.AddItem(_endCall);
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
            //Blips
            if (Settings.ShowBlips)
            {
                _cBlip1 = _bad1.AttachBlip();
                _cBlip1.Color = Color.Red;
                _cBlip1.Scale = .5f;
                _cBlip2 = _victim.AttachBlip();
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
                    try
                    {
                        GameFiber.Yield();
                        if (!_onScene && !_bad1.IsAnySpeechPlaying) _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                        if (!_onScene && !_victim.IsAnySpeechPlaying)
                            _victim.PlayAmbientSpeech("GENERIC_FRIGHTENED_HIGH");

                        if (Game.IsKeyDown(Settings.EndEvent)) End();

                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_bad1) < 10f)
                        {
                            _onScene = true;
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                            if (Settings.ShowHints)
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~Mugging", "Stop the suspect!");
                            var rNd = new Random();
                            var choices = rNd.Next(1, 3);
                            switch (choices)
                            {
                                case 1:
                                    var pursuit = Functions.CreatePursuit();
                                    _victim.Tasks.Cower(-1);
                                    Functions.AddPedToPursuit(pursuit, _bad1);
                                    Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                                    _cBlip2.Delete();
                                    break;
                                case 2:
                                    _bad1.Tasks.FightAgainst(_victim);
                                    break;
                                default:
                                    Game.DisplayNotification(
                                        "An error has been detected! Ending event early to prevent LSPDFR crash!");
                                    End();
                                    break;
                            }
                        }

                        if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;

                        if (_bad1.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(_bad1) > 200f) End();
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
                        Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
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
            if (_victim.Exists()) _victim.Dismiss();
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