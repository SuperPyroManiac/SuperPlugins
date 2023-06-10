using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;
using SuperCallouts.Callouts;
using SuperCallouts.SimpleFunctions;
using Fire = SuperCallouts.Callouts.Fire;

namespace SuperCallouts;

internal class Main : Plugin
{
    internal static bool UsingUb { get; set; }

    public override void Initialize()
    {
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        Settings.LoadSettings();
        Game.Console.Print("SuperCallouts " +
                        Assembly.GetExecutingAssembly().GetName().Version +
                        " by SuperPyroManiac has been initialised.");
        Game.Console.Print("Go on duty with LSPDFR to fully load SuperCallouts.");
        Game.AddConsoleCommands();
    }

    private void OnOnDutyStateChangedHandler(bool onDuty)
    {
        if (onDuty)
            GameFiber.StartNew(delegate
            {
                GameFiber.Wait(5000);
                UsingUb = CFunctions.IsLoaded("UltimateBackup");
                if (UsingUb) Game.Console.Print("SuperCallouts: Using UltimateBackup API.");
                RegisterCallouts();
                GameFiber.Wait(17000);
                VersionChecker.IsUpdateAvailable();
            });
    }

    private static void RegisterCallouts()
    {
        if (Settings.HotPursuit) { Functions.RegisterCallout(typeof(HotPursuit)); Game.Console.Print("SuperCallouts: HotPursuit Enabled"); }
        if (Settings.Robbery) { Functions.RegisterCallout(typeof(Robbery)); Game.Console.Print("SuperCallouts: Robbery Enabled"); }
        if (Settings.CarAccident) { Functions.RegisterCallout(typeof(CarAccident)); Game.Console.Print("SuperCallouts: CarAccident Enabled"); }
        if (Settings.CarAccident) { Functions.RegisterCallout(typeof(CarAccident2)); }
        if (Settings.CarAccident) { Functions.RegisterCallout(typeof(CarAccident3)); }
        if (Settings.Animals) { Functions.RegisterCallout(typeof(AngryAnimal)); Game.Console.Print("SuperCallouts: AngryAnimals Enabled"); }
        if (Settings.Kidnapping) { Functions.RegisterCallout(typeof(Kidnapping)); Game.Console.Print("SuperCallouts: Kidnapping Enabled"); }
        if (Settings.TruckCrash) { Functions.RegisterCallout(typeof(TruckCrash)); Game.Console.Print("SuperCallouts: TruckCrash Enabled"); }
        if (Settings.PrisonTransport) { Functions.RegisterCallout(typeof(PrisonTransport)); Game.Console.Print("SuperCallouts: PrisonTransport Enabled"); }
        if (Settings.HitRun) { Functions.RegisterCallout(typeof(HitRun)); Game.Console.Print("SuperCallouts: Hit and Run Enabled"); }
        if (Settings.StolenCopVehicle) { Functions.RegisterCallout(typeof(StolenCopVehicle)); Game.Console.Print("SuperCallouts: Stolen police vehicle Enabled"); }
        if (Settings.StolenDumptruck) { Functions.RegisterCallout(typeof(StolenDumptruck)); Game.Console.Print("SuperCallouts: Stolen dump truck Enabled"); }
        if (Settings.AmbulanceEscort) { Functions.RegisterCallout(typeof(AmbulanceEscort)); Game.Console.Print("SuperCallouts: AmbulanceEscort Enabled"); }
        if (Settings.Aliens) { Functions.RegisterCallout(typeof(Aliens)); Game.Console.Print("SuperCallouts: Aliens Enabled"); }
        if (Settings.OpenCarry) { Functions.RegisterCallout(typeof(OpenCarry)); Game.Console.Print("SuperCallouts: Open Carry Enabled"); }
        if (Settings.Fire) { Functions.RegisterCallout(typeof(Fire)); Game.Console.Print("SuperCallouts: Fire Enabled"); }
        if (Settings.OfficerShootout) { Functions.RegisterCallout(typeof(OfficerShootout)); Game.Console.Print("SuperCallouts: OfficerShootout Enabled"); }
        if (Settings.WeirdCar) { Functions.RegisterCallout(typeof(WeirdCar)); Game.Console.Print("SuperCallouts: Suspicious Enabled"); }
        if (Settings.Manhunt) { Functions.RegisterCallout(typeof(Manhunt)); Game.Console.Print("SuperCallouts: Manhunt Enabled"); }
        if (Settings.Impersonator) { Functions.RegisterCallout(typeof(Impersonator)); Game.Console.Print("SuperCallouts: Impersonator Enabled"); }
        if (Settings.ToiletPaperBandit) { Functions.RegisterCallout(typeof(ToiletPaperBandit)); Game.Console.Print("SuperCallouts: ToiletPaperBandit Enabled"); }
        if (Settings.BlockingTraffic) { Functions.RegisterCallout(typeof(BlockingTraffic)); Game.Console.Print("SuperCallouts: Blocking Traffic Enabled"); }
        if (Settings.IllegalParking) { Functions.RegisterCallout(typeof(IllegalParking)); Game.Console.Print("SuperCallouts: Illegal Parking Enabled"); }
        if (Settings.KnifeAttack) { Functions.RegisterCallout(typeof(KnifeAttack)); Game.Console.Print("SuperCallouts: Knife Attack Enabled"); }
        if (Settings.DeadBody) { Functions.RegisterCallout(typeof(DeadBody)); Game.Console.Print("SuperCallouts: Dead Body Enabled"); }
        if (Settings.FakeCall) { Functions.RegisterCallout(typeof(FakeCall)); Game.Console.Print("SuperCallouts: FakeCall Enabled"); }
        if (Settings.Trespassing) { Functions.RegisterCallout(typeof(Trespassing)); Game.Console.Print("SuperCallouts: Trespassing Enabled"); }
        if (Settings.PrisonBreak) { Functions.RegisterCallout(typeof(PrisonBreak)); Game.Console.Print("SuperCallouts: PrisonBreak Enabled"); }
        if (Settings.Mafia1) { Functions.RegisterCallout(typeof(Mafia1)); Game.Console.Print("SuperCallouts: Mafia1 Enabled"); }
        if (Settings.Mafia2) { Functions.RegisterCallout(typeof(Mafia2)); Game.Console.Print("SuperCallouts: Mafia2 Enabled"); }
        if (Settings.Mafia3) { Functions.RegisterCallout(typeof(Mafia3)); Game.Console.Print("SuperCallouts: Mafia3 Enabled"); }
        if (Settings.Mafia4) { Functions.RegisterCallout(typeof(Mafia4)); Game.Console.Print("SuperCallouts: Mafia4 Enabled"); }
        if (Settings.LostMc) { Functions.RegisterCallout(typeof(LostGang)); Game.Console.Print("SuperCallouts: LostMC Enabled"); }
        if (Settings.Lsgtf) { Functions.RegisterCallout(typeof(Lsgtf)); Game.Console.Print("SuperCallouts: LSGTF Enabled"); }
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SuperCallouts", "~g~Plugin Loaded.",
            "SuperCallouts version: " +
            Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
    }

    public override void Finally()
    {
        Game.Console.Print("SuperCallouts by SuperPyroManiac has been cleaned up.");
    }
}