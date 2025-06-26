using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LSPD_First_Response.Mod.API;
using PyroCommon.Utils;
using Rage;
using SuperEvents.EventFunctions;
using SuperEvents.Events;
using DependManager = PyroCommon.Services.DependencyService;

namespace SuperEvents;

internal class Main : Plugin
{
    internal static bool PluginRunning { get; private set; }

    public override void Initialize()
    {
        var dCheck = new DependManager();
        dCheck.AddDepend("PyroCommon.dll", "1.14.0.0");
        dCheck.AddDepend("RageNativeUI.dll", "1.9.3.0");
        if (!dCheck.CheckDepends())
            return;
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        Settings.LoadSettings();
    }

    private static void OnOnDutyStateChangedHandler(bool onDuty)
    {
        if (onDuty)
        {
            PyroCommon.Main.InitCommon("SuperEvents", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            LogUtils.Info("SuperEvents by SuperPyroManiac loaded successfully!");
            LogUtils.Info("======================================================");
            LogUtils.Info("Dependencies Found:");
            LogUtils.Info($"PyroCommon, Version: {new Version(FileVersionInfo.GetVersionInfo("PyroCommon.dll").FileVersion)}");
            LogUtils.Info($"Using Ultimate Backup: {PyroCommon.Main.UsingUb}");
            LogUtils.Info($"Using StopThePed: {PyroCommon.Main.UsingStp}");
            LogUtils.Info("======================================================");
            PluginRunning = true;
            RegisterAllEvents();
            Game.DisplayNotification(
                "3dtextures",
                "mpgroundlogo_cops",
                "~r~SuperEvents",
                "~g~Plugin Loaded.",
                "SuperEvents version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded."
            );
            GameFiber.StartNew(EventManager.InitEvents, "[SE] EventManager");
            return;
        }
        PyroCommon.Main.StopCommon();
    }

    private static void RegisterAllEvents()
    {
        if (Settings.CarAccident)
            EventManager.RegisterEvent(typeof(CarAccident), EventManager.Priority.High);
        if (Settings.CarFire)
            EventManager.RegisterEvent(typeof(CarFire));
        if (Settings.Fight)
            EventManager.RegisterEvent(typeof(Fight));
        if (Settings.OpenCarry)
            EventManager.RegisterEvent(typeof(OpenCarry), EventManager.Priority.Low);
        if (Settings.PulloverShooting)
            EventManager.RegisterEvent(typeof(PulloverShooting), EventManager.Priority.Low);
        if (Settings.RecklessDriver)
            EventManager.RegisterEvent(typeof(RecklessDriver));
        if (Settings.AbandonedCar)
            EventManager.RegisterEvent(typeof(WeirdCar));
        if (Settings.WildAnimal)
            EventManager.RegisterEvent(typeof(WildAnimal));
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
        PyroCommon.Main.StopCommon();
    }
}
