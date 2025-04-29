using System;
using System.Diagnostics;
using System.Reflection;
using DamageTrackerLib;
using DeadlyWeapons.Modules;
using LSPD_First_Response.Mod.API;
using PyroCommon.Utils;
using Rage;
using DependManager = PyroCommon.Services.DependencyService;

namespace DeadlyWeapons;

public class Main : Plugin
{
    internal static bool Running;
    private GameFiber _panicFiber;

    public override void Initialize()
    {
        var dCheck = new DependManager();
        dCheck.AddDepend("PyroCommon.dll", "1.13.0.0");
        dCheck.AddDepend("DamageTrackerLib.dll", "2.0.0");
        dCheck.AddDepend("RageNativeUI.dll", "1.9.3.0");
        if (!dCheck.CheckDepends())
            return;
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
    }

    private void OnOnDutyStateChangedHandler(bool onDuty)
    {
        if (onDuty)
        {
            PyroCommon.Main.InitCommon("DeadlyWeapons", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Running = true;
            LogUtils.Info("DeadlyWeapons by SuperPyroManiac loaded successfully!");
            LogUtils.Info("======================================================");
            LogUtils.Info("Dependencies Found:");
            LogUtils.Info($"PyroCommon, Version: {new Version(FileVersionInfo.GetVersionInfo("PyroCommon.dll").FileVersion)}");
            LogUtils.Info($"DamageTrackerLib, Version: {new Version(FileVersionInfo.GetVersionInfo("DamageTrackerLib.dll").FileVersion)}");
            LogUtils.Info($"Using Ultimate Backup: {PyroCommon.Main.UsingUb}");
            LogUtils.Info($"Using StopThePed: {PyroCommon.Main.UsingStp}");
            LogUtils.Info("======================================================");
            DamageTrackerService.Start();
            Settings.LoadSettings();
            if (Settings.PlayerDamage)
                DamageTrackerService.OnPlayerTookDamage += PlayerShot.OnPlayerDamaged;
            if (Settings.NpcDamage)
                DamageTrackerService.OnPedTookDamage += PedShot.OnPedDamaged;
            if (Settings.Panic)
                _panicFiber = GameFiber.StartNew(Panic.StartPanicFiber);
            Game.DisplayNotification(
                "3dtextures",
                "mpgroundlogo_cops",
                "~r~DeadlyWeapons",
                "~g~Plugin Loaded.",
                "DeadlyWeapons version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded."
            );
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
