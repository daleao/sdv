namespace DaLion.Shared.Enums;

#region using directives

using DaLion.Shared.Exceptions;
using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>The star quality of an <see cref="StardewValley.Object"/>.</summary>
[EnumExtensions]
public enum Quality
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

/// <summary>Extensions for the <see cref="Quality"/> enum.</summary>
public static partial class QualityExtensions
{
    /// <summary>Improves a <see cref="Quality"/> level by one stage.</summary>
    /// <param name="quality">The <see cref="Quality"/>.</param>
    /// <returns>The <see cref="Quality"/> one stage higher than <paramref name="quality"/>.</returns>
    public static Quality Increment(this Quality quality)
    {
        return quality switch
        {
            Quality.Regular => Quality.Silver,
            Quality.Silver => Quality.Gold,
            Quality.Gold => Quality.Iridium,
            Quality.Iridium => Quality.Iridium,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<Quality, Quality>(quality),
        };
    }

    /// <summary>Lowers a <see cref="Quality"/> level by one stage.</summary>
    /// <param name="quality">The <see cref="Quality"/>.</param>
    /// <returns>The <see cref="Quality"/> one stage lower than <paramref name="quality"/>.</returns>
    public static Quality Decrement(this Quality quality)
    {
        return quality switch
        {
            Quality.Regular => Quality.Regular,
            Quality.Silver => Quality.Regular,
            Quality.Gold => Quality.Silver,
            Quality.Iridium => Quality.Gold,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<Quality, Quality>(quality),
        };
    }
}
