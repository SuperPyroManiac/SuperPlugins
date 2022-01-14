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
    [CalloutInfo("ToiletPaperBandit", CalloutProbability.Medium)]
    class ToiletPaperBandit : Callout
    {
        private Ped _bad;
        private Vehicle _cVehicle;
        private Blip _cBlip;
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        private CState _state = CState.checkDistance;
        private LHandle _pursuit;
        private string _name1;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperCallouts", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new UIMenu("SuperCallouts", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem _questioning = new UIMenuItem("Speak With Subject");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Callout", "Ends the callout early.");
        private UIMenuItem _speakSuspect;

        public override bool OnBeforeCalloutDisplayed()
        {
            CFunctions.FindSideOfRoad(750, 280, out _spawnPoint, out _spawnPointH);
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~b~Dispatch:~s~ Reports of a sanitization transport robbery.";
            CalloutAdvisory = "Caller reports the vehicle of full of cleaning supplies. Possible fire hazard.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_GRAND_THEFT_AUTO_03 IN_OR_ON_POSITION",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: toilet paper bandit accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Robbery",
                "Reports of someone robbing a truck full of cleaning supplies, respond ~r~CODE-3");
            //cVehicle
            _cVehicle = new Vehicle("pounder", _spawnPoint) {IsPersistent = true, IsStolen = true, Heading = _spawnPointH};
            _cVehicle.Metadata.searchDriver = "~y~50 travel hand sanitizers~s~, ~y~48 toilet paper rolls~s~, ~g~lighters~s~, ~g~cigarettes~s~";
            _cVehicle.Metadata.searchPassenger = "~r~multiple packs of cleaning wipes~s~, ~r~box full of medical masks~s~";
            _cVehicle.Metadata.searchTrunk = "~r~multiple pallets of toilet paper~s~, ~r~hazmat suits~s~, ~r~12 molotov explosives~s~, ~y~22 packs of cigarettes~s~";
            //Bad
            _bad = new Ped("s_m_m_movspace_01", _spawnPoint.Around2D(20f), 0f){BlockPermanentEvents = true, IsPersistent = true};
            _bad.WarpIntoVehicle(_cVehicle, -1);
            _bad.Inventory.Weapons.Add(WeaponHash.Molotov);
            _bad.Metadata.searchPed = "~r~Molotov's~s~, ~g~multiple hand sanitizers~s~, ~g~cleaning wipes~s~";
            _bad.Metadata.stpDrugsDetected = true;
            _bad.Tasks.CruiseWithVehicle(_cVehicle, 10f, VehicleDrivingFlags.Normal);
            _name1 = Functions.GetPersonaForPed(_bad).FullName;
            //Blip
            _cBlip = _bad.AttachBlip();
            _cBlip.Color = Color.Red;
            _cBlip.EnableRoute(Color.Red);
            //Start UI
            _mainMenu.MouseControlsEnabled = false;
            _mainMenu.AllowCameraMovement = true;
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
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            try
            {
                switch(_state)
                {
                    case CState.checkDistance:
                        if (Game.LocalPlayer.Character.DistanceTo(_bad) < 30f)
                        {
                            _cBlip.DisableRoute();
                            _pursuit = Functions.CreatePursuit();
                            Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                            _state = CState.onScene;
                        }
                        break;
                    case CState.onScene:
                        Game.DisplayHelp("Suspect is fleeing!");
                        Functions.AddPedToPursuit(_pursuit, _bad);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit, EBackupUnitType.AirUnit);
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit, EBackupUnitType.SwatTeam);
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit, EBackupUnitType.LocalUnit);
                        _state = CState.Pursuit;
                        break;
                    case CState.Pursuit:
                        if (!Functions.IsPursuitStillRunning(_pursuit) || _bad.IsCuffed)
                        {
                            //_bad.Tasks.Clear();
                            Game.DisplaySubtitle(
                                "~r~" + _name1 + "~s~: I surrender!", 5000);
                            _state = CState.letsChat;
                        }
                        break;
                    case CState.letsChat:
                        Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                        _questioning.Enabled = true;
                        _state = CState.End;
                        break;
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
            CFunctions.Code4Message();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            if(_cVehicle) _cVehicle.Dismiss();
            if(_bad) _bad.Dismiss();
            if(_cBlip) _cBlip.Delete();
            _interaction.CloseAllMenus();
            base.End();
        }
        //UI Functions
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
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: What are you doing with this truck?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name1 + "~s~: Get away from me! You might have that virus!!!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: You need to calm down, is that why you stole a truck full of cleaning supplies?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name1 + "~s~: Everyone is infected.. EVERYONE! Let me go, give me my sanitizer!!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: I understand your fears but you need to calm down.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name1 + "~s~: Its everywhere.. EVERYWHERE! I need my sanitizer, I NEED IT! I NEED IT!", 5000);
                });
        }

        private enum CState
        {
            checkDistance,
            onScene,
            Pursuit,
            letsChat,
            End
        }
    }
}