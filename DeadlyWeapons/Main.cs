using System;
using System.Diagnostics;
using System.Reflection;
using DamageTrackerLib;
using DeadlyWeapons.Modules;
using LSPD_First_Response.Mod.API;
using PyroCommon.PyroFunctions;
using Rage;
using DependManager = PyroCommon.PyroFunctions.DependManager;

namespace DeadlyWeapons;

public class Main : Plugin
{
    internal static bool Running;
    private GameFiber _panicFiber;
    
    public override void Initialize()
    {
        var dCheck = new DependManager();
        dCheck.AddDepend("PyroCommon.dll", "1.10.0.0");
        dCheck.AddDepend("DamageTrackerLib.dll", "2.0.0");
        dCheck.AddDepend("RageNativeUI.dll", "1.9.2.0");
        if ( !dCheck.CheckDepends() ) return;
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
    }

    private void OnOnDutyStateChangedHandler(bool onDuty)
    {
        if (onDuty)
        {
            PyroCommon.Main.InitCommon("DeadlyWeapons", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Running = true;
            Log.Info("DeadlyWeapons by SuperPyroManiac loaded successfully!");
            Log.Info("======================================================");
            Log.Info("Dependencies Found:");
            Log.Info($"PyroCommon, Version: {new Version(FileVersionInfo.GetVersionInfo("PyroCommon.dll").FileVersion)}");
            Log.Info($"DamageTrackerLib, Version: {new Version(FileVersionInfo.GetVersionInfo("DamageTrackerLib.dll").FileVersion)}");
            Log.Info($"Using Ultimate Backup: {PyroCommon.Main.UsingUb}");
            Log.Info($"Using StopThePed: {PyroCommon.Main.UsingStp}");
            Log.Info("======================================================");
            DamageTrackerService.Start();
            Settings.LoadSettings();
            if (Settings.PlayerDamage) DamageTrackerService.OnPlayerTookDamage += PlayerShot.OnPlayerDamaged;
            if (Settings.NpcDamage) DamageTrackerService.OnPedTookDamage += PedShot.OnPedDamaged;
            if (Settings.Panic) _panicFiber = GameFiber.StartNew(Panic.StartPanicFiber);
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~DeadlyWeapons", "~g~Plugin Loaded.", "DeadlyWeapons version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
            return;
        }
        PyroCommon.Main.StopCommon();
    }
        
    public override void Finally()
    {
        Running = false;
        DamageTrackerService.OnPlayerTookDamage -= PlayerShot.OnPlayerDamaged;
        DamageTrackerService.OnPedTookDamage -= PedShot.OnPedDamaged;
        DamageTrackerService.Stop();
        _panicFiber?.Abort();
        PyroCommon.Main.StopCommon();
    }
}