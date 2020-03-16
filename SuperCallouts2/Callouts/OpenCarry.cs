using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Drawing;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts2.SimpleFunctions;

namespace SuperCallouts2.Callouts
{
    [CalloutInfo("OpenCarry", CalloutProbability.Medium)]
    internal class OpenCarry : Callout
    {
        #region Variables
        private Ped _bad1;
        private Blip _cBlip;
        private LHandle _pursuit;
        private readonly Random _rNd = new Random();
        private Vector3 _spawnPoint;
        private bool _onScene;
        private bool _startScene;
        private string _name1;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperCallouts", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new UIMenu("SuperCallouts", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem _questioning = new UIMenuItem("Speak With Subjects");
        private readonly UIMenuItem _stopSuspect =
            new UIMenuItem("~r~ Dismiss Suspect", "Lets the suspect leave.");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Callout", "Ends the callout early.");
        private UIMenuItem _speakSuspect;
        #endregion
        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~b~Dispatch:~s~ Reports of a person with a firearm.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: Open Carry callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Person With Gun",
                "Reports of a person walking around with an assault rifle. Respond ~y~CODE-2");
            //Bad
            _bad1 = new Ped(_spawnPoint) {IsPersistent = true};
            _bad1.Inventory.GiveNewWeapon(WeaponHash.AdvancedRifle, -1, true);
            _bad1.Tasks.Wander();
            _name1 = Functions.GetPersonaForPed(_bad1).FullName;
            CFunctions.SetDrunk(_bad1, true);
            _bad1.Metadata.stpAlcoholDetected = true;
            _bad1.Metadata.hasGunPermit = false;
            _bad1.Metadata.searchPed = "~r~assaultrifle~s~, ~y~pocket knife~s~, ~g~wallet~s~";
            //Blip
            _cBlip = _bad1.AttachBlip();
            _cBlip.EnableRoute(Color.Red);
            _cBlip.Color = Color.Red;
            //Start UI
            _interaction.Add(_mainMenu);
            _interaction.Add(_convoMenu);
            _mainMenu.AddItem(_stopSuspect);
            _mainMenu.AddItem(_questioning);
            _mainMenu.AddItem(_endCall);
            _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
            _mainMenu.RefreshIndex();
            _convoMenu.RefreshIndex();
            _mainMenu.BindMenuToItem(_convoMenu, _questioning);
            _mainMenu.OnItemSelect += Interactions;
            _convoMenu.OnItemSelect += Conversations;
            _convoMenu.ParentMenu = _mainMenu;
            _questioning.Enabled = false;
            _speakSuspect.Enabled = false;
            _stopSuspect.Enabled = false;
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            try
            {
                //Gameplay
                if (!_onScene && Game.LocalPlayer.Character.Position.DistanceTo(_bad1) < 20f)
                {
                    Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                    _stopSuspect.Enabled = true;
                }

                if (_startScene)
                {
                    _pursuit = Functions.CreatePursuit();
                    _cBlip.DisableRoute();
                    var choices = _rNd.Next(1, 6);
                    switch (choices)
                    {
                        case 1:
                            Game.DisplaySubtitle("~r~Suspect: ~s~I know my rights, leave me alone!", 5000);
                            Functions.AddPedToPursuit(_pursuit, _bad1);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            break;
                        case 2:
                            Game.DisplayNotification("Investigate the person.");
                            _bad1.Tasks.ClearImmediately();
                            _bad1.Inventory.Weapons.Clear();
                            NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                            _speakSuspect.Enabled = true;
                            break;
                        case 3:
                            Game.DisplaySubtitle("~r~Suspect: ~s~REEEEEE", 5000);
                            _bad1.Tasks.AimWeaponAt(Game.LocalPlayer.Character, -1);
                            break;
                        case 4:
                            Game.DisplayNotification("Investigate the person.");
                            _bad1.Tasks.ClearImmediately();
                            _bad1.Inventory.Weapons.Clear();
                            NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                            _bad1.Metadata.hasGunPermit = true;
                            _speakSuspect.Enabled = true;
                            break;
                        case 5:
                            _bad1.Tasks.FireWeaponAt(Game.LocalPlayer.Character, -1, FiringPattern.FullAutomatic);
                            break;
                        default:
                            Game.DisplayNotification(
                                "An error has been detected! Ending callout early to prevent LSPDFR crash!");
                            End();
                            break;
                    }
                }
                //Keybinds
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.IsKeyDown(Settings.Interact))
                {
                    _mainMenu.Visible = !_mainMenu.Visible;
                }
                _interaction.ProcessMenus();
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to SuperPyroManiac!");
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
            if (_bad1.Exists()) _bad1.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
        //UI Items
        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _stopSuspect)
            {
                Game.DisplaySubtitle("~g~You~s~: Hey, I need to speak with you.");
                _stopSuspect.Enabled = false;
                _startScene = true;
            }
            if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }
        private void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _speakSuspect)
            {
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: I'm with the police. What is the reason for carrying your weapon out?", 5000);
                    NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                    GameFiber.Wait(5000);
                    _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: It's my right officer. Nobody can tell me I can't have my gun.''", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: Alright, I understand your rights and with the proper license you can open carry, but you cannot carry your weapon in your hands like that.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: I don't see why not!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: It's the law, as well as it scares people to see someone walking around with a rifle in their hands. There's no reason to. Do you have a  for it?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: Check for yourself.", 5000);
                });
            }
        }
    }
}