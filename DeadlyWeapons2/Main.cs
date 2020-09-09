using System.Reflection;
using DeadlyWeapons2.DFunctions;
using DeadlyWeapons2.Modules;
using LSPD_First_Response.Mod.API;
using Rage;

namespace DeadlyWeapons2
{
    internal class Main : Plugin
    {
        private static Run startup = new Run();
        public override void Initialize()
        {
            Settings.LoadSettings();
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("Deadly Weapons " + Assembly.GetExecutingAssembly().GetName().Version +
                            " by SuperPyroManiac has been initialised.");
            Game.LogTrivial("Go on duty with LSPDFR to start the plugin.");
            Game.AddConsoleCommands(new[] { typeof(DFunctions.ConsoleCommands) });
        }
        
        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
                GameFiber.StartNew(delegate
                {
                    GameFiber.Wait(10000);
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Deadly Weapons",
                        "~g~Plugin Loaded.",
                        "Deadly Weapons by SuperPyroManiac version: " +
                        Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
                    VersionChecker.IsUpdateAvailable();
                    startup.Start();
                });
        }
        
        public override void Finally()
        {
            Game.LogTrivial("Deadly Weapons by SuperPyroManiac has been disabled.");
            startup.Stop();
        }
    }
}