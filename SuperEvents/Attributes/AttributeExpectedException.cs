using System;

namespace SuperEvents.Attributes;

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