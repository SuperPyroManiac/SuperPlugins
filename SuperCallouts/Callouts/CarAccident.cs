#region

using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using SuperCallouts.CustomScenes;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("CarAccident", CalloutProbability.High)]
    internal class CarAccident : Callout
    {
        private Blip _cBlip;
        private Vehicle _cVehicle;
        private bool _onScene;
        private Vector3 _spawnPoint;
        private Ped _victim;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(450f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~r~911 Report:~s~ Possible car accident.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_HIT_AND_RUN_03 UNITS_RESPOND_CODE_03_01",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: CarAccident callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Car Accident",
                "Someone reported a wrecked vehicle. Get to the scene as soon as possible!");
            SimpleFunctions.SpawnAnyCar(out _cVehicle, _spawnPoint);
            _cVehicle.IsPersistent = true;
            SimpleFunctions.Damage(_cVehicle, 200, 200);
            _victim = _cVehicle.CreateRandomDriver();
            _victim.IsPersistent = true;
            _victim.BlockPermanentEvents = true;
            _victim.Kill();
            _cBlip = _victim.AttachBlip();
            _cBlip.EnableRoute(Color.Yellow);
            _cBlip.Scale = .75f;
            _cBlip.Color = Color.Yellow;
            Game.DisplaySubtitle("Get to the ~r~scene~w~!", 10000);
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle.Position) < 30f)
            {
                Game.DisplayHelp("Investigate and clear the scene.", 5000);
                _cBlip.Delete();
                _onScene = true;
            }

            if (_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle.Position) > 100f) End();
            base.Process();
        }

        public override void End()
        {
            if (_victim.Exists()) _victim.Dismiss();
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}