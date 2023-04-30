namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Linq;
using DaLion.Shared.ModData;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class GameLocationExtensions
{
    /// <summary>Determines whether this <paramref name="location"/> has spawned enemies.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> is has at least one living monster and is not a <see cref="SlimeHutch"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasMonsters(this GameLocation location)
    {
        return location.characters.Any(c => c.IsMonster) && location is not SlimeHutch;
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
                    for (var i = 0; i < who.team.specialOrders.Count; i++)
                    {
                        var order = who.team.specialOrders[i];
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

        if (location.isCharacterAtTile(tile) is { } and not Monster)
        {
            return true;
        }

        if (location is IAnimalLocation animalLocation)
        {
            foreach (var animal in animalLocation.Animals.Values)
            {
                if (animal.GetCursorPetBoundingBox().Contains((int)x, (int)y))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <inheritdoc cref="ModDataIO.Read(GameLocation, string, string, string)"/>
    public static string Read(this GameLocation location, string field, string defaultValue = "", string modId = "")
    {
        return ModDataIO.Read(location, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Read{T}(GameLocation, string, T, string)"/>
    public static T Read<T>(this GameLocation location, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return ModDataIO.Read(location, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Write(GameLocation, string, string?)"/>
    public static void Write(this GameLocation location, string field, string? value)
    {
        ModDataIO.Write(location, field, value);
    }

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists(GameLocation, string, string?)"/>
    public static void WriteIfNotExists(this GameLocation location, string field, string? value)
    {
        ModDataIO.WriteIfNotExists(location, field, value);
    }

    /// <inheritdoc cref="ModDataIO.Append(GameLocation, string, string, string)"/>
    public static void Append(this GameLocation location, string field, string value, string separator = ",")
    {
        ModDataIO.Append(location, field, value, separator);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(GameLocation, string, T)"/>
    public static void Increment<T>(this GameLocation location, string field, T amount)
        where T : struct
    {
        ModDataIO.Increment(location, field, amount);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(GameLocation, string, T)"/>
    public static void Increment(this GameLocation location, string field)
    {
        ModDataIO.Increment(location, field, 1);
    }
}
