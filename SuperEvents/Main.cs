using System.IO;
using System.Linq;
using System.Reflection;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;
using SuperEvents.EventFunctions;
using SuperEvents.Events;
// ReSharper disable ClassNeverInstantiated.Global

namespace SuperEvents;

internal class Main : Plugin
{
    internal static bool PluginRunning { get; private set; }
    internal static bool PluginPaused { get; set; }
    // ReSharper disable once NotAccessedField.Local
    private static GameFiber _initFiber;

    public override void Initialize()
    {
        var missingDepend = string.Empty;
        if (!File.Exists("PyroCommon.dll")) missingDepend += "PyroCommon.dll~n~";
        if (!File.Exists("RageNativeUI.dll")) missingDepend += "RageNativeUI.dll~n~";
        if (missingDepend.Length > 0)
        {
            Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print($"SuperEvents: Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print($"These dependencies are not installed correctly!\r\n{missingDepend.Replace("~n~", "\r\n")}SuperEvents could not load!");
            Game.Console.Print("======================================================");
            Game.Console.Print($"SuperEvents: Error Report End");
            Game.DisplayNotification($"SuperEvents: These dependencies are not installed correctly!~n~{missingDepend}~r~Plugin is disabled!");
            return;
        }
        
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        Settings.LoadSettings();
        Log.Info( Assembly.GetExecutingAssembly().GetName().Version + " by SuperPyroManiac has been initialised.");
        Log.Info("Go on duty with LSPDFR to fully load SuperEvents.");
        Game.AddConsoleCommands(new[] { typeof(ConsoleCommands) });
    }

    private static void OnOnDutyStateChangedHandler(bool onDuty)
    {
        if (!onDuty) return;
        if (PyroCommon.Main.UsingUb) Log.Info("Using UltimateBackup API.");
        PluginRunning = true;
        RegisterAllEvents();
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SuperEvents", "~g~Plugin Loaded.",
            "SuperEvents version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
        _initFiber = GameFiber.StartNew(EventManager.InitEvents);
        EventInterface.StartInterface();
        GameFiber.StartNew(VersionChecker.IsUpdateAvailable);
    }

    private static void RegisterAllEvents()
    {
        if (Settings.CarAccident) EventManager.RegisterEvent(typeof(CarAccident), EventManager.Priority.High);
        if (Settings.CarFire) EventManager.RegisterEvent(typeof(CarFire));
        if (Settings.Fight) EventManager.RegisterEvent(typeof(Fight));
        //if (Settings.) EventManager.RegisterEvent(typeof(InjuredPed)); TODO: Complete IP Event
        if (Settings.OpenCarry) EventManager.RegisterEvent(typeof(OpenCarry), EventManager.Priority.Low);
        if (Settings.PulloverShooting) EventManager.RegisterEvent(typeof(PulloverShooting), EventManager.Priority.Low);
        if (Settings.RecklessDriver) EventManager.RegisterEvent(typeof(RecklessDriver));
        if (Settings.AbandonedCar) EventManager.RegisterEvent(typeof(WeirdCar));
        if (Settings.WildAnimal) EventManager.RegisterEvent(typeof(WildAnimal));
    }

    public override void Finally()
    {
        if (EventManager.CurrentEvent != null)
        {
            foreach (var entity in EventManager.CurrentEvent.EntitiesToClear.Where(entity => entity))
                entity.Delete();
            foreach (var blip in EventManager.CurrentEvent.BlipsToClear.Where(blip => blip))
                blip.Delete();
        }
        PluginRunning = false;
        Log.Info( "Plugin unloaded!");
    }

    internal static void PausePlugin()
    {
        PluginPaused = !PluginPaused;
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SuperEvents", "~g~Plugin Status:",
            "SuperEvents paused: " + PluginPaused);
        Log.Info("Plugin paused: " + PluginPaused);
    }
}