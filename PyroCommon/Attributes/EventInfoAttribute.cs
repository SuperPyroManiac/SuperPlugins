using System;

namespace SuperEvents.EventFunctions;

[AttributeUsage(AttributeTargets.Class)]
public class EventInfoAttribute : Attribute
{
    public readonly string EventTitle;
    public readonly string EventDescription;

    public EventInfoAttribute(string eventTitle, string eventDescription)
    {
        EventTitle = eventTitle;
        EventDescription = eventDescription;
    }
}