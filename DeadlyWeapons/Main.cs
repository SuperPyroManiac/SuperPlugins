using System;
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
    private static readonly Func<string, bool> IsLoaded = plugName =>
        Functions.GetAllUserPlugins().Any(assembly => assembly.GetName().Name.Equals(plugName));
    private GameFiber _panicFiber;
 
    public override void Initialize()
    {
        if (!IsLoaded("PyroCommon"))
        {
            Log.Error("PyroCommon.dll is not installed in the main GTA directory!\r\nDeadlyWeapons could not load!");
            Game.DisplayNotification("DeadlyWeapons: PyroCommon.dll is not installed correctly! Plugin is disabled!");
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
            GameFiber.StartNew(delegate
            {
                GameFiber.Wait(10000);
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~DeadlyWeapons",
                    "~g~Plugin Loaded.",
                    "DeadlyWeapons version: " +
                    Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
                VersionChecker.IsUpdateAvailable();
            });
        }
        else
        {
            DamageTrackerService.OnPlayerTookDamage -= PlayerShot.OnPlayerDamaged;
            DamageTrackerService.OnPedTookDamage -= PedShot.OnPedDamaged;
        }
    }
        
    public override void Finally()
    {
        DamageTrackerService.Stop();
        _panicFiber.Abort();
        Events.OnPulloverStarted -= CustomPullover.PulloverModule;
        Log.Info("DeadlyWeapons by SuperPyroManiac has been disabled.");
    }
}