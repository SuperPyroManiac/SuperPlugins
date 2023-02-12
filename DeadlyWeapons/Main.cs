using System.Reflection;
using DamageTrackerLib;
using DeadlyWeapons.DFunctions;
using DeadlyWeapons.Modules;
using LSPD_First_Response.Mod.API;
using Rage;

namespace DeadlyWeapons
{
    public class Main : Plugin
    {
 
        public override void Initialize()
        {
            Settings.LoadSettings();
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("DeadlyWeapons " + Assembly.GetExecutingAssembly().GetName().Version +
                            " by SuperPyroManiac has been initialised.");
            Game.LogTrivial("Go on duty with LSPDFR to start the plugin.");
            Game.AddConsoleCommands(new[] {typeof(ConsoleCommands)});
        }

        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
            {
                DamageTrackerService.Start();
                if (Settings.EnablePlayerDamageSystem)
                    DamageTrackerService.OnPlayerTookDamage += PlayerShot.OnPlayerDamaged;
                if (Settings.EnableAIDamageSystem)
                    DamageTrackerService.OnPedTookDamage += PedShot.OnPedDamaged;
                GameFiber.StartNew(delegate
                {
                    GameFiber.Wait(10000);
                    if (Settings.EnablePanic) Panic.StartPanicWatch();
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
            Panic.StopPanicWatch();
            Game.LogTrivial("DeadlyWeapons by SuperPyroManiac has been disabled.");
        }
    }
}