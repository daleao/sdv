﻿namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class GameLocationExtensions
{
    /// <summary>Determines whether this <paramref name="location"/> is a dungeon.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> is a <see cref="MineShaft"/> or one of several recognized dungeon locations, otherwise <see langword="false"/>.</returns>
    /// <remarks>Includes locations from Stardew Valley Expanded, Ridgeside Village and Moon Misadventures.</remarks>
    public static bool IsEnemyArea(this GameLocation location)
    {
        return location is MineShaft or BugLand or VolcanoDungeon ||
               location.NameOrUniqueName.ContainsAnyOf(
                   "CrimsonBadlands",
                   "DeepWoods",
                   "Highlands",
                   "RidgeForest",
                   "SpiritRealm",
                   "AsteroidsDungeon");
    }

    /// <summary>Determines whether there is a stone at the specified <paramref name="tile"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The <see cref="Vector2"/> tile position.</param>
    /// <returns><see langword="true"/> if there is a stone at the specified <paramref name="tile"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasStoneAt(this GameLocation location, Vector2 tile)
    {
        return location.Objects.TryGetValue(tile, out var @object) && @object.IsBreakableStone();
    }

    /// <summary>Determines whether there is anything to interact with in the specified <paramref name="tile"></paramref>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The tile's position as <see cref="Vector2"/>.</param>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="tile"/> has action properties or contains any actionable object or actor, otherwise <see langword="false"/>.</returns>
    public static bool IsActionableTile(this GameLocation location, Vector2 tile, Farmer? who = null)
    {
        who ??= Game1.player;
        var (x, y) = tile;

        var hasActionProperty = location.doesTileHaveProperty((int)x, (int)y, "Action", "Buildings");
        if (hasActionProperty is not null)
        {
            if (hasActionProperty.StartsWith("DropBox"))
            {
                var split = hasActionProperty.Split(' ');
                if (split.Length >= 2 && who.team.specialOrders is not null)
                {
                    foreach (var order in who.team.specialOrders)
                    {
                        if (order.UsesDropBox(split[1]))
                        {
                            continue;
                        }

                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
        }

        if (location.Objects.TryGetValue(tile, out var @object) && @object.isActionable(who))
        {
            return true;
        }

        if (!Game1.isFestival() && location.terrainFeatures.TryGetValue(tile, out var feature) && feature.isActionable())
        {
            return true;
        }

        if (location.isCharacterAtTile(tile) is not null and not Monster)
        {
            return true;
        }

        foreach (var animal in location.Animals.Values)
        {
            if (animal.GetCursorPetBoundingBox().Contains((int)x, (int)y))
            {
                return true;
            }
        }

        return false;
    }
}
