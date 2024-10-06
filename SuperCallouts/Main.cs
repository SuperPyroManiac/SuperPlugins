using System;
using System.Diagnostics;
using System.Reflection;
using LSPD_First_Response.Mod.API;
using PyroCommon.PyroFunctions;
using Rage;
using SuperCallouts.Callouts;
using SuperCallouts.RemasteredCallouts;
using SuperCallouts.SimpleFunctions;
using DependManager = PyroCommon.API.DependManager;

namespace SuperCallouts;

internal class Main : Plugin
{
    public override void Initialize()
    {
        DependManager.AddDepend("PyroCommon.dll", "1.10.0.0");
        DependManager.AddDepend("RageNativeUI.dll", "1.9.2.0");
        if ( !DependManager.CheckDepends() ) return;
        
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        Settings.LoadSettings();
        Game.AddConsoleCommands([typeof(ConsoleCommands)]);
    }

    private void OnOnDutyStateChangedHandler(bool onDuty)
    {
        if (onDuty)
        {
            PyroCommon.Main.InitCommon("SuperCallouts", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Log.Info("SuperCallouts by SuperPyroManiac loaded successfully!");
            Log.Info("======================================================");
            Log.Info("Dependencies Found:");
            Log.Info($"PyroCommon, Version: {new Version(FileVersionInfo.GetVersionInfo("PyroCommon.dll").FileVersion)}");
            Log.Info($"Using Ultimate Backup: {PyroCommon.Main.UsingUb}");
            Log.Info($"Using StopThePed: {PyroCommon.Main.UsingStp}");
            Log.Info("======================================================");
            RegisterCallouts();
            return;
        }
        PyroCommon.Main.StopCommon();
    }

    private static void RegisterCallouts()
    {
        //Reg
        if (Settings.HotPursuit) Functions.RegisterCallout(typeof(HotPursuit));
        if (Settings.Robbery) Functions.RegisterCallout(typeof(Robbery));
        if (Settings.CarAccident) Functions.RegisterCallout(typeof(CarAccident));
        if (Settings.CarAccident) Functions.RegisterCallout(typeof(CarAccident2));
        if (Settings.CarAccident) Functions.RegisterCallout(typeof(CarAccident3));
        if (Settings.Animals) Functions.RegisterCallout(typeof(AngryAnimal));
        if (Settings.Kidnapping) Functions.RegisterCallout(typeof(Kidnapping));
        if (Settings.TruckCrash) Functions.RegisterCallout(typeof(TruckCrash));
        if (Settings.PrisonTransport) Functions.RegisterCallout(typeof(PrisonTransport));
        if (Settings.HitRun) Functions.RegisterCallout(typeof(HitRun));
        if (Settings.StolenCopVehicle) Functions.RegisterCallout(typeof(StolenCopVehicle));
        if (Settings.StolenDumptruck) Functions.RegisterCallout(typeof(StolenDumptruck));
        if (Settings.AmbulanceEscort) Functions.RegisterCallout(typeof(AmbulanceEscort));
        if (Settings.Aliens) Functions.RegisterCallout(typeof(Aliens));
        if (Settings.OpenCarry) Functions.RegisterCallout(typeof(OpenCarry));
        if (Settings.Fire) Functions.RegisterCallout(typeof(RemasteredCallouts.Fire));
        if (Settings.OfficerShootout) Functions.RegisterCallout(typeof(OfficerShootout));
        if (Settings.WeirdCar) Functions.RegisterCallout(typeof(WeirdCar));
        if (Settings.Manhunt) Functions.RegisterCallout(typeof(Manhunt));
        if (Settings.Impersonator) Functions.RegisterCallout(typeof(Impersonator));
        if (Settings.ToiletPaperBandit) Functions.RegisterCallout(typeof(ToiletPaperBandit));
        if (Settings.BlockingTraffic) Functions.RegisterCallout(typeof(BlockingTraffic));
        if (Settings.IllegalParking) Functions.RegisterCallout(typeof(IllegalParking));
        if (Settings.KnifeAttack) Functions.RegisterCallout(typeof(KnifeAttack));
        if (Settings.DeadBody) Functions.RegisterCallout(typeof(DeadBody));
        if (Settings.FakeCall) Functions.RegisterCallout(typeof(FakeCall));
        if (Settings.Trespassing) Functions.RegisterCallout(typeof(Trespassing));
        if (Settings.Vandalizing) Functions.RegisterCallout(typeof(Vandalizing));
        if (Settings.InjuredCop) Functions.RegisterCallout(typeof(InjuredCop));
        if (Settings.IndecentExposure) Functions.RegisterCallout(typeof(IndecentExposure));
        if (Settings.Fight) Functions.RegisterCallout(typeof(Fight));
        //Swat
        if (Settings.PrisonBreak) Functions.RegisterCallout(typeof(PrisonBreak));
        if (Settings.Mafia1) Functions.RegisterCallout(typeof(Mafia1));
        if (Settings.Mafia2) Functions.RegisterCallout(typeof(Mafia2));
        if (Settings.Mafia3) Functions.RegisterCallout(typeof(Mafia3));
        if (Settings.Mafia4) Functions.RegisterCallout(typeof(Mafia4));
        if (Settings.LostMc) Functions.RegisterCallout(typeof(LostGang));
        if (Settings.Lsgtf) Functions.RegisterCallout(typeof(Lsgtf));
        Game.DisplayNotification("3dtextures",
            "mpgroundlogo_cops",
            "~r~SuperCallouts",
            "~g~Plugin Loaded.",
            "SuperCallouts version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
    }

    public override void Finally()
    {
        PyroCommon.Main.StopCommon();
    }
}