﻿namespace DaLion.Shared;

#region using directives

using DaLion.Shared.Enums;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Provides generally useful methods.</summary>
public static class VectorUtils
{
    /// <summary>A unit vector pointing up.</summary>
    /// <returns>A unit <see cref="Vector2"/> pointing up.</returns>
    public static Vector2 UpVector()
    {
        return Vector2.UnitY * -1f;
    }

    /// <summary>A unit vector pointing down.</summary>
    /// <returns>A unit <see cref="Vector2"/> pointing down.</returns>
    public static Vector2 DownVector()
    {
        return Vector2.UnitY;
    }

    /// <summary>A unit vector pointing right.</summary>
    /// <returns>A unit <see cref="Vector2"/> pointing right.</returns>
    public static Vector2 RightVector()
    {
        return Vector2.UnitX;
    }

    /// <summary>A unit vector pointing left.</summary>
    /// <returns>A unit <see cref="Vector2"/> pointing left.</returns>
    public static Vector2 LeftVector()
    {
        return Vector2.UnitX * -1f;
    }

    /// <summary>
    ///     Gets the unit vector which points towards the cursor's current position relative to the local player's
    ///     position.
    /// </summary>
    /// <param name="direction">The corresponding <see cref="Direction"/> for the player to face the cursor.</param>
    /// <returns>A unit <see cref="Vector2"/> which points from the local player's position to the cursor's position.</returns>
    public static Vector2 GetRelativeCursorDirection(out Direction direction)
    {
        var (x, y) = Game1.currentCursorTile - Game1.player.Tile;
        if (Math.Abs(x) > Math.Abs(y))
        {
            if (x < 0)
            {
                direction = Direction.Left;
                return Vector2.UnitX * -1f;
            }

            direction = Direction.Right;
            return Vector2.UnitX;
        }

        if (y > 0)
        {
            direction = Direction.Up;
            return Vector2.UnitY * -1f;
        }

        direction = Direction.Down;
        return Vector2.UnitY;
    }
}
