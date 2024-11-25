namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Checks whether the <paramref name="object"/> is a spawned forageable item.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a forage item and is a spawned object, otherwise <see langword="false"/>.</returns>
    public static bool IsForage(this SObject @object)
    {
        return @object.IsSpawnedObject && @object.isForage();
    }

    /// <summary>Gets the <see cref="Farmer"/> instance who owns this <paramref name="object"/>.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns>The <see cref="Farmer"/> instance who purchased, found or crafted the <paramref name="object"/>, or the host of the game session if not found.</returns>
    public static Farmer GetOwner(this SObject @object)
    {
        return Game1.GetPlayer(@object.owner.Value) ?? Game1.MasterPlayer;
    }

    /// <summary>Checks whether the <paramref name="object"/> is owned by the specified <see cref="Farmer"/>.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/>'s owner value is equal to the unique ID of the <paramref name="farmer"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsOwnedBy(this SObject @object, Farmer farmer)
    {
        return @object.owner.Value == farmer.UniqueMultiplayerID;
    }

    /// <summary>
    ///     Gets the squared pixel distance between this <paramref name="object"/> and the target <paramref name="position"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="position">The target tile.</param>
    /// <returns>The squared pixel distance between <paramref name="object"/> and the <paramref name="position"/>.</returns>
    public static float SquaredPixelDistance(this SObject @object, Vector2 position)
    {
        var dx = (@object.TileLocation.X * Game1.tileSize) - position.X;
        var dy = (@object.TileLocation.Y * Game1.tileSize) - position.Y;
        return (dx * dx) + (dy * dy);
    }

    /// <summary>
    ///     Gets the squared tile distance between this <paramref name="object"/> and the target <paramref name="tile"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="tile">The target tile.</param>
    /// <returns>The squared tile distance between <paramref name="object"/> and the <paramref name="tile"/>.</returns>
    public static float SquaredTileDistance(this SObject @object, Vector2 tile)
    {
        var dx = @object.TileLocation.X - tile.X;
        var dy = @object.TileLocation.Y - tile.Y;
        return (dx * dx) + (dy * dy);
    }

    /// <summary>
    ///     Finds the closest tile from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="object"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="candidates">The candidate <see cref="SObject"/>s, if already available.</param>
    /// <returns>The closest tile from among the specified <paramref name="candidates"/> to this <paramref name="object"/>.</returns>
    public static Vector2 GetClosestTile(this SObject @object, IEnumerable<Vector2> candidates)
    {
        var closest = @object.TileLocation;
        var distanceToClosest = float.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = @object.SquaredTileDistance(candidate);
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
    ///     Finds the closest target from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="object"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="T"/>s, if already available.</param>
    /// <param name="getPosition">A delegate to retrieve the tile coordinates of <typeparamref name="T"/>.</param>
    /// <param name="distance">The actual tile distance to the closest candidate found.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The closest target from among the specified <paramref name="candidates"/> to this <paramref name="object"/>.</returns>
    public static T? GetClosest<T>(
        this SObject @object,
        IEnumerable<T> candidates,
        Func<T, Vector2> getPosition,
        out float distance,
        Func<T, bool>? predicate = null)
        where T : class
    {
        predicate ??= _ => true;
        candidates = candidates.Where(c => predicate(c));
        T? closest = null;
        var distanceToClosest = float.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = @object.SquaredPixelDistance(getPosition(candidate));
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        distance = distanceToClosest;
        return closest;
    }

    /// <summary>
    ///     Find the closest <see cref="Building"/> of subtype <typeparamref name="TBuilding"/> to this
    ///     <paramref name="object"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TBuilding">A subtype of <see cref="Building"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TBuilding"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Building"/> of type <typeparamref name="TBuilding"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TBuilding? GetClosestBuilding<TBuilding>(
        this SObject @object,
        IEnumerable<TBuilding>? candidates = null,
        Func<TBuilding, bool>? predicate = null)
        where TBuilding : Building
    {
        candidates ??= @object.Location.buildings.OfType<TBuilding>();
        return @object.GetClosest(candidates, b => b.GetBoundingBox().Center.ToVector2(), out _, predicate);
    }

    /// <summary>
    ///     Find the closest <see cref="Character"/> of subtype <typeparamref name="TCharacter"/> to this <paramref name="object"/>
    ///     in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TCharacter">A subtype of <see cref="Character"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TCharacter"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Character"/> of type <typeparamref name="TCharacter"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TCharacter? GetClosestCharacter<TCharacter>(
        this SObject @object,
        IEnumerable<TCharacter>? candidates = null,
        Func<TCharacter, bool>? predicate = null)
        where TCharacter : Character
    {
        candidates ??= @object.Location.characters.OfType<TCharacter>();
        return @object.GetClosest(candidates, c => c.Position, out _, predicate);
    }

    /// <summary>
    ///     Find the closest <see cref="Farmer"/> to this <paramref name="object"/> in the current
    ///     <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="candidates">The candidate <see cref="Farmer"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Farmer"/> with the minimal distance to <paramref name="object"/>.</returns>
    /// <remarks>This version is required as <see cref="Farmer"/> references are stored in a different field of <see cref="GameLocation"/>.</remarks>
    public static Farmer? GetClosestFarmer(
        this SObject @object,
        IEnumerable<Farmer>? candidates = null,
        Func<Farmer, bool>? predicate = null)
    {
        candidates ??= @object.Location.farmers;
        return @object.GetClosest(candidates, f => f.Position, out _, predicate);
    }

    /// <summary>
    ///     Find the closest <see cref="SObject"/> to this one in the current <see cref="GameLocation"/>, and of the
    ///     specified subtype.
    /// </summary>
    /// <typeparam name="TObject">A subtype of <see cref="SObject"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TObject"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="SObject"/> of type <typeparamref name="TObject"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TObject? GetClosestObject<TObject>(
        this SObject @object,
        IEnumerable<TObject>? candidates = null,
        Func<TObject, bool>? predicate = null)
        where TObject : SObject
    {
        predicate ??= _ => true;
        candidates ??= @object.Location.Objects.Values.OfType<TObject>();
        return @object.GetClosest(
            candidates,
            o => o.TileLocation * Game1.tileSize,
            out _,
            o => !ReferenceEquals(o, @object) && predicate(o));
    }

    /// <summary>
    ///     Find the closest <see cref="TerrainFeature"/> of subtype <typeparamref name="TTerrainFeature"/> to this
    ///     <paramref name="object"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TTerrainFeature">A subtype of <see cref="TerrainFeature"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TTerrainFeature"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="TerrainFeature"/> of type <typeparamref name="TTerrainFeature"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TTerrainFeature? GetClosestTerrainFeature<TTerrainFeature>(
        this SObject @object,
        IEnumerable<TTerrainFeature>? candidates = null,
        Func<TTerrainFeature, bool>? predicate = null)
        where TTerrainFeature : TerrainFeature
    {
        candidates ??= @object.Location.terrainFeatures.Values.OfType<TTerrainFeature>();
        return @object.GetClosest(candidates, t => t.Tile * Game1.tileSize, out _, predicate);
    }
}
