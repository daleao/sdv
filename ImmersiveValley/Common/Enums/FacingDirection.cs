namespace DaLion.Common.Enums;

#region using directives

using DaLion.Common.Exceptions;
using Microsoft.Xna.Framework;
using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>The direction which a <see cref="Character"/> is facing.</summary>
[EnumExtensions]
public enum FacingDirection
{
    /// <summary>The up direction.</summary>
    Up,

    /// <summary>The right direction.</summary>
    Right,

    /// <summary>The down direction.</summary>
    Down,

    /// <summary>The left direction.</summary>
    Left,
}

/// <summary>Extensions for the <see cref="FacingDirection"/> enum.</summary>
public static partial class FacingDirectionExtensions
{
    /// <summary>Gets the opposite <see cref="FacingDirection"/>.</summary>
    /// <param name="direction">The <see cref="FacingDirection"/>.</param>
    /// <returns>The opposite direction.</returns>
    public static FacingDirection Opposite(this FacingDirection direction)
    {
        return direction + (2 % 4);
    }

    /// <summary>Gets a unit vector which points in the specified direction.</summary>
    /// <param name="direction">A <see cref="FacingDirection"/>.</param>
    /// <returns>A unit <see cref="Vector2"/> pointing towards <paramref name="direction"/>.</returns>
    public static Vector2 ToVector(this FacingDirection direction)
    {
        return direction switch
        {
            FacingDirection.Up => VectorUtils.UpVector(),
            FacingDirection.Right => VectorUtils.RightVector(),
            FacingDirection.Down => VectorUtils.DownVector(),
            FacingDirection.Left => VectorUtils.LeftVector(),
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, Vector2>(direction),
        };
    }
}
