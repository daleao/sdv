namespace DaLion.Common.Enums;

#region using directives

using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>The star quality of an <see cref="StardewValley.Object"/>.</summary>
[EnumExtensions]
internal enum Quality
{
    Regular,
    Silver,
    Gold,
    Iridium = 4
}