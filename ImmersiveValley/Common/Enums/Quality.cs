namespace DaLion.Common.Enums;

#region using directives

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
