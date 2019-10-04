#region

using System.Drawing;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.CustomScenes;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("HitAndRun", CalloutProbability.Medium)]
    internal class HitRun : Callout
    {
        private Ped _bad1;
        private Ped _bad2;
        private Blip _cBlip1;
        private Blip _cBlip2;
        private Blip _cBlip3;
        private MenuPool _conversation;
        private Vehicle _cVehicle1;
        private Vehicle _cVehicle2;
        private UIMenu _mainMenu;
        private bool _nearBad;
        private bool _nIce;
        private bool _onScene;
        private LHandle _pursuit;
        private Vector3 _spawnPoint;
        private Vector3 _spawnPointOffset;
        private UIMenuItem _startConv;
        private Ped _victim;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(450f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 40f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~r~911 Report:~s~ Vehicle hit and run.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_HIT_AND_RUN_01 IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public void LetsChatBois(UIMenu unUn, UIMenuItem selItem, int nanana)
        {
            if (selItem == _startConv)
                GameFiber.StartNew(delegate
                {
                    _startConv.Enabled = false;
                    NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _victim, Game.LocalPlayer.Character,
                        -1);
                    Game.DisplaySubtitle("~g~Me: ~w~What happened? Are you ok?", 4000);
                    GameFiber.Wait(4000);
                    Game.DisplaySubtitle("~y~Victim: ~w~Someone slammed into my car, and when I got out he drove off!",
                        4000);
                    GameFiber.Wait(4000);
                    Game.DisplaySubtitle("~g~Me: ~w~Did you see any details? Car type, color, or where they went?",
                        4000);
                    GameFiber.Wait(4000);
                    Game.DisplaySubtitle(
                        "~y~Victim: ~w~I can't remember, I did tell the 911 lady their licence number though.", 4000);
                    GameFiber.Wait(4000);
                    _cBlip1.Delete();
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Dispatch", "Suspect Spotted",
                        "ANPR systems have located the vehicle. Location has been sent to your GPS.");
                    _cBlip2 = _bad1.AttachBlip();
                    _cBlip2.EnableRoute(Color.Red);
                    _cBlip2.Color = Color.Red;
                    _cBlip2 = _bad1.AttachBlip();
                    _cBlip2.Color = Color.Red;
                    _nIce = true;
                    _mainMenu.Visible = false;
                    _nIce = true;
                });
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Hit And Run callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Car Accident",
                "Victim reports the other driver have left the scene. Get to the victim as soon as possible.");
            SimpleFunctions.SpawnNormalCar(out _cVehicle1, _spawnPoint);
            _cVehicle1.IsPersistent = true;
            _spawnPointOffset = _spawnPoint.Around(10f);
            SimpleFunctions.SpawnNormalCar(out _cVehicle2, _spawnPointOffset);
            _cVehicle2.IsPersistent = true;
            SimpleFunctions.Damage(_cVehicle2, 200, 200);
            SimpleFunctions.Damage(_cVehicle1, 50, 50);
            _victim = _cVehicle1.CreateRandomDriver();
            _victim.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
            _bad1 = _cVehicle2.CreateRandomDriver();
            _bad2 = new Ped(_spawnPoint);
            _bad2.WarpIntoVehicle(_cVehicle2, 0);
            _bad1.Tasks.CruiseWithVehicle(5f);
            Functions.SetVehicleOwnerName(_cVehicle1, Functions.GetPersonaForPed(_victim).FullName);
            Functions.SetVehicleOwnerName(_cVehicle2, Functions.GetPersonaForPed(_bad1).FullName);
            _cBlip1 = _victim.AttachBlip();
            _cBlip1.EnableRoute(Color.Yellow);
            _cBlip1.Color = Color.Yellow;
            _conversation = new MenuPool();
            _mainMenu = new UIMenu("Conversation", "Choose an option");
            _conversation.Add(_mainMenu);
            _mainMenu.AddItem(_startConv = new UIMenuItem("What happened?"));
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += LetsChatBois;
            _pursuit = Functions.CreatePursuit();
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            _conversation.ProcessMenus();
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 20f)
            {
                _onScene = true;
                Game.DisplayHelp("Press " + Settings.Interact + " to speak with the victim.");
            }

            if (Game.IsKeyDown(Settings.Interact) && !_nIce && _onScene) _mainMenu.Visible = !_mainMenu.Visible;
            if (!_nearBad && _nIce && Game.LocalPlayer.Character.DistanceTo(_cVehicle2) < 50f)
            {
                _nearBad = true;
                Functions.AddPedToPursuit(_pursuit, _bad1);
                Functions.AddPedToPursuit(_pursuit, _bad2);
                Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                _cBlip2.DisableRoute();
                _cBlip3 = _bad2.AttachBlip();
                _cBlip3.Color = Color.Red;
                Functions.AddPedContraband(_bad1, ContrabandType.Narcotics, "COCAINE");
                Functions.AddPedContraband(_bad2, ContrabandType.Narcotics, "COCAINE");
            }

            if (!Functions.IsPursuitStillRunning(_pursuit) && _nearBad) End();
            base.Process();
        }

        public override void End()
        {
            if (_bad1.Exists()) _bad1.Dismiss();
            if (_bad2.Exists()) _bad2.Dismiss();
            if (_victim.Exists()) _victim.Dismiss();
            if (_cVehicle1.Exists()) _cVehicle1.Dismiss();
            if (_cVehicle2.Exists()) _cVehicle2.Dismiss();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            if (_cBlip3.Exists()) _cBlip3.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}