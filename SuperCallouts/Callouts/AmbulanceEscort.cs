#region

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("AmbulanceEscort", CalloutProbability.High)]
    internal class AmbulanceEscort : Callout
    {
        private Blip _cBlip;
        private Blip _cBlip2;
        private Vehicle _cVehicle;
        private Vector3 _daRulez;
        private Ped _doc1;
        private Ped _doc2;
        private bool _onScene;
        private Vector3 _spawnPoint;
        private readonly List<Vector3> _sPots = new List<Vector3>();
        private Ped _victim;

        public override bool OnBeforeCalloutDisplayed()
        {
            _sPots.Add(new Vector3(1825, 3692, 34));
            _sPots.Add(new Vector3(-454, -339, 34));
            _sPots.Add(new Vector3(293, -1438, 29));
            _sPots.Add(new Vector3(-232, 6316, 30));
            _sPots.Add(new Vector3(294, -1439, 29));
            _daRulez = _sPots.OrderBy(x => x.DistanceTo(Game.LocalPlayer.Character.Position)).FirstOrDefault();
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~b~Dispatch:~s~ Ambulance requests police escort.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_AMBULANCE_REQUESTED_01 IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Ambulance Escort callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Escort Ambulance",
                "Follow the ambulance to the hospital. Ensure traffic is out of the way.");
            _cVehicle = new Vehicle("AMBULANCE", _spawnPoint);
            _cVehicle.IsPersistent = true;
            _doc1 = new Ped("s_m_m_paramedic_01", _spawnPoint, 0f);
            _doc2 = new Ped("s_m_m_paramedic_01", _spawnPoint, 0f);
            _doc1.WarpIntoVehicle(_cVehicle, -1);
            _doc2.WarpIntoVehicle(_cVehicle, 0);
            _doc1.IsPersistent = true;
            _doc2.IsPersistent = true;
            _victim = new Ped();
            _victim.WarpIntoVehicle(_cVehicle, 1);
            _victim.IsPersistent = true;
            _cBlip = _cVehicle.AttachBlip();
            _cBlip.EnableRoute(Color.Green);
            _cBlip.Color = Color.Green;
            _cVehicle.IsSirenOn = true;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (Game.LocalPlayer.Character.DistanceTo(_cVehicle) < 30f && !_onScene)
            {
                _onScene = true;
                _cBlip.DisableRoute();
                _doc1.Tasks.DriveToPosition(_cVehicle, _daRulez, 20f, VehicleDrivingFlags.Emergency, 10f);
                _cBlip2 = new Blip(_daRulez);
                _cBlip2.EnableRoute(Color.Blue);
                _cBlip2.Color = Color.Blue;
            }

            if (_cVehicle.DistanceTo(_daRulez) < 10f && _onScene)
            {
                _cVehicle.IsSirenSilent = true;
                _doc1.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
                _doc2.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
                _victim.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
                End();
            }

            base.Process();
        }

        public override void End()
        {
            if (_doc1.Exists()) _doc1.Dismiss();
            if (_doc2.Exists()) _doc2.Dismiss();
            if (_victim.Exists()) _victim.Dismiss();
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}