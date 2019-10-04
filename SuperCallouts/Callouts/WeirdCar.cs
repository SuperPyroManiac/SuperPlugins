#region

using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using SuperCallouts.CustomScenes;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("WeirdCar", CalloutProbability.High)]
    internal class WeirdCar : Callout
    {
        private Ped _bad;
        private Blip _cBlip;
        private Tuple<Vector3, float> _chosenSpawnData;
        private Vehicle _cVehicle;
        private bool _onScene;
        private readonly Random _rNd = new Random();
        private readonly Random _rNd2 = new Random();
        private readonly TupleList<Vector3, float> _sideOfRoads = new TupleList<Vector3, float>();
        private Vector3 _spawnPoint;
        private float _spawnPointH;

        public override bool OnBeforeCalloutDisplayed()
        {
            foreach (var tuple in SimpleFunctions.SideOfRoad)
                if (Vector3.Distance(tuple.Item1, Game.LocalPlayer.Character.Position) < 750f &&
                    Vector3.Distance(tuple.Item1, Game.LocalPlayer.Character.Position) > 280f)
                    _sideOfRoads.Add(tuple);
            if (_sideOfRoads.Count == 0) return false;
            _chosenSpawnData = _sideOfRoads[_rNd.Next(_sideOfRoads.Count)];
            _spawnPoint = _chosenSpawnData.Item1;
            _spawnPointH = _chosenSpawnData.Item2;
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~b~Dispatch:~s~ Suspicious vehicle.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Wierd Car callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Suspicious Vehicle",
                "Report of a suspicious vehicle on the side of the road. Respond ~y~CODE-2");
            SimpleFunctions.SpawnNormalCar(out _cVehicle, _spawnPoint);
            _cVehicle.Heading = _spawnPointH;
            _cVehicle.IsPersistent = true;
            _cBlip = _cVehicle.AttachBlip();
            _cBlip.EnableRoute(Color.Yellow);
            _cBlip.Color = Color.Yellow;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle) < 30f)
            {
                _onScene = true;
                _cBlip.DisableRoute();
                Game.DisplayHelp("Investigate the vehicle.");
                var choices = _rNd2.Next(1, 4);
                switch (choices)
                {
                    case 1:
                        SimpleFunctions.Damage(_cVehicle, 500, 500);
                        _cVehicle.IsStolen = true;
                        break;
                    case 2:
                        GameFiber.StartNew(delegate
                        {
                            _cVehicle.IsStolen = true;
                            _bad = _cVehicle.CreateRandomDriver();
                            _bad.IsPersistent = true;
                            _bad.BlockPermanentEvents = true;
                            _bad.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                            Game.DisplaySubtitle("~r~Driver:~s~ The world will end with fire!..");
                            GameFiber.Wait(5000);
                            _cVehicle.Explode();
                        });
                        break;
                    case 3:
                        _bad = _cVehicle.CreateRandomDriver();
                        _bad.IsPersistent = true;
                        _bad.BlockPermanentEvents = true;
                        SimpleFunctions.SetWanted(_bad, true);
                        _cVehicle.IsStolen = true;
                        break;
                    default:
                        Game.DisplayNotification(
                            "An error has been detected! Ending callout early to prevent LSPDFR crash!");
                        End();
                        break;
                }
            }

            if (_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle) > 50f) End();
            base.Process();
        }

        public override void End()
        {
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_bad.Exists()) _bad.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}