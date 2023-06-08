using System.Linq;
using Rage;
using Rage.Attributes;

namespace SuperEvents.EventFunctions;

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

    [ConsoleCommand]
    public static void Command_SEListEvents()
    {
        Game.LogTrivial("SuperEvents: Listing all Events========");
        var eventNames = EventManager.AllEvents.Select(s => s.Name).ToList();
        Game.LogTrivial(string.Join(", ", eventNames));
        Game.LogTrivial("=======================================");
    }

    [ConsoleCommand]
    public static void Command_SEForceEvent(
        [ConsoleCommandParameter(AutoCompleterType = typeof(EventParameterAutoCompleter))]
        string eventName) => EventManager.ForceEvent(eventName);
}