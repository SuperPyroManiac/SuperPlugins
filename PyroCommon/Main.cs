using System;
using System.Collections.Generic;
using System.Linq;
using LSPD_First_Response.Mod.API;
using PyroCommon.PyroFunctions;
using Rage;

namespace PyroCommon;

public static class Main
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
        if (InstalledPyroPlugins.ContainsKey(plugName)) InstalledPyroPlugins.Remove(plugName);
        InstalledPyroPlugins.Add(plugName, plugVersion);

        if (_init) return;
        _init = true;
        AssemblyLoader.Load();
        InitParticles();
        GameFiber.StartNew(CheckPluginVersions);
    }

    internal static void StopCommon()
    {
        InstalledPyroPlugins.Clear();
        _init = false;
    }

    private static void CheckPluginVersions()
    {
        GameFiber.Sleep(3000);
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