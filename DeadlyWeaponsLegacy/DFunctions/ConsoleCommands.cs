#region

using Rage.Attributes;

#endregion

namespace DeadlyWeaponsLegacy.DFunctions
{
    public class ConsoleCommands
    {
        [ConsoleCommand]
        public static void Command_DWReloadConfig()
        {
            Settings.LoadSettings();
        }
    }
}