using System.Reflection;
using System.Windows.Forms;

namespace PyroCommon.Utils.WrapperUtils;

internal static class SeSettings
{
    internal static bool Fight;
    internal static bool CarFire;
    internal static bool CarAccident;
    internal static bool PulloverShooting;
    internal static bool RecklessDriver;
    internal static bool AbandonedCar;
    internal static bool OpenCarry;
    internal static bool WildAnimal;
    internal static bool ShowBlips;
    internal static bool ShowHints;
    internal static int TimeBetweenEvents;
    internal static Keys Interact;
    internal static Keys EndEvent;

    internal static void GetSettings()
    {
        if (!Main.UsingSe)
            return;
        var settingsType = Assembly.Load("SuperEvents").GetType("SuperEvents.Settings");
        Fight = (bool)settingsType.GetField("Fight", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        CarFire = (bool)settingsType.GetField("CarFire", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        CarAccident = (bool)settingsType.GetField("CarAccident", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        PulloverShooting = (bool)settingsType.GetField("PulloverShooting", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        RecklessDriver = (bool)settingsType.GetField("RecklessDriver", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        AbandonedCar = (bool)settingsType.GetField("AbandonedCar", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        OpenCarry = (bool)settingsType.GetField("OpenCarry", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        WildAnimal = (bool)settingsType.GetField("WildAnimal", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        ShowBlips = (bool)settingsType.GetField("ShowBlips", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
        ShowHints = (bool)settingsType.GetField("ShowHints", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
        TimeBetweenEvents = (int)settingsType.GetField("TimeBetweenEvents", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
        Interact = (Keys)settingsType.GetField("Interact", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
        EndEvent = (Keys)settingsType.GetField("EndEvent", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
    }

    internal static void ApplySettings()
    {
        if (!Main.UsingSe)
            return;
        var settingsType = Assembly.Load("SuperEvents").GetType("SuperEvents.Settings");
        settingsType.GetField("Fight", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Fight);
        settingsType.GetField("CarFire", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, CarFire);
        settingsType.GetField("CarAccident", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, CarAccident);
        settingsType.GetField("PulloverShooting", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, PulloverShooting);
        settingsType.GetField("RecklessDriver", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, RecklessDriver);
        settingsType.GetField("AbandonedCar", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, AbandonedCar);
        settingsType.GetField("OpenCarry", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, OpenCarry);
        settingsType.GetField("WildAnimal", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, WildAnimal);
        settingsType.GetField("ShowBlips", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, ShowBlips);
        settingsType.GetField("ShowHints", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, ShowHints);
        settingsType.GetField("TimeBetweenEvents", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, TimeBetweenEvents);
        settingsType.GetField("Interact", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, Interact);
        settingsType.GetField("EndEvent", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, EndEvent);
    }

    internal static void SaveSettings()
    {
        if (!Main.UsingSe)
            return;
        var settingsType = Assembly.Load("SuperEvents").GetType("SuperEvents.Settings");
        var saveSettingsMethod = settingsType.GetMethod("SaveSettings", BindingFlags.Static | BindingFlags.NonPublic);
        saveSettingsMethod?.Invoke(null, null);
    }
}
