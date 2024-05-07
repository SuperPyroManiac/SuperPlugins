using System;
using System.Diagnostics;
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

    public override void Initialize()
    {
        if (!DependChecker.Start()) return;
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        Settings.LoadSettings();
        Game.AddConsoleCommands(new[] { typeof(ConsoleCommands) });
    }

    private static void OnOnDutyStateChangedHandler(bool onDuty)
    {
        if (onDuty)
        {
            Log.Info("SuperEvents by SuperPyroManiac loaded successfully!");
            Log.Info("======================================================");
            Log.Info("Dependencies Found:");
            Log.Info($"PyroCommon, Version: {new Version(FileVersionInfo.GetVersionInfo("PyroCommon.dll").FileVersion)}");
            Log.Info($"RageNativeUI, Version: {new Version(FileVersionInfo.GetVersionInfo("RageNativeUI.dll").FileVersion)}");
            Log.Info($"Using Ultimate Backup: {PyroCommon.Main.UsingUb}");
            Log.Info($"Using Policing Redefined: {PyroCommon.Main.UsingPr}");
            Log.Info("======================================================");
            PluginRunning = true;
            RegisterAllEvents();
            Game.DisplayNotification("3dtextures",
                "mpgroundlogo_cops",
                "~r~SuperEvents",
                "~g~Plugin Loaded.",
                "SuperEvents version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
            GameFiber.StartNew(EventManager.InitEvents);
            EventInterface.StartInterface();
            
            PyroCommon.Main.InitCommon();
            GameFiber.StartNew(VersionChecker.IsUpdateAvailable);
        }
        else
        {
            if (VersionChecker.UpdateThread.IsAlive)
            {
                Log.Warning("Version thread still running during shutdown! Aborting thread...");
                VersionChecker.UpdateThread.Abort();
            }
            Log.Info("Plugin unloaded!");
        }
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
        if (VersionChecker.UpdateThread.IsAlive)
        {
            Log.Warning("Version thread still running during shutdown! Aborting thread...");
            VersionChecker.UpdateThread.Abort();
        }
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