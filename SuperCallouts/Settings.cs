using System.Windows.Forms;
using PyroCommon.PyroFunctions;
using Rage;

namespace SuperCallouts;

internal static class Settings
{
    //Reg
    internal static bool Animals = true;
    internal static bool Robbery = true;
    internal static bool CarAccident = true;
    internal static bool HotPursuit = true;
    internal static bool Kidnapping = true;
    internal static bool TruckCrash = true;
    internal static bool PrisonTransport = true;
    internal static bool HitRun = true;
    internal static bool StolenCopVehicle = true;
    internal static bool StolenDumptruck = true;
    internal static bool AmbulanceEscort = true;
    internal static bool Aliens = true;
    internal static bool OpenCarry = true;
    internal static bool Fire = true;
    internal static bool OfficerShootout = true;
    internal static bool WeirdCar = true;
    internal static bool Manhunt = true;
    internal static bool Impersonator = true;
    internal static bool ToiletPaperBandit = true;
    internal static bool BlockingTraffic = true;
    internal static bool IllegalParking = true;
    internal static bool KnifeAttack = true;
    internal static bool DeadBody = true;
    internal static bool FakeCall = true;
    internal static bool Trespassing = true;
    internal static bool Vandalizing = true;
    internal static bool InjuredCop = true;
    internal static bool IndecentExposure = true;
    internal static bool Fight = true;
    //Swat
    internal static bool PrisonBreak = true;
    internal static bool Mafia1 = true;
    internal static bool Mafia2 = true;
    internal static bool Mafia3 = true;
    internal static bool Mafia4 = true;
    internal static bool LostMc = true;
    internal static bool Lsgtf = true;
    //Settings
    internal static Keys Interact = Keys.Y;
    internal static Keys EndCall = Keys.End;
    internal static string EmergencyNumber = "911";

    internal static void LoadSettings()
    {
        var ini = new InitializationFile("Plugins/LSPDFR/SuperCallouts.ini");
        ini.Create();
        //Reg
        CarAccident = ini.ReadBoolean("Settings", "CarAccident", true);
        HotPursuit = ini.ReadBoolean("Settings", "HighSpeedPursuit", true);
        Robbery = ini.ReadBoolean("Settings", "Robbery", true);
        Animals = ini.ReadBoolean("Settings", "AttackingAnimal", true);
        Kidnapping = ini.ReadBoolean("Settings", "Kidnapping", true);
        TruckCrash = ini.ReadBoolean("Settings", "TruckCrash", true);
        PrisonTransport = ini.ReadBoolean("Settings", "PrisonTransport", true);
        HitRun = ini.ReadBoolean("Settings", "HitAndRun", true);
        StolenCopVehicle = ini.ReadBoolean("Settings", "StolenCopVehicle", true);
        StolenDumptruck = ini.ReadBoolean("Settings", "StolenDumptruck", true);
        AmbulanceEscort = ini.ReadBoolean("Settings", "AmbulanceEscort", true);
        Aliens = ini.ReadBoolean("Settings", "Aliens", true);
        OpenCarry = ini.ReadBoolean("Settings", "OpenCarry", true);
        Fire = ini.ReadBoolean("Settings", "Fire", true);
        OfficerShootout = ini.ReadBoolean("Settings", "OfficerShootout", true);
        WeirdCar = ini.ReadBoolean("Settings", "SuspiciousCar", true);
        Manhunt = ini.ReadBoolean("Settings", "Manhunt", true);
        Impersonator = ini.ReadBoolean("Settings", "Impersonator", true);
        ToiletPaperBandit = ini.ReadBoolean("Settings", "ToiletPaperBandit", true);
        BlockingTraffic = ini.ReadBoolean("Settings", "BlockingTraffic", true);
        IllegalParking = ini.ReadBoolean("Settings", "IllegalParking", true);
        KnifeAttack = ini.ReadBoolean("Settings", "KnifeAttack", true);
        DeadBody = ini.ReadBoolean("Settings", "DeadBody", true);
        FakeCall = ini.ReadBoolean("Settings", "FakeCall", true);
        Trespassing = ini.ReadBoolean("Settings", "Trespassing", true);
        Vandalizing = ini.ReadBoolean("Settings", "Vandalizing", true);
        InjuredCop = ini.ReadBoolean("Settings", "InjuredCop", true);
        IndecentExposure = ini.ReadBoolean("Settings", "IndecentExposure", true);
        Fight = ini.ReadBoolean("Settings", "Fight", true);
        //Swat
        PrisonBreak = ini.ReadBoolean("Settings", "PrisonBreak", true);
        Mafia1 = ini.ReadBoolean("Settings", "Mafia1", true);
        Mafia2 = ini.ReadBoolean("Settings", "Mafia2", true);
        Mafia3 = ini.ReadBoolean("Settings", "Mafia3", true);
        Mafia4 = ini.ReadBoolean("Settings", "Mafia4", true);
        LostMc = ini.ReadBoolean("Settings", "LostMC", true);
        Lsgtf = ini.ReadBoolean("Settings", "LSGTF", true);
        //Settings
        Interact = ini.ReadEnum("Keys", "Interact", Keys.Y);
        EndCall = ini.ReadEnum("Keys", "EndCall", Keys.End);
        EmergencyNumber = ini.ReadString("Msc", "EmergencyNumber", "911");
    }
}