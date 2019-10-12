using System;
using System.Drawing;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents.SimpleFunctions;

namespace SuperEvents.Events
{
    public class CarFire : AmbientEvent
    {
        private Ped _victim;
        private Blip _cBlip;
        private Vehicle _cVehicle;
        private bool _onScene;
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperEvents", "~y~Choose an option.");
        private readonly UIMenuItem _callFd = new UIMenuItem("~r~ Call Fire Department", "Calls for ambulance and firetruck.");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Call", "Ends the callout early.");
        internal static void Launch()
        {
            var eventBooter = new CarFire();
            eventBooter.StartEvent();
        }
        protected override void StartEvent()
        {
            EFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
            if (_spawnPoint.DistanceTo(Game.LocalPlayer.Character) < 35f) {base.Failed(); return;}
            EFunctions.SpawnAnyCar(out _cVehicle, _spawnPoint);
            EFunctions.Damage(_cVehicle, 200, 200);
            for (var i = 0; i < 5; i++) EFunctions.FireControl(_spawnPoint.Around2D(1f,5f), 24, true);
            _victim = _cVehicle.CreateRandomDriver();
            _victim.IsPersistent = true;
            _victim.Kill();
            //Start UI
            _interaction.Add(_mainMenu);
            _mainMenu.AddItem(_callFd);
            _mainMenu.AddItem(_endCall);
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
            _callFd.SetLeftBadge(UIMenuItem.BadgeStyle.Alert);
            _callFd.Enabled = false;
            //Blips
            if (!Settings.ShowBlips) {base.StartEvent(); return;}
            _cBlip = _cVehicle.AttachBlip();
            _cBlip.Color = Color.Red;
            _cBlip.Scale = .5f;
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
                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 30f)
                        {
                            _onScene = true;
                            _cVehicle.IsOnFire = true;
                            for (var i = 0; i < 10; i++)
                                EFunctions.FireControl(_spawnPoint.Around2D(1f, 5f), 24, false);
                            if (Settings.ShowHints)
                            {
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~Car Fire", "Clear the scene.");
                            }
                            _callFd.Enabled = true;
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");   
                        }                     
                        
                        if (Game.IsKeyDown(Settings.Interact))
                        {
                            _mainMenu.Visible = !_mainMenu.Visible;
                        }

                        if (_cVehicle.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(_cVehicle) > 200) End();
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
                }
            });
            base.MainLogic();
        }
        protected override void End()
        {
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_victim.Exists()) _victim.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            base.End();
        }
        
        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _callFd)
            {
                Game.DisplaySubtitle("~g~You~s~: Dispatch, we got a large vehicle fire that's spreading. Looks like ~r~someone is inside!~s~ I need a rescue crew out here!");
                try
                {
                    //UltimateBackup.API.Functions.callAmbulance(); EMS just catches fire...
                    UltimateBackup.API.Functions.callFireDepartment();
                }
                catch (Exception e)
                {
                    Game.LogTrivial("SuperEvents Warning: Ultimate Backup is not installed! Backup was not automatically called!");
                    Game.DisplayHelp("~r~Ultimate Backup is not installed! Backup was not automatically called!", 8000);
                }
                _callFd.Enabled = false;
            }
            else if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Event Ended.");
                End();
            }
        }

    }
}
