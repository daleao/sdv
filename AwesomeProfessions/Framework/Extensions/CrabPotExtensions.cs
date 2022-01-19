namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

using ObjectLookups = Utility.ObjectLookups;

#endregion using directives

public static class CrabPotExtensions
{
    /// <summary>Whether the crab pot instance is using magnet as bait.</summary>
    public static bool HasMagnet(this CrabPot crabpot)
    {
        return crabpot.bait.Value is not null &&
               ObjectLookups.BaitById.TryGetValue(crabpot.bait.Value.ParentSheetIndex, out var baitName) &&
               baitName == "Magnet";
    }

    /// <summary>Whether the crab pot instance is using wild bait.</summary>
    public static bool HasWildBait(this CrabPot crabpot)
    {
        return crabpot.bait.Value is not null &&
               ObjectLookups.BaitById.TryGetValue(crabpot.bait.Value.ParentSheetIndex, out var baitName) &&
               baitName == "Wild Bait";
    }

    /// <summary>Whether the crab pot instance is using magic bait.</summary>
    public static bool HasMagicBait(this CrabPot crabpot)
    {
        return crabpot.bait.Value is not null &&
               ObjectLookups.BaitById.TryGetValue(crabpot.bait.Value.ParentSheetIndex, out var baitName) &&
               baitName == "Magic Bait";
    }

    /// <summary>Whether the crab pot instance should catch ocean-specific shellfish.</summary>
    /// <param name="location">The location of the crab pot.</param>
    public static bool ShouldCatchOceanFish(this CrabPot crabpot, GameLocation location)
    {
        return location is Beach ||
               location.catchOceanCrabPotFishFromThisSpot((int) crabpot.TileLocation.X,
                   (int) crabpot.TileLocation.Y);
    }

    /// <summary>Whether the given crab pot instance is holding an object that can only be caught via Luremaster profession.</summary>
    public static bool HasSpecialLuremasterCatch(this CrabPot crabpot)
    {
        var obj = crabpot.heldObject.Value;
        return obj is not null && (obj.IsFish() && !obj.IsTrapFish() || obj.IsAlgae() || obj.IsPirateTreasure());
    }
}