#region

using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("Aliens", CalloutProbability.Low)]
    internal class Aliens : Callout
    {
        private Ped _alien1;
        private Ped _alien2;
        private Ped _alien3;
        private Blip _cBlip;
        private Vehicle _cVehicle;
        private bool _onScene;
        private Vector3 _searcharea;
        private Vector3 _spawnPoint;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~b~Dispatch:~s~ Reports of aliens spotted. Possible hoax.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 SUSPECTS_LAST_SEEN_02 IN_OR_ON_POSITION",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Aliens callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Alien Sighting",
                "Investigate the scene. Low priority, respond ~y~CODE-2");
            _cVehicle = new Vehicle("DUNE2", _spawnPoint);
            _cVehicle.IsPersistent = true;
            _cVehicle.IsEngineOn = true;
            _alien1 = new Ped("S_M_M_MOVALIEN_01", _cVehicle.Position.Around(3f), 0f);
            _alien1.SetVariation(0, 0, 0);
            _alien1.SetVariation(3, 0, 0);
            _alien1.SetVariation(4, 0, 0);
            _alien1.SetVariation(5, 0, 0);
            _alien1.IsPersistent = true;
            _alien2 = new Ped("S_M_M_MOVALIEN_01", _cVehicle.Position.Around(3f), 0f);
            _alien2.SetVariation(0, 0, 0);
            _alien2.SetVariation(3, 0, 0);
            _alien2.SetVariation(4, 0, 0);
            _alien2.SetVariation(5, 0, 0);
            _alien2.IsPersistent = true;
            _alien3 = new Ped("S_M_M_MOVALIEN_01", _cVehicle.Position.Around(3f), 0f);
            _alien3.SetVariation(0, 0, 0);
            _alien3.SetVariation(3, 0, 0);
            _alien3.SetVariation(4, 0, 0);
            _alien3.SetVariation(5, 0, 0);
            _alien3.IsPersistent = true;
            var position = _cVehicle.Position;
            _searcharea = position.Around2D(40f, 75f);
            _cBlip = new Blip(_searcharea, 80f);
            _cBlip.Color = Color.Yellow;
            _cBlip.Alpha = .5f;
            _cBlip.EnableRoute(Color.Yellow);
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 20f)
                GameFiber.StartNew(delegate
                {
                    _onScene = true;
                    _cBlip.DisableRoute();
                    NativeFunction.CallByName<uint>("TASK_GO_TO_ENTITY", _alien1, Game.LocalPlayer.Character, -1, 2f, 2f,
                        0, 0);
                    NativeFunction.CallByName<uint>("TASK_GO_TO_ENTITY", _alien2, Game.LocalPlayer.Character, -1, 2f, 2f,
                        0, 0);
                    NativeFunction.CallByName<uint>("TASK_GO_TO_ENTITY", _alien3, Game.LocalPlayer.Character, -1, 2f, 2f,
                        0, 0);
                    GameFiber.Wait(4000);
                    _alien1.Velocity = new Vector3(0, 0, 70);
                    GameFiber.Wait(500);
                    _alien2.Velocity = new Vector3(0, 0, 70);
                    GameFiber.Wait(500);
                    _alien3.Velocity = new Vector3(0, 0, 70);
                    GameFiber.Wait(500);
                    _cVehicle.Velocity = new Vector3(0, 0, 70);
                    GameFiber.Wait(500);
                    End();
                });
            base.Process();
        }

        public override void End()
        {
            Game.DisplaySubtitle("~g~Me:~s~ The hell was that? I think I need a nap..");
            if (_alien1.Exists()) _alien1.Delete();
            if (_alien2.Exists()) _alien2.Delete();
            if (_alien3.Exists()) _alien3.Delete();
            if (_cVehicle.Exists()) _cVehicle.Delete();
            if (_cBlip.Exists()) _cBlip.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}