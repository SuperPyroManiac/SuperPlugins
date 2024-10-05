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
    internal static bool EventsPaused { get; set; }
    private static readonly Func<string, bool> IsLoaded = plugName => Functions.GetAllUserPlugins().Any(assembly => assembly.GetName().Name.Equals(plugName));
    internal static readonly Dictionary<string, string> InstalledPyroPlugins = new();

    internal static bool UsingUb { get; } = IsLoaded("UltimateBackup");
    internal static bool UsingStp { get; } = IsLoaded("StopThePed");
    internal static bool UsingPr { get; } = IsLoaded("PolicingRedefined");
    internal static bool UsingSc { get; } = IsLoaded("SuperCallouts");
    internal static bool UsingSe { get; } = IsLoaded("SuperEvents");
    internal static bool UsingDw { get; } = IsLoaded("DeadlyWeapons");

    internal static void InitCommon(string plugName, string plugVersion)
    {
        Settings.LoadSettings();
         if ( InstalledPyroPlugins.ContainsKey(plugName) ) InstalledPyroPlugins.Remove(plugName);
         InstalledPyroPlugins.Add(plugName, plugVersion);
        if (_init) return;
        _init = true;
        AssemblyLoader.Load();
        InitParticles();
        //GameFiber.StartNew(Runner);
        GameFiber.StartNew(DelayStart);
    }

    private static void DelayStart()
    {
        GameFiber.Sleep(7000);
        VersionChecker.IsUpdateAvailable(InstalledPyroPlugins);
        if ( UsingSc ) ScSettings.GetSettings();
        if ( UsingSe ) SeSettings.GetSettings();
        if ( UsingDw ) DwSettings.GetSettings();
        Manager.StartUi();
    }

    private static void InitParticles()
    {
        var particleDict = new List<string>
        {
            "scr_trevor3",//particle1: scr_trev3_trailer_plume - Large Fire/Smoke
            "scr_agencyheistb"//particle2: scr_env_agency3b_smoke - Misty Smoke
        };
        foreach (var part in particleDict)
            GameFiber.StartNew(() => Particles.LoadParticles(part));
    }

    private static void Runner()
    {
        while ( _init )
        {
            GameFiber.Yield();
        }
    }
    
    internal static void StopCommon()
    {
        InstalledPyroPlugins.Clear();
        _init = false;
        Manager.StopUi();
    }
}