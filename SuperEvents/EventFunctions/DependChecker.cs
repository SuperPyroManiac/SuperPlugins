using System;
using System.Diagnostics;
using System.IO;
using PyroCommon.API;
using Rage;

namespace SuperEvents.EventFunctions;

public class DependChecker
{
    internal static bool Start()
    {
        if (!File.Exists("PyroCommon.dll"))
        {
            Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print("SuperEvents: Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print($"PyroCommon.dll is missing! Please reinstall plugin correctly!");
            Game.Console.Print("======================================================");
            Game.Console.Print("SuperEvents: Error Report End");
            Game.DisplayNotification("new_editor", "warningtriangle", "~r~SuperEvents", "~y~Not Loaded!", "Plugin is installed incorrectly! Please see the RagePluginHook.log! Visit https://dsc.gg/ulss for help!"); 
            return false;
        }
        //TODO: Next version remove this as everyone should have updated PyroCommon 1.4
        var dependVersion = new Version(FileVersionInfo.GetVersionInfo("PyroCommon.dll").FileVersion);
        if (dependVersion < new Version("1.4.0.0"))
        {
            Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print("SuperEvents: Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print($"PyroCommon.dll is outdated! Please reinstall plugin correctly!");
            Game.Console.Print("======================================================");
            Game.Console.Print("SuperEvents: Error Report End");
            Game.DisplayNotification("new_editor", "warningtriangle", "~r~SuperEvents", "~y~Not Loaded!", "Plugin is installed incorrectly! Please see the RagePluginHook.log! Visit https://dsc.gg/ulss for help!");
            return false;
        }
        return File.Exists("PyroCommon.dll") && Check();
    }

    internal static bool Check()
    {
        DependManager.AddDepend("PyroCommon.dll", "1.4.0.0");
        DependManager.AddDepend("RageNativeUI.dll", "1.9.2.0");
        return DependManager.CheckDepends();
    }
}