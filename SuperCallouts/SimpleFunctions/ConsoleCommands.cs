#region

using Rage.Attributes;

#endregion

namespace SuperCallouts.SimpleFunctions
{
    public static class ConsoleCommands
    {
        [ConsoleCommand]
        public static void Command_SCReloadConfig()
        {
            Settings.LoadSettings();
        }
    }
}