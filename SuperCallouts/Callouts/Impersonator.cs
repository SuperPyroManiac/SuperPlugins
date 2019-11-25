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
    [CalloutInfo("Impersonator", CalloutProbability.High)]
    internal class Impersonator : Callout
    {
        private Ped _bad;
        private Blip _cBlip;
        private bool _chatter;
        private Tuple<Vector3, float> _chosenSpawnData;
        private Vector3 _cSpawnPoint;
        private Vehicle _cVehicle;
        private Vehicle _cVehicle2;
        private bool _onScene;
        private LHandle _pursuit;
        private bool _pursuitScene;
        private readonly Random _rNd = new Random();
        private readonly Random _rNd2 = new Random();
        private readonly TupleList<Vector3, float> _sideOfRoads = new TupleList<Vector3, float>();
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        private Ped _victim;

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
            CalloutMessage = "~b~Dispatch:~s~ Officer impersonator.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Officer Impersonator callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Suspicious Pullover",
                "911 call of someone being pulled over by a non uniformed officer. Description does not match our department for undercover cops. Respond ~r~CODE-3");
            SimpleFunctions.SpawnNormalCar(out _cVehicle, _spawnPoint);
            _cVehicle.Heading = _spawnPointH;
            _cVehicle.IsPersistent = true;
            _cSpawnPoint = _cVehicle.GetOffsetPositionFront(-9f);
            _cVehicle2 = new Vehicle("DILETTANTE2", _cSpawnPoint);
            _cVehicle2.Heading = _spawnPointH;
            _cVehicle2.IsPersistent = true;
            //cVehicle2.IsSirenOn = true;
            //cVehicle2.IsSirenSilent = true;
            _victim = _cVehicle.CreateRandomDriver();
            _victim.IsPersistent = true;
            _bad = _cVehicle2.CreateRandomDriver();
            _bad.IsPersistent = true;
            _cBlip = _cVehicle2.AttachBlip();
            _cBlip.EnableRoute(Color.Red);
            _cBlip.Color = Color.Red;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle2) < 30f)
            {
                _onScene = true;
                _cBlip.DisableRoute();
                _victim.Tasks.CruiseWithVehicle(10f, VehicleDrivingFlags.Normal);
                _pursuit = Functions.CreatePursuit();
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Suspicious Pullover",
                    "Be advised, caller has been instructed to leave scene by 911 operator.");
                var choices = _rNd2.Next(1, 4);
                switch (choices)
                {
                    case 1:
                        Game.DisplayHelp("Suspect is fleeing!");
                        Functions.AddPedToPursuit(_pursuit, _bad);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        _pursuitScene = true;
                        //cVehicle2.IsSirenOn = false;
                        break;
                    case 2:
                        GameFiber.StartNew(delegate
                        {
                            _bad.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(1500);
                            _bad.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
                            GameFiber.Wait(3000);
                            _bad.Tasks.FightAgainst(Game.LocalPlayer.Character, -1);
                            //cVehicle2.IsSirenOn = false;
                        });
                        break;
                    case 3:
                        _chatter = true;
                        //cVehicle2.IsSirenOn = false;

                        break;
                    default:
                        Game.DisplayNotification(
                            "An error has been detected! Ending callout early to prevent LSPDFR crash!");
                        End();
                        break;
                }
            }

            if (_chatter && Game.LocalPlayer.Character.DistanceTo(_bad) < 5f)
            {
                _chatter = false;
                Game.DisplaySubtitle("~y~Suspect:~s~ I didn't do anything wrong!");
            }

            if (_pursuitScene && !Functions.IsPursuitStillRunning(_pursuit)) End();
            if (_onScene && _bad.IsDead) End();
            if (_onScene && !_pursuitScene && Game.LocalPlayer.Character.DistanceTo(_bad) > 50f) End();
            base.Process();
        }

        public override void End()
        {
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_cVehicle2.Exists()) _cVehicle2.Dismiss();
            if (_victim.Exists()) _victim.Dismiss();
            if (_bad.Exists()) _bad.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}