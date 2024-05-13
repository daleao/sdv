namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Building"/> class.</summary>
public static class BuildingExtensions
{
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
    ///     Gets the squared pixel distance between this <paramref name="building"/> and the target <paramref name="position"/>.
    /// </summary>
    /// <param name="building">The <see cref="Character"/>.</param>
    /// <param name="position">The target position.</param>
    /// <returns>The squared pixel distance between <paramref name="building"/> and the <paramref name="position"/>.</returns>
    public static float SquaredPixelDistance(this Building building, Vector2 position)
    {
        var bbox = building.GetBoundingBox();
        var dx = bbox.Center.X - position.X;
        var dy = bbox.Center.Y - position.Y;
        return (dx * dx) + (dy * dy);
    }

    /// <summary>
    ///     Gets the squared tile distance between this <paramref name="building"/> and the target <paramref name="tile"/>.
    /// </summary>
    /// <param name="building">The <see cref="Character"/>.</param>
    /// <param name="tile">The target tile.</param>
    /// <returns>The squared tile distance between <paramref name="building"/> and the <paramref name="tile"/>.</returns>
    public static float SquaredTileDistance(this Building building, Vector2 tile)
    {
        var v = new Vector2(building.tileX.Value, building.tileY.Value);
        var dx = v.X - tile.X;
        var dy = v.Y - tile.Y;
        return (dx * dx) + (dy * dy);
    }

    /// <summary>
    ///     Finds the closest tile from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="building"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <returns>The closest tile from among the specified <paramref name="candidates"/> to this <paramref name="building"/>.</returns>
    public static Vector2 GetClosestTile(this Building building, IEnumerable<Vector2> candidates)
    {
        var closest = new Vector2(building.tileX.Value, building.tileY.Value);
        var distanceToClosest = float.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = building.SquaredTileDistance(candidate);
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
    ///     <paramref name="building"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <param name="getPosition">A delegate to retrieve the tile coordinates of <typeparamref name="T"/>.</param>
    /// <param name="distance">The actual tile distance to the closest candidate found.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The closest target from among the specified <paramref name="candidates"/> to this <paramref name="building"/>.</returns>
    public static T? GetClosest<T>(
        this Building building,
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
            var distanceToThisCandidate = building.SquaredPixelDistance(getPosition(candidate));
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
    ///     Finds the closest <see cref="Building"/> to this one in the current <see cref="GameLocation"/>, and of the
    ///     specified subtype.
    /// </summary>
    /// <typeparam name="TBuilding">A subtype of <see cref="Building"/>.</typeparam>
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
        candidates ??= building.GetParentLocation().buildings.OfType<TBuilding>();
        return building.GetClosest(
            candidates,
            b => b.GetBoundingBox().Center.ToVector2(),
            out _,
            b => !ReferenceEquals(b, building) && predicate(b));
    }

    /// <summary>
    ///     Finds the closest <see cref="Character"/> of subtype <typeparamref name="TCharacter"/> to this
    ///     <paramref name="building"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TCharacter">A subtype of <see cref="Character"/>.</typeparam>
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
        candidates ??= building.GetParentLocation().characters.OfType<TCharacter>();
        return building.GetClosest(candidates, c => c.Position, out _, predicate);
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
        candidates ??= building.GetParentLocation().farmers;
        return building.GetClosest(candidates, f => f.Position, out _, predicate);
    }

    /// <summary>
    ///     Finds the closest <see cref="SObject"/> of subtype <typeparamref name="TObject"/> to this
    ///     <paramref name="building"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TObject">A subtype of <see cref="SObject"/>.</typeparam>
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
        candidates ??= building.GetParentLocation().Objects.Values.OfType<TObject>();
        return building.GetClosest(candidates, o => o.TileLocation * Game1.tileSize, out _, predicate);
    }

    /// <summary>
    ///     Find the closest <see cref="TerrainFeature"/> of subtype <typeparamref name="TTerrainFeature"/> to this
    ///     <paramref name="building"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TTerrainFeature">A subtype of <see cref="TerrainFeature"/>.</typeparam>
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
        candidates ??= Game1.getFarm().terrainFeatures.Values.OfType<TTerrainFeature>();
        return building.GetClosest(candidates, t => t.Tile * Game1.tileSize, out _, predicate);
    }
}
