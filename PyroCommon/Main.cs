using System;
using System.Linq;
using LSPD_First_Response.Mod.API;

namespace PyroCommon;

public class Main
{
    private static readonly Func<string, bool> IsLoaded = plugName =>
        Functions.GetAllUserPlugins().Any(assembly => assembly.GetName().Name.Equals(plugName));
    internal static bool UsingUb { get; } = IsLoaded("UltimateBackup");
}