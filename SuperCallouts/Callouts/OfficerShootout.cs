#region

using System;
using System.Drawing;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using SuperCallouts.CustomScenes;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("OfficerShootout", CalloutProbability.Medium)]
    internal class OfficerShootout : Callout
    {
        private Ped _bad1;
        private Ped _bad2;
        private Blip _cBlip;
        private Tuple<Vector3, float> _chosenSpawnData;
        private Ped _cop1;
        private Ped _cop2;
        private Vehicle _copVehicle;
        private Vector3 _cSpawnPoint;
        private Vehicle _cVehicle;
        private bool _onScene;
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
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~b~Dispatch:~s~ Felony stop. Shots fired.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_SHOTS_FIRED_AT_AN_OFFICER_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_02",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Officer Shootout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Officer Shot",
                "Officer reports shots fired during felony stop, panic button hit. Respond ~r~CODE-99 EMERGENCY");
            SimpleFunctions.SpawnNormalCar(out _cVehicle, _spawnPoint);
            _cVehicle.Heading = _spawnPointH;
            _cVehicle.IsPersistent = true;
            _cSpawnPoint = _cVehicle.GetOffsetPositionFront(-9f);
            _copVehicle = new Vehicle("POLICE", _cSpawnPoint);
            _copVehicle.IsPersistent = true;
            _copVehicle.Heading = _spawnPointH;
            _copVehicle.IsSirenOn = true;
            _copVehicle.IsSirenSilent = true;
            _bad1 = new Ped();
            _bad1.IsPersistent = true;
            _bad1.Health = 400;
            _bad1.Inventory.Weapons.Add(WeaponHash.AssaultShotgun).Ammo = -1;
            _bad2 = new Ped();
            _bad2.IsPersistent = true;
            _bad2.Health = 400;
            _bad2.Inventory.Weapons.Add(WeaponHash.CarbineRifle).Ammo = -1;
            _bad1.WarpIntoVehicle(_cVehicle, -1);
            _bad2.WarpIntoVehicle(_cVehicle, 0);
            _cop1 = new Ped("s_m_y_cop_01", _spawnPoint, 0f);
            _cop1.IsPersistent = true;
            _cop1.WarpIntoVehicle(_copVehicle, -1);
            _cop1.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
            _cop2 = new Ped("s_f_y_cop_01", _spawnPoint, 0f);
            _cop2.IsPersistent = true;
            _cop2.WarpIntoVehicle(_copVehicle, 0);
            _cop2.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
            SimpleFunctions.SetWanted(_bad1, true);
            SimpleFunctions.SetWanted(_bad2, true);
            _cVehicle.IsStolen = true;
            _bad1.RelationshipGroup = new RelationshipGroup("BADGANG");
            _bad2.RelationshipGroup = new RelationshipGroup("BADGANG");
            _bad1.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.LeaveDoorOpen);
            _bad2.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.LeaveDoorOpen);
            _cop1.Tasks.LeaveVehicle(_copVehicle, LeaveVehicleFlags.LeaveDoorOpen);
            _cop2.Tasks.LeaveVehicle(_copVehicle, LeaveVehicleFlags.LeaveDoorOpen);
            _cBlip = _copVehicle.AttachBlip();
            _cBlip.Color = Color.Red;
            _cBlip.EnableRoute(Color.Red);
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_copVehicle.Position) < 50f)
            {
                _onScene = true;
                _cop1.Tasks.FightAgainst(_bad1, 60000);
                _bad1.Tasks.FightAgainst(_cop1, 60000);
                Functions.PlayScannerAudioUsingPosition("REQUEST_BACKUP", _spawnPoint);
                Game.SetRelationshipBetweenRelationshipGroups("BADGANG", "COP", Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups("BADGANG", "PLAYER", Relationship.Hate);
                Functions.RequestBackup(_copVehicle.Position, EBackupResponseType.Code3,
                    EBackupUnitType.LocalUnit);
                Functions.RequestBackup(_copVehicle.Position, EBackupResponseType.Code3,
                    EBackupUnitType.LocalUnit);
                _cBlip.DisableRoute();
            }

            if (_onScene && _bad1.IsDead && _bad2.IsDead) End();
            if (_onScene && Game.LocalPlayer.Character.DistanceTo(_bad1) > 65f &&
                Game.LocalPlayer.Character.DistanceTo(_bad2) > 65f) End();
            base.Process();
        }

        public override void End()
        {
            if (_cop1.Exists()) _cop1.Dismiss();
            if (_cop2.Exists()) _cop2.Dismiss();
            if (_bad1.Exists()) _bad1.Dismiss();
            if (_bad2.Exists()) _bad2.Dismiss();
            if (_cVehicle.Exists()) _cVehicle.Dismiss();
            if (_copVehicle.Exists()) _copVehicle.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}