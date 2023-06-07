using System;
using Rage;
using Rage.Attributes;
using Rage.ConsoleCommands;
using Rage.ConsoleCommands.AutoCompleters;

namespace SuperEvents.EventFunctions
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

        [ConsoleCommand]
        public static void Command_SEListEvents()
        {
            Game.LogTrivial("SuperEvents: Listing all Events========");
            Game.LogTrivial(string.Join(", ", EventManager.AllEvents));
            Game.LogTrivial("=======================================");
        }

        [ConsoleCommand]
        public static void Command_SEForceEvent(
            [ConsoleCommandParameter(AutoCompleterType = typeof(EventParameterAutoCompleter))] string eventName) =>
            EventManager.ForceEvent(eventName);
    }

    [ConsoleCommandParameterAutoCompleter(typeof(string))]
    public class EventParameterAutoCompleter : ConsoleCommandParameterAutoCompleter
    {
        public EventParameterAutoCompleter(Type type) : base(type)
        {
        }
    
        public override void UpdateOptions()
        {
            Options.Clear();
            foreach (var eventType in EventManager.AllEvents)
                Options.Add(new AutoCompleteOption(eventType.Name, eventType.Name, "An Event"));
        }
    }
}