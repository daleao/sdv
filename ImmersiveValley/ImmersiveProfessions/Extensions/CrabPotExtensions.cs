namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using DaLion.Stardew.Professions.Framework;
using DaLion.Stardew.Professions.Framework.Utility;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="CrabPot"/> class.</summary>
public static class CrabPotExtensions
{
    /// <summary>Determines whether the <paramref name="crabPot"/> is using magnet as bait.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="crabPot"/>'s bait value is the index of Magnet, otherwise <see langword="false"/>.</returns>
    public static bool HasMagnet(this CrabPot crabPot)
    {
        return crabPot.bait.Value?.ParentSheetIndex == 703;
    }

    /// <summary>Determines whether the <paramref name="crabPot"/> is using wild bait.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="crabPot"/>'s bait value is the index of Wild Bait, otherwise <see langword="false"/>.</returns>
    public static bool HasWildBait(this CrabPot crabPot)
    {
        return crabPot.bait.Value?.ParentSheetIndex == 774;
    }

    /// <summary>Determines whether the <paramref name="crabPot"/> is using magic bait.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="crabPot"/>'s bait value is the index of Magic Bait, otherwise <see langword="false"/>.</returns>
    public static bool HasMagicBait(this CrabPot crabPot)
    {
        return crabPot.bait.Value?.ParentSheetIndex == 908;
    }

    /// <summary>Determines whether the <paramref name="crabPot"/> should catch ocean-specific shellfish.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="location">The <see cref="GameLocation"/> of the <paramref name="crabPot"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="crabPot"/> is placed near ocean, otherwise <see langword="false"/>.</returns>
    public static bool ShouldCatchOceanFish(this CrabPot crabPot, GameLocation location)
    {
        return location is Beach ||
               location.catchOceanCrabPotFishFromThisSpot((int)crabPot.TileLocation.X, (int)crabPot.TileLocation.Y);
    }

    /// <summary>
    ///     Determines whether the <paramref name="crabPot"/> is holding an object that can only be caught via Luremaster
    ///     profession.
    /// </summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="crabPot"/> is holding anything other than trap fish, otherwise <see langword="false"/>.</returns>
    public static bool HasSpecialLuremasterCatch(this CrabPot crabPot)
    {
        if (crabPot.heldObject.Value is not { } obj)
        {
            return false;
        }

        return (obj.IsFish() && !obj.IsTrapFish()) || obj.IsAlgae() || obj.IsPirateTreasure();
    }

    /// <summary>Chooses a random fish index from amongst the allowed list of fish for the <paramref name="location"/>.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="fishData">Raw fish data from the game files.</param>
    /// <param name="location">The <see cref="GameLocation"/> of the <paramref name="crabPot"/>.</param>
    /// <param name="r">A random number generator.</param>
    /// <returns>The index of a random fish from the allowed list for the <paramref name="location"/>.</returns>
    public static int ChooseFish(
        this CrabPot crabPot, Dictionary<int, string> fishData, GameLocation location, Random r)
    {
        var rawFishData = crabPot.HasMagicBait()
            ? location.GetRawFishDataForAllSeasons()
            : location.GetRawFishDataForCurrentSeason();
        var rawFishDataWithLocation = GetRawFishDataWithLocation(rawFishData);

        var keys = rawFishDataWithLocation.Keys.ToArray();
        Utility.Shuffle(r, keys);
        var counter = 0;
        foreach (var key in keys)
        {
            var specificFishDataFields = fishData[Convert.ToInt32(key)].Split('/');
            if (Lookups.LegendaryFishNames.Contains(specificFishDataFields[0]))
            {
                continue;
            }

            var specificFishLocation = Convert.ToInt32(rawFishDataWithLocation[key]);
            if (!crabPot.HasMagicBait() &&
                (!IsCorrectLocationAndTimeForThisFish(
                     specificFishDataFields,
                     specificFishLocation,
                     crabPot.TileLocation,
                     location) ||
                 !IsCorrectWeatherForThisFish(specificFishDataFields, location)))
            {
                continue;
            }

            if (r.NextDouble() > Convert.ToDouble(specificFishDataFields[10]))
            {
                continue;
            }

            var whichFish = Convert.ToInt32(key);
            if (!whichFish.IsAlgaeIndex())
            {
                return whichFish; // if isn't algae
            }

            if (counter != 0)
            {
                return -1; // if already rerolled
            }

            ++counter;
        }

        return -1;
    }

    /// <summary>Chooses a random trap fish index from amongst the allowed list of fish for the <paramref name="location"/>.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="fishData">Raw fish data from the game files.</param>
    /// <param name="location">The <see cref="GameLocation"/> of the <paramref name="crabPot"/>.</param>
    /// <param name="r">A random number generator.</param>
    /// <param name="isLuremaster">Whether the owner of the crab pot is luremaster.</param>
    /// <returns>The index of a random trap fish from the allowed list for the <paramref name="location"/>.</returns>
    public static int ChooseTrapFish(
        this CrabPot crabPot, Dictionary<int, string> fishData, GameLocation location, Random r, bool isLuremaster)
    {
        List<int> keys = new();
        foreach (var (key, value) in fishData)
        {
            if (!value.Contains("trap"))
            {
                continue;
            }

            var shouldCatchOceanFish = crabPot.ShouldCatchOceanFish(location);
            var rawSplit = value.Split('/');
            if ((rawSplit[4] == "ocean" && !shouldCatchOceanFish) ||
                (rawSplit[4] == "freshwater" && shouldCatchOceanFish))
            {
                continue;
            }

            if (isLuremaster)
            {
                keys.Add(key);
                continue;
            }

            if (r.NextDouble() < Convert.ToDouble(rawSplit[2]))
            {
                return key;
            }
        }

        if (isLuremaster && keys.Count > 0)
        {
            return keys[r.Next(keys.Count)];
        }

        return -1;
    }

    /// <summary>Chooses a random treasure index from the pirate treasure loot table.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="owner">The player.</param>
    /// <param name="r">A random number generator.</param>
    /// <returns>The index of a random treasure <see cref="Item"/>.</returns>
    public static int ChoosePirateTreasure(this CrabPot crabPot, Farmer owner, Random r)
    {
        var keys = Lookups.TrapperPirateTreasureTable.Keys.ToArray();
        Utility.Shuffle(r, keys);
        foreach (var key in keys)
        {
            if ((key == 14 && owner.specialItems.Contains(14)) || (key == 51 && owner.specialItems.Contains(51)) ||
                (key == 890 && !owner.team.SpecialOrderRuleActive("DROP_QI_BEANS")))
            {
                continue;
            }

            if (r.NextDouble() > Convert.ToDouble(Lookups.TrapperPirateTreasureTable[key][0]))
            {
                continue;
            }

            return key;
        }

        return -1;
    }

    /// <summary>Gets the quality for the chosen <paramref name="trap"/> fish.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="trap">The chosen trap fish.</param>
    /// <param name="owner">The owner of the crab pot.</param>
    /// <param name="r">A random number generator.</param>
    /// <param name="isLuremaster">Whether the <paramref name="owner"/> has <see cref="Profession.Luremaster"/>.</param>
    /// <returns>A <see cref="SObject"/> quality value.</returns>
    public static int GetTrapQuality(this CrabPot crabPot, int trap, Farmer owner, Random r, bool isLuremaster)
    {
        if (isLuremaster && crabPot.HasMagicBait())
        {
            return SObject.bestQuality;
        }

        var fish = new SObject(trap, 1);
        if (!owner.HasProfession(Profession.Trapper) || fish.IsPirateTreasure() || fish.IsAlgae())
        {
            return SObject.lowQuality;
        }

        return owner.HasProfession(Profession.Trapper, true) && r.NextDouble() < owner.FishingLevel / 60d
            ? SObject.bestQuality
            : r.NextDouble() < owner.FishingLevel / 30d
                ? SObject.highQuality
                : r.NextDouble() < owner.FishingLevel / 15d
                    ? SObject.medQuality
                    : SObject.lowQuality;
    }

    /// <summary>Gets initial stack for the chosen <paramref name="trap"/> fish.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="trap">The chosen trap fish.</param>
    /// <param name="owner">The player.</param>
    /// <param name="r">A random number generator.</param>
    /// <returns>The stack value.</returns>
    public static int GetTrapQuantity(this CrabPot crabPot, int trap, Farmer owner, Random r)
    {
        return crabPot.HasWildBait() && r.NextDouble() < 0.25 + (owner.DailyLuck / 2.0)
            ? 2
            : Lookups.TrapperPirateTreasureTable.TryGetValue(trap, out var treasureData)
                ? r.Next(Convert.ToInt32(treasureData[1]), Convert.ToInt32(treasureData[2]) + 1)
                : 1;
    }

    /// <summary>Chooses a random, <paramref name="location"/>-appropriate, trash.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="location">The <see cref="GameLocation"/> of the <paramref name="crabPot"/>.</param>
    /// <param name="r">A random number generator.</param>
    /// <returns>The index of a random trash <see cref="Item"/>.</returns>
    public static int GetTrash(this CrabPot crabPot, GameLocation location, Random r)
    {
        if (!ModEntry.Config.SeaweedIsTrash || r.NextDouble() > 0.5)
        {
            return r.Next(167, 173);
        }

        int trash;
        switch (location)
        {
            case Beach:
            case IslandSouth:
            case IslandWest when location.getFishingLocation(crabPot.TileLocation) == 1:
                trash = 152; // seaweed
                break;
            case MineShaft:
            case Sewer:
            case BugLand:
                trash = r.Next(2) == 0 ? 153 : 157; // green or white algae
                break;
            default:
                if (location.NameOrUniqueName == "WithSwamp")
                {
                    trash = r.Next(2) == 0 ? 153 : 157;
                }
                else
                {
                    trash = 153; // green algae
                }

                break;
        }

        return trash;
    }

    #region private methods

    /// <summary>Converts raw fish data into a look-up for fishing locations by fish indices.</summary>
    /// <param name="rawFishData">String array of available fish indices and fishing locations.</param>
    private static Dictionary<string, string> GetRawFishDataWithLocation(string[] rawFishData)
    {
        Dictionary<string, string> rawFishDataWithLocation = new();
        if (rawFishData.Length <= 1)
        {
            return rawFishDataWithLocation;
        }

        for (var i = 0; i < rawFishData.Length; i += 2)
        {
            rawFishDataWithLocation[rawFishData[i]] = rawFishData[i + 1];
        }

        return rawFishDataWithLocation;
    }

    /// <summary>Determines whether the current fishing location and game time match the specific fish data.</summary>
    /// <param name="specificFishData">Raw game file data for this fish.</param>
    /// <param name="specificFishLocation">The fishing location index for this fish.</param>
    /// <param name="tileLocation">The crab pot tile location.</param>
    /// <param name="location">The game location of the crab pot.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> matches the <paramref name="specificFishLocation"/>.</returns>
    /// <remarks>
    ///     The time portion is doesn't actually make sense for <see cref="CrabPot"/>s since they (theoretically) update only once during the
    ///     night. Therefore <paramref name="specificFishData"/> is ignored.
    /// </remarks>
    private static bool IsCorrectLocationAndTimeForThisFish(
        string[] specificFishData, int specificFishLocation, Vector2 tileLocation, GameLocation location)
    {
        return specificFishLocation == -1 ||
               specificFishLocation ==
               location.getFishingLocation(tileLocation);
    }

    /// <summary>Determines whether the current weather matches the specific fish data.</summary>
    /// <param name="specificFishData">Raw game file data for this fish.</param>
    /// <param name="location">The <see cref="GameLocation"/> of the <see cref="CrabPot"/> which would catch the fish.</param>
    private static bool IsCorrectWeatherForThisFish(string[] specificFishData, GameLocation location)
    {
        if (specificFishData[7] == "both")
        {
            return true;
        }

        return (specificFishData[7] == "rainy" && !Game1.IsRainingHere(location)) ||
               (specificFishData[7] == "sunny" && Game1.IsRainingHere(location));
    }

    #endregion private methods
}
