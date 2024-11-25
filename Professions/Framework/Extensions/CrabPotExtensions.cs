namespace DaLion.Professions.Framework.Extensions;

#region using directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.GameData.Locations;
using StardewValley.Internal;
using StardewValley.Locations;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="CrabPot"/> class.</summary>
internal static class CrabPotExtensions
{
    private static CheckGenericFishRequirementsDelegate _checkGenericFishRequirements =
        Reflector.GetStaticMethodDelegate<CheckGenericFishRequirementsDelegate>(
            typeof(GameLocation),
            "CheckGenericFishRequirements");

    private delegate bool CheckGenericFishRequirementsDelegate(
        Item fish,
        Dictionary<string, string> allFishData,
        GameLocation location,
        Farmer player,
        SpawnFishData spawn,
        int waterDepth,
        bool usingMagicBait,
        bool hasCuriosityLure,
        bool usingTargetBait,
        bool isTutorialCatch);

    /// <summary>Gets the treasure items that can be trapped by magnet bait.</summary>
    internal static ImmutableDictionary<string, string[]> TrapperPirateTreasureTable { get; } =
        new Dictionary<string, string[]>
        {
            { "(W)14", new[] { "0.003", "1", "1" } }, // neptune's glaive
            { "(W)51", new[] { "0.003", "1", "1" } }, // broken trident
            { "(O)166", new[] { "0.03", "1", "1" } }, // treasure chest
            { "(O)109", new[] { "0.009", "1", "1" } }, // ancient sword
            { "(O)110", new[] { "0.009", "1", "1" } }, // rusty spoon
            { "(O)111", new[] { "0.009", "1", "1" } }, // rusty spur
            { "(O)112", new[] { "0.009", "1", "1" } }, // rusty cog
            { "(O)117", new[] { "0.009", "1", "1" } }, // anchor
            { "(O)378", new[] { "1.0", "1", "24" } }, // copper ore
            { "(O)380", new[] { "0.48", "1", "24" } }, // iron ore
            { "(O)384", new[] { "0.24", "1", "24" } }, // gold ore
            { "(O)386", new[] { "0.12", "1", "2" } }, // iridium ore
            { "(R)516", new[] { "0.02", "1", "1" } }, // small glow ring
            { "(R)517", new[] { "0.009", "1", "1" } }, // glow ring
            { "(R)518", new[] { "0.02", "1", "1" } }, // small magnet ring
            { "(R)519", new[] { "0.009", "1", "1" } }, // magnet ring
            { "(R)527", new[] { "0.005", "1", "1" } }, // iridium band
            { "(R)529", new[] { "0.005", "1", "1" } }, // amethyst ring
            { "(R)530", new[] { "0.005", "1", "1" } }, // topaz ring
            { "(R)531", new[] { "0.005", "1", "1" } }, // aquamarine ring
            { "(R)532", new[] { "0.005", "1", "1" } }, // jade ring
            { "(R)533", new[] { "0.005", "1", "1" } }, // emerald ring
            { "(R)534", new[] { "0.005", "1", "1" } }, // ruby ring
            { "(O)890", new[] { "0.03", "1", "3" } }, // qi bean
        }.ToImmutableDictionary();

    /// <summary>Determines whether the <paramref name="crabPot"/> is using magnet as bait.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="crabPot"/>'s bait value is the index of Magnet, otherwise <see langword="false"/>.</returns>
    internal static bool HasMagnet(this CrabPot crabPot)
    {
        return crabPot.bait.Value?.QualifiedItemId == QualifiedObjectIds.MagnetBait;
    }

    /// <summary>Determines whether the <paramref name="crabPot"/> is using wild bait.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="crabPot"/>'s bait value is the index of Wild Bait, otherwise <see langword="false"/>.</returns>
    internal static bool HasWildBait(this CrabPot crabPot)
    {
        return crabPot.bait.Value?.QualifiedItemId == QualifiedObjectIds.WildBait;
    }

    /// <summary>Determines whether the <paramref name="crabPot"/> is using magic bait.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="crabPot"/>'s bait value is the index of Magic Bait, otherwise <see langword="false"/>.</returns>
    internal static bool HasMagicBait(this CrabPot crabPot)
    {
        return crabPot.bait.Value?.QualifiedItemId == QualifiedObjectIds.MagicBait;
    }

    /// <summary>Determines whether the <paramref name="crabPot"/> is using specific fish bait.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="crabPot"/>'s bait value is the index of Specific Bait and the bait contains fish metadata, otherwise <see langword="false"/>.</returns>
    internal static bool HasSpecificBait(this CrabPot crabPot)
    {
        return crabPot.bait.Value is { QualifiedItemId: "(O)SpecificBait", preservedParentSheetIndex.Value: not null };
    }

    /// <summary>Determines whether the <paramref name="crabPot"/> should catch ocean-specific shellfish.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="location">The <see cref="GameLocation"/> of the <paramref name="crabPot"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="crabPot"/> is placed near ocean, otherwise <see langword="false"/>.</returns>
    internal static bool ShouldCatchOceanFish(this CrabPot crabPot, GameLocation location)
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
    internal static bool HasSpecialLuremasterCatch(this CrabPot crabPot)
    {
        return crabPot.heldObject.Value is { } obj && ((obj.IsFish() && !obj.IsTrapFish()) || obj.IsAlgae() ||
                                                       TrapperPirateTreasureTable.ContainsKey(obj.QualifiedItemId));
    }

    /// <summary>Choose the ID of a random fish appropriate for the current location and conditions.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="owner">The <see cref="Farmer"/> who owns the instance.</param>
    /// <param name="r"><see cref="Random"/> number generator.</param>
    /// <returns>The ID of a random fish appropriate for the current location and conditions.</returns>
    /// <remarks>Based on <see cref="GameLocation.GetFishFromLocationData"/>.</remarks>
    internal static string ChooseFish(this CrabPot crabPot, Farmer? owner = null, Random? r = null)
    {
        owner ??= crabPot.GetOwner();
        r ??= Game1.random;
        var location = crabPot.Location;
        var itemQueryContext = new ItemQueryContext(location, owner, r, null);
        var dictionary = DataLoader.Locations(Game1.content);
        var locationData = location.GetData();
        var allFishData = DataLoader.Fish(Game1.content);
        var season = Game1.GetSeasonForLocation(location);
        var bobberTile = crabPot.TileLocation;
        if (!location.TryGetFishAreaForTile(bobberTile, out var fishAreaId, out var _))
        {
            fishAreaId = null;
        }

        var usingMagicBait = crabPot.HasMagicBait();
        string? baitTargetFish = null;
        IEnumerable<SpawnFishData> possibleFish = dictionary["Default"].Fish;
        if (locationData is not null && locationData.Fish?.Count > 0)
        {
            possibleFish = possibleFish.Concat(locationData.Fish);
        }

        possibleFish = from p in possibleFish
            orderby p.Precedence, r.Next()
            select p;
        var targetedBaitTries = 0;
        var ignoreQueryKeys =
            usingMagicBait
                ? GameStateQuery.MagicBaitIgnoreQueryKeys
                : ["TIME"];
        Item? firstNonTargetFish = null;
        Item? fish = null;
        for (var i = 0; i < 2; i++)
        {
            foreach (var spawn in possibleFish)
            {
                if (spawn.IsBossFish || (spawn.FishAreaId != null && fishAreaId != spawn.FishAreaId) ||
                    (spawn.Season.HasValue && !usingMagicBait && spawn.Season != season))
                {
                    continue;
                }

                var chance = spawn.GetChance(
                    false,
                    owner.DailyLuck,
                    owner.LuckLevel,
                    (value, modifiers, mode) => Utility.ApplyQuantityModifiers(value, modifiers, mode, location));
                if (spawn.UseFishCaughtSeededRandom)
                {
                    if (!Utility.CreateRandom(Game1.uniqueIDForThisGame, owner.stats.Get("PreciseFishCaught") * 859)
                            .NextBool(chance))
                    {
                        continue;
                    }
                }
                else if (!r.NextBool(chance))
                {
                    continue;
                }

                if (spawn.Condition is not null && !GameStateQuery.CheckConditions(
                        spawn.Condition,
                        location,
                        null,
                        null,
                        null,
                        null,
                        ignoreQueryKeys))
                {
                    continue;
                }

                var item = ItemQueryResolver.TryResolveRandomItem(
                    spawn,
                    itemQueryContext,
                    false,
                    null,
                    query => query
                        .Replace("BOBBER_X", ((int)bobberTile.X).ToString())
                        .Replace("BOBBER_Y", ((int)bobberTile.Y).ToString())
                        .Replace("WATER_DEPTH", "1"),
                    null,
                    delegate(string query, string error)
                    {
                        Log.E(
                            $"Location '{location.NameOrUniqueName}' failed parsing item query '{query}' for fish '{spawn.Id}': {error}");
                    });
                if (item is null || item.TypeDefinitionId != "(O)")
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(spawn.SetFlagOnCatch))
                {
                    item.SetFlagOnPickup = spawn.SetFlagOnCatch;
                }

                fish = item;
                var belowCatchLimit = spawn.CatchLimit <= -1 ||
                                      !owner.fishCaught.TryGetValue(fish.QualifiedItemId, out var values) ||
                                      values[0] < spawn.CatchLimit;
                var meetsFishRequirements = _checkGenericFishRequirements(
                    fish,
                    allFishData,
                    location,
                    owner,
                    spawn,
                    1,
                    usingMagicBait,
                    false,
                    spawn.ItemId == baitTargetFish,
                    false);
                if (!belowCatchLimit || !meetsFishRequirements)
                {
                    continue;
                }

                if (baitTargetFish is null || fish.QualifiedItemId == baitTargetFish || targetedBaitTries >= 2)
                {
                    return fish.QualifiedItemId;
                }

                firstNonTargetFish ??= fish;
                targetedBaitTries++;
            }
        }

        return fish?.IsAlgae() == false ? fish.QualifiedItemId : string.Empty;
    }

    /// <summary>Chooses the ID of a random trap fish appropriate for the current location and conditions..</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="isLuremaster">Whether the <paramref name="owner"/>> has the <see cref="Profession.Luremaster"/>> profession.</param>
    /// <param name="owner">The <see cref="Farmer"/> who owns the instance.</param>
    /// <param name="r"><see cref="Random"/> number generator.</param>
    /// <returns>The ID of a random trap fish appropriate for the current location and conditions.</returns>
    internal static string ChooseTrapFish(this CrabPot crabPot, bool isLuremaster = false, Farmer? owner = null, Random? r = null)
    {
        owner ??= crabPot.GetOwner();
        r ??= Game1.random;
        var location = crabPot.Location;
        var fishData = DataLoader.Fish(Game1.content);
        if (!location.TryGetFishAreaForTile(crabPot.TileLocation, out var _, out var fishArea))
        {
            fishArea = null;
        }

        var chanceForJunk = isLuremaster ? 0d : (double?)fishArea?.CrabPotJunkChance ?? 0.2;
        var bait = crabPot.bait.Value;
        string? baitTargetFish = null;
        if (bait is not null)
        {
            if (bait.QualifiedItemId is QualifiedObjectIds.DeluxeBait or QualifiedObjectIds.WildBait)
            {
                chanceForJunk /= 2d;
            }
            else if (bait.preservedParentSheetIndex is not null && bait.preserve.Value.HasValue)
            {
                baitTargetFish = bait.preservedParentSheetIndex.Value;
                chanceForJunk /= 2.0;
            }
        }

        if (r.NextBool(chanceForJunk))
        {
            return string.Empty;
        }

        List<string> keys = [];
        var targetAreas = location.GetCrabPotFishForTile(crabPot.TileLocation);
        foreach (var (key, value) in fishData)
        {
            if (!value.Contains("trap"))
            {
                continue;
            }

            var rawSplit = value.SplitWithoutAllocation('/');
            var areaSplit = ArgUtility.SplitBySpace(rawSplit[4].ToString());
            var found = false;
            foreach (var crabPotArea in areaSplit)
            {
                foreach (var targetArea in targetAreas)
                {
                    if (crabPotArea != targetArea)
                    {
                        continue;
                    }

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                continue;
            }

            if (isLuremaster)
            {
                keys.Add(key);
                continue;
            }

            var chanceForCatch = Convert.ToDouble(rawSplit[2].ToString());
            if (baitTargetFish is not null && baitTargetFish == key)
            {
                chanceForCatch *= chanceForCatch < 0.1 ? 4d : chanceForCatch < 0.2 ? 3d : 2d;
            }

            if (r.NextBool(chanceForCatch))
            {
                return "(O)" + key;
            }
        }

        return isLuremaster && keys.Count > 0 ? "(O)" + keys.Choose(r) : string.Empty;
    }

    /// <summary>Chooses a random treasure index from the pirate treasure loot table.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="owner">The <see cref="Farmer"/> who owns the instance.</param>
    /// <param name="r"><see cref="Random"/> number generator.</param>
    /// <returns>The index of a random treasure <see cref="Item"/>.</returns>
    internal static string ChoosePirateTreasure(this CrabPot crabPot, Farmer? owner = null, Random? r = null)
    {
        owner ??= crabPot.GetOwner();
        r ??= Game1.random;
        var keys = TrapperPirateTreasureTable.Keys.ToArray();
        Utility.Shuffle(r, keys);
        foreach (var key in keys)
        {
            if ((key == QualifiedWeaponIds.BrokenTrident && owner.specialItems.Contains(QualifiedWeaponIds.BrokenTrident)) ||
                (key == QualifiedWeaponIds.NeptuneGlaive && owner.specialItems.Contains(QualifiedWeaponIds.NeptuneGlaive)) ||
                (key == QualifiedObjectIds.QiBean && !owner.team.SpecialOrderRuleActive("DROP_QI_BEANS")) ||
                (key == QualifiedObjectIds.IridiumOre && !owner.hasSkullKey))
            {
                continue;
            }

            if (r.NextDouble() > Convert.ToDouble(TrapperPirateTreasureTable[key][0]))
            {
                continue;
            }

            return key;
        }

        return string.Empty;
    }

    /// <summary>Gets the quality for the chosen <paramref name="trap"/> fish.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="trap">The chosen trap fish.</param>
    /// <param name="isLuremaster">Whether the <paramref name="owner"/>> has the <see cref="Profession.Luremaster"/>> profession.</param>
    /// <param name="owner">The <see cref="Farmer"/> who owns the instance.</param>
    /// <param name="r"><see cref="Random"/> number generator.</param>
    /// <returns>A <see cref="SObject"/> quality value.</returns>
    internal static ObjectQuality GetTrapQuality(this CrabPot crabPot, string trap, bool isLuremaster = false, Farmer? owner = null, Random? r = null)
    {
        owner ??= crabPot.GetOwner();
        r ??= Game1.random;
        var fish = ItemRegistry.Create<SObject>(trap);
        if (TrapperPirateTreasureTable.ContainsKey(fish.QualifiedItemId) || fish.IsAlgae())
        {
            return SObject.lowQuality;
        }

        var quality = ObjectQuality.Regular;
        if (owner.HasProfession(Profession.Trapper))
        {
            if (r.NextDouble() < owner.FishingLevel / 30d)
            {
                quality = ObjectQuality.Gold;
            }
            else if (r.NextDouble() < owner.FishingLevel / 15d)
            {
                quality = ObjectQuality.Silver;
            }

            if (owner.HasProfession(Profession.Trapper, true))
            {
                quality = quality.Increment();
            }
        }

        if (crabPot.bait.Value is { QualifiedItemId: QualifiedObjectIds.DeluxeBait })
        {
            quality = quality.Increment();
            if (isLuremaster)
            {
                quality = quality.Increment();
            }
        }

        return quality;
    }

    /// <summary>Gets initial stack for the chosen <paramref name="trap"/> fish.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="trap">The chosen trap fish.</param>
    /// <param name="isLuremaster">Whether the <paramref name="owner"/>> has the <see cref="Profession.Luremaster"/>> profession.</param>
    /// <param name="isSpecialOceanographerCondition">Whether to apply special Oceanographer conditions.</param>
    /// <param name="owner">The <see cref="Farmer"/> who owns the instance.</param>
    /// <param name="r"><see cref="Random"/> number generator.</param>
    /// <returns>The stack value.</returns>
    internal static int GetTrapQuantity(this CrabPot crabPot, string trap, bool isLuremaster = false, bool isSpecialOceanographerCondition = false, Farmer? owner = null, Random? r = null)
    {
        owner ??= crabPot.GetOwner();
        r ??= Game1.random;
        return isSpecialOceanographerCondition
            ? r.Next(20)
            : crabPot.HasWildBait() && isLuremaster && r.NextBool(0.25 + (owner.DailyLuck / 2d))
                ? 2
                : TrapperPirateTreasureTable.TryGetValue(trap, out var treasureData)
                    ? r.Next(Convert.ToInt32(treasureData[1]), Convert.ToInt32(treasureData[2]) + 1)
                    : 1;
    }

    /// <summary>Chooses the ID of a random trash item.</summary>
    /// <param name="crabPot">The <see cref="CrabPot"/>.</param>
    /// <param name="r"><see cref="Random"/> number generator.</param>
    /// <returns>The index of a random trash <see cref="Item"/>.</returns>
    internal static string GetTrash(this CrabPot crabPot, Random? r = null)
    {
        r ??= Game1.random;
        var location = crabPot.Location;
        if (r.NextDouble() > 0.5)
        {
            return "(O)" + r.Next(167, 173);
        }

        string trash;
        switch (location)
        {
            case Beach:
            case BeachNightMarket:
            case IslandSouth:
            case IslandWest:
            case Farm when Game1.whichFarm == Farm.beach_layout:
                trash = QualifiedObjectIds.Seaweed;
                break;
            case MineShaft:
            case Sewer:
            case BugLand:
                trash = r.Next(2) == 0 ? QualifiedObjectIds.GreenAlgae : QualifiedObjectIds.WhiteAlgae;
                break;
            default:
                trash = location.Name == "WitchSwamp"
                    ? r.Next(2) == 0 ? QualifiedObjectIds.GreenAlgae : QualifiedObjectIds.WhiteAlgae
                    : QualifiedObjectIds.GreenAlgae;
                break;
        }

        return trash;
    }
}
