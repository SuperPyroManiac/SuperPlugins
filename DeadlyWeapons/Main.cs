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
        var missingDepend = string.Empty;
        var outdatedDepend = string.Empty;
        if (!File.Exists("PyroCommon.dll")) missingDepend += "PyroCommon.dll~n~";
        if (!File.Exists("RageNativeUI.dll")) missingDepend += "RageNativeUI.dll~n~";
        if (!File.Exists("DamageTrackerLib.dll")) missingDepend += "DamageTrackerLib.dll~n~";
        if (missingDepend.Length > 0)
        {
            Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print($"DeadlyWeapons: Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print($"These dependencies are not installed correctly!\r\n{missingDepend.Replace("~n~", "\r\n")}DeadlyWeapons could not load!");
            Game.Console.Print("======================================================");
            Game.Console.Print($"DeadlyWeapons: Error Report End");
            Game.DisplayNotification($"DeadlyWeapons: These dependencies are not installed correctly!~n~{missingDepend}~r~Plugin is disabled!");
            return;
        }
        var pc = new Version(FileVersionInfo.GetVersionInfo("PyroCommon.dll").FileVersion);
        if (pc < new Version("1.1.0.0")) outdatedDepend += "PyroCommon.dll~n~";
        var rn = new Version(FileVersionInfo.GetVersionInfo("RageNativeUI.dll").FileVersion);
        if (rn < new Version("1.9.2.0")) outdatedDepend += "RageNativeUI.dll~n~";
        var dtf = new Version(FileVersionInfo.GetVersionInfo("DamageTrackerLib.dll").FileVersion);
        if (dtf < new Version("1.0.1.0")) outdatedDepend += "DamageTrackerLib.dll~n~";
        if (outdatedDepend.Length > 0)
        {
            Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print("DeadlyWeapons: Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print($"These dependencies are outdated!\r\n{outdatedDepend.Replace("~n~", "\r\n")}DeadlyWeapons could not load!");
            Game.Console.Print("======================================================");
            Game.Console.Print("DeadlyWeapons: Error Report End");
            Game.DisplayNotification($"~o~DeadlyWeapons: These dependencies are outdated!~n~~r~{missingDepend}~o~Plugin is disabled!");
            return;
        }
        
        Settings.LoadSettings();
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        Log.Info("DeadlyWeapons by SuperPyroManiac loaded! Go on duty to enable it!");
        Log.Info("======================================================");
        Log.Info("Dependencies Found:");
        Log.Info($"PyroCommon, Version: {pc}");
        Log.Info($"RageNativeUI, Version: {rn}");
        Log.Info($"DamageTrackerLib, Version: {dtf}");
        Log.Info($"Using UltimateBackup: {PyroCommon.Main.UsingUb}");
        Log.Info("======================================================");
        Game.AddConsoleCommands(new[] {typeof(ConsoleCommands)});
    }

    private void OnOnDutyStateChangedHandler(bool onDuty)
    {
        if (onDuty)
        {
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