namespace DaLion.Shared.Enums;

#region using directives

using DaLion.Shared.Exceptions;
using Microsoft.Xna.Framework;
using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>Represents a cardinal direction in the game world.</summary>
[EnumExtensions]
public enum Direction
{
    /// <summary>The up direction.</summary>
    Up = Game1.up,

    /// <summary>The right direction.</summary>
    Right = Game1.right,

    /// <summary>The down direction.</summary>
    Down = Game1.down,

    /// <summary>The left direction.</summary>
    Left = Game1.left,
}

/// <summary>Extensions for the <see cref="Direction"/> enum.</summary>
public static partial class FacingDirectionExtensions
{
    /// <summary>Checks whether the <see cref="Direction"/> is left or right.</summary>
    /// <param name="direction">The <see cref="Direction"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="direction"/> is left or right, otherwise <see langword="false"/>.</returns>
    public static bool IsHorizontal(this Direction direction)
    {
        return direction is Direction.Left or Direction.Right;
    }

    /// <summary>Checks whether the <see cref="Direction"/> is up or down.</summary>
    /// <param name="direction">The <see cref="Direction"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="direction"/> is up or down, otherwise <see langword="false"/>.</returns>
    public static bool IsVertical(this Direction direction)
    {
        return direction is Direction.Up or Direction.Down;
    }

    /// <summary>Gets the opposite <see cref="Direction"/>.</summary>
    /// <param name="direction">The <see cref="Direction"/>.</param>
    /// <returns>The opposite direction.</returns>
    public static Direction Invert(this Direction direction)
    {
        return (Direction)(((int)direction + 2) % 4);
    }

    /// <summary>Gets the opposite <see cref="Direction"/>.</summary>
    /// <param name="direction">The <see cref="Direction"/>.</param>
    /// <returns>The opposite direction.</returns>
    public static Direction NextClockwise(this Direction direction)
    {
        return (Direction)(((int)direction + 1) % 4);
    }

    /// <summary>Gets the opposite <see cref="Direction"/>.</summary>
    /// <param name="direction">The <see cref="Direction"/>.</param>
    /// <returns>The opposite direction.</returns>
    public static Direction NextAntiClockwise(this Direction direction)
    {
        return (Direction)(((int)direction + 3) % 4);
    }

    /// <summary>Gets the angle between two <see cref="Direction"/>s.</summary>
    /// <param name="direction">The <see cref="Direction"/>.</param>
    /// <param name="other">Another <see cref="Direction"/>.</param>
    /// <returns>The angle between the two directions, in degrees.</returns>
    public static int AngleWith(this Direction direction, Direction other)
    {
        if (direction == other)
        {
            return 0;
        }

        var delta = other - direction;
        if (delta is 1 or -3)
        {
            return -90;
        }

        if (-delta is 1 or -3)
        {
            return 90;
        }

        if (delta % 2 == 0)
        {
            return 180;
        }

        return 0; // should never happen
    }

    /// <summary>Gets a unit vector which points in the specified direction.</summary>
    /// <param name="direction">A <see cref="Direction"/>.</param>
    /// <returns>A unit <see cref="Vector2"/> pointing towards <paramref name="direction"/>.</returns>
    public static Vector2 ToVector(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => VectorUtils.UpVector(),
            Direction.Right => VectorUtils.RightVector(),
            Direction.Down => VectorUtils.DownVector(),
            Direction.Left => VectorUtils.LeftVector(),
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<Direction, Vector2>(direction),
        };
    }
}
