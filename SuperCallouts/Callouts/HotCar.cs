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
    [CalloutInfo("HotCar", CalloutProbability.Medium)]
    internal class HotCar : Callout
    {
        private Ped _caller;
        private Blip _cBlip;
        private Tuple<Vector3, float> _chosenSpawnData;
        private Vehicle _cVehicle;
        private Ped _doggo;
        private bool _endCall;
        private bool _looper;
        private bool _onScene;
        private Ped _owner;
        private LHandle _pursuit;
        private readonly Random _rNd = new Random();
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
            //SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(450f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~r~911 Report:~s~ Animal(s) locked inside a vehicle.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "CITIZENS_REPORT_03 CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override void OnCalloutNotAccepted()
        {
            End();
            base.OnCalloutNotAccepted();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: HotCar callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Animal Abuse",
                "3rd party reports a dog locked in a hot vehicle. Speak with the caller on scene, respond ~y~CODE-2");
            SimpleFunctions.SpawnNormalCar(out _cVehicle, _spawnPoint);
            _cVehicle.IsPersistent = true;
            _cVehicle.Heading = _spawnPointH;
            _cBlip = _cVehicle.AttachBlip();
            _cBlip.Color = Color.Yellow;
            _cBlip.EnableRoute(Color.Yellow);
            _caller = new Ped(_cVehicle.GetOffsetPositionRight(1f));
            _caller.IsPersistent = true;
            Model[] animal = {"a_c_rottweiler", "a_c_retriever", "a_c_poodle", "a_c_husky", "a_c_cat_01"};
            _doggo = new Ped(animal[new Random().Next(animal.Length)], _spawnPoint, 50);
            _doggo.IsPersistent = true;
            _doggo.BlockPermanentEvents = true;
            _doggo.WarpIntoVehicle(_cVehicle, 0);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle) < 20f)
            {
                _onScene = true;
                Game.DisplayHelp("To speak with caller, get close & press: " + Settings.Interact, 10000);
                _cBlip.DisableRoute();
            }

            if (!_looper && _onScene && Game.IsKeyDown(Settings.Interact))
                GameFiber.StartNew(delegate
                {
                    _pursuit = Functions.CreatePursuit();
                    var pPos = Game.LocalPlayer.Character.GetOffsetPositionFront(-5);
                    _looper = true;
                    var choices = _rNd.Next(1, 3);
                    switch (choices)
                    {
                        case 1:
                            Game.DisplaySubtitle(
                                "~g~Caller:~w~ Hi officer! I have to go but I found this dog, I don't know who the owner is!",
                                3000);
                            GameFiber.Wait(6000);
                            _caller.Dismiss();
                            _owner = new Ped(pPos);
                            _owner.IsPersistent = true;
                            _owner.Tasks.EnterVehicle(_cVehicle, -1);
                            Game.DisplaySubtitle("~r~Owner:~w~ I was only gone a second! Don't tow my car!!", 5000);
                            GameFiber.Wait(6000);
                            _owner.WarpIntoVehicle(_cVehicle, -1);
                            Functions.AddPedToPursuit(_pursuit, _owner);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            _endCall = true;
                            break;
                        case 2:
                            Game.DisplaySubtitle(
                                "~g~Caller:~w~ Hi officer! I have to go but I found this dog, I don't know who the owner is!",
                                3000);
                            GameFiber.Wait(6000);
                            _caller.Dismiss();
                            _owner = new Ped(pPos);
                            _owner.IsPersistent = true;
                            _owner.Tasks.EnterVehicle(_cVehicle, -1);
                            Game.DisplaySubtitle("~r~Owner:~w~ I'm sorry I was only gone a few minutes.", 5000);
                            //GameFiber.Wait(4000);
                            //Owner.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);// This might be the issue
                            GameFiber.Wait(10000);
                            End();
                            break;
                        case 3:
                            Game.DisplaySubtitle(
                                "~g~Caller:~w~ Hi officer! I found this dog, I don't know who the owner is!", 3000);
                            GameFiber.Wait(3001);
                            Game.DisplaySubtitle(
                                "~g~Caller:~w~ I work for an animal shelter down the street, I'll take them with me for you.",
                                3000);
                            GameFiber.Wait(3000);
                            _doggo.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                            GameFiber.Wait(4000);
                            _doggo.Tasks.Wander();
                            _caller.Tasks.Wander();
                            GameFiber.Wait(6000);
                            End();
                            break;
                        default:
                            Game.DisplayNotification(
                                "An error has been detected! Ending callout early to prevent LSPDFR crash!");
                            End();
                            break;
                    }
                });
            if (_endCall)
                if (!Functions.IsPursuitStillRunning(_pursuit))
                    End();
            base.Process();
        }

        public override void End()
        {
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            if (_doggo.Exists()) _doggo.Dismiss();
            if (_owner.Exists()) _owner.Dismiss();
            if (_caller.Exists()) _caller.Dismiss();
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            base.End();
        }
    }
}