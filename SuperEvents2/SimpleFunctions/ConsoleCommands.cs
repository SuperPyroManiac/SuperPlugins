using System.Reflection;
using Rage;

namespace SuperEvents2.SimpleFunctions
{
    internal static class ConsoleCommands
    {
        [Rage.Attributes.ConsoleCommand]
        internal static void Command_SEReloadConfig()
        {
            Settings.LoadSettings();
        }
        [Rage.Attributes.ConsoleCommand]
        internal static void Command_SEPauseEvents()
        {
            Main.PluginPaused = !Main.PluginPaused;
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~SuperEvents", "~g~Plugin Status:", "SuperEvents paused: " + Main.PluginPaused);

        }
    }
}