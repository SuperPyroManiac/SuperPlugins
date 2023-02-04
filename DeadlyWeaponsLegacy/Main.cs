#region

using System.Reflection;
using DeadlyWeaponsLegacy.DFunctions;
using DeadlyWeaponsLegacy.Modules;
using LSPD_First_Response.Mod.API;
using Rage;

#endregion

namespace DeadlyWeaponsLegacy
{
    internal class Main : Plugin
    {
        private static readonly Run Startup = new();

        public override void Initialize()
        {
            Settings.LoadSettings();
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("Deadly Weapons " + Assembly.GetExecutingAssembly().GetName().Version +
                            " by SuperPyroManiac has been initialised.");
            Game.LogTrivial("Go on duty with LSPDFR to start the plugin.");
            Game.AddConsoleCommands(new[] {typeof(ConsoleCommands)});
        }

        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
                GameFiber.StartNew(delegate
                {
                    GameFiber.Wait(10000);
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Deadly Weapons",
                        "~g~Plugin Loaded.",
                        "Deadly Weapons version: " +
                        Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
                    VersionChecker.IsUpdateAvailable();
                    Startup.Start();
                });
        }
        
        public override void Finally()
        {
            Game.LogTrivial("Deadly Weapons by SuperPyroManiac has been disabled.");
            Startup.Stop();
        }
    }
}