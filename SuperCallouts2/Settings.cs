using System.Windows.Forms;
using Rage;

namespace SuperCallouts2
{
    internal static class Settings
    {
        internal static bool Animals = true;
        internal static bool Robbery = true;
        internal static bool OrganizedCrime = true;
        internal static bool OrganizedCrime2 = true;
        internal static bool CarAccident = true;
        internal static bool HotPursuit = true;
        internal static bool Kidnapping = true;
        internal static bool PrisonBreak = true;
        internal static bool TruckCrash = true;
        internal static bool Lsgtf = true;
        internal static bool PrisonTransport = true;
        internal static bool HitRun = true;
        internal static bool AmbulanceEscort = true;
        internal static bool Aliens = true;
        internal static bool OpenCarry = true;
        internal static bool Fire = true;
        internal static bool OfficerShootout = true;
        internal static bool WeirdCar = true;
        internal static bool Manhunt = true;
        internal static bool Impersonator = true;
        internal static bool ToiletPaperBandit = true;
        internal static bool BikerAttack = true;
        internal static Keys Interact = Keys.Y;
        internal static Keys EndCall = Keys.End;

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading SuperCallouts config.");
            var path = "Plugins/LSPDFR/SuperCallouts.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            CarAccident = ini.ReadBoolean("Settings", "CarAccident", true);
            HotPursuit = ini.ReadBoolean("Settings", "HighSpeedPursuit", true);
            Robbery = ini.ReadBoolean("Settings", "Robbery", true);
            OrganizedCrime = ini.ReadBoolean("Settings", "OrganizedCrime", true);
            OrganizedCrime2 = ini.ReadBoolean("Settings", "OrganizedCrime2", true);
            Animals = ini.ReadBoolean("Settings", "AttackingAnimal", true);
            Kidnapping = ini.ReadBoolean("Settings", "Kidnapping", true);
            PrisonBreak = ini.ReadBoolean("Settings", "PrisonBreak", true);
            TruckCrash = ini.ReadBoolean("Settings", "TruckCrash", true);
            Lsgtf = ini.ReadBoolean("Settings", "GangRaid", true);
            PrisonTransport = ini.ReadBoolean("Settings", "PrisonTransport", true);
            HitRun = ini.ReadBoolean("Settings", "HitAndRun", true);
            AmbulanceEscort = ini.ReadBoolean("Settings", "AmbulanceEscort", true);
            Aliens = ini.ReadBoolean("Settings", "Aliens", true);
            OpenCarry = ini.ReadBoolean("Settings", "OpenCarry", true);
            Fire = ini.ReadBoolean("Settings", "Fire", true);
            OfficerShootout = ini.ReadBoolean("Settings", "OfficerShootout", true);
            WeirdCar = ini.ReadBoolean("Settings", "SuspiciousCar", true);
            Manhunt = ini.ReadBoolean("Settings", "Manhunt", true);
            Impersonator = ini.ReadBoolean("Settings", "Impersonator", true);
            ToiletPaperBandit = ini.ReadBoolean("Settings", "ToiletPaperBandit", true);
            BikerAttack = ini.ReadBoolean("Settings", "BikerAttack", true);
            Interact = ini.ReadEnum("Keys", "Interact", Keys.Y);
            EndCall = ini.ReadEnum("Keys", "EndCall", Keys.End);
            Game.LogTrivial("SuperCallouts: Config loaded.");
        }
    }
}