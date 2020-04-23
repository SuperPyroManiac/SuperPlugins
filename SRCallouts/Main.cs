using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;
using SRCallouts.Callouts;

namespace SRCallouts
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Settings.LoadSettings();
            Game.LogTrivial("SR Callouts " +
                            Assembly.GetExecutingAssembly().GetName().Version +
                            " by SuperPyroManiac has been initialised.");
            Game.LogTrivial("Go on duty with LSPDFR to fully load SR Callouts.");
        }

        private void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
                GameFiber.StartNew(delegate
                {
                    GameFiber.Wait(10000);
                    RegisterCallouts();
                });
        }

        private static void RegisterCallouts()
        {
            if (Settings.Mafia1) { Functions.RegisterCallout(typeof(Mafia1)); Game.LogTrivial("SR Callouts: Mafia1 Enabled"); }
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SR Callouts", "~g~Plugin Loaded.",
                "SR Callouts version: " +
                Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
        }

        public override void Finally()
        {
            Game.LogTrivial("SR Callouts by SuperPyroManiac has been cleaned up.");
        }
    }
}