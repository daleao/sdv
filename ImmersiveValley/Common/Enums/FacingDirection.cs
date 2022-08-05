namespace DaLion.Common.Enums;

#region using directives

using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>The direction which a character is facing.</summary>
[EnumExtensions]
public enum FacingDirection
{
    Up,
    Right,
    Down,
    Left
}

public static partial class FacingDirectionExtensions
{
    public static FacingDirection GetOppositeDirection(this FacingDirection direction) => direction + 2 % 4;
}