#region

using Rage.Attributes;

#endregion

namespace DeadlyWeapons.DFunctions;

public class ConsoleCommands
{
    [ConsoleCommand]
    public static void Command_DWReloadConfig()
    {
        Settings.LoadSettings();
    }
}