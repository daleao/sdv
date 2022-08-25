namespace DaLion.Common.Exceptions;

#region using directives

using System;

#endregion using directives

/// <summary>Thrown when an unexpected enum value is received.</summary>
/// <typeparam name="TEnum">The enum type that received an unexpected value.</typeparam>
public class UnexpectedEnumValueException<TEnum> : Exception
{
    /// <summary>Initializes a new instance of the <see cref="UnexpectedEnumValueException{T}"/> class.</summary>
    /// <param name="value">The unexpected enum value.</param>
    public UnexpectedEnumValueException(TEnum value)
        : base($"Enum {typeof(TEnum).Name} recieved unexpected value {value}")
    {
    }
}