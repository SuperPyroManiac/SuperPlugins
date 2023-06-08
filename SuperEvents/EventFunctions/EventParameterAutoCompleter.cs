using System;
using System.Linq;
using Rage.Attributes;
using Rage.ConsoleCommands;
using Rage.ConsoleCommands.AutoCompleters;

namespace SuperEvents.EventFunctions;

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
            Options.Add(new AutoCompleteOption(eventType.FullName,
                $"[{eventType.Namespace.Split('.').First()}] {eventType.Name}",
                $"The {eventType.Name} event from {eventType.Namespace.Split('.').First()}"));
    }
}