using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;

namespace SuperCallouts2.Callouts
{
    [CalloutInfo("Aliens", CalloutProbability.Low)]
    internal class Aliens : Callout
    {
        #region Variables
        private Ped _alien1;
        private Ped _alien2;
        private Ped _alien3;
        private Blip _cBlip1;
        private Vehicle _cVehicle1;
        private Vector3 _spawnPoint;
        private bool _onScene;
        #endregion
        
        //TODO: Remake to be extra spooky
        /*
         * Idea: Spooky music for a few seconds,
         * Take control away from player, walk to center of bright light, 3 aliens walk up around all sides,
         * they then get sucked up into the light, player ragdolls and light fades away. Backup arrives and
         * that intro plays, then the backup leaves and the callout ends. Spo0ky
         */
        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~b~Dispatch:~s~ Reports of strange people.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 SUSPECTS_LAST_SEEN_02 IN_OR_ON_POSITION",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Aliens callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Alien Sighting",
                "Caller claims that the subjects are aliens. Low priority, respond ~y~CODE-2");
            _cVehicle1 = new Vehicle("DUNE2", _spawnPoint);
            _cVehicle1.IsPersistent = true;
            _cVehicle1.IsEngineOn = true;
            _alien1 = new Ped("S_M_M_MOVALIEN_01", _cVehicle1.Position.Around(5f), 0f);
            _alien1.SetVariation(0, 0, 0);
            _alien1.SetVariation(3, 0, 0);
            _alien1.SetVariation(4, 0, 0);
            _alien1.SetVariation(5, 0, 0);
            _alien1.IsPersistent = true;
            _alien2 = new Ped("S_M_M_MOVALIEN_01", _cVehicle1.Position.Around(5f), 0f);
            _alien2.SetVariation(0, 0, 0);
            _alien2.SetVariation(3, 0, 0);
            _alien2.SetVariation(4, 0, 0);
            _alien2.SetVariation(5, 0, 0);
            _alien2.IsPersistent = true;
            _alien3 = new Ped("S_M_M_MOVALIEN_01", _cVehicle1.Position.Around(5f), 0f);
            _alien3.SetVariation(0, 0, 0);
            _alien3.SetVariation(3, 0, 0);
            _alien3.SetVariation(4, 0, 0);
            _alien3.SetVariation(5, 0, 0);
            _alien3.IsPersistent = true;
            var position = _cVehicle1.Position;
            var searcharea = position.Around2D(40f, 75f);
            _cBlip1 = new Blip(searcharea, 80f) {Color = Color.Yellow, Alpha = .5f};
            _cBlip1.EnableRoute(Color.Yellow);
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 20f)
                GameFiber.StartNew(delegate
                {
                    _onScene = true;
                    _cBlip1.DisableRoute();
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
                    _cVehicle1.Velocity = new Vector3(0, 0, 70);
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
            if (_cVehicle1.Exists()) _cVehicle1.Delete();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}