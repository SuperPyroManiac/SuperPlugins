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
        EventManager.PauseEvents();
    }

    [ConsoleCommand]
    public static void Command_SEListEvents()
    {
        Game.Console.Print("========Listing all Events========");
        var eventNames = EventManager.AllEvents.Select(s => s.Name).ToList();
        Game.Console.Print(string.Join(", ", eventNames));
        Game.Console.Print("=======================================");
    }

    [ConsoleCommand]
    public static void Command_SEForceEvent(
        [ConsoleCommandParameter(AutoCompleterType = typeof(EventParameterAutoCompleter))]
        string eventName) => EventManager.ForceEvent(eventName);
}