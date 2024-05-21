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
            ? dx >= 0 ? FacingDirection.Right : FacingDirection.Left
            : dy >= 0 ? FacingDirection.Down : FacingDirection.Up;
        npc.SetMoving(direction);
    }

    /// <summary>Sets the <paramref name="npc"/> in motion in the direction of the specified <paramref name="tile"/>.</summary>
    /// <param name="npc">The <see cref="NPC"/>.</param>
    /// <param name="tile">The <see cref="Point"/> tile.</param>
    public static void SetMovingTowardTile(this NPC npc, Point tile)
    {
        var (dx, dy) = tile - npc.TilePoint;
        var direction = Math.Abs(dx) > Math.Abs(dy)
            ? dx >= 0 ? FacingDirection.Right : FacingDirection.Left
            : dy >= 0 ? FacingDirection.Down : FacingDirection.Up;
        npc.SetMoving(direction);
    }

    /// <summary>Sets the <paramref name="npc"/> in motion in the specified <paramref name="direction"/>.</summary>
    /// <param name="npc">The <see cref="NPC"/>.</param>
    /// <param name="direction">The <see cref="FacingDirection"/>.</param>
    public static void SetMoving(this NPC npc, FacingDirection direction)
    {
        switch (direction)
        {
            case FacingDirection.Up:
                npc.SetMovingOnlyUp();
                break;
            case FacingDirection.Down:
                npc.SetMovingOnlyDown();
                break;
            case FacingDirection.Left:
                npc.SetMovingOnlyLeft();
                break;
            case FacingDirection.Right:
                npc.SetMovingOnlyRight();
                break;
        }
    }
}
