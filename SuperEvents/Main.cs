#region

using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;
using SuperEvents.SimpleFunctions;

#endregion

namespace SuperEvents
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Settings.LoadSettings();
            Game.LogTrivial("SuperEvents " + Assembly.GetExecutingAssembly().GetName().Version + " by SuperPyroManiac has been initialised.");
            Game.LogTrivial("Go on duty with LSPDFR to fully load SuperEvents.");
        }

        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
                GameFiber.StartNew(delegate
                {
                    GameFiber.Wait(10000);
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SuperEvents", "~g~Plugin Loaded.", "SuperEvents version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
                    SimpleFunctions.Events.InitEvents();
                    EventTimer.TimerStart();
                });
        }

        public override void Finally()
        {
            Game.LogTrivial("SuperEvents by SuperPyroManiac has been cleaned up.");
        }
    }
}
