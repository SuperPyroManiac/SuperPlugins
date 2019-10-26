using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts2.SimpleFunctions;

namespace SuperCallouts2.Callouts
{
    [CalloutInfo("HitRun", CalloutProbability.Medium)]
    class HitRun : Callout
    {
        #region Variables
        private Ped _victim;
        private Ped _bad1;
        private Ped _bad2;
        private Vehicle _cVehicle1;
        private Vehicle _cVehicle2;
        private Blip _cBlip1;
        private Blip _cBlip2;
        private Blip _cBlip3;
        private LHandle _pursuit;
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        private Vector3 _spawnPointOffset;
        private bool _onScene;
        private bool _onScene2;
        private bool _startPursuit;
        private string _name1;
        private string _name2;
        private string _name3;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperCallouts", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new UIMenu("SuperCallouts", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem _questioning = new UIMenuItem("Speak With Subjects");
        private readonly UIMenuItem _dissmisVictim =
            new UIMenuItem("~r~ Dismiss Victim", "Lets the victim leave.");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Callout", "Ends the callout early.");
        private UIMenuItem _speakVictim;
        private UIMenuItem _speakSuspect1;
        private UIMenuItem _speakSuspect2;
        #endregion
        
        public override bool OnBeforeCalloutDisplayed()
        {
            CFunctions.FindSideOfRoad(500, 100, out _spawnPoint, out _spawnPointH);
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 40f);
            CalloutMessage = "~r~911 Report:~s~ Vehicle hit and run.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition(
                "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_HIT_AND_RUN_01 IN_OR_ON_POSITION", _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: Hit And Run callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Car Accident",
                "Victim reports the other driver have left the scene. Get to the victim as soon as possible.");
            //cVehicle
            CFunctions.SpawnNormalCar(out _cVehicle1, _spawnPoint);
            _cVehicle1.Heading = _spawnPointH;
            CFunctions.Damage(_cVehicle1, 50, 50);
            _spawnPointOffset = World.GetNextPositionOnStreet(_cVehicle1.Position.Around(100f));
            //cVehicle2
            CFunctions.SpawnNormalCar(out _cVehicle2, _spawnPointOffset);
            CFunctions.Damage(_cVehicle2, 200, 200);
            //Victim
            _victim = _cVehicle1.CreateRandomDriver();
            _victim.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
            Functions.SetVehicleOwnerName(_cVehicle1, Functions.GetPersonaForPed(_victim).FullName);
            _name1 = Functions.GetPersonaForPed(_victim).FullName;
            //Bad1
            _bad1 = _cVehicle2.CreateRandomDriver();
            _bad1.Tasks.CruiseWithVehicle(8f);
            _bad1.IsPersistent = true;
            Functions.SetVehicleOwnerName(_cVehicle2, Functions.GetPersonaForPed(_bad1).FullName);
            _name2 = Functions.GetPersonaForPed(_bad1).FullName;
            //Bad2
            _bad2 = new Ped(_spawnPoint);
            _bad2.WarpIntoVehicle(_cVehicle2, 0);
            _bad2.IsPersistent = true;
            _name3 = Functions.GetPersonaForPed(_bad2).FullName;
            //Start UI
            _interaction.Add(_mainMenu);
            _interaction.Add(_convoMenu);
            _mainMenu.AddItem(_dissmisVictim);
            _mainMenu.AddItem(_questioning);
            _mainMenu.AddItem(_endCall);
            _speakVictim = new UIMenuItem("Speak with ~y~" + _name1);
            _speakSuspect1 = new UIMenuItem("Speak with ~y~" + _name2);
            _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name3);
            _mainMenu.RefreshIndex();
            _convoMenu.RefreshIndex();
            _mainMenu.BindMenuToItem(_convoMenu, _questioning);
            _mainMenu.OnItemSelect += Interactions;
            _convoMenu.OnItemSelect += Conversations;
            _convoMenu.ParentMenu = _mainMenu;
            _questioning.Enabled = false;
            _speakVictim.Enabled = false;
            _speakSuspect1.Enabled = false;
            _speakSuspect2.Enabled = false;
            _dissmisVictim.Enabled = false;
            //Blips
            _cBlip1 = _victim.AttachBlip();
            _cBlip1.EnableRoute(Color.Yellow);
            _cBlip1.Color = Color.Yellow;
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            try
            {
                //GamePlay
                if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle1) < 20f)
                {
                    _onScene = true;
                    Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                    _questioning.Enabled = true;
                    _speakVictim.Enabled = true;
                }
                if (_startPursuit && !_onScene2 && Game.LocalPlayer.Character.DistanceTo(_cVehicle2) < 50f)
                {
                    _startPursuit = false;
                    _onScene2 = true;
                    _pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(_pursuit, _bad1);
                    Functions.AddPedToPursuit(_pursuit, _bad2);
                    Functions.AddCopToPursuit(_pursuit, Game.LocalPlayer.Character);
                }
                if (_onScene2 && Game.LocalPlayer.Character.DistanceTo(_cVehicle2) < 50f && !Functions.IsPursuitStillRunning(_pursuit))
                {
                    _onScene2 = false;
                    Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                    _speakSuspect1.Enabled = true;
                    _speakSuspect2.Enabled = true;
                }
                //KeyBinds
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.IsKeyDown(Settings.Interact))
                {
                    _mainMenu.Visible = !_mainMenu.Visible;
                }
                _interaction.ProcessMenus();
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to SuperPyroManiac!");
                Game.LogTrivial("SuperCallouts Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperCallouts Error Report End");
                End();
            }
            base.Process();
        }
        public override void End()
        {
            if (_bad1.Exists()) _bad1.Dismiss();
            if (_bad2.Exists()) _bad2.Dismiss();
            if (_victim.Exists()) _victim.Dismiss();
            if (_cVehicle1.Exists()) _cVehicle1.Dismiss();
            if (_cVehicle2.Exists()) _cVehicle2.Dismiss();
            if (_cBlip1.Exists()) _cBlip1.Delete();
            if (_cBlip2.Exists()) _cBlip2.Delete();
            if (_cBlip3.Exists()) _cBlip3.Delete();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
        //UI Items
        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _dissmisVictim)
            {
                Game.DisplaySubtitle("~g~You~s~: You are good to go, we will be in contact once we get more information on the suspect.");
                if (_victim.Exists()) _victim.Dismiss();
                if (_cVehicle1.Exists()) _cVehicle1.Dismiss();
                if (_cBlip1.Exists()) _cBlip1.Delete();
                _dissmisVictim.Enabled = false;
                _startPursuit = true;
                _cBlip2 = _bad1.AttachBlip();
                _cBlip2.Color = Color.Red;
                _cBlip2.EnableRoute(Color.Red);
                _cBlip3 = _bad2.AttachBlip();
                _cBlip3.Color = Color.Red;
            }
            if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }
        private void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _speakVictim)
            {
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: What's going on here? Are you ok?", 5000);
                    NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _victim, Game.LocalPlayer.Character, -1);
                    GameFiber.Wait(5000);
                    _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: I'm ok, someone hit my car and when I got out they drove off!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: Alright, well did you get any information? What did they look like or a vehicle description?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: I gave the 911 lady the license number, but it was so fast I don't recall any details. Im sorry, can I leave?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~y~Dispatch~s~: ANPR has located a vehicle matching the license given to us. Dismiss victim and respond ~r~CODE-3", 5000);
                    _dissmisVictim.Enabled = true;
                });
            }
            if (selItem == _speakSuspect1)
            {
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: Why are you running? This could have been a simple ticket and court date for the accident, now you're facing serious charges!", 5000);
                    NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad1, Game.LocalPlayer.Character, -1);
                    GameFiber.Wait(5000);
                    _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: Screw you pig, I aint talkin to you!", 5000);
                });
            }
            if (selItem == _speakSuspect2)
            {
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: What's going on? Why were you guys running?", 5000);
                    GameFiber.Wait(5000);
                    NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _bad2, Game.LocalPlayer.Character, -1);
                    _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: I didnt do nothing at all, I was just chilling and they hit someone and started running, I was like bro, and they were like bruh, so we dipped.", 5000);
                });
            }
        }
    }
}