using System;
using System.Drawing;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts
{
    [CalloutInfo("AngryAnimal", CalloutProbability.Medium)]
    internal class AngryAnimal : Callout
    {
        private readonly UIMenuItem _callEms = new("~r~ Call EMS", "Calls for a medical team.");

        private readonly UIMenuItem _endCall = new("~y~End Callout", "Ends the callout early.");

        //UI Items
        private readonly MenuPool _interaction = new();
        private readonly UIMenu _mainMenu = new("SuperCallouts", "~y~Choose an option.");
        private Ped _animal;
        private Blip _cBlip;
        private Blip _cBlip2;
        private bool _onScene;
        private Vector3 _spawnPoint;
        private Ped _victim;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(450f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
            CalloutMessage = "~r~" + Settings.EmergencyNumber +
                             " Report:~s~ Person(s) being attacked by a wild animal.";
            CalloutAdvisory = "Caller says a wild animal is attacking people.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_11_351_02 UNITS_RESPOND_CODE_03_01",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: Angry Animal callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Help Civilian",
                "Details are unknown, get to the scene as soon as possible! Respond ~r~CODE-3");
            if (Main.UsingCi) Wrapper.StartCi(this, "11-14");
            //_animal
            Model[] meanAnimal = { "A_C_MTLION", "A_C_COYOTE" };
            _animal = new Ped(meanAnimal[new Random().Next(meanAnimal.Length)], _spawnPoint, 50);
            _animal.IsPersistent = true;
            _animal.BlockPermanentEvents = true;
            //_victim
            _victim = new Ped(_animal.GetOffsetPosition(new Vector3(0, 1.8f, 0)));
            _victim.IsPersistent = true;
            _victim.BlockPermanentEvents = true;
            _victim.Health = 500;
            //Start UI
            _mainMenu.MouseControlsEnabled = false;
            _mainMenu.AllowCameraMovement = true;
            _interaction.Add(_mainMenu);
            _mainMenu.AddItem(_callEms);
            _mainMenu.AddItem(_endCall);
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
            _callEms.LeftBadge = UIMenuItem.BadgeStyle.Alert;
            _callEms.Enabled = false;
            //Blips
            _cBlip = _animal.AttachBlip();
            _cBlip.EnableRoute(Color.Red);
            _cBlip.Color = Color.Red;
            _cBlip2 = _victim.AttachBlip();
            _cBlip2.Color = Color.Blue;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            try
            {
                //GamePlay
                if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 30f)
                {
                    _onScene = true;
                    if (Main.UsingCi) Wrapper.CiSendMessage(this, "Officer on scene.");
                    _callEms.Enabled = true;
                    _cBlip.DisableRoute();
                    _animal.Tasks.FightAgainst(_victim, -1);
                    _victim.Tasks.ReactAndFlee(_animal);
                }

                //Keybinds
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
                _interaction.ProcessMenus();
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
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
            if (_victim.Exists()) _victim.Dismiss();
            if (_animal.Exists()) _animal.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            _mainMenu.Visible = false;
            CFunctions.Code4Message();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            if (Main.UsingCi) Wrapper.CiSendMessage(this, "Scene is code4.");
            base.End();
        }

        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _callEms)
            {
                Game.DisplaySubtitle(
                    "~g~You~s~: Dispatch, we have a person that has been attacked by an animal! We need a medical crew here ASAP!");
                if (Main.UsingCi)
                    Wrapper.CiSendMessage(this, "EMS has been notified and is on route. 11-78");
                if (Main.UsingUb)
                {
                    Wrapper.CallEms();
                    Wrapper.CallFd();
                }
                else
                {
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                        EBackupUnitType.Ambulance);
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                        EBackupUnitType.Firetruck);
                }

                _callEms.Enabled = false;
            }
            else if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }
    }
}