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
            Game.LogTrivial("Deadly Weapons " + Assembly.GetExecutingAssembly().GetName().Version +
                            " by SuperPyroManiac has been initialised.");
            Game.LogTrivial("Go on duty with LSPDFR to start the plugin.");
            Game.AddConsoleCommands(new[] {typeof(DFunctions.ConsoleCommands)});
        }

        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
                GameFiber.StartNew(delegate
                {
                    GameFiber.Wait(10000);
                    DamageTrackerService.Start();
                    if (Settings.EnablePanic) Panic.StartPanicWatch();

                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Deadly Weapons",
                        "~g~Plugin Loaded.",
                        "Deadly Weapons version: " +
                        Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
                    VersionChecker.IsUpdateAvailable();
                });
        }
        
        
        
        public override void Finally()
        {
            DamageTrackerService.Stop();
            Panic.StopPanicWatch();
            Game.LogTrivial("Deadly Weapons by SuperPyroManiac has been disabled.");
        }
    }
}