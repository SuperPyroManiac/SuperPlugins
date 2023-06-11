using System;
using System.Linq;
using LSPD_First_Response.Mod.API;

namespace PyroCommon;

public class Main
{
    internal static readonly Func<string, bool> IsLoaded = plugName =>
        Functions.GetAllUserPlugins().Any(assembly => assembly.GetName().Name.Equals(plugName));
    internal static bool UsingUB { get; set; } = IsLoaded("UltimateBackup");
}