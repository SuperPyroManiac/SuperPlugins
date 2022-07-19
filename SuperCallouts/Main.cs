using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;
using SuperCallouts.Callouts;
using SuperCallouts.SimpleFunctions;
using Fire = SuperCallouts.Callouts.Fire;

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

        private static void RegisterCallouts()
        {
            if (Settings.HotPursuit) { Functions.RegisterCallout(typeof(HotPursuit)); Game.LogTrivial("SuperCallouts: HotPursuit Enabled"); }
            if (Settings.Robbery) { Functions.RegisterCallout(typeof(Robbery)); Game.LogTrivial("SuperCallouts: Robbery Enabled"); }
            if (Settings.CarAccident) { Functions.RegisterCallout(typeof(CarAccident)); Game.LogTrivial("SuperCallouts: CarAccident Enabled"); }
            if (Settings.CarAccident) { Functions.RegisterCallout(typeof(CarAccident2)); }
            if (Settings.CarAccident) { Functions.RegisterCallout(typeof(CarAccident3)); }
            if (Settings.Animals) { Functions.RegisterCallout(typeof(AngryAnimal)); Game.LogTrivial("SuperCallouts: AngryAnimals Enabled"); }
            if (Settings.Kidnapping) { Functions.RegisterCallout(typeof(Kidnapping)); Game.LogTrivial("SuperCallouts: Kidnapping Enabled"); }
            if (Settings.TruckCrash) { Functions.RegisterCallout(typeof(TruckCrash)); Game.LogTrivial("SuperCallouts: TruckCrash Enabled"); }
            if (Settings.PrisonTransport) { Functions.RegisterCallout(typeof(PrisonTransport)); Game.LogTrivial("SuperCallouts: PrisonTransport Enabled"); }
            if (Settings.HitRun) { Functions.RegisterCallout(typeof(HitRun)); Game.LogTrivial("SuperCallouts: Hit and Run Enabled"); }
            if (Settings.StolenCopVehicle) { Functions.RegisterCallout(typeof(StolenCopVehicle)); Game.LogTrivial("SuperCallouts: Stolen police vehicle Enabled"); }
            if (Settings.AmbulanceEscort) { Functions.RegisterCallout(typeof(AmbulanceEscort)); Game.LogTrivial("SuperCallouts: AmbulanceEscort Enabled"); }
            if (Settings.Aliens) { Functions.RegisterCallout(typeof(Aliens)); Game.LogTrivial("SuperCallouts: Aliens Enabled"); }
            if (Settings.OpenCarry) { Functions.RegisterCallout(typeof(OpenCarry)); Game.LogTrivial("SuperCallouts: Open Carry Enabled"); }
            if (Settings.Fire) { Functions.RegisterCallout(typeof(Fire)); Game.LogTrivial("SuperCallouts: Fire Enabled"); }
            if (Settings.OfficerShootout) { Functions.RegisterCallout(typeof(OfficerShootout)); Game.LogTrivial("SuperCallouts: OfficerShootout Enabled"); }
            if (Settings.WeirdCar) { Functions.RegisterCallout(typeof(WeirdCar)); Game.LogTrivial("SuperCallouts: Suspicious Enabled"); }
            if (Settings.Manhunt) { Functions.RegisterCallout(typeof(Manhunt)); Game.LogTrivial("SuperCallouts: Manhunt Enabled"); }
            if (Settings.Impersonator) { Functions.RegisterCallout(typeof(Impersonator)); Game.LogTrivial("SuperCallouts: Impersonator Enabled"); }
            if (Settings.ToiletPaperBandit) { Functions.RegisterCallout(typeof(ToiletPaperBandit)); Game.LogTrivial("SuperCallouts: ToiletPaperBandit Enabled"); }
            if (Settings.BlockingTraffic) { Functions.RegisterCallout(typeof(BlockingTraffic)); Game.LogTrivial("SuperCallouts: Blocking Traffic Enabled"); }
            if (Settings.IllegalParking) { Functions.RegisterCallout(typeof(IllegalParking)); Game.LogTrivial("SuperCallouts: Illegal Parking Enabled"); }
            if (Settings.KnifeAttack) { Functions.RegisterCallout(typeof(KnifeAttack)); Game.LogTrivial("SuperCallouts: Knife Attack Enabled"); }
            if (Settings.PrisonBreak) { Functions.RegisterCallout(typeof(PrisonBreak)); Game.LogTrivial("SuperCallouts: PrisonBreak Enabled"); }
            if (Settings.Mafia1) { Functions.RegisterCallout(typeof(Mafia1)); Game.LogTrivial("SuperCallouts: Mafia1 Enabled"); }
            if (Settings.Mafia2) { Functions.RegisterCallout(typeof(Mafia2)); Game.LogTrivial("SuperCallouts: Mafia2 Enabled"); }
            if (Settings.Mafia3) { Functions.RegisterCallout(typeof(Mafia3)); Game.LogTrivial("SuperCallouts: Mafia3 Enabled"); }
            if (Settings.Mafia4) { Functions.RegisterCallout(typeof(Mafia4)); Game.LogTrivial("SuperCallouts: Mafia4 Enabled"); }
            if (Settings.LostMc) { Functions.RegisterCallout(typeof(LostGang)); Game.LogTrivial("SuperCallouts: LostMC Enabled"); }
            if (Settings.Lsgtf) { Functions.RegisterCallout(typeof(Lsgtf)); Game.LogTrivial("SuperCallouts: LSGTF Enabled"); }
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SuperCallouts", "~g~Plugin Loaded.",
                "SuperCallouts version: " +
                Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
        }

        public override void Finally()
        {
            Game.LogTrivial("SuperCallouts by SuperPyroManiac has been cleaned up.");
        }
    }
}