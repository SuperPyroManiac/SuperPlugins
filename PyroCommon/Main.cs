using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;

namespace PyroCommon;

public class Main
{
    private static bool _init;
    private static readonly Func<string, bool> IsLoaded = plugName =>
        Functions.GetAllUserPlugins().Any(assembly => assembly.GetName().Name.Equals(plugName));
    private static readonly Dictionary<string, string> InstalledPyroPlugins = new();

    internal static bool UsingUb { get; } = IsLoaded("UltimateBackup");
    internal static bool UsingStp { get; } = IsLoaded("StopThePed");
    internal static bool UsingPr { get; } = IsLoaded("PolicingRedefined");

    internal static void InitCommon(string plugName, string plugVersion)
    {
        InstalledPyroPlugins.Add(plugName, plugVersion);

        if (_init) return;
        _init = true;
        
        InitParticles();
        GameFiber.StartNew(CheckPluginVersions);
    }

    internal static void StopCommon()
    {
        InstalledPyroPlugins.Clear();
    }

    private static void CheckPluginVersions()
    {
        GameFiber.Sleep(5000);
        VersionChecker.IsUpdateAvailable(InstalledPyroPlugins);
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
}