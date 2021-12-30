using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Extensions;
using TheLion.Stardew.Professions.Framework.Utility;
using SObject = StardewValley.Object;
using SUtility = StardewValley.Utility;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class CrabPotDayUpdatePatch : BasePatch
{
    private const double CHANCE_TO_CATCH_FISH_D = 0.25, CHANCE_TO_CATCH_TRASH_D = 0.5;

    /// <summary>Construct an instance.</summary>
    internal CrabPotDayUpdatePatch()
    {
        Original = RequireMethod<CrabPot>(nameof(CrabPot.DayUpdate));
    }

    #region harmony patches

    /// <summary>Patch for Trapper fish quality + Luremaster bait mechanics + Conservationist trash collection mechanics.</summary>
    [HarmonyPrefix]
    private static bool CrabPotDayUpdatePrefix(ref CrabPot __instance, GameLocation location)
    {
        try
        {
            var who = Game1.getFarmer(__instance.owner.Value);
            var isConservationist = who.HasProfession("Conservationist");
            if (__instance.bait.Value is null && !isConservationist || __instance.heldObject.Value is not null)
                return false; // don't run original logic

            __instance.tileIndexToShow = 714;
            __instance.readyForHarvest.Value = true;

            //var r = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 +
            //                   (int) __instance.TileLocation.X * 1000 + (int) __instance.TileLocation.Y);
            var r = new Random(Guid.NewGuid().GetHashCode());
            var fishData =
                Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"));
            var isLuremaster = who.HasProfession("Luremaster");
            var whichFish = -1;
            if (__instance.bait.Value is not null)
            {
                if (isLuremaster)
                {
                    if (__instance.HasMagnet())
                    {
                        whichFish = ChoosePirateTreasure(r, who);
                    }
                    else if (Game1.random.NextDouble() < CHANCE_TO_CATCH_FISH_D)
                    {
                        var rawFishData = __instance.HasMagicBait()
                            ? location.GetRawFishDataForAllSeasons()
                            : location.GetRawFishDataForCurrentSeason();
                        var rawFishDataWithLocation = GetRawFishDataWithLocation(rawFishData);
                        whichFish = ChooseFish(__instance, fishData, rawFishDataWithLocation, location, r);
                        if (whichFish < 0) whichFish = ChooseTrapFish(__instance, fishData, location, r, true);
                    }
                    else
                    {
                        whichFish = ChooseTrapFish(__instance, fishData, location, r, true);
                    }
                }
                else
                {
                    whichFish = ChooseTrapFish(__instance, fishData, location, r, false);
                }
            }

            if (whichFish.IsAnyOf(14, 51, 516, 517, 518, 519, 527, 529, 530, 531, 532, 533, 534)) // ring or weapon
            {
                var equipment = new SObject(whichFish, 1);
                __instance.heldObject.Value = equipment;
                return false; // don't run original logic
            }

            var fishQuality = 0;
            if (whichFish < 0)
            {
                if (__instance.bait.Value is not null || isConservationist && r.NextDouble() < CHANCE_TO_CATCH_TRASH_D)
                {
                    whichFish = GetTrash(r);
                    if (isConservationist)
                    {
                        ModEntry.Data.Increment<uint>("WaterTrashCollectedThisSeason");
                        if (who.HasPrestigedProfession("Conservationist") &&
                            ModEntry.Data.Read<uint>("WaterTrashCollectedThisSeason") %
                            ModEntry.Config.TrashNeededPerFriendshipPoint == 0)
                            SUtility.improveFriendshipWithEveryoneInRegion(who, 1, 2);
                    }
                }
                else
                {
                    return false; // don't run original logic
                }
            }
            else
            {
                fishQuality = GetTrapFishQuality(whichFish, who, r, __instance, isLuremaster);
            }

            var fishQuantity = GetTrapFishQuantity(__instance, whichFish, who, r);
            __instance.heldObject.Value = new(whichFish, fishQuantity, quality: fishQuality);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }
    }

    #endregion harmony patches

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
    /// <param name="crabpot">The crab pot instance.</param>
    /// <param name="location">The location of the crab pot.</param>
    private static bool IsCorrectLocationAndTimeForThisFish(string[] specificFishData, int specificFishLocation,
        Vector2 tileLocation, GameLocation gameLocation)
    {
        var specificFishSpawnTimes = specificFishData[5].Split(' ');
        if (specificFishLocation == -1 || gameLocation.getFishingLocation(tileLocation) == specificFishLocation)
            for (var t = 0; t < specificFishSpawnTimes.Length; t += 2)
                if (Game1.timeOfDay >= Convert.ToInt32(specificFishSpawnTimes[t]) &&
                    Game1.timeOfDay < Convert.ToInt32(specificFishSpawnTimes[t + 1]))
                    return true;

        return false;
    }

    /// <summary>Whether the current weather matches the specific fish data.</summary>
    /// <param name="specificFishData">Raw game file data for this fish.</param>
    /// <param name="location">The location of the crab pot.</param>
    private static bool IsCorrectWeatherForThisFish(string[] specificFishData, GameLocation location)
    {
        if (specificFishData[7] == "both") return true;

        return specificFishData[7] == "rainy" && !Game1.IsRainingHere(location) ||
               specificFishData[7] == "sunny" && Game1.IsRainingHere(location);
    }

    /// <summary>Choose amongst a pre-select list of fish.</summary>
    /// <param name="crabpot">The crab pot instance.</param>
    /// <param name="fishData">Raw fish data from the game files.</param>
    /// <param name="rawFishDataWithLocation">Dictionary of pre-select fish and their fishing locations.</param>
    /// <param name="location">The location of the crab pot.</param>
    /// <param name="r">Random number generator.</param>
    private static int ChooseFish(CrabPot crabpot, Dictionary<int, string> fishData,
        Dictionary<string, string> rawFishDataWithLocation, GameLocation location, Random r)
    {
        var keys = rawFishDataWithLocation.Keys.ToArray();
        SUtility.Shuffle(r, keys);
        var counter = 0;
        foreach (var key in keys)
        {
            var specificFishDataFields = fishData[Convert.ToInt32(key)].Split('/');
            if (Objects.LegendaryFishNames.Contains(specificFishDataFields[0])) continue;

            var specificFishLocation = Convert.ToInt32(rawFishDataWithLocation[key]);
            if (!crabpot.HasMagicBait() &&
                (!IsCorrectLocationAndTimeForThisFish(specificFishDataFields, specificFishLocation,
                     crabpot.TileLocation, location) ||
                 !IsCorrectWeatherForThisFish(specificFishDataFields, location)))
                continue;

            if (r.NextDouble() > GetChanceForThisFish(specificFishDataFields)) continue;

            var whichFish = Convert.ToInt32(key);
            if (!whichFish.IsAnyOf(152, 152, 157)) return whichFish; // if isn't algae

            if (counter != 0) return -1; // if already rerolled
            ++counter;
        }

        return -1;
    }

    /// <summary>Get the chance of selecting a specific fish from the fish pool.</summary>
    /// <param name="specificFishData">Raw game file data for this fish.</param>
    private static double GetChanceForThisFish(string[] specificFishData)
    {
        return Convert.ToDouble(specificFishData[10]);
    }

    /// <summary>Choose amongst a pre-select list of shellfish.</summary>
    /// <param name="crabpot">The crab pot instance.</param>
    /// <param name="fishData">Raw fish data from the game files.</param>
    /// <param name="location">The location of the crab pot.</param>
    /// <param name="r">Random number generator.</param>
    /// <param name="isLuremaster">Whether the owner of the crab pot is luremaster.</param>
    private static int ChooseTrapFish(CrabPot crabpot, Dictionary<int, string> fishData, GameLocation location,
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

            if (r.NextDouble() < GetChanceForThisTrapFish(rawSplit)) return key;
        }

        if (isLuremaster && keys.Count > 0) return keys[r.Next(keys.Count)];

        return -1;
    }

    /// <summary>Get the chance of selecting a specific shellfish from the shellfish pool.</summary>
    /// <param name="rawSplit">Raw game file data for this shellfish.</param>
    private static double GetChanceForThisTrapFish(string[] rawSplit)
    {
        return Convert.ToDouble(rawSplit[2]);
    }

    /// <summary>Choose a treasure from the pirate treasure loot table.</summary>
    /// <param name="r">Random number generator.</param>
    /// <param name="who">The player.</param>
    private static int ChoosePirateTreasure(Random r, Farmer who)
    {
        var keys = Objects.TrapperPirateTreasureTable.Keys.ToArray();
        SUtility.Shuffle(r, keys);
        foreach (var key in keys)
        {
            if (key == 890 && !who.team.SpecialOrderRuleActive("DROP_QI_BEANS")) continue;

            if (r.NextDouble() < GetChanceForThisTreasure(key)) return key;
        }

        return -1;
    }

    /// <summary>Get the chance of selecting a specific pirate treasure from the pirate treasure table.</summary>
    /// <param name="index">The treasure item index.</param>
    private static double GetChanceForThisTreasure(int index)
    {
        return Convert.ToDouble(Objects.TrapperPirateTreasureTable[index][0]);
    }

    /// <summary>Get the quality for the chosen catch.</summary>
    /// <param name="whichFish">The chosen catch.</param>
    /// <param name="who">The owner of the crab pot.</param>
    /// <param name="r">Random number generator.</param>
    private static int GetTrapFishQuality(int whichFish, Farmer who, Random r, CrabPot crabpot, bool isLuremaster)
    {
        if (isLuremaster && crabpot.HasMagicBait()) return SObject.bestQuality;

        var fish = new SObject(whichFish, 1);
        if (!who.HasProfession("Trapper") || fish.IsPirateTreasure() || fish.IsAlgae()) return SObject.lowQuality;
        return who.HasPrestigedProfession("Trapper") && r.NextDouble() < who.FishingLevel / 60.0
            ?
            SObject.bestQuality
            :
            r.NextDouble() < who.FishingLevel / 30.0
                ? SObject.highQuality
                :
                r.NextDouble() < who.FishingLevel / 15.0
                    ? SObject.medQuality
                    : SObject.lowQuality;
    }

    /// <summary>Get initial stack for the chosen stack.</summary>
    /// <param name="crabpot">The crab pot instance.</param>
    /// <param name="whichFish">The chosen fish</param>
    /// <param name="who">The player.</param>
    /// <param name="r">Random number generator.</param>
    private static int GetTrapFishQuantity(CrabPot crabpot, int whichFish, Farmer who, Random r)
    {
        return crabpot.HasWildBait() && r.NextDouble() < 0.25 + who.DailyLuck / 2.0
            ? 2
            : Objects.TrapperPirateTreasureTable.TryGetValue(whichFish, out var treasureData)
                ? r.Next(Convert.ToInt32(treasureData[1]), Convert.ToInt32(treasureData[2]) + 1)
                : 1;
    }

    /// <summary>Get random trash.</summary>
    /// <param name="r">Random number generator.</param>
    private static int GetTrash(Random r)
    {
        return r.Next(168, 173);
    }

    #endregion private methods
}