using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts2.SimpleFunctions;

namespace SuperCallouts2.Callouts
{
    [CalloutInfo("HotPursuit", CalloutProbability.Medium)]
    internal class HotPursuit : Callout
    {
        #region Variables
        private Ped _bad1;
        private Ped _bad2;
        private Vehicle _cVehicle;
        private LHandle _pursuit;
        private Blip _cBlip1;
        private Blip _cBlip2;
        private Vector3 _spawnPoint;
        private string _name1;
        private string _name2;
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
            CalloutMessage = "~o~Traffic ANPR Report:~s~ High value stolen vehicle located.";
            CalloutAdvisory = "This is a powerful vehicle known to evade police in the past.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_RESIST_ARREST IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: HotPursuit callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Stolen Car",
                "ANPR has spotted a stolen vehicle. This vehicle is high performance and has fled before. Respond ~r~CODE-3");
            //cVehicle
            Model[] vehicleModels = {"ZENTORNO", "TEMPESTA", "AUTARCH", "cheetah", "nero2", "tezeract", "visione", "prototipo", "emerus"};
            _cVehicle = new Vehicle(vehicleModels[new Random().Next(vehicleModels.Length)], _spawnPoint) {IsPersistent = true, IsStolen = true};
            _cVehicle.Metadata.searchDriver = "~r~exposed console wires~s~, ~y~wire cutters~s~";
            _cVehicle.Metadata.searchPassenger = "~r~empty beer cans~s~, ~r~opened box of ammo~s~";
            //bad1
            _bad1 = _cVehicle.CreateRandomDriver();
            _bad1.IsPersistent = true;
            _bad1.BlockPermanentEvents = true;
            _name1 = Functions.GetPersonaForPed(_bad1).FullName;
            _bad1.Inventory.Weapons.Add(WeaponHash.Pistol);
            _bad1.Metadata.stpDrugsDetected = true;
            _bad1.Metadata.stpAlcoholDetected = true;
            _bad1.Metadata.searchPed = "~r~pistol~s~, ~r~used meth pipe~s~, ~y~hotwire tools~s~, ~g~suspicious taco~s~, ~g~wallet~s~";
            _bad1.Metadata.hasGunPermit = false;
            CFunctions.SetWanted(_bad1, true);
            CFunctions.SetDrunk(_bad1, true);
            //bad2
            _bad2 = new Ped();
            _bad2.WarpIntoVehicle(_cVehicle, 0);
            _bad2.IsPersistent = true;
            _bad2.BlockPermanentEvents = true;
            _name2 = Functions.GetPersonaForPed(_bad2).FullName;
            _bad2.Metadata.stpAlcoholDetected = true;
            CFunctions.SetDrunk(_bad2, true);
            //Start UI
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
            _cBlip2 = _bad2.AttachBlip();
            _cBlip2.Color = Color.Red;
            _cBlip2.Scale = .5f;
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
                    _cBlip2.Delete();
                    _bad1.BlockPermanentEvents = false;
                    _bad2.BlockPermanentEvents = false;
                    _pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(_pursuit, _bad1);
                    Functions.AddPedToPursuit(_pursuit, _bad2);
                    Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                    Game.DisplayHelp("~r~Suspects are evading!");
                    _onScene = true;
                }
                if (_onScene && !Functions.IsPursuitStillRunning(_pursuit) && !_pursuitOver)
                {
                    _pursuitOver = true;
                    if (Game.LocalPlayer.Character.DistanceTo(_bad1) > 70f &&
                        Game.LocalPlayer.Character.DistanceTo(_bad2) > 70f)
                    {
                        Game.DisplaySubtitle("~r~Suspects escaped!");
                        End();
                        return;
                    }
                    Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                    _questioning.Enabled = true;
                    if (_bad1.IsDead)
                    {
                        _speakSuspect.Enabled = false;
                        _speakSuspect.SetRightLabel("~r~Dead");
                    }
                    if (_bad2.IsDead)
                    {
                        _speakSuspect2.Enabled = false;
                        _speakSuspect2.SetRightLabel("~r~Dead");
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
            if (_bad2.Exists()) _bad2.Dismiss();
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            _mainMenu.Visible = false;
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
            if (selItem == _speakSuspect)
            {
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: Why are you running?", 5000);
                    NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                    GameFiber.Wait(5000);
                    _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: I don't know, why do you think?", 5000);
                });
            }
            if (selItem == _speakSuspect2)
            {
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: You know this is a stolen vehicle right? What are you guys doing?",
                        5000);
                    NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad2, Game.LocalPlayer.Character, -1);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name2 +
                        "~s~: I didn't do anything wrong, I was just hanging out with my buddy and all this happened.",
                        5000);
                });
            }
        }
    }
}