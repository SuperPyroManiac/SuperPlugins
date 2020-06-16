#region

using System.Reflection;
using DeadlyWeapons.DFunctions;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;

#endregion

namespace DeadlyWeapons
{
    public class Main : Plugin
    {
        private static readonly DeadlyWeapons StartDamageCheck = new DeadlyWeapons();

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
                    StartDamageCheck.Start();
                    //if (Settings.EnableEMS)
                    //{
                     //   StartEMSCheck.Start();
                    //}
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Deadly Weapons",
                        "~g~Plugin Loaded.",
                        "Deadly Weapons by SuperPyroManiac version: " +
                        Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
                });
        }

        public override void Finally()
        {
            Game.DisableAutomaticRespawn = false;
            Game.FadeScreenOutOnDeath = true;
            StartDamageCheck.ProcessFiber.Abort();
            Game.LogTrivial("Deadly Weapons by SuperPyroManiac has been disabled.");
        }
    }
}