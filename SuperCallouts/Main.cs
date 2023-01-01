using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;

namespace SuperCallouts
{
    internal class Main : Plugin
    {
        internal static bool UsingUb { get; set; }
        internal static bool UsingCi { get; set; }
        
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Settings.LoadSettings();
            Game.LogTrivial("SuperCallouts " +
                            Assembly.GetExecutingAssembly().GetName().Version +
                            " by SuperPyroManiac has been initialised.");
            Game.LogTrivial("Go on duty with LSPDFR to fully load SuperCallouts.");
            Game.AddConsoleCommands();
        }
        
        private void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
                GameFiber.StartNew(delegate
                {
                    GameFiber.Wait(10000);
                    UsingUb = CFunctions.IsLoaded("UltimateBackup");
                    if (UsingUb) Game.LogTrivial("SuperCallouts: Using UltimateBackup API.");
                    UsingCi = CFunctions.IsLoaded("CalloutInterface");
                    if (UsingCi) Game.LogTrivial("SuperCallouts: Using CalloutInterface API.");
                    RegisterCallouts();
                    VersionChecker.IsUpdateAvailable();
                });
        }
        
        public override void Finally()
        {
            Game.LogTrivial("SuperCallouts - WARNING: LSPDFR has been unloaded or player is off duty!");
            Game.LogTrivial("SuperCallouts by SuperPyroManiac has been disabled.");
        }
        
        private static void RegisterCallouts()
        {
            if (Settings.HotPursuit) { Functions.RegisterCallout(typeof(HotPursuit)); Game.LogTrivial("SuperCallouts: HotPursuit Enabled"); }
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SuperCallouts", "~g~Plugin Loaded.",
                "SuperCallouts version: " +
                Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
        }
    }
}