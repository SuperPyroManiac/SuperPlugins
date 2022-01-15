using Rage;
using Rage.Attributes;

namespace SuperEvents.SimpleFunctions
{
    public static class ConsoleCommands
    {
        [ConsoleCommand]
        public static void Command_SEReloadConfig()
        {
            Settings.LoadSettings();
        }

        [ConsoleCommand]
        public static void Command_SEPauseEvents()
        {
            Main.PluginPaused = !Main.PluginPaused;
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SuperEvents", "~g~Plugin Status:",
                "SuperEvents paused: " + Main.PluginPaused);
        }
    }
}