using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using Rage;

namespace PyroCommon;

internal static class DependManager
{
    private static List<Dependency> _depends = [];
    
    internal static void AddDepend(string name, string version)
    {
        var newDepend = new Dependency()
        {
            PluginName = Assembly.GetCallingAssembly().FullName.Split(',').First(),
            DependName = name,
            DependVersion = version
        };
        if (!_depends.Contains(newDepend)) _depends.Add(newDepend);
    }

    internal static bool CheckDepends()
    {
        var plugName = Assembly.GetCallingAssembly().FullName.Split(',').First();
        var missingDepend = string.Empty;
        var outdatedDepend = string.Empty;
        var pluginDepends = _depends.Where(depend => depend.PluginName == plugName).ToList();

        foreach (var depend in pluginDepends)
            if (!File.Exists(depend.DependName)) missingDepend += $"{depend.DependName}~n~";

        if (missingDepend.Length > 0)
        {
            Log.Error($"These dependencies are not installed correctly!\r\n{missingDepend.Replace("~n~", "\r\n")}{plugName} could not load!");
            Game.DisplayNotification("new_editor", "warningtriangle", $"~r~{plugName}", "~y~Not Loaded!", "Plugin is installed incorrectly! Please see the RagePluginHook.log! Visit https://dsc.PyrosFun.com for help!");
            return false;
        }

        foreach (var depend in pluginDepends)
        {
            var dependVersion = new Version(FileVersionInfo.GetVersionInfo(depend.DependName).FileVersion);
            if (dependVersion < new Version(depend.DependVersion)) outdatedDepend += $"{depend.DependName}~n~";
        }

        if (outdatedDepend.Length > 0)
        {
            Log.Error($"These dependencies are outdated!\r\n{outdatedDepend.Replace("~n~", "\r\n")}{plugName} could not load!");
            Game.DisplayNotification("new_editor", "warningtriangle", $"~r~{plugName}", "~y~Not Loaded!", "Plugin is installed incorrectly! Please see the RagePluginHook.log! Visit https://dsc.PyrosFun.com for help!");
            return false;
        }
        return true;
    }
}