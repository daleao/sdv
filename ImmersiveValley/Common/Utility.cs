namespace DaLion.Common;

#region using directives

using Enums;
using Exceptions;
using Microsoft.Xna.Framework;
using System;

#endregion using directives

public static class Utility
{
    /// <summary>A unit vector pointing up.</summary>
    public static Vector2 UpVector() => Vector2.UnitY * -1f;

    /// <summary>A unit vector pointing down.</summary>
    public static Vector2 DownVector() => Vector2.UnitY;

    /// <summary>A unit vector pointing right.</summary>
    public static Vector2 RightVector() => Vector2.UnitX;

    /// <summary>A unit vector pointing left.</summary>
    public static Vector2 LeftVector() => Vector2.UnitX * -1f;

    /// <summary>Get a unit vector which points in the specified direction.</summary>
    /// <param name="direction">A <see cref="FacingDirection"/>.</param>
    public static Vector2 VectorFromFacingDirection(FacingDirection direction) => direction switch
    {
        FacingDirection.Up => UpVector(),
        FacingDirection.Right => RightVector(),
        FacingDirection.Down => DownVector(),
        FacingDirection.Left => LeftVector(),
        _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, Vector2>(direction)
    };

    /// <summary>Get the unit vector which points towards the cursor's current position relative to the local player's position.</summary>
    public static Vector2 GetRelativeCursorDirection(out FacingDirection direction)
    {
        var (x, y) = Game1.currentCursorTile - Game1.player.getTileLocation();
        if (Math.Abs(x) > Math.Abs(y))
        {
            if (x < 0)
            {
                direction = FacingDirection.Left;
                return Vector2.UnitX * -1f;
            }

            direction = FacingDirection.Right;
            return Vector2.UnitX;
        }

        if (y > 0)
        {
            direction = FacingDirection.Up;
            return Vector2.UnitY * -1f;
        }

        direction = FacingDirection.Down;
        return Vector2.UnitY;
    }
}