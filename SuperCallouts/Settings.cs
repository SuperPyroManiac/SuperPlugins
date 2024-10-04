using System.Windows.Forms;
using Rage;

namespace SuperCallouts;

internal static class Settings
{
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
    internal static bool PrisonBreak = true;
    internal static bool Mafia1 = true;
    internal static bool Mafia2 = true;
    internal static bool Mafia3 = true;
    internal static bool Mafia4 = true;
    internal static bool LostMc = true;
    internal static bool Lsgtf = true;
    internal static Keys Interact = Keys.Y;
    internal static Keys EndCall = Keys.End;
    internal static string EmergencyNumber = "911";

    internal static void LoadSettings()
    {
        var ini = new InitializationFile("Plugins/LSPDFR/SuperCallouts.ini");
        ini.Create();
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
        PrisonBreak = ini.ReadBoolean("Settings", "PrisonBreak", true);
        Mafia1 = ini.ReadBoolean("Settings", "Mafia1", true);
        Mafia2 = ini.ReadBoolean("Settings", "Mafia2", true);
        Mafia3 = ini.ReadBoolean("Settings", "Mafia3", true);
        Mafia4 = ini.ReadBoolean("Settings", "Mafia4", true);
        LostMc = ini.ReadBoolean("Settings", "LostMC", true);
        Lsgtf = ini.ReadBoolean("Settings", "LSGTF", true);
        Interact = ini.ReadEnum("Keys", "Interact", Keys.Y);
        EndCall = ini.ReadEnum("Keys", "EndCall", Keys.End);
        EmergencyNumber = ini.ReadString("Msc", "EmergencyNumber", "911");
    }
    
    internal static void SaveSettings()
    {
        var ini = new InitializationFile("Plugins/LSPDFR/SuperCallouts.ini");
        ini.Create();
        ini.Write("Settings", "CarAccident", CarAccident);
        ini.Write("Settings", "HighSpeedPursuit", HotPursuit);
        ini.Write("Settings", "Robbery", Robbery);
        ini.Write("Settings", "AttackingAnimal", Animals);
        ini.Write("Settings", "Kidnapping", Kidnapping);
        ini.Write("Settings", "TruckCrash", TruckCrash);
        ini.Write("Settings", "PrisonTransport", PrisonTransport);
        ini.Write("Settings", "HitAndRun", HitRun);
        ini.Write("Settings", "StolenCopVehicle", StolenCopVehicle);
        ini.Write("Settings", "StolenDumptruck", StolenDumptruck);
        ini.Write("Settings", "AmbulanceEscort", AmbulanceEscort);
        ini.Write("Settings", "Aliens", Aliens);
        ini.Write("Settings", "OpenCarry", OpenCarry);
        ini.Write("Settings", "Fire", Fire);
        ini.Write("Settings", "OfficerShootout", OfficerShootout);
        ini.Write("Settings", "SuspiciousCar", WeirdCar);
        ini.Write("Settings", "Manhunt", Manhunt);
        ini.Write("Settings", "Impersonator", Impersonator);
        ini.Write("Settings", "ToiletPaperBandit", ToiletPaperBandit);
        ini.Write("Settings", "BlockingTraffic", BlockingTraffic);
        ini.Write("Settings", "IllegalParking", IllegalParking);
        ini.Write("Settings", "KnifeAttack", KnifeAttack);
        ini.Write("Settings", "DeadBody", DeadBody);
        ini.Write("Settings", "FakeCall", FakeCall);
        ini.Write("Settings", "Trespassing", Trespassing);
        ini.Write("Settings", "Vandalizing", Vandalizing);
        ini.Write("Settings", "InjuredCop", InjuredCop);
        ini.Write("Settings", "IndecentExposure", IndecentExposure);
        ini.Write("Settings", "Fight", Fight);
        ini.Write("Settings", "PrisonBreak", PrisonBreak);
        ini.Write("Settings", "Mafia1", Mafia1);
        ini.Write("Settings", "Mafia2", Mafia2);
        ini.Write("Settings", "Mafia3", Mafia3);
        ini.Write("Settings", "Mafia4", Mafia4);
        ini.Write("Settings", "LostMC", LostMc);
        ini.Write("Settings", "LSGTF", Lsgtf);
        ini.Write("Keys", "Interact", Interact);
        ini.Write("Keys", "EndCall", EndCall);
        ini.Write("Msc", "EmergencyNumber", EmergencyNumber);
    }
}