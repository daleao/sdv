namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System;
using DaLion.Shared.Enums;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Character"/> class.</summary>
public static class NpcExtensions
{
    /// <summary>Sets the <paramref name="npc"/> in motion in the direction of the specified <paramref name="tile"/>.</summary>
    /// <param name="npc">The <see cref="NPC"/>.</param>
    /// <param name="tile">The <see cref="Vector2"/> tile.</param>
    public static void SetMovingTowardTile(this NPC npc, Vector2 tile)
    {
        var (dx, dy) = npc.Tile - tile;
        var direction = Math.Abs(dx) > Math.Abs(dy)
            ? dx >= 0 ? Direction.Right : Direction.Left
            : dy >= 0 ? Direction.Down : Direction.Up;
        npc.SetMoving(direction);
    }

    /// <summary>Sets the <paramref name="npc"/> in motion in the direction of the specified <paramref name="tile"/>.</summary>
    /// <param name="npc">The <see cref="NPC"/>.</param>
    /// <param name="tile">The <see cref="Point"/> tile.</param>
    public static void SetMovingTowardTile(this NPC npc, Point tile)
    {
        var (dx, dy) = tile - npc.TilePoint;
        var direction = Math.Abs(dx) > Math.Abs(dy)
            ? dx >= 0 ? Direction.Right : Direction.Left
            : dy >= 0 ? Direction.Down : Direction.Up;
        npc.SetMoving(direction);
    }

    /// <summary>Sets the <paramref name="npc"/> in motion in the specified <paramref name="direction"/>.</summary>
    /// <param name="npc">The <see cref="NPC"/>.</param>
    /// <param name="direction">The <see cref="Direction"/>.</param>
    public static void SetMoving(this NPC npc, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                npc.SetMovingOnlyUp();
                break;
            case Direction.Down:
                npc.SetMovingOnlyDown();
                break;
            case Direction.Left:
                npc.SetMovingOnlyLeft();
                break;
            case Direction.Right:
                npc.SetMovingOnlyRight();
                break;
        }
    }
}
