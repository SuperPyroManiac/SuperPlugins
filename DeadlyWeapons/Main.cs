using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DamageTrackerLib;
using DeadlyWeapons.DFunctions;
using DeadlyWeapons.Modules;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;

namespace DeadlyWeapons;

public class Main : Plugin
{
    private GameFiber _panicFiber;
 
    public override void Initialize()
    {
        if (!DependChecker.Start()) return;
        Settings.LoadSettings();
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        Game.AddConsoleCommands(new[] {typeof(ConsoleCommands)});
    }

    private void OnOnDutyStateChangedHandler(bool onDuty)
    {
        if (onDuty)
        {
            Log.Info("SuperEvents by SuperPyroManiac loaded successfully!");
            Log.Info("======================================================");
            Log.Info("Dependencies Found:");
            Log.Info($"PyroCommon, Version: {new Version(FileVersionInfo.GetVersionInfo("PyroCommon.dll").FileVersion)}");
            Log.Info($"RageNativeUI, Version: {new Version(FileVersionInfo.GetVersionInfo("RageNativeUI.dll").FileVersion)}");
            Log.Info($"DamageTrackerLib, Version: {new Version(FileVersionInfo.GetVersionInfo("DamageTrackerLib.dll").FileVersion)}");
            Log.Info($"Using Ultimate Backup: {PyroCommon.Main.UsingUb}");
            Log.Info("======================================================");
            DamageTrackerService.Start();
            if (Settings.EnablePlayerDamageSystem)
                DamageTrackerService.OnPlayerTookDamage += PlayerShot.OnPlayerDamaged;
            if (Settings.EnableAIDamageSystem)
                DamageTrackerService.OnPedTookDamage += PedShot.OnPedDamaged;
            if (Settings.EnablePanic) _panicFiber = GameFiber.StartNew(Panic.StartPanicWatch);
            if (Settings.EnablePulloverAi)
                Events.OnPulloverStarted += CustomPullover.PulloverModule;
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~DeadlyWeapons", "~g~Plugin Loaded.", "DeadlyWeapons version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
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
        
    public override void Finally()
    {
        if (VersionChecker.UpdateThread.IsAlive)
        {
            Log.Warning("Version thread still running during shutdown! Aborting thread...");
            VersionChecker.UpdateThread.Abort();
        }
        DamageTrackerService.Stop();
        _panicFiber.Abort();
        Events.OnPulloverStarted -= CustomPullover.PulloverModule;
        Log.Info("Plugin unloaded!");
    }
}