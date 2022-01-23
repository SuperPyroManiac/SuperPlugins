using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts
{
    [CalloutInfo("PrisonTransport", CalloutProbability.Medium)]
    internal class PrisonTransport : Callout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 50f);
            CalloutMessage = "~b~Dispatch:~s~ Prisoner escaped transport.";
            CalloutAdvisory = "Officers report a suspect has jumped out of a moving transport vehicle.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("OFFICERS_REPORT_01 CRIME_SUSPECT_ON_THE_RUN_01 IN_OR_ON_POSITION",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: Prison Truck callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Escaped Prisoner",
                "DOC reports a prisoner has unlocked the transport vehicle and is on the run. ~r~10-98");
            if (Main.UsingCi) Wrapper.StartCi(this, "Code 9");
            //cVehicle
            _cVehicle = new Vehicle("POLICET", _spawnPoint) { IsPersistent = true };
            //Cop
            _cop = new Ped("csb_cop", _spawnPoint, 0f);
            _cop.IsPersistent = true;
            _cop.WarpIntoVehicle(_cVehicle, -1);
            _cop.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
            //Bad
            _badguy = new Ped("s_m_y_prisoner_01", _spawnPoint, 0f);
            _badguy.IsPersistent = true;
            _badguy.WarpIntoVehicle(_cVehicle, 1);
            _badguy.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
            //Blips
            _cBlip1 = _cVehicle.AttachBlip();
            _cBlip1.EnableRoute(Color.Red);
            _cBlip1.Color = Color.Red;
            //Start UI
            _mainMenu.MouseControlsEnabled = false;
            _mainMenu.AllowCameraMovement = true;
            _interaction.Add(_mainMenu);
            _mainMenu.AddItem(_endCall);
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            try
            {
                //Gameplay
                if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 90f)
                {
                    _onScene = true;
                    _cBlip1.Delete();
                    _cBlip2 = _badguy.AttachBlip();
                    _cBlip2.Color = Color.Red;
                    _pursuit = Functions.CreatePursuit();
                    var choices = _rNd.Next(1, 3);
                    switch (choices)
                    {
                        case 1:
                            GameFiber.StartNew(delegate
                            {
                                _badguy.Inventory.Weapons.Add(WeaponHash.MicroSMG).Ammo = -1;
                                _badguy.Tasks.FightAgainst(_cop);
                                _badguy.Health = 250;
                                GameFiber.Wait(6000);
                                if (_badguy.IsAlive)
                                {
                                    Functions.AddPedToPursuit(_pursuit, _badguy);
                                    Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                                    if (_cop.IsAlive) _cop.Kill();
                                }
                            });
                            break;
                        case 2:
                            Functions.AddPedToPursuit(_pursuit, _badguy);
                            Functions.AddCopToPursuit(_pursuit, _cop);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
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
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_badguy.Exists()) _badguy.Dismiss();
            if (_cop.Exists()) _cop.Delete();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            _mainMenu.Visible = false;
            CFunctions.Code4Message();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            if (Main.UsingCi) Wrapper.CiSendMessage(this, "Scene clear, Code4");
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

        #region Variables

        private Ped _badguy;
        private Blip _cBlip1;
        private Blip _cBlip2;
        private Ped _cop;
        private Vehicle _cVehicle;
        private bool _onScene;
        private LHandle _pursuit;
        private readonly Random _rNd = new();

        private Vector3 _spawnPoint;

        //UI Items
        private readonly MenuPool _interaction = new();
        private readonly UIMenu _mainMenu = new("SuperCallouts", "~y~Choose an option.");
        private readonly UIMenuItem _endCall = new("~y~End Callout", "Ends the callout early.");

        #endregion
    }
}