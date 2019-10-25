#region

using System.Drawing;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.CustomScenes;

#endregion

namespace SuperCallouts.Callouts
{
    [CalloutInfo("HitAndRun", CalloutProbability.Medium)]
    internal class HitRun : Callout
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
            _conversation.ProcessMenus();
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 20f)
            {
                _onScene = true;
                Game.DisplayHelp("Press " + Settings.Interact + " to speak with the victim.");
            }

            if (Game.IsKeyDown(Settings.Interact) && !_nIce && _onScene) _mainMenu.Visible = !_mainMenu.Visible;
            if (!_nearBad && _nIce && Game.LocalPlayer.Character.DistanceTo(_cVehicle2) < 50f)
            {
                _nearBad = true;
                Functions.AddPedToPursuit(_pursuit, _bad1);
                Functions.AddPedToPursuit(_pursuit, _bad2);
                Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                _cBlip2.DisableRoute();
                _cBlip3 = _bad2.AttachBlip();
                _cBlip3.Color = Color.Red;
                Functions.AddPedContraband(_bad1, ContrabandType.Narcotics, "COCAINE");
                Functions.AddPedContraband(_bad2, ContrabandType.Narcotics, "COCAINE");
            }

            if (!Functions.IsPursuitStillRunning(_pursuit) && _nearBad) End();
            base.Process();
        }

        public override void End()
        {

            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            base.End();
        }
    }
}