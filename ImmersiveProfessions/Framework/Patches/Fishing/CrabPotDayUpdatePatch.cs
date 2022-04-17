namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

using Stardew.Common.Extensions;
using Extensions;

using ObjectLookups = Utility.ObjectLookups;
using SObject = StardewValley.Object;
using SUtility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
internal class CrabPotDayUpdatePatch : BasePatch
{
    private const double CHANCE_TO_CATCH_FISH_D = 0.25;

    /// <summary>Construct an instance.</summary>
    internal CrabPotDayUpdatePatch()
    {
        Original = RequireMethod<CrabPot>(nameof(CrabPot.DayUpdate));
    }

    #region harmony patches

    /// <summary>Patch for Trapper fish quality + Luremaster bait mechanics + Conservationist trash collection mechanics.</summary>
    [HarmonyPrefix]
    private static bool CrabPotDayUpdatePrefix(CrabPot __instance, GameLocation location)
    {
        try
        {
            var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
            var isConservationist = owner.HasProfession(Profession.Conservationist);
            if (__instance.bait.Value is null && !isConservationist || __instance.heldObject.Value is not null)
                return false; // don't run original logic

            var r = new Random(Guid.NewGuid().GetHashCode());
            var fishData =
                Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"));
            var isLuremaster = owner.HasProfession(Profession.Luremaster);
            var whichFish = -1;
            if (__instance.bait.Value is not null)
            {
                if (isLuremaster)
                {
                    if (__instance.HasMagnet())
                    {
                        whichFish = ChoosePirateTreasure(r, owner);
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
                if (__instance.bait.Value is not null || isConservationist)
                {
                    whichFish = GetTrash(__instance.TileLocation, location, r);
                    if (isConservationist && whichFish.IsTrash())
                    {
                        owner.IncrementData<uint>(DataField.ConservationistTrashCollectedThisSeason);
                        if (owner.HasProfession(Profession.Conservationist, true) &&
                            owner.ReadDataAs<uint>(DataField.ConservationistTrashCollectedThisSeason) %
                            ModEntry.Config.TrashNeededPerFriendshipPoint == 0)
                            SUtility.improveFriendshipWithEveryoneInRegion(owner, 1, 2);
                    }
                }
                else
                {
                    return false; // don't run original logic
                }
            }
            else
            {
                fishQuality = GetTrapFishQuality(whichFish, owner, r, __instance, isLuremaster);
            }

            var fishQuantity = GetTrapFishQuantity(__instance, whichFish, owner, r);
            __instance.heldObject.Value = new(whichFish, fishQuantity, quality: fishQuality);
            __instance.tileIndexToShow = 714;
            __instance.readyForHarvest.Value = true;

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
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
    /// <param name="tileLocation">The crab pot tile location.</param>
    /// <param name="location">The game location of the crab pot.</param>
    /// <remarks>The time portion is commented out because doesn't make sense for crab pots that only update once during the night.</remarks>
    private static bool IsCorrectLocationAndTimeForThisFish(string[] specificFishData, int specificFishLocation,
        Vector2 tileLocation, GameLocation location)
    {
        return specificFishLocation == -1 || specificFishLocation == location.getFishingLocation(tileLocation);
        
        //var specificFishSpawnTimes = specificFishData[5].Split(' ');
        //if (specificFishLocation == -1 || specificFishLocation == location.getFishingLocation(tileLocation))
        //    for (var t = 0; t < specificFishSpawnTimes.Length; t += 2)
        //        if (Game1.timeOfDay >= Convert.ToInt32(specificFishSpawnTimes[t]) &&
        //            Game1.timeOfDay < Convert.ToInt32(specificFishSpawnTimes[t + 1]))
        //            return true;

        //return false;
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
    /// <param name="location">The game location of the crab pot.</param>
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
            if (ObjectLookups.LegendaryFishNames.Contains(specificFishDataFields[0])) continue;

            var specificFishLocation = Convert.ToInt32(rawFishDataWithLocation[key]);
            if (!crabpot.HasMagicBait() &&
                (!IsCorrectLocationAndTimeForThisFish(specificFishDataFields, specificFishLocation,
                     crabpot.TileLocation, location) ||
                 !IsCorrectWeatherForThisFish(specificFishDataFields, location)))
                continue;

            if (r.NextDouble() > GetChanceForThisFish(specificFishDataFields)) continue;

            var whichFish = Convert.ToInt32(key);
            if (!whichFish.IsAlgae()) return whichFish; // if isn't algae

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
    /// <param name="location">The game location of the crab pot.</param>
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
    /// <param name="owner">The player.</param>
    private static int ChoosePirateTreasure(Random r, Farmer owner)
    {
        var keys = ObjectLookups.TrapperPirateTreasureTable.Keys.ToArray();
        SUtility.Shuffle(r, keys);
        foreach (var key in keys)
        {
            if (key == 890 && !owner.team.SpecialOrderRuleActive("DROP_QI_BEANS")) continue;

            if (r.NextDouble() > GetChanceForThisTreasure(key)) continue;

            return key;
        }

        return -1;
    }

    /// <summary>Get the chance of selecting a specific pirate treasure from the pirate treasure table.</summary>
    /// <param name="index">The treasure item index.</param>
    private static double GetChanceForThisTreasure(int index)
    {
        return Convert.ToDouble(ObjectLookups.TrapperPirateTreasureTable[index][0]);
    }

    /// <summary>Get the quality for the chosen catch.</summary>
    /// <param name="whichFish">The chosen catch.</param>
    /// <param name="owner">The owner of the crab pot.</param>
    /// <param name="r">Random number generator.</param>
    private static int GetTrapFishQuality(int whichFish, Farmer owner, Random r, CrabPot crabpot, bool isLuremaster)
    {
        if (isLuremaster && crabpot.HasMagicBait()) return SObject.bestQuality;

        var fish = new SObject(whichFish, 1);
        if (owner is null || !owner.HasProfession(Profession.Trapper) || fish.IsPirateTreasure() || fish.IsAlgae())
            return SObject.lowQuality;

        return owner.HasProfession(Profession.Trapper, true) && r.NextDouble() < owner.FishingLevel / 60.0
            ? SObject.bestQuality
            : r.NextDouble() < owner.FishingLevel / 30.0
                ? SObject.highQuality
                : r.NextDouble() < owner.FishingLevel / 15.0
                    ? SObject.medQuality
                    : SObject.lowQuality;
    }

    /// <summary>Get initial stack for the chosen stack.</summary>
    /// <param name="crabpot">The crab pot instance.</param>
    /// <param name="whichFish">The chosen fish</param>
    /// <param name="owner">The player.</param>
    /// <param name="r">Random number generator.</param>
    private static int GetTrapFishQuantity(CrabPot crabpot, int whichFish, Farmer owner, Random r)
    {
        if (owner is null) return 1;

        return crabpot.HasWildBait() && r.NextDouble() < 0.25 + owner.DailyLuck / 2.0
            ? 2
            : ObjectLookups.TrapperPirateTreasureTable.TryGetValue(whichFish, out var treasureData)
                ? r.Next(Convert.ToInt32(treasureData[1]), Convert.ToInt32(treasureData[2]) + 1)
                : 1;
    }

    /// <summary>Get random trash.</summary>
    /// <param name="r">Random number generator.</param>
    /// <param name="tileLocation">The crab pot tile location.</param>
    /// <param name="location">The game location of the crab pot.</param>
    private static int GetTrash(Vector2 tileLocation, GameLocation location, Random r)
    {
        if (r.NextDouble() > 1.0 / 3.0) return r.Next(167, 173);

        int trash;
        switch (location)
        {
            case Beach:
            case IslandSouth:
            case IslandWest when location.getFishingLocation(tileLocation) == 1:
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

    #endregion private methods
}