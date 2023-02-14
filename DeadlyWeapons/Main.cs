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
        internal GameFiber _panicFiber;
 
        public override void Initialize()
        {
            Settings.LoadSettings();
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("DeadlyWeapons " + Assembly.GetExecutingAssembly().GetName().Version +
                            " by SuperPyroManiac has been initialised.");
            Game.LogTrivial("Go on duty with LSPDFR to start the plugin.");
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
            Game.LogTrivial("DeadlyWeapons by SuperPyroManiac has been disabled.");
        }
    }
}