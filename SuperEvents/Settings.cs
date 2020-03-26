#region

using System.Windows.Forms;
using Rage;

#endregion

namespace SuperEvents
{
    internal static class Settings
    {
        internal static bool Fight = true;
        internal static bool OpenCarry = true;
        internal static bool CarFire = true;
        internal static bool CarAccident = true;
        internal static bool PulloverShooting = true;
        internal static bool InjuredPed = true;
        internal static bool RecklessDriver = true;
        internal static bool SuicidalPed = true;
        internal static bool Mugging = true;
        internal static bool RoadRage = true;
        internal static bool WildAnimal = true;
        internal static bool ShowBlips = true;
        internal static bool ShowHints = true;
        internal static int TimeBetweenEvents = 110;
        internal static Keys Interact = Keys.Y;
        internal static Keys EndEvent = Keys.End;

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading SuperEvents config.");
            var path = "Plugins/LSPDFR/SuperEvents.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            Fight = ini.ReadBoolean("Events", "Fight", true);
            OpenCarry = ini.ReadBoolean("Events", "OpenCarry", true);
            PulloverShooting = ini.ReadBoolean("Events", "PulloverShooting", true);
            CarFire = ini.ReadBoolean("Events", "CarFire", true);
            CarAccident = ini.ReadBoolean("Events", "CarAccident", true);
            InjuredPed = ini.ReadBoolean("Events", "InjuredPed", true);
            RecklessDriver = ini.ReadBoolean("Events", "RecklessDriver", true);
            SuicidalPed = ini.ReadBoolean("Events", "SuicidalPed", true);
            Mugging = ini.ReadBoolean("Events", "Mugging", true);
            RoadRage = ini.ReadBoolean("Events", "RoadRage", true);
            WildAnimal = ini.ReadBoolean("Events", "WildAnimal", true);
            ShowBlips = ini.ReadBoolean("Settings", "ShowBlips", true);
            ShowHints = ini.ReadBoolean("Settings", "ShowHints", true);
            TimeBetweenEvents = ini.ReadInt32("Settings", "TimeBetweenEvents", 110);
            Interact = ini.ReadEnum("Keys", "Interact", Keys.Y);
            EndEvent = ini.ReadEnum("Keys", "EndEvent", Keys.End);
            Game.LogTrivial("SuperCallouts: Config loaded.");
        }
    }
}