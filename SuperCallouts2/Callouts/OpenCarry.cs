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

namespace SuperCallouts2.Callouts
{
    [CalloutInfo("OpenCarry", CalloutProbability.Medium)]
    internal class OpenCarry : Callout
    {
        #region Variables
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
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            try
            {
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