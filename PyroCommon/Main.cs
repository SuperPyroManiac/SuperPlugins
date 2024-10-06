using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LSPD_First_Response.Mod.API;
using PyroCommon.PyroFunctions;
using PyroCommon.UIManager;
using PyroCommon.Wrappers;
using Rage;

namespace PyroCommon;

public static class Main
{
    private static bool _init;
    internal static bool EventsPaused { get; set; }
    internal static readonly Dictionary<string, string> InstalledPyroPlugins = new();
    private static readonly Func<string, bool> IsLoaded = plugName => Functions.GetAllUserPlugins().Any(assembly => assembly.GetName().Name.Equals(plugName));
    internal static bool UsingUb => IsLoaded("UltimateBackup");
    internal static bool UsingStp => IsLoaded("StopThePed");
    internal static bool UsingPr => IsLoaded("PolicingRedefined");
    internal static bool UsingSc => IsLoaded("SuperCallouts");
    internal static bool UsingSe => IsLoaded("SuperEvents");
    internal static bool UsingDw => IsLoaded("DeadlyWeapons");

    internal static void InitCommon(string plugName, string plugVersion)
    {
        InstalledPyroPlugins[plugName] = plugVersion;
        if (_init) return;
        _init = true;
        AssemblyLoader.Load();
        Settings.LoadSettings();
        DependManager.AddDepend("RageNativeUI.dll", "1.9.2.0");
        Particles.InitParticles();
        GameFiber.StartNew(DelayStart);
    }

    private static void DelayStart()
    {
        GameFiber.WaitUntil(() =>
        {
            var pluginsToCheck = new List<string>();
            if (UsingSc) pluginsToCheck.Add("SuperCallouts");
            if (UsingSe) pluginsToCheck.Add("SuperEvents");
            if (UsingDw) pluginsToCheck.Add("DeadlyWeapons");
            return pluginsToCheck.All(InstalledPyroPlugins.ContainsKey);
        });
        VersionChecker.IsUpdateAvailable(InstalledPyroPlugins);
        if (UsingSc) ScSettings.GetSettings();
        if (UsingSe) SeSettings.GetSettings();
        if (UsingDw) DwSettings.GetSettings();
        Manager.StartUi();
    }

    internal static void StopCommon()
    {
        InstalledPyroPlugins.Clear();
        _init = false;
        Manager.StopUi();
    }
}