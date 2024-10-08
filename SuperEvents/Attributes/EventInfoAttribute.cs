using System;

namespace SuperEvents.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EventInfoAttribute(string eventTitle, string eventDescription) : Attribute
{
    public readonly string EventTitle = eventTitle;
    public readonly string EventDescription = eventDescription;
}