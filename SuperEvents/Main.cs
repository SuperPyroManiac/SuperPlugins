using System.Linq;
using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;
using SuperEvents.EventFunctions;
using SuperEvents.Events;

namespace SuperEvents
{
    internal class Main : Plugin
    {
        internal static bool PluginRunning { get; private set; }
        internal static bool PluginPaused { get; set; }
        private static GameFiber _initFiber;
        
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Settings.LoadSettings();
            Game.LogTrivial("SuperEvents " + Assembly.GetExecutingAssembly().GetName().Version +
                            " by SuperPyroManiac has been initialised.");
            Game.LogTrivial("Go on duty with LSPDFR to fully load SuperEvents.");
            Game.AddConsoleCommands(new[] {typeof(ConsoleCommands)});
        }

        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
                GameFiber.StartNew(delegate
                {
                    PluginRunning = true;
                    RegisterAllEvents();
                    GameFiber.Wait(5000);
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SuperEvents", "~g~Plugin Loaded.",
                        "SuperEvents version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
                    var InitEvents = new EventFunctions.Events();
                    _initFiber = GameFiber.StartNew(EventFunctions.Events.InitEvents);
                    EventTimer.TimerStart();
                    GameFiber.Wait(17000);
                    VersionChecker.IsUpdateAvailable();
                });
        }

        private static void RegisterAllEvents()
        {
            API.RegisterEvent(typeof(PulloverShooting), API.Priority.Low);
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