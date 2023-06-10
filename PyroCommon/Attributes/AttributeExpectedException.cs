using System;

namespace SuperEvents.EventFunctions;

public class AttributeExpectedException : Exception
{
    public AttributeExpectedException()
    {
    }

    public AttributeExpectedException(string message) : base(message)
    {
    }

    public AttributeExpectedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}