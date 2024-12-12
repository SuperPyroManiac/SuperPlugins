using System;
using System.Collections.Generic;
using System.Linq;
using LSPD_First_Response.Mod.API;
using PyroCommon.PyroFunctions;
using PyroCommon.UIManager;
using PyroCommon.Wrappers;
using Rage;

namespace PyroCommon;
public static class Main
{
    private static bool _init;
    internal static bool _outdated = false;
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
        var dCheck = new DependManager();
        dCheck.AddDepend("RageNativeUI.dll", "1.9.3.0");
        if ( !dCheck.CheckDepends() ) return;
        InstalledPyroPlugins[plugName] = plugVersion;
        if ( _init ) return;
        _init = true;
        InstalledPyroPlugins["PyroCommon"] = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        Game.AddConsoleCommands([typeof(ConsoleCommands)]);
        AssemblyLoader.Load();
        GameFiber.StartNew(Run, "[PC] Main");
    }

    private static void Run()
    {
        Settings.LoadSettings();
        Particles.InitParticles();
        GameFiber.WaitUntil(() =>
        {
            var pluginsToCheck = new List<string>();
            if ( UsingSc ) pluginsToCheck.Add("SuperCallouts");
            if ( UsingSe ) pluginsToCheck.Add("SuperEvents");
            if ( UsingDw ) pluginsToCheck.Add("DeadlyWeapons");
            return pluginsToCheck.All(InstalledPyroPlugins.ContainsKey);
        });
        VersionChecker.Validate(InstalledPyroPlugins);
        if ( UsingSc ) ScSettings.GetSettings();
        if ( UsingSe ) SeSettings.GetSettings();
        if ( UsingDw ) DwSettings.GetSettings();
        Manager.StartUi();
    }

    internal static void StopCommon()
    {
        InstalledPyroPlugins.Clear();
        _init = false;
        Manager.StopUi();
    }
}