using System.Linq;
using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;
using SuperEvents2.SimpleFunctions;

namespace SuperEvents2
{
    public class Main : Plugin
    {
        public static bool PluginRunning { get; set; }
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
                        PluginRunning = true;
                        SimpleFunctions.Events.InitEvents();
                        EventTimer.TimerStart();
                    });
            }
    
            public override void Finally()
            {
                foreach (var entity in AmbientEvent.EntitiesToClear.Where(entity => entity))
                    entity.Delete();
                foreach (var blip in AmbientEvent.BlipsToClear.Where(blip => blip))
                    blip.Delete();
                Game.LogTrivial("SuperEvents by SuperPyroManiac has been cleaned up.");
            }
    }
}