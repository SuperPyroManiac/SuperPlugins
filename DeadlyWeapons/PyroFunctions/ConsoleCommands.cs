#region

using Rage.Attributes;

#endregion

namespace DeadlyWeapons.PyroFunctions;

public class ConsoleCommands
{
    [ConsoleCommand]
    public static void Command_DWReloadConfig()
    {
        Settings.LoadSettings();
    }
}