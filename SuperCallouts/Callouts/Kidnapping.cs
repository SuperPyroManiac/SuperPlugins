using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts
{
    [CalloutInfo("Kidnapping", CalloutProbability.Medium)]
    internal class Kidnapping : Callout
    {
        #region Variables
        private Ped _bad1;
        private Ped _victim1;
        private Vehicle _cVehicle;
        private LHandle _pursuit;
        private Blip _cBlip1;
        private Vector3 _spawnPoint;
        private string _name1;
        private string _name2;
        private readonly Random _rNd = new Random();
        private bool _pursuitOver;
        private bool _onScene;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperCallouts", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new UIMenu("SuperCallouts", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem _questioning = new UIMenuItem("Speak With Subjects");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Call", "Ends the callout.");
        private UIMenuItem _speakSuspect;
        private UIMenuItem _speakSuspect2;
        #endregion

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
            CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Person(s) from amber alert spotted.";
            CalloutAdvisory = "Caller says people in the back of a vehicle match the description of a missing person(s) report.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_RESIST_ARREST IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: Kidnapping callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch",
                "~r~Possible Missing Person Found",
                "A person reported missing last week has been recognized. Possible kidnapping. Respond ~r~CODE-3");
            //cVehicle
            CFunctions.SpawnNormalCar(out _cVehicle, _spawnPoint);
            //bad1
            _bad1 = _cVehicle.CreateRandomDriver();
            _bad1.IsPersistent = true;
            _bad1.BlockPermanentEvents = true;
            _name1 = Functions.GetPersonaForPed(_bad1).FullName;
            _bad1.Inventory.Weapons.Add(WeaponHash.Pistol);
            _bad1.Metadata.stpDrugsDetected = true;
            _bad1.Metadata.searchPed = "~r~pistol~s~, ~r~handcuffs~s~, ~y~hunting knife~s~, ~g~candy bar~s~, ~g~loose change~s~";
            _bad1.Metadata.hasGunPermit = true;
            //victim1
            _victim1 = new Ped();
            _victim1.WarpIntoVehicle(_cVehicle, 0);
            _victim1.IsPersistent = true;
            _victim1.BlockPermanentEvents = true;
            _name2 = Functions.GetPersonaForPed(_victim1).FullName;
            _victim1.Metadata.searchPed = "~r~fake ID~s~";
            //Start UI
            _mainMenu.MouseControlsEnabled = false;
            _mainMenu.AllowCameraMovement = true;
            _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
            _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name2);
            _interaction.Add(_mainMenu);
            _interaction.Add(_convoMenu);
            _mainMenu.AddItem(_questioning);
            _mainMenu.AddItem(_endCall);
            _convoMenu.AddItem(_speakSuspect);
            _convoMenu.AddItem(_speakSuspect2);
            _mainMenu.RefreshIndex();
            _convoMenu.RefreshIndex();
            _mainMenu.BindMenuToItem(_convoMenu, _questioning);
            _mainMenu.OnItemSelect += Interactions;
            _convoMenu.OnItemSelect += Conversations;
            _convoMenu.ParentMenu = _mainMenu;
            _questioning.Enabled = false;
            //Blips
            _cBlip1 = _bad1.AttachBlip();
            _cBlip1.EnableRoute(Color.Red);
            _cBlip1.Color = Color.Red;
            _cBlip1.Scale = .5f;
            //Tasks
            _bad1.Tasks.CruiseWithVehicle(_cVehicle, 10f, VehicleDrivingFlags.Normal);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            try
            {
                //GamePlay
                if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle) < 25f)
                {
                    _cBlip1.Delete();
                    _bad1.BlockPermanentEvents = false;
                    _pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(_pursuit, _bad1);
                    Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                    Game.DisplayHelp("~r~Suspect is evading!");
                    var choices = _rNd.Next(1, 6);
                    switch (choices)
                    {
                        case 1:
                            _victim1.Kill();
                            break;
                        case 2:
                            _victim1.Tasks.LeaveVehicle(LeaveVehicleFlags.BailOut).WaitForCompletion();
                            _victim1.Tasks.Cower(-1);
                            break;
                        default:
                            Game.LogTrivial("Default scenorio loaded.");
                            break;
                    }
                    _onScene = true;
                }
                if (_onScene && !Functions.IsPursuitStillRunning(_pursuit) && !_pursuitOver)
                {
                    _pursuitOver = true;
                    if (Game.LocalPlayer.Character.DistanceTo(_bad1) > 70f)
                    {
                        Game.DisplaySubtitle("~r~Suspect escaped!");
                        End();
                        return;
                    }
                    Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                    _questioning.Enabled = true;
                    if (_bad1.IsDead)
                    {
                        _speakSuspect.Enabled = false;
                        _speakSuspect.RightLabel = "~r~Dead";
                    }
                    if (_victim1.IsDead)
                    {
                        _speakSuspect2.Enabled = false;
                        _speakSuspect2.RightLabel = "~r~Dead";
                    }
                }
                //Keybinds
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.IsKeyDown(Settings.Interact))
                {
                    _mainMenu.Visible = !_mainMenu.Visible;
                    _convoMenu.Visible = false;
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
            if (_bad1.Exists()) _bad1.Dismiss();
            if (_victim1.Exists()) _victim1.Dismiss();
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            _mainMenu.Visible = false;
            CFunctions.Code4Message();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
        //UI Items
        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }
        private void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
            try
            {
                if (selItem == _speakSuspect)
                {
                    GameFiber.StartNew(delegate
                    {
                        Game.DisplaySubtitle("~g~You~s~: Why are you running?", 5000);
                        NativeFunction.Natives.x5AD23D40115353AC(_bad1, Game.LocalPlayer.Character, -1);
                        GameFiber.Wait(5000);
                        _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                        Game.DisplaySubtitle("~r~" + _name1 + "~s~: I don't know, why do you think?'", 5000);
                    });
                }
                if (selItem == _speakSuspect2)
                {
                    GameFiber.StartNew(delegate
                    {
                        Game.DisplaySubtitle("~g~You~s~: Don't worry, i'm a police officer. I'm here to help and you're safe now. Can you tell me what happened?",5000);
                        NativeFunction.Natives.x5AD23D40115353AC(_victim1, Game.LocalPlayer.Character, -1);
                        GameFiber.Wait(5000);
                        Game.DisplaySubtitle("~b~" + _name2 + "~s~: My real name is Bailey, they took me forever ago. I don't even know how long! I've been stuck in a cage in a dark room. Please help me where is my family.", 5000);
                        GameFiber.Wait(5000);
                        Game.DisplaySubtitle("~g~You~s~: Well listen, we are here to help. We will find your family and get you home. Can you tell me what was going on today?", 5000);
                        _victim1.Tasks.Cower(-1);
                        Game.DisplaySubtitle("~b~Bailey Smith~s~: They gave me this fake id.. They were going to give me away I think! Please I want to go home!");
                    });
                }
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
        }
    }
}