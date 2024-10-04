using System;
using System.Reflection;
using System.Windows.Forms;

namespace PyroCommon.Wrappers;

internal static class SeSettings
{
    internal static bool _fight;
    internal static bool _carFire;
    internal static bool _carAccident;
    internal static bool _pulloverShooting;
    internal static bool _recklessDriver;
    internal static bool _abandonedCar;
    internal static bool _openCarry;
    internal static bool _wildAnimal;
    internal static bool _showBlips;
    internal static bool _showHints;
    internal static int _timeBetweenEvents;
    internal static Keys _interact;
    internal static Keys _endEvent;

    internal static readonly Type SettingsType = Assembly.Load("SuperEvents").GetType("SuperEvents.Settings");

    internal static void GetSettings()
    {
        if ( !Main.UsingSe ) return;
        _fight = (bool)SettingsType.GetField("Fight", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _carFire = (bool)SettingsType.GetField("CarFire", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _carAccident = (bool)SettingsType.GetField("CarAccident", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _pulloverShooting = (bool)SettingsType.GetField("PulloverShooting", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _recklessDriver = (bool)SettingsType.GetField("RecklessDriver", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _abandonedCar = (bool)SettingsType.GetField("AbandonedCar", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _openCarry = (bool)SettingsType.GetField("OpenCarry", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _wildAnimal = (bool)SettingsType.GetField("WildAnimal", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _showBlips = (bool)SettingsType.GetField("ShowBlips", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
        _showHints = (bool)SettingsType.GetField("ShowHints", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
        _timeBetweenEvents = (int)SettingsType.GetField("TimeBetweenEvents", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
        _interact = (Keys)SettingsType.GetField("Interact", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
        _endEvent = (Keys)SettingsType.GetField("EndEvent", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
    }

    internal static void ApplySettings()
    {
        if ( !Main.UsingSe ) return;
        SettingsType.GetField("Fight", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _fight);
        SettingsType.GetField("CarFire", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _carFire);
        SettingsType.GetField("CarAccident", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _carAccident);
        SettingsType.GetField("PulloverShooting", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _pulloverShooting);
        SettingsType.GetField("RecklessDriver", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _recklessDriver);
        SettingsType.GetField("AbandonedCar", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _abandonedCar);
        SettingsType.GetField("OpenCarry", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _openCarry);
        SettingsType.GetField("WildAnimal", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _wildAnimal);
        SettingsType.GetField("ShowBlips", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, _showBlips);
        SettingsType.GetField("ShowHints", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, _showHints);
        SettingsType.GetField("TimeBetweenEvents", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, _timeBetweenEvents);
        SettingsType.GetField("Interact", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, _interact);
        SettingsType.GetField("EndEvent", BindingFlags.Static | BindingFlags.Public)?.SetValue(null, _endEvent);
    }
    
    internal static void SaveSettings()
    {
        if ( !Main.UsingSe ) return;
        var saveSettingsMethod = SettingsType.GetMethod("SaveSettings", BindingFlags.Static | BindingFlags.NonPublic);
        saveSettingsMethod?.Invoke(null, null);
    }
}