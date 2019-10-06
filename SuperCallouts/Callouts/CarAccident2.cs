#region

using System.Drawing;
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
    [CalloutInfo("CarAccident2", CalloutProbability.High)]
    internal class CarAccident2 : Callout
    {
        private Blip _cBlip;
        private Blip _cBlip2;
        private MenuPool _conversation;
        private Vehicle _cVehicle;
        private Vehicle _cVehicle2;
        private UIMenu _mainMenu;
        private bool _nIce;
        private bool _onScene;
        private Vector3 _spawnPoint;
        private Vector3 _spawnPointoffset;
        private UIMenuItem _startConv;
        private UIMenuItem _startConv2;
        private Ped _victim;
        private Ped _victim2;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(450f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
            CalloutMessage = "~r~911 Report:~s~ Possible car accident.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "CITIZENS_REPORT_04 CRIME_AMBULANCE_REQUESTED_03 UNITS_RESPOND_CODE_03_01", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        private void LetsChatBois(UIMenu unUn, UIMenuItem selItem, int nanana)
        {
            if (selItem == _startConv)
            {
                Game.DisplaySubtitle("~g~Me: ~w~Break it up, stop fighting now! Put your hands where I can see them!",
                    5000);
                _victim.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                _victim2.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                _startConv.Enabled = false;
            }

            if (selItem == _startConv2)
                GameFiber.StartNew(delegate
                {
                    _startConv2.Enabled = false;
                    _startConv.Enabled = false;
                    NativeFunction.CallByName<uint>("TASK_LOOK_AT_ENTITY", _victim, Game.LocalPlayer.Character, -1, 2048,
                        3);
                    NativeFunction.CallByName<uint>("TASK_LOOK_AT_ENTITY", _victim2, Game.LocalPlayer.Character, -1,
                        2048, 3);
                    Game.DisplaySubtitle("~g~Me: ~w~What happened? Are you all ok?", 4000);
                    GameFiber.Wait(4000);
                    Game.DisplaySubtitle("~y~Victim1: ~w~My back is a little sore but I think i'll be ok.", 4000);
                    GameFiber.Wait(4000);
                    Game.DisplaySubtitle(
                        "~r~Victim2: ~w~No im NOT ok, they slammed on the breaks in front of me! My car.. It was new..",
                        4000);
                    GameFiber.Wait(4000);
                    Game.DisplaySubtitle("~g~Me: ~w~Well here is what's going to happ-", 3000);
                    GameFiber.Wait(3000);
                    Game.DisplaySubtitle("~r~Victim2: ~w~You are going to pay for this!! Get over here!", 4000);
                    NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _victim2, _victim, 0, 1);
                    _victim.Tasks.Cower(-1);
                    Game.DisplayHelp("~r~Stop the fight!~g~ Press " + Settings.Interact);
                    _mainMenu.Visible = false;
                    _nIce = true;
                });
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: CarAccident2 callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Car Accident",
                "One of the victims reported an accident involving two cars. Get to the scene as soon as possible!");
            SimpleFunctions.SpawnNormalCar(out _cVehicle, _spawnPoint);
            _cVehicle.IsPersistent = true;
            _cVehicle.EngineHealth = 0;
            _spawnPointoffset = _cVehicle.GetOffsetPosition(new Vector3(0, 7.0f, 0));
            SimpleFunctions.SpawnNormalCar(out _cVehicle2, _spawnPointoffset);
            _cVehicle2.IsPersistent = true;
            _cVehicle2.EngineHealth = 0;
            _cVehicle2.Rotation = new Rotator(0f, 0f, 180f);
            SimpleFunctions.Damage(_cVehicle, 200, 200);
            SimpleFunctions.Damage(_cVehicle2, 200, 200);
            _victim = _cVehicle.CreateRandomDriver();
            _victim.IsPersistent = true;
            _victim.BlockPermanentEvents = true;
            _victim2 = _cVehicle2.CreateRandomDriver();
            _victim2.IsPersistent = true;
            _victim2.BlockPermanentEvents = true;
            _victim.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.LeaveDoorOpen);
            _victim2.Tasks.LeaveVehicle(_cVehicle2, LeaveVehicleFlags.LeaveDoorOpen);
            _victim.Health = 200;
            _victim2.Health = 200;
            NativeFunction.Natives.SET_PED_IS_DRUNK(_victim2, true);
            Functions.SetVehicleOwnerName(_cVehicle, Functions.GetPersonaForPed(_victim).FullName);
            Functions.SetVehicleOwnerName(_cVehicle2, Functions.GetPersonaForPed(_victim2).FullName);
            _cBlip = _victim.AttachBlip();
            _cBlip.EnableRoute(Color.Yellow);
            _cBlip.Scale = .75f;
            _cBlip.Color = Color.Yellow;
            _cBlip2 = _victim2.AttachBlip();
            _cBlip2.Scale = .75f;
            _cBlip2.Color = Color.Yellow;
            Game.DisplaySubtitle("Get to the ~r~scene~w~!", 10000);
            _conversation = new MenuPool();
            _mainMenu = new UIMenu("Conversation", "Choose an option");
            _conversation.Add(_mainMenu);
            _mainMenu.AddItem(_startConv = new UIMenuItem("Stop fighting! Now!"));
            _mainMenu.AddItem(_startConv2 = new UIMenuItem("What happened here?"));
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += LetsChatBois;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            _conversation.ProcessMenus();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle.Position) < 40f)
            {
                Game.DisplayHelp("Investigate and clear the scene.", 5000);
                _cBlip.Delete();
                GameFiber.StartNew(delegate
                {
                    NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _victim, _victim2, 0, 1);
                    NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _victim2, _victim, 0, 1);
                    GameFiber.Wait(3000);
                    _victim.Tasks.Cower(-1);
                    _victim2.Tasks.Wander();
                    Game.DisplayHelp("To speak with the victims, press: " + Settings.Interact);
                });
                _onScene = true;
            }

            if (!_nIce)
                if (Game.IsKeyDown(Settings.Interact) && _onScene)
                {
                    if (Game.LocalPlayer.Character.DistanceTo(_victim) < 10f ||
                        Game.LocalPlayer.Character.DistanceTo(_victim2) < 10f)
                        _mainMenu.Visible = !_mainMenu.Visible;
                    else
                        Game.DisplayHelp("Get closer to speak.");
                }

            if (_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle.Position) > 200f &&
                Game.LocalPlayer.Character.DistanceTo(_cVehicle2.Position) > 200f) End();
            base.Process();
        }

        public override void End()
        {
            if (_victim.Exists()) _victim.Dismiss();
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_victim2.Exists()) _victim2.Dismiss();
            if (_cVehicle2.Exists()) _cVehicle2.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}