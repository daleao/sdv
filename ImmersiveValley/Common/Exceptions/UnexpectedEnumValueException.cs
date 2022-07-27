namespace DaLion.Common.Exceptions;

#region using directives

using System;

#endregion using directives

/// <summary>Thrown when an unexpected enum value is received.</summary>
/// <typeparam name="T">The enum type that received an unexpected value.</typeparam>
public class UnexpectedEnumValueException<T> : Exception
{
    /// <summary>Initializes a new instance of the <see cref="UnexpectedEnumValueException{T}"/> class.</summary>
    /// <param name="value">The unexpected enum value.</param>
    public UnexpectedEnumValueException(T value)
        : base($"Enum {typeof(T).Name} recieved unexpected value {value}")
    {
    }
}