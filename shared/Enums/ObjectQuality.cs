namespace DaLion.Shared.Enums;

#region using directives

using DaLion.Shared.Exceptions;
using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>The star quality of an <see cref="StardewValley.Object"/>.</summary>
[EnumExtensions]
public enum ObjectQuality
{
    /// <summary>Regular quality.</summary>
    Regular,

    /// <summary>Silver quality.</summary>
    Silver,

    /// <summary>Gold quality.</summary>
    Gold,

    /// <summary>Iridium quality.</summary>
    Iridium = 4,
}

/// <summary>Extensions for the <see cref="ObjectQuality"/> enum.</summary>
public static partial class QualityExtensions
{
    /// <summary>Improves a <see cref="ObjectQuality"/> level by one stage.</summary>
    /// <param name="quality">The <see cref="ObjectQuality"/>.</param>
    /// <returns>The <see cref="ObjectQuality"/> one stage higher than <paramref name="quality"/>.</returns>
    public static ObjectQuality Increment(this ObjectQuality quality)
    {
        return quality switch
        {
            ObjectQuality.Regular => ObjectQuality.Silver,
            ObjectQuality.Silver => ObjectQuality.Gold,
            ObjectQuality.Gold => ObjectQuality.Iridium,
            ObjectQuality.Iridium => ObjectQuality.Iridium,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<ObjectQuality, ObjectQuality>(quality),
        };
    }

    /// <summary>Lowers a <see cref="ObjectQuality"/> level by one stage.</summary>
    /// <param name="quality">The <see cref="ObjectQuality"/>.</param>
    /// <returns>The <see cref="ObjectQuality"/> one stage lower than <paramref name="quality"/>.</returns>
    public static ObjectQuality Decrement(this ObjectQuality quality)
    {
        return quality switch
        {
            ObjectQuality.Regular => ObjectQuality.Regular,
            ObjectQuality.Silver => ObjectQuality.Regular,
            ObjectQuality.Gold => ObjectQuality.Silver,
            ObjectQuality.Iridium => ObjectQuality.Gold,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<ObjectQuality, ObjectQuality>(quality),
        };
    }
}
