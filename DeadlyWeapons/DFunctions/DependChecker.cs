using System;
using System.Diagnostics;
using System.IO;
using PyroCommon.API;
using Rage;

namespace DeadlyWeapons.DFunctions;

public class DependChecker
{
    internal static bool Start()
    {
        if (!File.Exists("PyroCommon.dll"))
        {
            Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print("DeadlyWeapons: Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print("PyroCommon.dll is missing! Please reinstall plugin correctly!");
            Game.Console.Print("======================================================");
            Game.Console.Print("DeadlyWeapons: Error Report End");
            Game.DisplayNotification("new_editor", "warningtriangle", "~r~DeadlyWeapons", "~y~Not Loaded!", "Plugin is installed incorrectly! Please see the RagePluginHook.log! Visit https://dsc.gg/ulss for help!"); 
            return false;
        }
        //TODO: Next version remove this as everyone should have updated PyroCommon 1.5
        var dependVersion = new Version(FileVersionInfo.GetVersionInfo("PyroCommon.dll").FileVersion);
        if (dependVersion < new Version("1.5.0.1"))
        {
            Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print("DeadlyWeapons: Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print("PyroCommon.dll is outdated! Please reinstall plugin correctly!");
            Game.Console.Print("======================================================");
            Game.Console.Print("DeadlyWeapons: Error Report End");
            Game.DisplayNotification("new_editor", "warningtriangle", "~r~DeadlyWeapons", "~y~Not Loaded!", "Plugin is installed incorrectly! Please see the RagePluginHook.log! Visit https://dsc.gg/ulss for help!");
            return false;
        }
        return File.Exists("PyroCommon.dll") && Check();
    }

    internal static bool Check()
    {
        DependManager.AddDepend("PyroCommon.dll", "1.5.0.1");
        DependManager.AddDepend("RageNativeUI.dll", "1.9.2.0");
        DependManager.AddDepend("DamageTrackerLib.dll", "1.0.1");
        return DependManager.CheckDepends();
    }
}