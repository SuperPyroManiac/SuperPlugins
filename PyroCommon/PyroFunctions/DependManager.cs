using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;

namespace PyroCommon.PyroFunctions;

internal class DependManager
{
    private readonly Dictionary<string, (string PluginName, string Version)> _depends = new();

    internal void AddDepend(string name, string version)
    {
        var pluginName = Assembly.GetCallingAssembly().FullName.Split(',').First();
        _depends[name] = (pluginName, version);
    }

    internal bool CheckDepends()
    {
        var plugName = Assembly.GetCallingAssembly().FullName.Split(',').First();
        var missingDepend = new List<string>();
        var outdatedDepend = new List<string>();
        
        if ( Functions.GetVersion() < new Version("0.4.9") )
        {
            Log.Error($"LSPDFR is too far outdated! You require at least version 0.4.9.\r\nYour version: {Functions.GetVersion()}", false);
            Game.DisplayNotification("new_editor", "warningtriangle", $"~r~{plugName}", "~y~Not Loaded!", "Plugin is installed incorrectly! Please see the RagePluginHook.log! Visit https://dsc.PyrosFun.com for help!");
            return false;
        }

        foreach (var depend in _depends)
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
