using System;
using System.Collections.Generic;

namespace SuperEvents.EventFunctions;

public class API
{
    internal static List<Type> RegisteredEvents = new();
    internal static List<Type> AllEvents = new();
    public enum Priority
    {
        Low,
        Normal,
        High
    }
    public static void RegisterEvent(Type type, Priority EventPriority = Priority.Normal)
    {
        AllEvents.Add(type);
        var PRI = EventPriority switch
        {
            Priority.Low => 1,
            Priority.Normal => 2,
            Priority.High => 3,
            _ => 0
        };
        while (PRI > 0)
        {
            RegisteredEvents.Add(type);
            PRI--;
        }
    }
}