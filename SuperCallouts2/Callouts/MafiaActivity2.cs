#region

using System.Collections.Generic;
using System.Drawing;
using LSPD_First_Response;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using SuperCallouts2.CustomScenes;

#endregion

namespace SuperCallouts2.Callouts
{
    [CalloutInfo("MafiaActivity2", CalloutProbability.Low)]
    internal class MafiaActivity2 : Callout
    {
        private readonly Vector3 _callPos = new Vector3(1543.173f, 3606.55f, 35.19303f);
        private bool _onScene;
        private Blip _cBlip;
        private Ped _mafiaDude1;
        private Ped _mafiaDude2;
        private Ped _mafiaDude3;
        private Ped _mafiaDude4;
        private Ped _mafiaDude5;
        private Ped _mafiaDude6;
        private Ped _mafiaDude7;
        private Ped _mafiaDude8;
        private Ped _mafiaDude9;
        private Ped _mafiaDude10;
        private Ped _mafiaDude11;
        private Ped _mafiaDude12;
        private Ped _mafiaDude13;
        private Ped _mafiaDude14;
        private Ped _mafiaDude15;
        private Vehicle _cVehicle1;
        private Vehicle _cVehicle2;
        private Vehicle _cVehicle3;
        private Vehicle _cVehicle4;
        private readonly List<Ped> _mafiaDudes = new List<Ped>();
        private readonly List<Vehicle> _mafiaCars = new List<Vehicle>();

        public override bool OnBeforeCalloutDisplayed()
        {
            ShowCalloutAreaBlipBeforeAccepting(_callPos, 80f);
            CalloutMessage = "~b~FIB Report:~s~ Organized crime members spotted.";
            CalloutPosition = _callPos;
            Functions.PlayScannerAudioUsingPosition(
                "ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_BRANDISHING_WEAPON_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
                _callPos);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {//TODO: FIX THE LOCATIONS
            Mafia2Setup.ConstructMafia2Scene(out _cVehicle1, out _cVehicle2, out _cVehicle3, out _cVehicle4, out _mafiaDude1, out _mafiaDude2, out _mafiaDude3, out _mafiaDude4, out _mafiaDude5, out _mafiaDude6, out _mafiaDude7, out _mafiaDude8, out _mafiaDude9, out _mafiaDude10, out _mafiaDude11, out _mafiaDude12, out _mafiaDude13, out _mafiaDude14, out _mafiaDude15);
            Game.LogTrivial("SuperCallouts Log: MafiaActivity callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~The Mafia",
                "FIB and IAA reports the Mafia have been spotted near Sandy Shores. Possible large scale drug trafficking. Investigate the scene.");
            Game.LocalPlayer.Character.RelationshipGroup = "COP";
            Game.DisplaySubtitle("Get to the ~r~scene~w~! Proceed with ~r~CAUTION~w~!", 10000);
            _cBlip = _mafiaDude2.AttachBlip();
            _cBlip.EnableRoute(Color.Red);
            _cBlip.Color = Color.Red;
            _mafiaCars.Add(_cVehicle1);
            _mafiaCars.Add(_cVehicle2);
            _mafiaCars.Add(_cVehicle3);
            _mafiaCars.Add(_cVehicle4);
            _mafiaDudes.Add(_mafiaDude1);
            _mafiaDudes.Add(_mafiaDude2);
            _mafiaDudes.Add(_mafiaDude3);
            _mafiaDudes.Add(_mafiaDude4);
            _mafiaDudes.Add(_mafiaDude5);
            _mafiaDudes.Add(_mafiaDude6);
            _mafiaDudes.Add(_mafiaDude7);
            _mafiaDudes.Add(_mafiaDude8);
            _mafiaDudes.Add(_mafiaDude9);
            _mafiaDudes.Add(_mafiaDude10);
            _mafiaDudes.Add(_mafiaDude11);
            _mafiaDudes.Add(_mafiaDude12);
            _mafiaDudes.Add(_mafiaDude13);
            _mafiaDudes.Add(_mafiaDude14);
            _mafiaDudes.Add(_mafiaDude15);
            foreach (var mafiaCars in _mafiaCars) mafiaCars.IsPersistent = true;
            foreach (var mafiaDudes in _mafiaDudes)
            {
                mafiaDudes.IsPersistent = true;
                mafiaDudes.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
                SimpleFunctions.CFunctions.SetWanted(mafiaDudes, true);
                Functions.AddPedContraband(mafiaDudes, ContrabandType.Narcotics, "Cocaine");
            }
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_callPos) < 100f)
            {
                Game.DisplaySubtitle(
                    "Suspects spotted, appear to be ~r~armed~w~ and ~r~wanted~w~! Proceed with caution or wait for backup.",
                    5000);
                Game.DisplayNotification(
                    "~r~Dispatch:~s~ Officer on scene, mafia activity spotted. Dispatching specialized units.");
                Functions.PlayScannerAudioUsingPosition(
                    "DISPATCH_SWAT_UNITS_FROM_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_01", _callPos);
                Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                    EBackupUnitType.NooseTeam);
                Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                    EBackupUnitType.LocalUnit);
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                    EBackupUnitType.LocalUnit);
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                    EBackupUnitType.LocalUnit);
                Game.LocalPlayer.Character.RelationshipGroup = "COP";
                _mafiaDude13.Tasks.FightAgainst(Game.LocalPlayer.Character, -1);
                Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Hate);
                _onScene = true;
                _cBlip.Delete();
            }
            if (_onScene && Game.LocalPlayer.Character.DistanceTo(_callPos) > 120f) End();
            base.Process();
        }

        public override void End()
        {
            foreach (var mafiaCars in _mafiaCars) {if(mafiaCars.Exists()) {mafiaCars.Dismiss();}}
            foreach (var mafiaDudes in _mafiaDudes) {if(mafiaDudes.Exists()) {mafiaDudes.Dismiss();}}
            if (_cBlip.Exists()) {_cBlip.Delete();}
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}