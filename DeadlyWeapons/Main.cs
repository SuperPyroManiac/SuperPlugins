using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;

namespace DeadlyWeapons
{
    public class Main : Plugin
    {
        private static readonly DeadlyWeapons StartScript = new DeadlyWeapons();
        public override void Initialize()
        {
            Settings.LoadSettings();
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("Deadly Weapons " + Assembly.GetExecutingAssembly().GetName().Version + " by SuperPyroManiac has been initialised.");
            Game.LogTrivial("Go on duty with LSPDFR to start the plugin.");
        }
        
        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
                GameFiber.StartNew(delegate
                {
                    GameFiber.Wait(10000);
                    StartScript.Start();
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Deadly Weapons", "~g~Plugin Loaded.", "Deadly Weapons by SuperPyroManiac version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
                });
        }

        public override void Finally()
        {
            StartScript.ProcessFiber.Abort();
            Game.LogTrivial("Deadly Weapons by SuperPyroManiac has been disabled.");
        }
    }
}