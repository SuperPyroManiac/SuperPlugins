#region

using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("HotPursuit", CalloutProbability.Medium)]
    internal class HotPursuit : Callout
    {
        private Ped _passenger;
        private LHandle _pursuit;
        private bool _pursuitCreated;
        private Vector3 _spawnPoint;
        private Ped _suspect;
        private Blip _suspectBlip;
        private Vehicle _suspectVehicle;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
            CalloutMessage = "~o~Traffic ANPR Report:~s~ High value stolen vehicle located.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_RESIST_ARREST IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: HotPursuit callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Stolen Car",
                "ANPR has spotted a stolen vehicle. This vehicle is high performance and has fled before. Respond ~r~CODE-3");
            Model[] vehicleModels = {"ZENTORNO", "TEMPESTA", "AUTARCH"};
            _suspectVehicle = new Vehicle(vehicleModels[new Random().Next(vehicleModels.Length)], _spawnPoint) {IsPersistent = true, IsStolen = true};
            _suspect = _suspectVehicle.CreateRandomDriver();
            _suspect.IsPersistent = true;
            _suspect.BlockPermanentEvents = true;
            _passenger = new Ped();
            _passenger.IsPersistent = true;
            _passenger.BlockPermanentEvents = true;
            _passenger.WarpIntoVehicle(_suspectVehicle, 0);
            _suspectBlip = _suspect.AttachBlip();
            _suspectBlip.Color = Color.Red;
            _suspectBlip.EnableRoute(Color.Red);
            _suspectBlip.IsFriendly = false;
            _suspect.Tasks.CruiseWithVehicle(30f, VehicleDrivingFlags.Emergency);
            Game.DisplaySubtitle("Get to the ~r~reported location~w~!", 10000);
            _suspect.Tasks.CruiseWithVehicle(30f, VehicleDrivingFlags.Normal);
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_pursuitCreated && Game.LocalPlayer.Character.DistanceTo(_suspect.Position) < 50f)
            {
                _pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(_pursuit, _suspect);
                Functions.AddPedToPursuit(_pursuit, _passenger);
                Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                _suspect.Tasks.CruiseWithVehicle(30f, VehicleDrivingFlags.Emergency);
                _suspect.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
                _passenger.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
                NativeFunction.Natives.SetPedCombatAttributes(_passenger, 1, true);
                _suspect.RelationshipGroup = "MadLads";
                _passenger.RelationshipGroup = "MadLads";
                Game.LocalPlayer.Character.RelationshipGroup = "COP";
                Game.SetRelationshipBetweenRelationshipGroups("MadLads", "COP", Relationship.Hate);
                _passenger.Tasks.FightAgainstClosestHatedTarget(50f);
                _suspectBlip.Delete();
                _pursuitCreated = true;
                Game.DisplayHelp("Stop the suspect!", 5000);
            }

            if (_pursuitCreated && !Functions.IsPursuitStillRunning(_pursuit)) End();
            base.Process();
        }

        public override void End()
        {
            if (_suspect.Exists()) _suspect.Dismiss();
            if (_passenger.Exists()) _passenger.Dismiss();
            if (_suspectVehicle.Exists()) _suspectVehicle.Dismiss();
            if (_suspectBlip.Exists()) _suspectBlip.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}