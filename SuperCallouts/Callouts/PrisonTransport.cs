#region

using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("PrisonTransport", CalloutProbability.High)]
    internal class PrisonTransport : Callout
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
            if (Game.IsKeyDown(Settings.EndCall)) End();


            if (_onScene && !Functions.IsPursuitStillRunning(_pursuit)) End();
            base.Process();
        }

        public override void End()
        {
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);

            base.End();
        }
    }
}