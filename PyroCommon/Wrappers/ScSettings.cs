using System.Reflection;
using System.Windows.Forms;

namespace PyroCommon.Wrappers;

internal static class ScSettings
{
    internal static bool Animals;
    internal static bool Robbery;
    internal static bool CarAccident;
    internal static bool HotPursuit;
    internal static bool Kidnapping;
    internal static bool TruckCrash;
    internal static bool PrisonTransport;
    internal static bool HitRun;
    internal static bool StolenCopVehicle;
    internal static bool StolenDumptruck;
    internal static bool AmbulanceEscort;
    internal static bool Aliens;
    internal static bool OpenCarry;
    internal static bool Fire;
    internal static bool OfficerShootout;
    internal static bool WeirdCar;
    internal static bool Manhunt;
    internal static bool Impersonator;
    internal static bool ToiletPaperBandit;
    internal static bool BlockingTraffic;
    internal static bool IllegalParking;
    internal static bool KnifeAttack;
    internal static bool DeadBody;
    internal static bool FakeCall;
    internal static bool Trespassing;
    internal static bool Vandalizing;
    internal static bool InjuredCop;
    internal static bool IndecentExposure;
    internal static bool Fight;
    internal static bool PrisonBreak;
    internal static bool Mafia1;
    internal static bool Mafia2;
    internal static bool Mafia3;
    internal static bool Mafia4;
    internal static bool LostMc;
    internal static bool Lsgtf;
    internal static Keys Interact;
    internal static Keys EndCall;
    internal static string EmergencyNumber = "911";

    internal static void GetSettings()
    {
        if ( !Main.UsingSc ) return;
        var settingsType = Assembly.Load("SuperCallouts").GetType("SuperCallouts.Settings");
        Animals = ( bool )settingsType.GetField("Animals", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Robbery = ( bool )settingsType.GetField("Robbery", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        CarAccident = ( bool )settingsType.GetField("CarAccident", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        HotPursuit = ( bool )settingsType.GetField("HotPursuit", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Kidnapping = ( bool )settingsType.GetField("Kidnapping", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        TruckCrash = ( bool )settingsType.GetField("TruckCrash", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        PrisonTransport = ( bool )settingsType.GetField("PrisonTransport", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        HitRun = ( bool )settingsType.GetField("HitRun", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        StolenCopVehicle = ( bool )settingsType.GetField("StolenCopVehicle", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        StolenDumptruck = ( bool )settingsType.GetField("StolenDumptruck", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        AmbulanceEscort = ( bool )settingsType.GetField("AmbulanceEscort", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Aliens = ( bool )settingsType.GetField("Aliens", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        OpenCarry = ( bool )settingsType.GetField("OpenCarry", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Fire = ( bool )settingsType.GetField("Fire", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        OfficerShootout = ( bool )settingsType.GetField("OfficerShootout", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        WeirdCar = ( bool )settingsType.GetField("WeirdCar", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Manhunt = ( bool )settingsType.GetField("Manhunt", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Impersonator = ( bool )settingsType.GetField("Impersonator", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        ToiletPaperBandit = ( bool )settingsType.GetField("ToiletPaperBandit", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        BlockingTraffic = ( bool )settingsType.GetField("BlockingTraffic", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        IllegalParking = ( bool )settingsType.GetField("IllegalParking", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        KnifeAttack = ( bool )settingsType.GetField("KnifeAttack", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        DeadBody = ( bool )settingsType.GetField("DeadBody", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        FakeCall = ( bool )settingsType.GetField("FakeCall", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Trespassing = ( bool )settingsType.GetField("Trespassing", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Vandalizing = ( bool )settingsType.GetField("Vandalizing", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        InjuredCop = ( bool )settingsType.GetField("InjuredCop", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        IndecentExposure = ( bool )settingsType.GetField("IndecentExposure", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Fight = ( bool )settingsType.GetField("Fight", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        PrisonBreak = ( bool )settingsType.GetField("PrisonBreak", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Mafia1 = ( bool )settingsType.GetField("Mafia1", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Mafia2 = ( bool )settingsType.GetField("Mafia2", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Mafia3 = ( bool )settingsType.GetField("Mafia3", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Mafia4 = ( bool )settingsType.GetField("Mafia4", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        LostMc = ( bool )settingsType.GetField("LostMc", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Lsgtf = ( bool )settingsType.GetField("Lsgtf", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Interact = ( Keys )settingsType.GetField("Interact", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        EndCall = ( Keys )settingsType.GetField("EndCall", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        EmergencyNumber = ( string )settingsType.GetField("EmergencyNumber", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
    }

    internal static void ApplySettings()
    {
        if ( !Main.UsingSc ) return;
        var settingsType = Assembly.Load("SuperCallouts").GetType("SuperCallouts.Settings");
        settingsType.GetField("Animals", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Animals);
        settingsType.GetField("Robbery", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Robbery);
        settingsType.GetField("CarAccident", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, CarAccident);
        settingsType.GetField("HotPursuit", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, HotPursuit);
        settingsType.GetField("Kidnapping", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Kidnapping);
        settingsType.GetField("TruckCrash", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, TruckCrash);
        settingsType.GetField("PrisonTransport", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, PrisonTransport);
        settingsType.GetField("HitRun", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, HitRun);
        settingsType.GetField("StolenCopVehicle", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, StolenCopVehicle);
        settingsType.GetField("StolenDumptruck", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, StolenDumptruck);
        settingsType.GetField("AmbulanceEscort", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, AmbulanceEscort);
        settingsType.GetField("Aliens", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Aliens);
        settingsType.GetField("OpenCarry", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, OpenCarry);
        settingsType.GetField("Fire", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Fire);
        settingsType.GetField("OfficerShootout", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, OfficerShootout);
        settingsType.GetField("WeirdCar", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, WeirdCar);
        settingsType.GetField("Manhunt", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Manhunt);
        settingsType.GetField("Impersonator", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Impersonator);
        settingsType.GetField("ToiletPaperBandit", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, ToiletPaperBandit);
        settingsType.GetField("BlockingTraffic", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, BlockingTraffic);
        settingsType.GetField("IllegalParking", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, IllegalParking);
        settingsType.GetField("KnifeAttack", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, KnifeAttack);
        settingsType.GetField("DeadBody", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, DeadBody);
        settingsType.GetField("FakeCall", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, FakeCall);
        settingsType.GetField("Trespassing", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Trespassing);
        settingsType.GetField("Vandalizing", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Vandalizing);
        settingsType.GetField("InjuredCop", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, InjuredCop);
        settingsType.GetField("IndecentExposure", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, IndecentExposure);
        settingsType.GetField("Fight", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Fight);
        settingsType.GetField("PrisonBreak", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, PrisonBreak);
        settingsType.GetField("Mafia1", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Mafia1);
        settingsType.GetField("Mafia2", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Mafia2);
        settingsType.GetField("Mafia3", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Mafia3);
        settingsType.GetField("Mafia4", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Mafia4);
        settingsType.GetField("LostMc", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, LostMc);
        settingsType.GetField("Lsgtf", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Lsgtf);
        settingsType.GetField("Interact", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Interact);
        settingsType.GetField("EndCall", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, EndCall);
        settingsType.GetField("EmergencyNumber", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, EmergencyNumber);
    }

    internal static void SaveSettings()
    {
        if ( !Main.UsingSc ) return;
        var settingsType = Assembly.Load("SuperCallouts").GetType("SuperCallouts.Settings");
        var saveSettingsMethod = settingsType.GetMethod("SaveSettings", BindingFlags.Static | BindingFlags.NonPublic);
        saveSettingsMethod?.Invoke(null, null);
    }
}