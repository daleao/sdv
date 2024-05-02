namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Determines whether the <paramref name="obj"/> is an artisan good.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is created by an artisan machine, otherwise <see langword="false"/>.</returns>
    public static bool IsArtisanGood(this SObject obj)
    {
        return obj.Category is (int)ObjectCategory.ArtisanGoods or (int)ObjectCategory.Syrups ||
               obj.ParentSheetIndex == ObjectIds.Coffee;
    }

    /// <summary>Determines whether the <paramref name="object"/> is a bee house.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a Bee House, otherwise <see langword="false"/>.</returns>
    public static bool IsBeeHouse(this SObject @object)
    {
        return @object.bigCraftable.Value && @object.ParentSheetIndex == BigCraftableIds.BeeHouse;
    }

    /// <summary>Determines whether the <paramref name="object"/> is a mushroom box.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a Mushroom Box, otherwise <see langword="false"/>.</returns>
    public static bool IsMushroomBox(this SObject @object)
    {
        return @object.bigCraftable.Value && @object.ParentSheetIndex == BigCraftableIds.MushroomBox;
    }

    /// <summary>Determines whether <paramref name="object"/> is Spring Onion.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is Spring Onion, otherwise <see langword="false"/>.</returns>
    public static bool IsSpringOnion(this SObject @object)
    {
        return @object.ParentSheetIndex == ObjectIds.SpringOnion;
    }

    /// <summary>Determines whether <paramref name="object"/> is Salmonberry or Blackberry.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is Salmonberry or Blackberry, otherwise <see langword="false"/>.</returns>
    public static bool IsWildBerry(this SObject @object)
    {
        return @object.ParentSheetIndex is ObjectIds.Salmonberry or ObjectIds.Blackberry;
    }

    /// <summary>Determines whether <paramref name="object"/> is an Artifact Spot.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is an Artifact Spot, otherwise <see langword="false"/>.</returns>
    public static bool IsArtifactSpot(this SObject @object)
    {
        return @object.ParentSheetIndex == ObjectIds.ArtifactSpot;
    }

    /// <summary>
    ///     Determines whether <paramref name="object"/> is a fish typically caught with a
    ///     <see cref="StardewValley.Tools.FishingRod"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is any fish, otherwise <see langword="false"/>.</returns>
    public static bool IsFish(this SObject @object)
    {
        return @object.Category == SObject.FishCategory;
    }

    /// <summary>
    ///     Determines whether <paramref name="object"/> is a fish typically caught with a
    ///     <see cref="CrabPot"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a <see cref="CrabPot"/> fish, otherwise <see langword="false"/>.</returns>
    public static bool IsTrapFish(this SObject @object)
    {
        return @object.ParentSheetIndex.IsTrapFishIndex();
    }

    /// <summary>Determines whether the <paramref name="obj"/> is algae or seaweed.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is any of the three algae, otherwise <see langword="false"/>.</returns>
    public static bool IsAlgae(this SObject obj)
    {
        return obj.ParentSheetIndex.IsAlgaeIndex();
    }

    /// <summary>Determines whether <paramref name="object"/> is trash.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is any trash item, otherwise <see langword="false"/>.</returns>
    public static bool IsTrash(this SObject @object)
    {
        return @object.Category == SObject.junkCategory;
    }

    /// <summary>Determines whether the <paramref name="object"/> is a gem or mineral.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a gem or mineral, otherwise <see langword="false"/>.</returns>
    public static bool IsGemOrMineral(this SObject @object)
    {
        return @object.Category is SObject.GemCategory or SObject.mineralsCategory;
    }

    /// <summary>Determines whether <paramref name="object"/> is a foraged mineral.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a Quartz, Fire Quartz, Frozen Tear or Earth Crystal, otherwise <see langword="false"/>.</returns>
    public static bool IsForagedMineral(this SObject @object)
    {
        return @object.ParentSheetIndex is ObjectIds.Quartz or ObjectIds.FireQuartz or ObjectIds.FrozenTear
            or ObjectIds.EarthCrystal;
    }

    /// <summary>Determines whether <paramref name="object"/> is a simple Stone.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a mining node not containing only stone, otherwise <see langword="false"/>.</returns>
    public static bool IsStone(this SObject @object)
    {
        return @object.Name == "Stone";
    }

    /// <summary>Determines whether the <paramref name="object"/> is a twig.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is twig, otherwise <see langword="false"/>.</returns>
    public static bool IsTwig(this SObject @object)
    {
        return @object.ParentSheetIndex is ObjectIds.Twig0 or ObjectIds.Twig1;
    }

    /// <summary>Determines whether the <paramref name="object"/> is a weed.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is weed, otherwise <see langword="false"/>.</returns>
    public static bool IsWeed(this SObject @object)
    {
        return @object is not Chest && @object.Name == "Weeds";
    }

    /// <summary>Gets the <see cref="Farmer"/> instance who owns this <paramref name="object"/>.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns>The <see cref="Farmer"/> instance who purchased, found or crafted the <paramref name="object"/>, or the host of the game session if not found.</returns>
    public static Farmer GetOwner(this SObject @object)
    {
        return Game1.getFarmerMaybeOffline(@object.owner.Value) ?? Game1.MasterPlayer;
    }

    /// <summary>Checks whether the <paramref name="object"/> is owned by the specified <see cref="Farmer"/>.</summary>
    /// <param name="object">The <see cref="Building"/>.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/>'s owner value is equal to the unique ID of the <paramref name="farmer"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsOwnedBy(this SObject @object, Farmer farmer)
    {
        return @object.owner.Value == farmer.UniqueMultiplayerID;
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="object"/> and the target <paramref name="tile"/>.
    /// </summary>
    /// <param name="object">The <see cref="Character"/>.</param>
    /// <param name="tile">The target tile.</param>
    /// <returns>The tile distance between <paramref name="object"/> and the <paramref name="tile"/>.</returns>
    public static double DistanceTo(this SObject @object, Vector2 tile)
    {
        return (@object.TileLocation - tile).Length();
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="object"/> and the target <paramref name="building"/> in
    ///     the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="building">The target <see cref="Building"/>.</param>
    /// <returns>The tile distance between <paramref name="object"/> and <paramref name="building"/>.</returns>
    public static double DistanceTo(this SObject @object, Building building)
    {
        return @object.DistanceTo(new Vector2(building.tileX.Value, building.tileY.Value));
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="object"/> and the target <paramref name="character"/>
    ///     in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="character">The target <see cref="Character"/>.</param>
    /// <returns>The tile distance between <paramref name="object"/> and <paramref name="character"/>.</returns>
    public static double DistanceTo(this SObject @object, Character character)
    {
        return @object.DistanceTo(character.getTileLocation());
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="object"/> and some <paramref name="other"/> in the same
    ///     <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="other">The target <see cref="SObject"/>.</param>
    /// <returns>The tile distance between <paramref name="object"/> and <paramref name="other"/>.</returns>
    public static double DistanceTo(this SObject @object, SObject other)
    {
        return @object.DistanceTo(other.TileLocation);
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="object"/> and the target
    ///     <paramref name="terrainFeature"/> in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="terrainFeature">The target <see cref="TerrainFeature"/>.</param>
    /// <returns>The tile distance between <paramref name="object"/> and <paramref name="terrainFeature"/>.</returns>
    public static double DistanceTo(this SObject @object, TerrainFeature terrainFeature)
    {
        return @object.DistanceTo(terrainFeature.currentTileLocation);
    }

    /// <summary>
    ///     Finds the closest target from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="object"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <param name="getPosition">A delegate to retrieve the tile coordinates of <typeparamref name="T"/>.</param>
    /// <param name="distance">The distance to the closest <see cref="Building"/>, in number of tiles.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The closest target from among the specified <paramref name="candidates"/> to this <paramref name="object"/>.</returns>
    public static T? GetClosest<T>(
        this SObject @object,
        IEnumerable<T> candidates,
        Func<T, Vector2> getPosition,
        out double distance,
        Func<T, bool>? predicate = null)
        where T : class
    {
        predicate ??= _ => true;
        candidates = candidates.Where(c => predicate(c));
        T? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = @object.DistanceTo(getPosition(candidate));
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
    ///     Find the closest <see cref="Building"/> of sub-type <typeparamref name="TBuilding"/> to this
    ///     <paramref name="object"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TBuilding">A sub-type of <see cref="Building"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Building"/> of type <typeparamref name="TBuilding"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TBuilding? GetClosestBuilding<TBuilding>(
        this SObject @object,
        GameLocation location,
        IEnumerable<TBuilding>? candidates = null,
        Func<TBuilding, bool>? predicate = null)
        where TBuilding : Building
    {
        if (location is not BuildableGameLocation buildable)
        {
            return null;
        }

        predicate ??= _ => true;
        candidates ??= buildable.buildings
            .OfType<TBuilding>()
            .Where(t => predicate(t));
        TBuilding? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = @object.DistanceTo(candidate);
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
    ///     Find the closest <see cref="Character"/> of sub-type <typeparamref name="TCharacter"/> to this <paramref name="object"/>
    ///     in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TCharacter">A sub-type of <see cref="Character"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate <see cref="Character"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Character"/> of type <typeparamref name="TCharacter"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TCharacter? GetClosestCharacter<TCharacter>(
        this SObject @object,
        GameLocation location,
        IEnumerable<TCharacter>? candidates = null,
        Func<TCharacter, bool>? predicate = null)
        where TCharacter : Character
    {
        predicate ??= _ => true;
        candidates ??= location.characters
            .OfType<TCharacter>()
            .Where(t => predicate(t));
        TCharacter? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = @object.DistanceTo(candidate);
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
    ///     Find the closest <see cref="Farmer"/> to this <paramref name="object"/> in the current
    ///     <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate <see cref="Farmer"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Farmer"/> with the minimal distance to <paramref name="object"/>.</returns>
    /// <remarks>This version is required as <see cref="Farmer"/> references are stored in a different field of <see cref="GameLocation"/>.</remarks>
    public static Farmer? GetClosestFarmer(
        this SObject @object,
        GameLocation location,
        IEnumerable<Farmer>? candidates = null,
        Func<Farmer, bool>? predicate = null)
    {
        predicate ??= _ => true;
        candidates ??= location.farmers.Where(f => predicate(f));
        Farmer? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = @object.DistanceTo(candidate);
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
    ///     Find the closest <see cref="SObject"/> to this one in the current <see cref="GameLocation"/>, and of the
    ///     specified sub-type.
    /// </summary>
    /// <typeparam name="TObject">A sub-type of <see cref="SObject"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate <see cref="SObject"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="SObject"/> of type <typeparamref name="TObject"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TObject? GetClosestObject<TObject>(
        this SObject @object,
        GameLocation location,
        IEnumerable<TObject>? candidates = null,
        Func<TObject, bool>? predicate = null)
        where TObject : SObject
    {
        predicate ??= _ => true;
        candidates ??= location.Objects.Values
            .OfType<TObject>()
            .Where(o => predicate(o));
        TObject? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = @object.DistanceTo(candidate);
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
    ///     <paramref name="object"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TTerrainFeature">A sub-type of <see cref="TerrainFeature"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate <see cref="TerrainFeature"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="TerrainFeature"/> of type <typeparamref name="TTerrainFeature"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TTerrainFeature? GetClosestTerrainFeature<TTerrainFeature>(
        this SObject @object,
        GameLocation location,
        IEnumerable<TTerrainFeature>? candidates = null,
        Func<TTerrainFeature, bool>? predicate = null)
        where TTerrainFeature : TerrainFeature
    {
        predicate ??= _ => true;
        candidates ??= location.terrainFeatures.Values
            .OfType<TTerrainFeature>()
            .Where(t => predicate(t));
        TTerrainFeature? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = @object.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }
}
