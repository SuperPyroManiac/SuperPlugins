using System.Reflection;
using System.Windows.Forms;
using Rage;

namespace SuperCallouts2
{
    internal static class Settings
    {
        internal static bool Animals = true;
        internal static bool Robbery = true;
        internal static bool CarAccident = true;
        internal static bool HotPursuit = true;
        internal static bool Kidnapping = true;
        internal static bool TruckCrash = true;
        internal static bool HitRun = true;
        internal static bool StolenCopVehicle = true;
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
        internal static bool PrisonTransport = true;
        internal static bool PrisonBreak = true;
        internal static bool Mafia1 = true;
        internal static bool Mafia2 = true;
        internal static bool LostMC = true;
        internal static bool LSGTF = true;
        internal static Keys Interact = Keys.Y;
        internal static Keys EndCall = Keys.End;
        internal static readonly string CalloutVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading SuperCallouts config.");
            var path = "Plugins/LSPDFR/SuperCallouts.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            CarAccident = ini.ReadBoolean("Settings", "CarAccident", true);
            HotPursuit = ini.ReadBoolean("Settings", "HighSpeedPursuit", true);
            Robbery = ini.ReadBoolean("Settings", "Robbery", true);
            Animals = ini.ReadBoolean("Settings", "AttackingAnimal", true);
            Kidnapping = ini.ReadBoolean("Settings", "Kidnapping", true);
            TruckCrash = ini.ReadBoolean("Settings", "TruckCrash", true);
            HitRun = ini.ReadBoolean("Settings", "HitAndRun", true);
            StolenCopVehicle = ini.ReadBoolean("Settings", "StolenCopVehicle", true);
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
            PrisonTransport = ini.ReadBoolean("Settings", "PrisonTransport", true);
            PrisonBreak = ini.ReadBoolean("Settings", "PrisonBreak", true);
            Mafia1 = ini.ReadBoolean("Settings", "Mafia1", true);
            Mafia2 = ini.ReadBoolean("Settings", "Mafia2", true);
            LostMC = ini.ReadBoolean("Settings", "LostMC", true);
            LSGTF = ini.ReadBoolean("Settings", "LSGTF", true);
            Interact = ini.ReadEnum("Keys", "Interact", Keys.Y);
            EndCall = ini.ReadEnum("Keys", "EndCall", Keys.End);
            Game.LogTrivial("SuperCallouts: Config loaded.");
        }
    }
}