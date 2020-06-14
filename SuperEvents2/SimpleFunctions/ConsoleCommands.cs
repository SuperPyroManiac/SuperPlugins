using System.Reflection;
using Rage;

namespace SuperEvents2.SimpleFunctions
{
    public static class ConsoleCommands
    {
        [Rage.Attributes.ConsoleCommand]
        public static void Command_SEReloadConfig()
        {
            Settings.LoadSettings();
        }
        public static void Command_SEPauseEvents()
        {
            Main.PluginPaused = !Main.PluginPaused;
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SuperEvents", "~g~Plugin Status:", "SuperEvents paused: " + Main.PluginPaused);

        }
    }
}