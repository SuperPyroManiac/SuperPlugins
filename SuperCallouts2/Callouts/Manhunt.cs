using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Drawing;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts2.SimpleFunctions;

namespace SuperCallouts2.Callouts
{
    [CalloutInfo("Manhunt", CalloutProbability.Medium)]
    class Manhunt : Callout
    {
        #region Variables

        private Ped _bad;
        private Blip _cBlip;
        private Blip _cBlip2;
        private LHandle _pursuit;
        private Vector3 _searcharea;
        private Vector3 _spawnPoint;
        private bool _onScene;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperCallouts", "~y~Choose an option.");
        private readonly UIMenu _convoMenu = new UIMenu("SuperCallouts", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem _callAr =
            new UIMenuItem("~r~ Call Air Unit", "Calls for an air unit.");
        private readonly UIMenuItem _questioning = new UIMenuItem("Speak With Subjects");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Callout", "Ends the event early.");
        private UIMenuItem _speakSuspect;
        #endregion
        public override bool OnBeforeCalloutDisplayed()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
            CalloutMessage = "~b~Dispatch:~s~ Wanted suspect on the run.";
            CalloutPosition = _spawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 SUSPECTS_LAST_SEEN_02 IN_OR_ON_POSITION",
                _spawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SuperCallouts Log: Manhunt callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Manhunt",
                "Search for the suspect. High priority, respond ~r~CODE-3");
            //Bad
            _bad = new Ped(_spawnPoint) {IsPersistent = true};
            CFunctions.SetWanted(_bad, true);
            //AreaBlip
            //UI
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            try
            {
                //GamePlay
                
                //Keybinds
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
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}