using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;
using SuperCallouts.Callouts;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts;

internal class Main : Plugin
{
    public override void Initialize()
    {
        var missingDepend = string.Empty;
        if (!File.Exists("PyroCommon.dll")) missingDepend += "PyroCommon.dll~n~";
        if (!File.Exists("RageNativeUI.dll")) missingDepend += "RageNativeUI.dll~n~";
        if (missingDepend.Length > 0)
        {
            Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print($"SuperCallouts: Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print($"These dependencies are not installed correctly!\r\n{missingDepend.Replace("~n~", "\r\n")}SuperCallouts could not load!");
            Game.Console.Print("======================================================");
            Game.Console.Print($"SuperCallouts: Error Report End");
            Game.DisplayNotification($"SuperCallouts: These dependencies are not installed correctly!~n~{missingDepend}~r~Plugin is disabled!");
            return;
        }
        
        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        Settings.LoadSettings();
        Log.Info(Assembly.GetExecutingAssembly().GetName().Version +
                        " by SuperPyroManiac has been initialised.");
        Log.Info("Go on duty with LSPDFR to fully load SuperCallouts.");
        Game.AddConsoleCommands();
    }

    private void OnOnDutyStateChangedHandler(bool onDuty)
    {
        if (!onDuty) return;
        if (PyroCommon.Main.UsingUb) Log.Info("Using UltimateBackup API.");
        RegisterCallouts();
        GameFiber.StartNew(VersionChecker.IsUpdateAvailable);
    }

    private static void RegisterCallouts()
    {
        //Reg
        if (Settings.HotPursuit) { Functions.RegisterCallout(typeof(HotPursuit)); Log.Info("HotPursuit Enabled"); }
        if (Settings.Robbery) { Functions.RegisterCallout(typeof(Robbery)); Log.Info("Robbery Enabled"); }
        if (Settings.CarAccident) { Functions.RegisterCallout(typeof(CarAccident)); Log.Info("CarAccidents Enabled"); }
        if (Settings.CarAccident) { Functions.RegisterCallout(typeof(CarAccident2)); }
        if (Settings.CarAccident) { Functions.RegisterCallout(typeof(CarAccident3)); }
        if (Settings.Animals) { Functions.RegisterCallout(typeof(AngryAnimal)); Log.Info("AngryAnimals Enabled"); }
        if (Settings.Kidnapping) { Functions.RegisterCallout(typeof(Kidnapping)); Log.Info("Kidnapping Enabled"); }
        if (Settings.TruckCrash) { Functions.RegisterCallout(typeof(TruckCrash)); Log.Info("TruckCrash Enabled"); }
        if (Settings.PrisonTransport) { Functions.RegisterCallout(typeof(PrisonTransport)); Log.Info("PrisonTransport Enabled"); }
        if (Settings.HitRun) { Functions.RegisterCallout(typeof(HitRun)); Log.Info("Hit and Run Enabled"); }
        if (Settings.StolenCopVehicle) { Functions.RegisterCallout(typeof(StolenCopVehicle)); Log.Info("Stolen police vehicle Enabled"); }
        if (Settings.StolenDumptruck) { Functions.RegisterCallout(typeof(StolenDumptruck)); Log.Info("Stolen dump truck Enabled"); }
        if (Settings.AmbulanceEscort) { Functions.RegisterCallout(typeof(AmbulanceEscort)); Log.Info("AmbulanceEscort Enabled"); }
        if (Settings.Aliens) { Functions.RegisterCallout(typeof(Aliens)); Log.Info("Aliens Enabled"); }
        if (Settings.OpenCarry) { Functions.RegisterCallout(typeof(OpenCarry)); Log.Info("Open Carry Enabled"); }
        if (Settings.Fire) { Functions.RegisterCallout(typeof(Callouts.Fire)); Log.Info("Fire Enabled"); }
        if (Settings.OfficerShootout) { Functions.RegisterCallout(typeof(OfficerShootout)); Log.Info("OfficerShootout Enabled"); }
        if (Settings.WeirdCar) { Functions.RegisterCallout(typeof(WeirdCar)); Log.Info("Suspicious Enabled"); }
        if (Settings.Manhunt) { Functions.RegisterCallout(typeof(Manhunt)); Log.Info("Manhunt Enabled"); }
        if (Settings.Impersonator) { Functions.RegisterCallout(typeof(Impersonator)); Log.Info("Impersonator Enabled"); }
        if (Settings.ToiletPaperBandit) { Functions.RegisterCallout(typeof(ToiletPaperBandit)); Log.Info("ToiletPaperBandit Enabled"); }
        if (Settings.BlockingTraffic) { Functions.RegisterCallout(typeof(BlockingTraffic)); Log.Info("Blocking Traffic Enabled"); }
        if (Settings.IllegalParking) { Functions.RegisterCallout(typeof(IllegalParking)); Log.Info("Illegal Parking Enabled"); }
        if (Settings.KnifeAttack) { Functions.RegisterCallout(typeof(KnifeAttack)); Log.Info("Knife Attack Enabled"); }
        if (Settings.DeadBody) { Functions.RegisterCallout(typeof(DeadBody)); Log.Info("Dead Body Enabled"); }
        if (Settings.FakeCall) { Functions.RegisterCallout(typeof(FakeCall)); Log.Info("FakeCall Enabled"); }
        if (Settings.Trespassing) { Functions.RegisterCallout(typeof(Trespassing)); Log.Info("Trespassing Enabled"); }
        if (Settings.Vandalizing) {Functions.RegisterCallout(typeof(Vandalizing)); Log.Info("Vandalizing Enabled");}
        if (Settings.InjuredCop) {Functions.RegisterCallout(typeof(InjuredCop)); Log.Info("InjuredCop Enabled");}
        if (Settings.IndecentExposure) {Functions.RegisterCallout(typeof(IndecentExposure)); Log.Info("IndecentExposure Enabled");}
        //Swat
        if (Settings.PrisonBreak) { Functions.RegisterCallout(typeof(PrisonBreak)); Log.Info("PrisonBreak Enabled"); }
        if (Settings.Mafia1) { Functions.RegisterCallout(typeof(Mafia1)); Log.Info("Mafia1 Enabled"); }
        if (Settings.Mafia2) { Functions.RegisterCallout(typeof(Mafia2)); Log.Info("Mafia2 Enabled"); }
        if (Settings.Mafia3) { Functions.RegisterCallout(typeof(Mafia3)); Log.Info("Mafia3 Enabled"); }
        if (Settings.Mafia4) { Functions.RegisterCallout(typeof(Mafia4)); Log.Info("Mafia4 Enabled"); }
        if (Settings.LostMc) { Functions.RegisterCallout(typeof(LostGang)); Log.Info("LostMC Enabled"); }
        if (Settings.Lsgtf) { Functions.RegisterCallout(typeof(Lsgtf)); Log.Info("LSGTF Enabled"); }
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SuperCallouts", "~g~Plugin Loaded.",
            "SuperCallouts version: " +
            Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
    }

    public override void Finally()
    {
        Log.Info("Plugin has been cleaned up.");
    }
}