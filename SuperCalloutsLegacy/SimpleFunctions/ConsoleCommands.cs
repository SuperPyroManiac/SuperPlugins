#region

using Rage.Attributes;

#endregion

namespace SuperCalloutsLegacy.SimpleFunctions;

public static class ConsoleCommands
{
    [ConsoleCommand]
    public static void Command_SCReloadConfig()
    {
        Settings.LoadSettings();
    }
}