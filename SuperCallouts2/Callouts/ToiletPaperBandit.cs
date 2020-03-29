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
    [CalloutInfo("ToiletPaperBandit", CalloutProbability.Medium)]
    class ToiletPaperBandit : Callout
    {
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