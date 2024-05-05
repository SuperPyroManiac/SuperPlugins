#region

using PyroCommon.API;
using PyroCommon.API.Wrappers;
using Rage;
using Rage.Attributes;

#endregion

namespace SuperCallouts.SimpleFunctions;

public static class ConsoleCommands
{
    [ConsoleCommand]
    public static void Command_SCReloadConfig()
    {
        Settings.LoadSettings();
    }
}