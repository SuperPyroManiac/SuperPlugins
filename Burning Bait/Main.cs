using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;

namespace Burning_Bait
{
    internal class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("Burning Bait " + Assembly.GetExecutingAssembly().GetName().Version +
                            " by SuperPyroManiac has been initialised.");
            Game.LogTrivial("Go on duty with LSPDFR to start the plugin.");
            Game.AddConsoleCommands(new[] { typeof(SimpleFunctions.ConsoleCommands) });
        }
        
        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
                GameFiber.StartNew(delegate
                {
                    GameFiber.Wait(10000);
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Burning Bait",
                        "~g~Plugin Loaded.",
                        "Burning Bait by SuperPyroManiac version: " +
                        Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
                });
        }

        public override void Finally()
        {
            Game.LogTrivial("Burning Bait by SuperPyroManiac has been disabled.");
        }
    }
}