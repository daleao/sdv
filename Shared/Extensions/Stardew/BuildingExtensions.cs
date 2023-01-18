namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.ModData;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Building"/> class.</summary>
public static class BuildingExtensions
{
    /// <summary>Determines whether the <paramref name="building"/> can accommodate no further occupants.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="building"/>'s current occupation is equal to the max, otherwise <see langword="false"/>.</returns>
    public static bool IsFull(this Building building)
    {
        return building.currentOccupants.Value >= building.maxOccupants.Value;
    }

    /// <summary>Gets the <see cref="Farmer"/> instance who owns this <paramref name="building"/>.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <returns>The <see cref="Farmer"/> instance who constructed the <paramref name="building"/>, or the host of the game session if not found.</returns>
    public static Farmer GetOwner(this Building building)
    {
        return Game1.getFarmerMaybeOffline(building.owner.Value) ?? Game1.MasterPlayer;
    }

    /// <summary>Checks whether the <paramref name="building"/> is owned by the specified <see cref="Farmer"/>.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="building"/>'s owned value is equal to the unique ID of the <paramref name="farmer"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsOwnedBy(this Building building, Farmer farmer)
    {
        return building.owner.Value == farmer.UniqueMultiplayerID;
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="building"/> and some <paramref name="other"/> in the
    ///     same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="other">The target <see cref="Building"/>.</param>
    /// <returns>The tile distance between <paramref name="building"/> and <paramref name="other"/>.</returns>
    public static double DistanceTo(this Building building, Building other)
    {
        return (new Vector2(building.tileX.Value, building.tileY.Value) -
                new Vector2(other.tileX.Value, other.tileY.Value))
            .Length();
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="building"/> and the target <paramref name="character"/>
    ///     in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="character">The target <see cref="Character"/>.</param>
    /// <returns>The tile distance between <paramref name="building"/> and <paramref name="character"/>.</returns>
    public static double DistanceTo(this Building building, Character character)
    {
        return (new Vector2(building.tileX.Value, building.tileY.Value) - character.getTileLocation()).Length();
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="building"/> and the target <paramref name="obj"/> in
    ///     the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="obj">The target <see cref="SObject"/>.</param>
    /// <returns>The tile distance between <paramref name="building"/> and <paramref name="obj"/>.</returns>
    public static double DistanceTo(this Building building, SObject obj)
    {
        return (new Vector2(building.tileX.Value, building.tileY.Value) - obj.TileLocation).Length();
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="building"/> and the target
    ///     <paramref name="terrainFeature"/> in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="terrainFeature">The target <see cref="TerrainFeature"/>.</param>
    /// <returns>The tile distance between <paramref name="building"/> and <paramref name="terrainFeature"/>.</returns>
    public static double DistanceTo(this Building building, TerrainFeature terrainFeature)
    {
        return (new Vector2(building.tileX.Value, building.tileY.Value) - terrainFeature.currentTileLocation).Length();
    }

    /// <summary>
    ///     Finds the closest <see cref="Building"/> to this one in the current <see cref="GameLocation"/>, and of the
    ///     specified sub-type.
    /// </summary>
    /// <typeparam name="TBuilding">A sub-type of <see cref="Building"/>.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Building"/> of type <typeparamref name="TBuilding"/> with the minimal distance to <paramref name="building"/>.</returns>
    /// <remarks>
    ///     As the <see cref="Building"/> class does not hold a reference to its <see cref="GameLocation"/>, it is
    ///     assumed to be the <see cref="Farm"/>.
    /// </remarks>
    public static TBuilding? GetClosestBuilding<TBuilding>(
        this Building building,
        IEnumerable<TBuilding>? candidates = null,
        Func<TBuilding, bool>? predicate = null)
        where TBuilding : Building
    {
        predicate ??= _ => true;
        candidates ??= Game1.getFarm().buildings
            .OfType<TBuilding>()
            .Where(t => predicate(t));
        TBuilding? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>
    ///     Finds the closest <see cref="Character"/> of sub-type <typeparamref name="TCharacter"/> to this
    ///     <paramref name="building"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TCharacter">A sub-type of <see cref="Character"/>.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="Character"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Character"/> of type <typeparamref name="TCharacter"/> with the minimal distance to <paramref name="building"/>.</returns>
    /// <remarks>
    ///     As the <see cref="Building"/> class does not hold a reference to its <see cref="GameLocation"/>, it is
    ///     assumed to be the <see cref="Farm"/>.
    /// </remarks>
    public static TCharacter? GetClosestCharacter<TCharacter>(
        this Building building,
        IEnumerable<TCharacter>? candidates = null,
        Func<TCharacter, bool>? predicate = null)
        where TCharacter : Character
    {
        predicate ??= _ => true;
        candidates ??= Game1.getFarm().characters
            .OfType<TCharacter>()
            .Where(t => predicate(t));
        TCharacter? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>
    ///     Finds the closest <see cref="Farmer"/> to this <paramref name="building"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="Character"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Farmer"/> with the minimal distance to <paramref name="building"/>.</returns>
    /// <remarks>
    ///     As the <see cref="Building"/> class does not hold a reference to its <see cref="GameLocation"/>, it is
    ///     assumed to be the <see cref="Farm"/>.
    ///     This version is required as <see cref="Farmer"/> references are stored in a different field of <see cref="GameLocation"/>.
    /// </remarks>
    public static Farmer? GetClosestFarmer(
        this Building building,
        IEnumerable<Farmer>? candidates = null,
        Func<Farmer, bool>? predicate = null)
    {
        predicate ??= _ => true;
        candidates ??= Game1.getFarm().farmers
            .Where(t => predicate(t));
        Farmer? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>
    ///     Finds the closest <see cref="SObject"/> of sub-type <typeparamref name="TObject"/> to this
    ///     <paramref name="building"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TObject">A sub-type of <see cref="SObject"/>.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="SObject"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="SObject"/> of type <typeparamref name="TObject"/> with the minimal distance to <paramref name="building"/>.</returns>
    /// <remarks>
    ///     As the <see cref="Building"/> class does not hold a reference to its <see cref="GameLocation"/>, it is
    ///     assumed to be the <see cref="Farm"/>.
    /// </remarks>
    public static TObject? GetClosestObject<TObject>(
        this Building building,
        IEnumerable<TObject>? candidates = null,
        Func<TObject, bool>? predicate = null)
        where TObject : SObject
    {
        predicate ??= _ => true;
        candidates ??= Game1.getFarm().Objects.Values
            .OfType<TObject>()
            .Where(o => predicate(o));
        TObject? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>
    ///     Find the closest <see cref="TerrainFeature"/> of sub-type <typeparamref name="TTerrainFeature"/> to this
    ///     <paramref name="building"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TTerrainFeature">A sub-type of <see cref="TerrainFeature"/>.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="TerrainFeature"/>, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="TerrainFeature"/> of type <typeparamref name="TTerrainFeature"/> with the minimal distance to <paramref name="building"/>.</returns>
    /// <remarks>
    ///     As the <see cref="Building"/> class does not hold a reference to its <see cref="GameLocation"/>, it is
    ///     assumed to be the <see cref="Farm"/>.
    /// </remarks>
    public static TTerrainFeature? GetClosestTerrainFeature<TTerrainFeature>(
        this Building building,
        IEnumerable<TTerrainFeature>? candidates = null,
        Func<TTerrainFeature, bool>? predicate = null)
        where TTerrainFeature : TerrainFeature
    {
        predicate ??= _ => true;
        candidates ??= Game1.getFarm().terrainFeatures.Values
            .OfType<TTerrainFeature>()
            .Where(t => predicate(t));
        TTerrainFeature? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <inheritdoc cref="ModDataIO.Read(Building, string, string, string)"/>
    public static string Read(this Building building, string field, string defaultValue = "", string modId = "")
    {
        return ModDataIO.Read(building, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Read{T}(Building, string, T, string)"/>
    public static T Read<T>(this Building building, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return ModDataIO.Read(building, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Write(Building, string, string?)"/>
    public static void Write(this Building building, string field, string? value)
    {
        ModDataIO.Write(building, field, value);
    }

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists(Building, string, string?)"/>
    public static void WriteIfNotExists(this Building building, string field, string? value)
    {
        ModDataIO.WriteIfNotExists(building, field, value);
    }

    /// <inheritdoc cref="ModDataIO.Append(Building, string, string, string)"/>
    public static void Append(this Building building, string field, string value, string separator = ",")
    {
        ModDataIO.Append(building, field, value, separator);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(Building, string, T)"/>
    public static void Increment<T>(this Building building, string field, T amount)
        where T : struct
    {
        ModDataIO.Increment(building, field, amount);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(Building, string)"/>
    public static void Increment(this Building building, string field)
    {
        ModDataIO.Increment(building, field, 1);
    }
}
