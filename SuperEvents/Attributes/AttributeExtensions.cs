using System;
using System.Linq;

namespace SuperEvents.Attributes;

internal static class AttributeExtensions
{
    /// <summary>
    /// Retrieves information metadata about the event. (The title and description.)
    /// </summary>
    /// <param name="type">The Type of the AmbientEvent</param>
    /// <returns>The Title and Description of the Event.</returns>
    /// <exception cref="ArgumentException">Thrown if the type was not of type <see cref="AmbientEvent"/>.</exception>
    /// <exception cref="AttributeExpectedException">Thrown if the event does not have an <see cref="EventInfoAttribute"/> assigned.</exception>
    internal static (string Title, string Description) GetEventInfo(this Type type)
    {
        if ( !type.IsSubclassOf(typeof(AmbientEvent)) )
            throw new ArgumentException($"SuperEvents: ERROR: {type.Name} Type was not of Type AmbientEvent");
        if ( type.GetCustomAttributes(typeof(EventInfoAttribute), true).FirstOrDefault() is not EventInfoAttribute att )
            throw new AttributeExpectedException(
                $"SuperEvents: ERROR: Attribute was not assigned to the {type.Name} event from {type.Namespace}.");
        return (att.EventTitle, att.EventDescription);
    }
}