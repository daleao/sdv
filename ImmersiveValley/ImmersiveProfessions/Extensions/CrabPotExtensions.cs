namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Framework;
using Framework.Utility;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="CrabPot"/> class.</summary>
public static class CrabPotExtensions
{
    /// <summary>Whether the crab pot instance is using magnet as bait.</summary>
    public static bool HasMagnet(this CrabPot crabpot) =>
        crabpot.bait.Value?.ParentSheetIndex == 703;

    /// <summary>Whether the crab pot instance is using wild bait.</summary>
    public static bool HasWildBait(this CrabPot crabpot) =>
        crabpot.bait.Value?.ParentSheetIndex == 774;

    /// <summary>Whether the crab pot instance is using magic bait.</summary>
    public static bool HasMagicBait(this CrabPot crabpot) =>
        crabpot.bait.Value?.ParentSheetIndex == 908;

    /// <summary>Whether the crab pot instance should catch ocean-specific shellfish.</summary>
    /// <param name="location">The location of the crab pot.</param>
    public static bool ShouldCatchOceanFish(this CrabPot crabpot, GameLocation location) =>
        location is Beach ||
        location.catchOceanCrabPotFishFromThisSpot((int)crabpot.TileLocation.X, (int)crabpot.TileLocation.Y);

    /// <summary>Whether the given crab pot instance is holding an object that can only be caught via Luremaster profession.</summary>
    public static bool HasSpecialLuremasterCatch(this CrabPot crabpot)
    {
        if (crabpot.heldObject.Value is not { } @object) return false;
        return @object.IsFish() && !@object.IsTrapFish() || @object.IsAlgae() || @object.IsPirateTreasure();
    }

    /// <summary>Choose amongst a pre-select list of fish.</summary>
    /// <param name="crabpot">The crab pot instance.</param>
    /// <param name="fishData">Raw fish data from the game files.</param>
    /// <param name="location">The game location of the crab pot.</param>
    /// <param name="r">Random number generator.</param>
    public static int ChooseFish(this CrabPot crabpot, Dictionary<int, string> fishData, GameLocation location, Random r)
    {
        var rawFishData = crabpot.HasMagicBait()
            ? location.GetRawFishDataForAllSeasons()
            : location.GetRawFishDataForCurrentSeason();
        var rawFishDataWithLocation = GetRawFishDataWithLocation(rawFishData);

        var keys = rawFishDataWithLocation.Keys.ToArray();
        Utility.Shuffle(r, keys);
        var counter = 0;
        foreach (var key in keys)
        {
            var specificFishDataFields = fishData[Convert.ToInt32(key)].Split('/');
            if (ObjectLookups.LegendaryFishNames.Contains(specificFishDataFields[0])) continue;

            var specificFishLocation = Convert.ToInt32(rawFishDataWithLocation[key]);
            if (!crabpot.HasMagicBait() &&
                (!IsCorrectLocationAndTimeForThisFish(specificFishDataFields, specificFishLocation,
                     crabpot.TileLocation, location) ||
                 !IsCorrectWeatherForThisFish(specificFishDataFields, location)))
                continue;

            if (r.NextDouble() > Convert.ToDouble(specificFishDataFields[10])) continue;

            var whichFish = Convert.ToInt32(key);
            if (!whichFish.IsAlgaeIndex()) return whichFish; // if isn't algae

            if (counter != 0) return -1; // if already rerolled
            ++counter;
        }

        return -1;
    }

    /// <summary>Choose amongst a pre-select list of shellfish.</summary>
    /// <param name="crabpot">The crab pot instance.</param>
    /// <param name="fishData">Raw fish data from the game files.</param>
    /// <param name="location">The game location of the crab pot.</param>
    /// <param name="r">Random number generator.</param>
    /// <param name="isLuremaster">Whether the owner of the crab pot is luremaster.</param>
    public static int ChooseTrapFish(this CrabPot crabpot, Dictionary<int, string> fishData, GameLocation location,
        Random r, bool isLuremaster)
    {
        List<int> keys = new();
        foreach (var (key, value) in fishData)
        {
            if (!value.Contains("trap")) continue;

            var shouldCatchOceanFish = crabpot.ShouldCatchOceanFish(location);
            var rawSplit = value.Split('/');
            if (rawSplit[4] == "ocean" && !shouldCatchOceanFish ||
                rawSplit[4] == "freshwater" && shouldCatchOceanFish)
                continue;

            if (isLuremaster)
            {
                keys.Add(key);
                continue;
            }

            if (r.NextDouble() < Convert.ToDouble(rawSplit[2])) return key;
        }

        if (isLuremaster && keys.Count > 0) return keys[r.Next(keys.Count)];

        return -1;
    }

    /// <summary>Choose a treasure from the pirate treasure loot table.</summary>
    /// <param name="owner">The player.</param>
    /// <param name="r">Random number generator.</param>
    public static int ChoosePirateTreasure(this CrabPot crabpot, Farmer owner, Random r)
    {
        var keys = ObjectLookups.TrapperPirateTreasureTable.Keys.ToArray();
        Utility.Shuffle(r, keys);
        foreach (var key in keys)
        {
            if (key == 14 && owner.specialItems.Contains(14) || key == 51 && owner.specialItems.Contains(51) ||
                key == 890 && !owner.team.SpecialOrderRuleActive("DROP_QI_BEANS")) continue;

            if (r.NextDouble() > Convert.ToDouble(ObjectLookups.TrapperPirateTreasureTable[key][0])) continue;

            return key;
        }

        return -1;
    }

    /// <summary>Get the quality for the chosen catch.</summary>
    /// <param name="whichFish">The chosen catch.</param>
    /// <param name="owner">The owner of the crab pot.</param>
    /// <param name="r">Random number generator.</param>
    public static int GetTrapQuality(this CrabPot crabpot, int whichFish, Farmer owner, Random r, bool isLuremaster)
    {
        if (isLuremaster && crabpot.HasMagicBait()) return SObject.bestQuality;

        var fish = new SObject(whichFish, 1);
        if (!owner.HasProfession(Profession.Trapper) || fish.IsPirateTreasure() || fish.IsAlgae())
            return SObject.lowQuality;

        return owner.HasProfession(Profession.Trapper, true) && r.NextDouble() < owner.FishingLevel / 60d
            ? SObject.bestQuality
            : r.NextDouble() < owner.FishingLevel / 30d
                ? SObject.highQuality
                : r.NextDouble() < owner.FishingLevel / 15d
                    ? SObject.medQuality
                    : SObject.lowQuality;
    }

    /// <summary>Get initial stack for the chosen stack.</summary>
    /// <param name="crabpot">The crab pot instance.</param>
    /// <param name="whichFish">The chosen fish</param>
    /// <param name="owner">The player.</param>
    /// <param name="r">Random number generator.</param>
    public static int GetTrapQuantity(this CrabPot crabpot, int whichFish, Farmer owner, Random r) =>
        crabpot.HasWildBait() && r.NextDouble() < 0.25 + owner.DailyLuck / 2.0
            ? 2
            : ObjectLookups.TrapperPirateTreasureTable.TryGetValue(whichFish, out var treasureData)
                ? r.Next(Convert.ToInt32(treasureData[1]), Convert.ToInt32(treasureData[2]) + 1)
                : 1;

    /// <summary>Get random trash.</summary>
    /// <param name="r">Random number generator.</param>
    /// <param name="location">The game location of the crab pot.</param>
    public static int GetTrash(this CrabPot crabpot, GameLocation location, Random r)
    {
        if (ModEntry.Config.SeaweedIsTrash && r.NextDouble() > 0.5) return r.Next(167, 173);

        int trash;
        switch (location)
        {
            case Beach:
            case IslandSouth:
            case IslandWest when location.getFishingLocation(crabpot.TileLocation) == 1:
                trash = 152; // seaweed
                break;
            case MineShaft:
            case Sewer:
            case BugLand:
                trash = r.Next(2) == 0 ? 153 : 157; // green or white algae
                break;
            default:
                if (location.NameOrUniqueName == "WithSwamp") trash = r.Next(2) == 0 ? 153 : 157;
                else trash = 153; // green algae
                break;
        }

        return trash;

    }

    #region private methods

    /// <summary>Convert raw fish data into a look-up dictionary for fishing locations from fish indices.</summary>
    /// <param name="rawFishData">String array of catchable fish indices and fishing locations.</param>
    private static Dictionary<string, string> GetRawFishDataWithLocation(string[] rawFishData)
    {
        Dictionary<string, string> rawFishDataWithLocation = new();
        if (rawFishData.Length > 1)
            for (var i = 0; i < rawFishData.Length; i += 2)
                rawFishDataWithLocation[rawFishData[i]] = rawFishData[i + 1];
        return rawFishDataWithLocation;
    }

    /// <summary>Whether the current fishing location and game time match the specific fish data.</summary>
    /// <param name="specificFishData">Raw game file data for this fish.</param>
    /// <param name="specificFishLocation">The fishing location index for this fish.</param>
    /// <param name="tileLocation">The crab pot tile location.</param>
    /// <param name="location">The game location of the crab pot.</param>
    /// <remarks>The time portion is commented out because doesn't make sense for crab pots that only update once during the night.</remarks>
    private static bool IsCorrectLocationAndTimeForThisFish(string[] specificFishData, int specificFishLocation,
        Vector2 tileLocation, GameLocation location) => specificFishLocation == -1 ||
                                                        specificFishLocation ==
                                                        location.getFishingLocation(tileLocation);

    /// <summary>Whether the current weather matches the specific fish data.</summary>
    /// <param name="specificFishData">Raw game file data for this fish.</param>
    /// <param name="location">The location of the crab pot.</param>
    private static bool IsCorrectWeatherForThisFish(string[] specificFishData, GameLocation location)
    {
        if (specificFishData[7] == "both") return true;

        return specificFishData[7] == "rainy" && !Game1.IsRainingHere(location) ||
               specificFishData[7] == "sunny" && Game1.IsRainingHere(location);
    }

    #endregion private methods
}