namespace DeadlyWeapons2.DFunctions
{
    public class ConsoleCommands
    {
        [Rage.Attributes.ConsoleCommand]
        public static void Command_DWReloadConfig()
        {
            Settings.LoadSettings();
        }
    }
}