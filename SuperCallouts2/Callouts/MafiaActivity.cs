#region

using System.Drawing;
using LSPD_First_Response;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using SuperCallouts.CustomScenes;

#endregion

namespace SuperCallouts2.Callouts
{
    [CalloutInfo("MafiaActivity", CalloutProbability.Medium)]
    internal class MafiaActivity : Callout
    {
        private readonly Vector3 _callPos = new Vector3(918.88f, 38.91f, 81.09f);
        private Blip _cBlip;
        private Vehicle _mafiaCars1;
        private Vehicle _mafiaCars2;
        private Vehicle _mafiaCars3;
        private Vehicle _mafiaCars4;
        private Ped _mafiaDudes1;
        private Ped _mafiaDudes2;
        private Ped _mafiaDudes3;
        private Ped _mafiaDudes4;
        private Ped _mafiaDudes5;
        private Ped _mafiaDudes6;
        private bool _onScene;

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
        {
            Game.LogTrivial("SuperCallouts Log: MafiaActivity callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~The Mafia",
                "FIB and IAA reports the Mafia have been using the casino as a drug trafficking hotspot. Investigate the scene.");
            MafiaSetup.ConstructMafia1Scene(out _mafiaDudes2, out _mafiaDudes6, out _mafiaDudes3,
                out _mafiaDudes1, out _mafiaDudes4, out _mafiaCars1, out _mafiaCars2, out _mafiaCars4, out _mafiaCars3,
                out _mafiaDudes5);
            SuperCallouts.CustomScenes.SimpleFunctions.SetWanted(_mafiaDudes1, true);
            SuperCallouts.CustomScenes.SimpleFunctions.SetWanted(_mafiaDudes2, true);
            SuperCallouts.CustomScenes.SimpleFunctions.SetWanted(_mafiaDudes3, true);
            SuperCallouts.CustomScenes.SimpleFunctions.SetWanted(_mafiaDudes4, true);
            SuperCallouts.CustomScenes.SimpleFunctions.SetWanted(_mafiaDudes5, true);
            SuperCallouts.CustomScenes.SimpleFunctions.SetWanted(_mafiaDudes6, true);
            _mafiaDudes1.IsPersistent = true;
            _mafiaDudes2.IsPersistent = true;
            _mafiaDudes3.IsPersistent = true;
            _mafiaDudes4.IsPersistent = true;
            _mafiaDudes5.IsPersistent = true;
            _mafiaDudes6.IsPersistent = true;
            _mafiaCars1.IsPersistent = true;
            _mafiaCars2.IsPersistent = true;
            _mafiaCars3.IsPersistent = true;
            _mafiaCars4.IsPersistent = true;
            _mafiaCars1.IsStolen = true;
            _mafiaCars2.IsStolen = true;
            _mafiaCars3.IsStolen = true;
            _mafiaCars4.IsStolen = true;
            _cBlip = _mafiaDudes2.AttachBlip();
            _cBlip.EnableRoute(Color.Red);
            _cBlip.Color = Color.Red;
            Game.LocalPlayer.Character.RelationshipGroup = "COP";
            Functions.AddPedContraband(_mafiaDudes1, ContrabandType.Narcotics, "COCAINE");
            Functions.AddPedContraband(_mafiaDudes2, ContrabandType.Narcotics, "COCAINE");
            Functions.AddPedContraband(_mafiaDudes3, ContrabandType.Narcotics, "COCAINE");
            Functions.AddPedContraband(_mafiaDudes4, ContrabandType.Narcotics, "COCAINE");
            Functions.AddPedContraband(_mafiaDudes5, ContrabandType.Narcotics, "COCAINE");
            Functions.AddPedContraband(_mafiaDudes6, ContrabandType.Narcotics, "COCAINE");
            Game.DisplaySubtitle("Get to the ~r~scene~w~! Proceed with ~r~CAUTION~w~!", 10000);
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_callPos) < 120f)
            {
                Game.DisplaySubtitle(
                    "Suspects spotted, appear to be ~r~armed~w~ and ~r~wanted~w~! Proceed with caution or wait for backup.",
                    5000);
                Game.DisplayNotification(
                    "~r~Dispatch:~s~ Officer on scene, mafia activity spotted. Dispatching specialized units.");
                _mafiaDudes2.Tasks.FireWeaponAt(Game.LocalPlayer.Character, -1, FiringPattern.FullAutomatic);
                _mafiaDudes3.Tasks.FireWeaponAt(Game.LocalPlayer.Character, -1, FiringPattern.FullAutomatic);
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _mafiaDudes1, Game.LocalPlayer.Character, 0, 1);
                Functions.PlayScannerAudioUsingPosition(
                    "DISPATCH_SWAT_UNITS_FROM_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_01", _callPos);
                Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                    EBackupUnitType.NooseTeam);
                Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                    EBackupUnitType.LocalUnit);
                _cBlip.Delete();
                Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Hate);
                _onScene = true;
            }

            if (_onScene && Game.LocalPlayer.Character.DistanceTo(_mafiaDudes1.Position) > 500f &&
                Game.LocalPlayer.Character.DistanceTo(_mafiaDudes2.Position) > 500f &&
                Game.LocalPlayer.Character.DistanceTo(_mafiaDudes3.Position) > 500f &&
                Game.LocalPlayer.Character.DistanceTo(_mafiaDudes4.Position) > 500f &&
                Game.LocalPlayer.Character.DistanceTo(_mafiaDudes5.Position) > 500f &&
                Game.LocalPlayer.Character.DistanceTo(_mafiaDudes6.Position) > 500f) End();
            if (_onScene && _mafiaDudes1.IsDead && _mafiaDudes2.IsDead && _mafiaDudes3.IsDead && _mafiaDudes4.IsDead &&
                _mafiaDudes5.IsDead && _mafiaDudes6.IsDead) End();
            base.Process();
        }

        public override void End()
        {
            if (_mafiaDudes1.Exists()) _mafiaDudes1.Dismiss();
            if (_mafiaDudes2.Exists()) _mafiaDudes2.Dismiss();
            if (_mafiaDudes3.Exists()) _mafiaDudes3.Dismiss();
            if (_mafiaDudes4.Exists()) _mafiaDudes4.Dismiss();
            if (_mafiaDudes5.Exists()) _mafiaDudes5.Dismiss();
            if (_mafiaDudes6.Exists()) _mafiaDudes6.Dismiss();
            if (_mafiaCars1.Exists()) _mafiaCars1.Dismiss();
            if (_mafiaCars2.Exists()) _mafiaCars2.Dismiss();
            if (_mafiaCars3.Exists()) _mafiaCars3.Dismiss();
            if (_mafiaCars4.Exists()) _mafiaCars4.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}