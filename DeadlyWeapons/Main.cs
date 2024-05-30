using System;
using System.Diagnostics;
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
    private GameFiber _accuracyFiber;
 
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
            Log.Info("DeadlyWeapons by SuperPyroManiac loaded successfully!");
            Log.Info("======================================================");
            Log.Info("Dependencies Found:");
            Log.Info($"PyroCommon, Version: {new Version(FileVersionInfo.GetVersionInfo("PyroCommon.dll").FileVersion)}");
            Log.Info($"RageNativeUI, Version: {new Version(FileVersionInfo.GetVersionInfo("RageNativeUI.dll").FileVersion)}");
            Log.Info($"DamageTrackerLib, Version: {new Version(FileVersionInfo.GetVersionInfo("DamageTrackerLib.dll").FileVersion)}");
            Log.Info($"Using Ultimate Backup: {PyroCommon.Main.UsingUb}");
            Log.Info($"Using StopThePed: {PyroCommon.Main.UsingStp}");
            Log.Info($"Using Policing Redefined: {PyroCommon.Main.UsingPr}");
            Log.Info("======================================================");
            DamageTrackerService.Start();
            if (Settings.EnablePlayerDamageSystem)
                DamageTrackerService.OnPlayerTookDamage += PlayerShot.OnPlayerDamaged;
            if (Settings.EnableAiDamageSystem)
                DamageTrackerService.OnPedTookDamage += PedShot.OnPedDamaged;
            if (Settings.EnablePanic) _panicFiber = GameFiber.StartNew(Panic.StartPanicFiber);
            if (Settings.AiAccuracy <= 100) _accuracyFiber = GameFiber.StartNew(Accuracy.StartAccuracyFiber);
            if (Settings.EnablePulloverAi)
                Events.OnPulloverStarted += CustomPullover.PulloverModule;
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~DeadlyWeapons", "~g~Plugin Loaded.", "DeadlyWeapons version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
            PyroCommon.Main.InitCommon("DeadlyWeapons", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
    }
        
    public override void Finally()
    {
        DamageTrackerService.Stop();
        _panicFiber.Abort();
        _accuracyFiber.Abort();
        Events.OnPulloverStarted -= CustomPullover.PulloverModule;
        PyroCommon.Main.StopCommon();
        Log.Info("Plugin unloaded!");
    }
}