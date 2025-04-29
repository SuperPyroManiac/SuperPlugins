using System.Windows.Forms;
using Rage;

namespace SuperEvents;

public static class Settings
{
    internal static bool Fight = true;
    internal static bool CarFire = true;
    internal static bool CarAccident = true;
    internal static bool PulloverShooting = true;
    internal static bool RecklessDriver = true;
    internal static bool AbandonedCar = true;
    internal static bool OpenCarry = true;
    internal static bool WildAnimal = true;
    public static bool ShowBlips = true;
    public static bool ShowHints = true;
    public static int TimeBetweenEvents = 300;
    public static Keys Interact = Keys.Y;
    public static Keys EndEvent = Keys.End;

    internal static void LoadSettings()
    {
        var ini = new InitializationFile("Plugins/LSPDFR/SuperEvents.ini");
        ini.Create();
        Fight = ini.ReadBoolean("Events", "Fight", true);
        PulloverShooting = ini.ReadBoolean("Events", "PulloverShooting", true);
        CarFire = ini.ReadBoolean("Events", "CarFire", true);
        CarAccident = ini.ReadBoolean("Events", "CarAccident", true);
        RecklessDriver = ini.ReadBoolean("Events", "RecklessDriver", true);
        AbandonedCar = ini.ReadBoolean("Events", "AbandonedCar", true);
        OpenCarry = ini.ReadBoolean("Events", "OpenCarry", true);
        WildAnimal = ini.ReadBoolean("Events", "WildAnimal", true);
        ShowBlips = ini.ReadBoolean("Settings", "ShowBlips", true);
        ShowHints = ini.ReadBoolean("Settings", "ShowHints", true);
        TimeBetweenEvents = ini.ReadInt32("Settings", "TimeBetweenEvents", 150);
        Interact = ini.ReadEnum("Keys", "Interact", Keys.Y);
        EndEvent = ini.ReadEnum("Keys", "EndEvent", Keys.End);
    }

    internal static void SaveSettings()
    {
        var ini = new InitializationFile("Plugins/LSPDFR/SuperEvents.ini");
        ini.Create();
        ini.Write("Events", "Fight", Fight);
        ini.Write("Events", "PulloverShooting", PulloverShooting);
        ini.Write("Events", "CarFire", CarFire);
        ini.Write("Events", "CarAccident", CarAccident);
        ini.Write("Events", "RecklessDriver", RecklessDriver);
        ini.Write("Events", "AbandonedCar", AbandonedCar);
        ini.Write("Events", "OpenCarry", OpenCarry);
        ini.Write("Events", "WildAnimal", WildAnimal);
        ini.Write("Settings", "ShowBlips", ShowBlips);
        ini.Write("Settings", "ShowHints", ShowHints);
        ini.Write("Settings", "TimeBetweenEvents", TimeBetweenEvents);
        ini.Write("Keys", "Interact", Interact);
        ini.Write("Keys", "EndEvent", EndEvent);
    }
}
