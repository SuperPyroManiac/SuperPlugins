using Rage;

namespace DeadlyWeapons.DFunctions
{
    internal static class ConsoleCommands
    {
        [Rage.Attributes.ConsoleCommand]
        internal static void Command_DWReloadConfig()
        {
            Settings.LoadSettings();
        }
    }
}