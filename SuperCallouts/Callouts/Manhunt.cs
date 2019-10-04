#region

using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using SuperCallouts.CustomScenes;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("Manhunt", CalloutProbability.Medium)]
    internal class Manhunt : Callout
    {
        private Ped _bad;
        private Blip _cBlip;
        private Blip _cBlip2;
        private bool _looper;
        private bool _onScene;
        private LHandle _pursuit;
        private Vector3 _searcharea;
        private Vector3 _spawnPoint;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~b~Dispatch:~s~ Wanted suspect on the run.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 SUSPECTS_LAST_SEEN_02 IN_OR_ON_POSITION",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Manhunt callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Manhunt",
                "Search for the suspect. High priority, respond ~r~CODE-3");
            _bad = new Ped(_spawnPoint);
            _bad.IsPersistent = true;
            var position = _bad.Position;
            _bad.Tasks.Wander();
            SimpleFunctions.SetWanted(_bad, true);
            _searcharea = position.Around2D(40f, 75f);
            _cBlip = new Blip(_searcharea, 90f);
            _cBlip.Color = Color.Yellow;
            _cBlip.Alpha = .5f;
            _cBlip.EnableRoute(Color.Yellow);
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_bad) < 80f)
            {
                _onScene = true;
                _pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(_pursuit, _bad);
                Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
            }

            if (!_looper && _onScene && Game.LocalPlayer.Character.DistanceTo(_bad) < 15f)
            {
                _looper = true;
                _cBlip.Delete();
                _cBlip2 = _bad.AttachBlip();
                _cBlip2.Color = Color.Red;
            }

            if (_onScene && !Functions.IsPursuitStillRunning(_pursuit)) End();
            base.Process();
        }

        public override void End()
        {
            if (_bad.Exists()) _bad.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);

            base.End();
        }
    }
}