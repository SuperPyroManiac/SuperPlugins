#region

using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using SuperCallouts.CustomScenes;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("Fire", CalloutProbability.High)]
    internal class Fire : Callout
    {
        private Blip _cBlip;
        private Vehicle _cVehicle;
        private bool _onScene;
        private Vector3 _spawnPoint;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~b~Dispatch:~s~ Reports of a fire";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: fire callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Fire",
                "Reports of a fire, respond ~r~CODE-3");
            SimpleFunctions.SpawnAnyCar(out _cVehicle, _spawnPoint);
            SimpleFunctions.Damage(_cVehicle, 200, 200);
            for (var i = 0; i < 5; i++) SimpleFunctions.FireControl(_spawnPoint.Around2D(1f,5f), 24, true);
            _cVehicle.IsPersistent = true;
            _cBlip = _cVehicle.AttachBlip();
            _cBlip.EnableRoute(Color.Red);
            _cBlip.Color = Color.Red;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle) < 60f)
            {
                _onScene = true;
                _cVehicle.Explode();
                for (var i = 0; i < 10; i++) SimpleFunctions.FireControl(_spawnPoint.Around2D(1f,5f), 24, false);
                _cBlip.DisableRoute();
                Game.DisplayHelp("Put out the fire, or call the fire department!");
            }

            if (_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle) > 120) End();
            base.Process();
        }

        public override void End()
        {
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}