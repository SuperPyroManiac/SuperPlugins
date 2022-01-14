using System;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using LSPD_First_Response;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts
{
    [CalloutInfo("Impersonator", CalloutProbability.Medium)]
    internal class Impersonator : Callout
    {
        #region Variables
        private Ped _bad;
        private Ped _victim;
        private Vehicle _cVehicle1;
        private Vehicle _cVehicle2;
        private Blip _cBlip;
        private Vector3 _spawnPoint;
        private LHandle _pursuit;
        private string _name1;
        private float _spawnPointH;
        private bool _onScene;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperCallouts", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new UIMenu("SuperCallouts", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem _callSecond =
            new UIMenuItem("~r~ Call Secondary", "Calls for a second unit to assist.");
        private readonly UIMenuItem _questioning = new UIMenuItem("Speak With Subject");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Callout", "Ends the callout early.");
        private UIMenuItem _speakSuspect;
        #endregion

        public override bool OnBeforeCalloutDisplayed()
        {
            CFunctions.FindSideOfRoad(400, 100, out _spawnPoint, out _spawnPointH);
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~b~Dispatch:~s~ Officer impersonator.";
            CalloutAdvisory = "Caller says they have been stopped by someone that does not look like an officer.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: Officer Impersonator callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Suspicious Pullover",
                Settings.EmergencyNumber+ " call of someone being pulled over by a non uniformed officer. Description does not match our department for undercover cops. Respond ~r~CODE-3");
            //cVehicle1
            CFunctions.SpawnNormalCar(out _cVehicle1, _spawnPoint);
            _cVehicle1.Heading = _spawnPointH;
            //cVehicle2
            var cSpawnPoint = _cVehicle1.GetOffsetPositionFront(-9f);
            _cVehicle2 = new Vehicle("DILETTANTE2", cSpawnPoint) {Heading = _spawnPointH, IsPersistent = true};
            _cVehicle2.Metadata.searchDriver = "~y~police radio scanner~s~, ~y~handcuffs~s~, ~g~parking ticket~s~, ~g~cigarettes~s~";
            //Bad
            _bad = _cVehicle2.CreateRandomDriver();
            _bad.IsPersistent = true;
            _bad.Inventory.Weapons.Add(WeaponHash.Pistol);
            _bad.Metadata.searchPed = "~r~kids plastic police badge~s~, ~r~loaded pistol~s~, ~g~wallet~s~";
            _name1 = Functions.GetPersonaForPed(_bad).FullName;
            //Victim
            _victim = _cVehicle1.CreateRandomDriver();
            _victim.IsPersistent = true;
            //Start UI
            _mainMenu.MouseControlsEnabled = false;
            _mainMenu.AllowCameraMovement = true;
            _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
            _interaction.Add(_mainMenu);
            _interaction.Add(_convoMenu);
            _mainMenu.AddItem(_callSecond);
            _mainMenu.AddItem(_questioning);
            _mainMenu.AddItem(_endCall);
            _convoMenu.AddItem(_speakSuspect);
            _mainMenu.RefreshIndex();
            _convoMenu.RefreshIndex();
            _mainMenu.BindMenuToItem(_convoMenu, _questioning);
            _mainMenu.OnItemSelect += Interactions;
            _convoMenu.OnItemSelect += Conversations;
            _callSecond.LeftBadge = UIMenuItem.BadgeStyle.Alert;
            _convoMenu.ParentMenu = _mainMenu;
            _callSecond.Enabled = false;
            _questioning.Enabled = false;
            //Blips
            _cBlip = _bad.AttachBlip();
            _cBlip.Color = Color.Red;
            _cBlip.EnableRoute(Color.Red);
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            try
            {
                //GamePlay
                if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle2) < 30f)
                {
                    _onScene = true;
                    _cBlip.DisableRoute();
                    _victim.Tasks.CruiseWithVehicle(10f, VehicleDrivingFlags.Normal);
                    _pursuit = Functions.CreatePursuit();
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Suspicious Pullover",
                        "Be advised, caller has been instructed to leave scene by the dispatcher.");
                    Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                    var rNd = new Random();
                    var choices = rNd.Next(1, 4);
                    switch (choices)
                    {
                        case 1:
                            Game.DisplayHelp("Suspect is fleeing!");
                            Functions.AddPedToPursuit(_pursuit, _bad);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            //cVehicle2.IsSirenOn = false;
                            break;
                        case 2:
                            GameFiber.StartNew(delegate
                            {
                                _bad.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(1500);
                                _bad.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
                                GameFiber.Wait(3000);
                                _bad.Tasks.FightAgainst(Game.LocalPlayer.Character, -1);
                                //cVehicle2.IsSirenOn = false;
                            });
                            break;
                        case 3:
                            GameFiber.StartNew(delegate
                            {
                                GameFiber.Wait(2000);
                                _callSecond.Enabled = true;
                                _questioning.Enabled = true;
                                //cVehicle2.IsSirenOn = false;
                            });
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
                        BigMessageThread bigMessage = new BigMessageThread();
            bigMessage.MessageInstance.ShowColoredShard("Code 4", "Callout Ended", HudColor.Green, HudColor.Black,
                2);
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            _interaction.CloseAllMenus();
            if (_bad.Exists()) _bad.Dismiss();
            if (_victim.Exists()) _victim.Dismiss();
            if (_cVehicle1.Exists()) _cVehicle1.Dismiss();
            if (_cVehicle2.Exists()) _cVehicle2.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            base.End();
        }
                //UI Items
        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _callSecond)
            {
                Game.DisplaySubtitle("~g~You~s~: Dispatch, can I get another unit.");
                if (Main.UsingUB)
                {
                    Wrapper.callCode2();
                }
                else
                {
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code2, EBackupUnitType.LocalUnit);
                }
                
                _callSecond.Enabled = false;
            }
            else if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }
        private void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _speakSuspect)
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: What's going on? Why did you have that person stopped?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name1 + "~s~: I'm off duty, that person was driving really dangerously.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: Alright, even if you are off duty you can't be doing that. What department do you work with?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name1 + "~s~: I'm with a secret department in Los Santos. I can't disclose it to you.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: If that's the case you may want to call your supervisor. Do you have any identification or a badge?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name1 + "~s~: I'll have you fired for this officer. I'm not going to talk to you anymore.", 5000);
                });
        }
    }
}