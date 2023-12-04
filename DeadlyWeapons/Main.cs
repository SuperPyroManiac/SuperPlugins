using System;
using System.IO;
using System.Linq;
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
        if (!File.Exists("PyroCommon.dll")) missingDepend += "PyroCommon.dll~n~";
        if (!File.Exists("RageNativeUI.dll")) missingDepend += "RageNativeUI.dll~n~";
        if (missingDepend.Length > 0)
        {
            Log.Error($"These dependencies are not installed correctly!\r\n{missingDepend.Replace("~n~", "\r\n")}\r\nDeadlyWeapons could not load!");
            Game.DisplayNotification($"DeadlyWeapons: These dependencies are not installed correctly!~n~{missingDepend}~r~Plugin is disabled!");
            return;
        }
        
        Settings.LoadSettings();
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        Log.Info(Assembly.GetExecutingAssembly().GetName().Version +
                        " by SuperPyroManiac has been initialised.");
        Log.Info("Go on duty with LSPDFR to start the plugin.");
        Game.AddConsoleCommands(new[] {typeof(ConsoleCommands)});
    }

    private void OnOnDutyStateChangedHandler(bool onDuty)
    {
        if (!onDuty) return;
        if (PyroCommon.Main.UsingUb) Log.Info("Using UltimateBackup API.");
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
        
    public override void Finally()
    {
        DamageTrackerService.Stop();
        _panicFiber.Abort();
        Events.OnPulloverStarted -= CustomPullover.PulloverModule;
        Log.Info("DeadlyWeapons by SuperPyroManiac has been disabled.");
    }
}