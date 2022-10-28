namespace DaLion.Shared.Exceptions;

/// <summary>Thrown when an unexpected enum value is received.</summary>
/// <typeparam name="TEnum">The enum type that received an unexpected value.</typeparam>
public sealed class UnexpectedEnumValueException<TEnum> : Exception
{
    /// <summary>Initializes a new instance of the <see cref="UnexpectedEnumValueException{T}"/> class.</summary>
    /// <param name="value">The unexpected enum value.</param>
    public UnexpectedEnumValueException(int value)
        : base($"Enum {typeof(TEnum).Name} recieved unexpected value {value}")
    {
    }

    /// <summary>Initializes a new instance of the <see cref="UnexpectedEnumValueException{T}"/> class.</summary>
    /// <param name="value">The unexpected enum value.</param>
    public UnexpectedEnumValueException(TEnum value)
        : base($"Enum {typeof(TEnum).Name} recieved unexpected value {value}")
    {
    }
}
