using System;
using System.Collections.Generic;
using System.Linq;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;

namespace PyroCommon;

public class Main
{
    private static bool init;
    private static readonly Func<string, bool> IsLoaded = plugName =>
        Functions.GetAllUserPlugins().Any(assembly => assembly.GetName().Name.Equals(plugName));
    internal static bool UsingUb { get; } = IsLoaded("UltimateBackup");
    internal static bool UsingPr { get; } = IsLoaded("PolicingRedefined");

    internal static void InitCommon()
    {
        if (init) return;
        init = true;
        
        InitParticles();
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