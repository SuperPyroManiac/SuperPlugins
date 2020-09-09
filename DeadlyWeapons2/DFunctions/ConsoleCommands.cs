#region

using Rage.Attributes;

#endregion

namespace DeadlyWeapons2.DFunctions
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