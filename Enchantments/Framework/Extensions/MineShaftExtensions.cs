namespace DaLion.Enchantments.Framework.Extensions;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Locations;

#endregion using directives

/// <summary>Extensions for the <see cref="MineShaft"/> class.</summary>
internal static class MineShaftExtensions
{
    /// <summary>Determines whether there is an excavatable wall at the coordinates (<paramref name="tileX"/>, <paramref name="tileY"/>).</summary>
    /// <param name="shaft">The <see cref="MineShaft"/>.</param>
    /// <param name="tileX">The x-coordinate of a tile.</param>
    /// <param name="tileY">The y-coordinate of a tile.</param>
    /// <returns><see langword="true"/> if the tile is a wall and there is a passable tile directly ahead, otherwise <see langword="false"/>.</returns>
    internal static bool IsExcavatable(this MineShaft shaft, int tileX, int tileY)
    {
        var layers = shaft.Map.Layers;
        var back = layers[0];
        var walls = layers[1];

        if (walls.Tiles[tileX, tileY] is null)
        {
            return false;
        }

        var (dx, dy, boundary) = Game1.player.FacingDirection switch
        {
            Game1.up => (0, -1, tileY),
            Game1.right => (1, 0, back.TileWidth - 1 - tileX),
            Game1.down => (0, 1, back.TileHeight - 1 - tileY),
            Game1.left => (-1, 0, tileX),
            _ => (0, 0, 0),
        };

        const int maxWallThickness = 6;
        for (var i = 1; i <= Math.Min(maxWallThickness, boundary); i++)
        {
            if (shaft.isTilePassable(new Vector2(tileX + (dx * i), tileY + (dy * i))))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Excavates the wall at the coordinates (<paramref name="tileX"/>, <paramref name="tileY"/>).</summary>
    /// <param name="shaft">The <see cref="MineShaft"/>.</param>
    /// <param name="tileX">The x-coordinate of a tile.</param>
    /// <param name="tileY">The y-coordinate of a tile.</param>
    /// <returns><see langword="true"/> if the excavation was successful, otherwise <see langword="false"/>.</returns>
    internal static bool Excavate(this MineShaft shaft, int tileX, int tileY)
    {
        var map = shaft.Map;
        var layers = map.Layers;
        var back = layers[0];
        var walls = layers[1];
        var front = layers[2];

        var direction = (Direction)Game1.player.FacingDirection;

        bool[,] mask;
        switch (direction)
        {
            case Direction.Up:
                mask = new bool[3, 6];
                for (var j = 5; j >= 0; j--)
                {
                    for (var i = -1; i <= 1; i++)
                    {
                        var x = tileX + i;
                        var y = tileY + j;
                        mask[i + 1, j] = walls.Tiles[x, y] is not null;
                    }
                }

                break;
            case Direction.Right:
                mask = new bool[3, 3];
                for (var i = 0; i < 3; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        var x = tileX + i;
                        var y = tileY + j;
                        mask[i, j + 1] = walls.Tiles[x, y] is not null;
                    }
                }

                break;
            case Direction.Left:
                mask = new bool[3, 3];
                for (var i = 2; i >= 0; i--)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        var x = tileX + i;
                        var y = tileY + j;
                        mask[i, j + 1] = walls.Tiles[x, y] is not null;
                    }
                }

                break;
            case Direction.Down:
                mask = new bool[3, 6];
                for (var j = 0; j < 6; j++)
                {
                    for (var i = -1; i <= 1; i++)
                    {
                        var x = tileX + i;
                        var y = tileY + j;
                        mask[i + 1, j] = walls.Tiles[x, y] is not null;
                    }
                }

                break;

            default:
                mask = new bool[0, 0];
                break;
        }

        if (!ExcavationRules.TryGet(direction, mask.ToMaskString(), out var rule))
        {
            return false;
        }

        foreach (var change in rule.Changes)
        {
            map.Layers[(int)change.Layer].Tiles[change.X, change.Y].TileIndex = change.TileIndex;
        }

        Game1.Multiplayer.broadcastSprites(
            shaft,
            new TemporaryAnimatedSprite(5, new Vector2((64f * tileX) - 32f, 64f * (tileY - 1f)), Color.Gray, 8, Game1.random.NextBool(), 50f)
            {
                delayBeforeAnimationStart = 0,
            });

        Game1.Multiplayer.broadcastSprites(
            shaft,
            new TemporaryAnimatedSprite(5, new Vector2((64f * tileX) + 32f, 64f * (tileY - 1f)), Color.Gray, 8, Game1.random.NextBool(), 50f)
            {
                delayBeforeAnimationStart = 200,
            });
        Game1.Multiplayer.broadcastSprites(
            shaft,
            new TemporaryAnimatedSprite(5, new Vector2(64f * tileX, (64f * (tileY - 1f)) - 32f), Color.Gray, 8, Game1.random.NextBool(), 50f)
            {
                delayBeforeAnimationStart = 400,
            });
        Game1.Multiplayer.broadcastSprites(
            shaft,
            new TemporaryAnimatedSprite(5, new Vector2(64f * tileX, (64f * tileY) - 32f), Color.Gray, 8, Game1.random.NextBool(), 50f)
            {
                delayBeforeAnimationStart = 600,
            });
        Game1.Multiplayer.broadcastSprites(
            shaft,
            new TemporaryAnimatedSprite(25, new Vector2(64f * tileX, 64f * tileY), Color.White, 8, Game1.random.NextBool(), 50f, 0, -1, -1f, 128));
        Game1.Multiplayer.broadcastSprites(
            shaft,
            new TemporaryAnimatedSprite(25, new Vector2((64f * tileX) + 32f, 64f * tileY), Color.White, 8, Game1.random.NextBool(), 50f, 0, -1, -1f, 128)
            {
                delayBeforeAnimationStart = 250,
            });
        Game1.Multiplayer.broadcastSprites(
            shaft,
            new TemporaryAnimatedSprite(25, new Vector2((64f * tileX) - 32f, 64f * tileY), Color.White, 8, Game1.random.NextBool(), 50f, 0, -1, -1f, 128)
            {
                delayBeforeAnimationStart = 500,
            });

        shaft.playSound("boulderBreak");
        return true;
    }
}
