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
    [CalloutInfo("AngryAnimal", CalloutProbability.High)]
    internal class AngryAnimal : Callout
    {
        private Ped _animal;
        private Blip _cBlip;
        private Blip _cBlip2;
        private bool _onScene;
        private Vector3 _spawnPoint;
        private Ped _victim;

        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(450f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "~r~911 Report:~s~ Person(s) being attacked by a wild animal.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_11_351_02 UNITS_RESPOND_CODE_03_01",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("SuperCallouts Log: Angry Animal callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Help Civilian",
                "Details are unknown, get to the scene as soon as possible!");
            Model[] meanAnimal = {"A_C_PIG", "A_C_COW", "A_C_BOAR", "A_C_MTLION", "A_C_CHIMP", "A_C_COYOTE"};
            _animal = new Ped(meanAnimal[new Random().Next(meanAnimal.Length)], _spawnPoint, 50);
            _animal.IsPersistent = true;
            _victim = new Ped(_animal.GetOffsetPosition(new Vector3(0, 1.8f, 0)));
            _victim.IsPersistent = true;
            _victim.Health = 500;
            _cBlip = _animal.AttachBlip();
            _cBlip.EnableRoute(Color.Red);
            _cBlip.Color = Color.Red;
            _cBlip2 = _victim.AttachBlip();
            _cBlip2.Color = Color.Yellow;
            Game.DisplaySubtitle("Get to the ~r~scene~w~!", 10000);
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 30f)
            {
                Game.DisplayHelp("Stop the animal from attacking people!", 5000);
                Game.DisplaySubtitle("~y~Victim: ~w~Help this thing is crazy!", 5000);
                _cBlip.Delete();
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _animal, _victim, 0, 1);
                NativeFunction.CallByName<uint>("TASK_REACT_AND_FLEE_PED", _victim, _animal);
                _onScene = true;
            }

            if (_onScene && _animal.IsDead)
                End();
            else if (_onScene && Game.LocalPlayer.Character.DistanceTo(_animal) > 200f) End();
            base.Process();
        }

        public override void End()
        {
            if (_victim.Exists()) _victim.Dismiss();
            if (_animal.Exists()) _animal.Dismiss();
            if (_cBlip.Exists()) _cBlip.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}