using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Rage;

namespace PyroCommon.PyroFunctions;

internal static class DependManager
{
    private static readonly Dictionary<string, (string PluginName, string Version)> Depends = new();

    internal static void AddDepend(string name, string version)
    {
        var pluginName = Assembly.GetCallingAssembly().FullName.Split(',').First();
        Depends[name] = (pluginName, version);
    }

    internal static bool CheckDepends()
    {
        var plugName = Assembly.GetCallingAssembly().FullName.Split(',').First();
        var missingDepend = new List<string>();
        var outdatedDepend = new List<string>();

        foreach (var depend in Depends)
        {
            if (!File.Exists(depend.Key)) missingDepend.Add(depend.Key);
            else if (new Version(FileVersionInfo.GetVersionInfo(depend.Key).FileVersion) < new Version(depend.Value.Version)) outdatedDepend.Add(depend.Key);
        }

        if (missingDepend.Count > 0)
        {
            var missingMessage = string.Join("\r\n", missingDepend);
            Log.Error($"These dependencies are not installed correctly!\r\n{missingMessage}\r\n{plugName} could not load!", false);
            Game.DisplayNotification("new_editor", "warningtriangle", $"~r~{plugName}", "~y~Not Loaded!", "Plugin is installed incorrectly! Please see the RagePluginHook.log! Visit https://dsc.PyrosFun.com for help!");
            return false;
        }

        if (outdatedDepend.Count > 0)
        {
            var outdatedMessage = string.Join("\r\n", outdatedDepend);
            Log.Error($"These dependencies are outdated!\r\n{outdatedMessage}\r\n{plugName} could not load!", false);
            Game.DisplayNotification("new_editor", "warningtriangle", $"~r~{plugName}", "~y~Not Loaded!", "Plugin is installed incorrectly! Please see the RagePluginHook.log! Visit https://dsc.PyrosFun.com for help!");
            return false;
        }
        return true;
    }
}
