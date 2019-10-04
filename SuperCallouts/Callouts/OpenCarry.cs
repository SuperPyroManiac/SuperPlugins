#region

using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("OpenCarry", CalloutProbability.High)]
    internal class OpenCarry : Callout
    {
        private Ped _bad1;
        private Blip _cBlip;
        private bool _onScene;
        private LHandle _pursuit;
        private readonly Random _rNd = new Random();
        private bool _socks;
        private Vector3 _spawnPoint;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~b~Dispatch:~s~ Reports of a person with a firearm.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Open Carry callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Person With Gun",
                "Reports of a person walking around with an assault rifle. respond ~y~CODE-2");
            _bad1 = new Ped(_spawnPoint);
            _bad1.IsPersistent = true;
            _bad1.Inventory.GiveNewWeapon(WeaponHash.AdvancedRifle, -1, true);
            _bad1.Tasks.Wander();
            _bad1.Metadata.stpAlcoholDetected = true;
            _bad1.Metadata.hasGunPermit = false;
            _bad1.Metadata.searchPed = "~r~assaultrifle~s~, ~y~pocket knife~s~, ~g~wallet~s~, ~r~SWAGGER~s~";
            _cBlip = _bad1.AttachBlip();
            _cBlip.EnableRoute(Color.Red);
            _cBlip.Color = Color.Red;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.Position.DistanceTo(_bad1) < 20f)
            {
                _onScene = true;
                _pursuit = Functions.CreatePursuit();
                _cBlip.DisableRoute();
                var choices = _rNd.Next(1, 5);
                switch (choices)
                {
                    case 1:
                        Game.DisplaySubtitle("~r~Suspect: ~s~I know my rights, leave me alone!", 5000);
                        Functions.AddPedToPursuit(_pursuit, _bad1);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        _socks = true;
                        break;
                    case 2:
                        Game.DisplayNotification("Investigate the person.");
                        break;
                    case 3:
                        Game.DisplaySubtitle("~r~Suspect: ~s~REEEEEE", 5000);
                        _bad1.Tasks.AimWeaponAt(Game.LocalPlayer.Character, -1);
                        break;
                    case 4:
                        Game.DisplayNotification("Investigate the person.");
                        break;
                    default:
                        Game.DisplayNotification(
                            "An error has been detected! Ending callout early to prevent LSPDFR crash!");
                        End();
                        break;
                }
            }

            if (_socks && !Functions.IsPursuitStillRunning(_pursuit)) End();
            if (_onScene && _bad1.IsDead) End();
            if (_onScene && Game.LocalPlayer.Character.DistanceTo(_bad1) > 60f) End();
            base.Process();
        }

        public override void End()
        {
            if (_bad1.Exists()) _bad1.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}