using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DamageTrackerLib;
using DeadlyWeapons.Modules;
using DeadlyWeapons.PyroFunctions;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;

namespace DeadlyWeapons;

public class Main : Plugin
{
    private GameFiber _panicFiber;
 
    static Main()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            var assemblyName = new AssemblyName(args.Name).Name + ".dll";
            var resourceName = $"DeadlyWeapons.Libs.{assemblyName}";

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null) return null;

                var assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        };
    }
    
    public override void Initialize()
    {
        DependManager.AddDepend("PyroCommon.dll", "1.6.0.0");
        DependManager.AddDepend("RageNativeUI.dll", "1.9.2.0");
        DependManager.AddDepend("DamageTrackerLib.dll", "1.0.2");
        if ( !DependManager.CheckDepends() ) return;
        
        Settings.LoadSettings();
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        Game.AddConsoleCommands([typeof(ConsoleCommands)]);
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
            Log.Info("======================================================");
            DamageTrackerService.Start();
            if (Settings.PlayerDamage) DamageTrackerService.OnPlayerTookDamage += PlayerShot.OnPlayerDamaged;
            if (Settings.NpcDamage) DamageTrackerService.OnPedTookDamage += PedShot.OnPedDamaged;
            if (Settings.Panic) _panicFiber = GameFiber.StartNew(Panic.StartPanicFiber);
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~DeadlyWeapons", "~g~Plugin Loaded.", "DeadlyWeapons version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
            PyroCommon.Main.InitCommon("DeadlyWeapons", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            return;
        }
        PyroCommon.Main.StopCommon();
    }
        
    public override void Finally()
    {
        DamageTrackerService.Stop();
        _panicFiber.Abort();
        PyroCommon.Main.StopCommon();
        Log.Info("Plugin unloaded!");
    }
}