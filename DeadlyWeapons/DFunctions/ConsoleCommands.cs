using Rage;

namespace DeadlyWeapons.DFunctions
{
    public static class ConsoleCommands
    {
        [Rage.Attributes.ConsoleCommand]
        public static void Command_DWReloadConfig()
        {
            Settings.LoadSettings();
        }
    }
}