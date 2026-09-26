namespace DaLion.Professions.Framework;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.DayEnding;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Locations;
using StardewValley.Internal;
using StardewValley.Tools;

#endregion using directives

internal static class FishDiversityManager
{
    private static readonly Dictionary<(string Location, string? FishAreaId), Dictionary<string, double>> _cache = [];

    private static readonly Dictionary<(string Location, string? FishAreaId), Dictionary<string, double>> _magicCache = [];

    internal static double GetOrEvaluate(Item fish, Farmer player)
    {
        if (!player.IsLocalPlayer || player.CurrentTool is not FishingRod rod)
        {
            return 1d;
        }

        var location = Game1.currentLocation;
        var fishId = fish.ItemId;
        var usingMagicBait = rod.HasMagicBait();
        return !TryGetNearestWaterTile(player, location, out var nearestWaterTile)
            ? 1d
            : GetOrEvaluate(fishId, player, location, nearestWaterTile, usingMagicBait);
    }

    internal static double GetOrEvaluate(string fishId, Farmer player, GameLocation location, Vector2 bobberTile, bool usingMagicBait)
    {
        var locationName = location.Name;
        var locationCache = usingMagicBait ? _magicCache : _cache;
        if (!location.TryGetFishAreaForTile(bobberTile, out var fishAreaId, out var _))
        {
            fishAreaId = null;
        }

        if (_cache.TryGetValue((locationName, fishAreaId), out var cachedValues))
        {
            if (cachedValues.TryGetValue(fishId, out var chance))
            {
                return chance;
            }
            else if (cachedValues.Count > 0)
            {
                return 1d;
            }
        }

        return Evaluate(player, location, bobberTile, fishAreaId, usingMagicBait, fishId);
    }

    internal static void ClearCache()
    {
        _cache.Clear();
        _magicCache.Clear();
    }

    private static bool TryGetNearestWaterTile(Farmer player, GameLocation location, out Vector2 nearestWaterTile)
    {
        nearestWaterTile = new Vector2(99999f, 99999f);
        var foundWater = false;
        if (!location.canFishHere())
        {
            return false;
        }

        var x = (int)Game1.currentCursorTile.X;
        var y = (int)Game1.currentCursorTile.Y;
        if (!location.isTileBuildingFishable(x, y) && location.isTileFishable(x, y))
        {
            nearestWaterTile = new Vector2(x, y);
            foundWater = true;
        }

        if (!foundWater)
        {
            var maxDistance = player.GetAddedFishingDistance() + 4;
            var direction = player.FacingDirection;
            var horizontally = direction == Game1.right || direction == Game1.left;
            var positive = direction > 1;
            if (!horizontally)
            {
                maxDistance--;
            }

            for (var i = maxDistance; i >= 0; i--)
            {
                var x2 = (int)player.Tile.X;
                var y2 = (int)player.Tile.Y;
                if (!horizontally)
                {
                    y2 = positive ? (y2 + i) : (y2 - i);
                }
                else
                {
                    x2 = positive ? (x2 - i) : (x2 + i);
                }

                if (!location.isTileBuildingFishable(x2, y2) && location.isTileFishable(x2, y2))
                {
                    nearestWaterTile = new Vector2(x2, y2);
                    foundWater = true;
                    break;
                }
            }
        }

        if (!foundWater)
        {
            var scanRadius = 10;
            var scanTopLeft = player.Tile - new Vector2(scanRadius + 1);
            var scanBottomRight = player.Tile + new Vector2(scanRadius + 2);
            for (var j = (int)scanTopLeft.X; j < (int)scanBottomRight.X; j++)
            {
                for (var k = (int)scanTopLeft.Y; k < (int)scanBottomRight.Y; k++)
                {
                    if (!location.isTileBuildingFishable(j, k) && location.isTileFishable(j, k))
                    {
                        var tile = new Vector2(j, k);
                        var distance = Vector2.DistanceSquared(player.Tile, tile);
                        var distanceNearest = Vector2.DistanceSquared(player.Tile, nearestWaterTile);
                        if (distance < distanceNearest || (distance == distanceNearest && player.GetGrabTile() == tile))
                        {
                            nearestWaterTile = tile;
                        }

                        foundWater = true;
                    }
                }
            }
        }

        return foundWater;
    }

    private static double Evaluate(Farmer player, GameLocation location, Vector2 bobberTile, string? fishAreaId, bool usingMagicBait, string fishId)
    {
        Dictionary<string, double> cache = [];
        var data = GetFishFromLocationData(location.Name, bobberTile, fishAreaId, 5, player, false, location, null, usingMagicBait);
        var total = data
            .Where(f => ItemRegistry.GetDataOrErrorItem(f.Key).Category != SObject.junkCategory)
            .Sum(f => f.Value);
        var count = data
            .Count(f => ItemRegistry.GetDataOrErrorItem(f.Key).Category != SObject.junkCategory);
        var biodiversityFactor = Data.ReadAs<double>(Game1.MasterPlayer, DataKeys.GlobalBiodiversityFactor);
        var alpha = 1d - biodiversityFactor;
        foreach (var entry in data)
        {
            var item = ItemRegistry.GetDataOrErrorItem(entry.Key);
            if (item.Category == SObject.junkCategory)
            {
                cache[item.ItemId] = alpha;
                continue;
            }

            var adjustedChance = (entry.Value * alpha) + (biodiversityFactor * ((double)total / count));
            cache[item.ItemId] = entry.Value > 0f ? adjustedChance / entry.Value : 1f;
            if (entry.Value <= 0f)
            {
                Log.W($"Found imnpossible fish {entry.Key} with zero spawn chance!");
            }
        }

        if (usingMagicBait)
        {
            _magicCache[(location.Name, fishAreaId)] = cache;
        }
        else
        {
            _cache[(location.Name, fishAreaId)] = cache;
        }

        ProfessionsMod.Events.Enable<FishChanceDistributionDayEndingEvent>();
        return cache.TryGetValue(fishId, out var chance) ? chance : 1d;
    }

    private static Dictionary<string, float> GetFishFromLocationData(
        string locationName,
        Vector2 bobberTile,
        string? fishAreaId,
        int waterDepth,
        Farmer player,
        bool isInherited,
        GameLocation location,
        ItemQueryContext? itemQueryContext,
        bool usingMagicBait = false)
    {
        Dictionary<string, float> passed = [];
        location ??= Game1.getLocationFromName(locationName);
        if (location is null)
        {
            return [];
        }

        var locationData = location.GetData();
        var allFishData = DataLoader.Fish(Game1.content);
        var season = Game1.GetSeasonForLocation(location);
        player ??= Game1.player;
        var playerTile = player.TilePoint;
        itemQueryContext ??= new ItemQueryContext(location, null, Game1.random, "location '" + locationName + "' > fish data");
        IEnumerable<SpawnFishData> possibleFish = Game1.locationData["Default"].Fish;
        if (locationData is not null && locationData.Fish?.Count > 0)
        {
            possibleFish = possibleFish.Concat(locationData.Fish);
        }

        possibleFish = from p in possibleFish
                       orderby p.Precedence
                       select p;
        var ignoreQueryKeys = usingMagicBait ? GameStateQuery.MagicBaitIgnoreQueryKeys : ["TIME"];
        Item? firstNonTargetFish = null;
        foreach (var spawn in possibleFish)
        {
            if ((isInherited && !spawn.CanBeInherited) ||
                (spawn.FishAreaId != null && fishAreaId != spawn.FishAreaId) ||
                (spawn.Season.HasValue && !usingMagicBait && spawn.Season != season))
            {
                continue;
            }

            var playerPosition = spawn.PlayerPosition;
            if (playerPosition.HasValue && !playerPosition.GetValueOrDefault().Contains(playerTile.X, playerTile.Y))
            {
                continue;
            }

            if (player.FishingLevel < spawn.MinFishingLevel ||
                waterDepth < spawn.MinDistanceFromShore ||
                (spawn.MaxDistanceFromShore > -1 && waterDepth > spawn.MaxDistanceFromShore) ||
                (spawn.RequireMagicBait && !usingMagicBait))
            {
                continue;
            }

            var chance = spawn.GetChance(
                false,
                player.DailyLuck,
                player.LuckLevel,
                (float value, IList<QuantityModifier> modifiers, QuantityModifier.QuantityModifierMode mode) =>
                    Utility.ApplyQuantityModifiers(value, modifiers, mode, location),
                false);
            if (spawn.UseFishCaughtSeededRandom)
            {
                chance = 0.25f;
            }

            if (spawn.Condition is not null && !GameStateQuery.CheckConditions(spawn.Condition, location, null, null, null, null, ignoreQueryKeys))
            {
                continue;
            }

            List<string> queries = [];
            if (spawn.RandomItemId is not null && spawn.RandomItemId.Count > 0)
            {
                queries = spawn.RandomItemId;
            }
            else if (spawn.ItemId is not null)
            {
                queries.Add(spawn.ItemId);
            }

            foreach (var q in queries)
            {
                var query = q;
                if (!q.StartsWith('('))
                {
                    query = q
                        .Replace("BOBBER_X", ((int)bobberTile.X).ToString())
                        .Replace("BOBBER_Y", ((int)bobberTile.Y).ToString())
                        .Replace("WATER_DEPTH", waterDepth.ToString());
                }

                var attempt = 0;
                var items = ItemQueryResolver.TryResolve(
                    query,
                    itemQueryContext,
                    ItemQuerySearchMode.All,
                    spawn.PerItemCondition,
                    spawn.MaxItems,
                    avoidRepeat: true);
                while (items.Length == 0 && attempt < 10)
                {
                    items = ItemQueryResolver.TryResolve(
                        query,
                        itemQueryContext,
                        ItemQuerySearchMode.All,
                        spawn.PerItemCondition,
                        spawn.MaxItems,
                        avoidRepeat: true);
                    attempt++;
                }

                var array = items;
                foreach (var item in array)
                {
                    if (item.Item is not Item fish || passed.ContainsKey(fish.QualifiedItemId))
                    {
                        continue;
                    }

                    if (fish.Category == SObject.junkCategory)
                    {
                        fish.ItemId = "168"; // trash
                    }

                    if (!string.IsNullOrWhiteSpace(spawn.SetFlagOnCatch))
                    {
                        fish.SetFlagOnPickup = spawn.SetFlagOnCatch;
                    }

                    if (spawn.IsBossFish)
                    {
                        fish.SetTempData("IsBossFish", value: true);
                    }

                    if (spawn.CatchLimit > -1 && player.fishCaught.TryGetValue(fish.QualifiedItemId, out var values) &&
                        values[0] >= spawn.CatchLimit)
                    {
                        continue;
                    }

                    var c = CheckGenericFishRequirements(
                        fish,
                        allFishData,
                        location,
                        player,
                        spawn,
                        waterDepth,
                        usingMagicBait);
                    if (c > 0f)
                    {
                        passed[fish.QualifiedItemId] = chance * c;
                        firstNonTargetFish ??= fish;
                    }
                }
            }
        }

        return passed;
    }

    /// <summary>Gets a fish's spawn chance based on its requirements in Data/Fish, if applicable.</summary>
    /// <param name="fish">The fish being checked.</param>
    /// <param name="allFishData">The Data/Fish data to check.</param>
    /// <param name="location">The location for which fish are being caught.</param>
    /// <param name="player">The player catching fish.</param>
    /// <param name="spawn">The fish spawn rule for which a fish is being checked.</param>
    /// <param name="waterDepth">The current water depth for the fishing bobber.</param>
    /// <param name="usingMagicBait">Whether the player has the magic bait equipped.</param>
    /// <returns>The chance to spawn the <paramref name="fish"/>.</returns>
    private static float CheckGenericFishRequirements(
        Item fish,
        Dictionary<string, string> allFishData,
        GameLocation location,
        Farmer player,
        SpawnFishData spawn,
        int waterDepth,
        bool usingMagicBait)
    {
        if (!fish.HasTypeObject() || !allFishData.TryGetValue(fish.ItemId, out var rawSpecificFishData))
        {
            return 1f;
        }

        var specificFishData = rawSpecificFishData.Split('/');
        if (ArgUtility.Get(specificFishData, 1) == "trap")
        {
            return 1f;
        }

        player ??= Game1.player;
        string error;
        if (!spawn.IgnoreFishDataRequirements)
        {
            if (!usingMagicBait)
            {
                if (!ArgUtility.TryGet(specificFishData, 5, out var rawTimeSpans, out error, allowBlank: true, "string rawTimeSpans"))
                {
                    return 0f;
                }

                var timeSpans = ArgUtility.SplitBySpace(rawTimeSpans);
                var found = false;
                for (var i = 0; i < timeSpans.Length; i += 2)
                {
                    if (!ArgUtility.TryGetInt(timeSpans, i, out var startTime, out error, "int startTime") ||
                        !ArgUtility.TryGetInt(timeSpans, i + 1, out var endTime, out error, "int endTime"))
                    {
                        return 0f;
                    }

                    if (Game1.timeOfDay >= startTime && Game1.timeOfDay < endTime)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return 0f;
                }
            }

            if (!usingMagicBait)
            {
                if (!ArgUtility.TryGet(specificFishData, 7, out var weather, out error, allowBlank: true, "string weather"))
                {
                    return 0f;
                }

                if (!(weather == "rainy"))
                {
                    if (weather == "sunny" && location.IsRainingHere())
                    {
                        return 0f;
                    }
                }
                else if (!location.IsRainingHere())
                {
                    return 0f;
                }
            }

            if (!ArgUtility.TryGetInt(specificFishData, 12, out var minFishingLevel, out error, "int minFishingLevel"))
            {
                return 0f;
            }

            if (player.FishingLevel < minFishingLevel)
            {
                return 0f;
            }

            if (!ArgUtility.TryGetInt(specificFishData, 9, out var maxDepth, out error, "int maxDepth") ||
                !ArgUtility.TryGetFloat(specificFishData, 10, out var chance, out error, "float chance") ||
                !ArgUtility.TryGetFloat(specificFishData, 11, out var depthMultiplier, out error, "float depthMultiplier"))
            {
                return 0f;
            }

            var dropOffAmount = depthMultiplier * chance;
            chance -= Math.Max(0, maxDepth - waterDepth) * dropOffAmount;
            chance += player.FishingLevel / 50f;
            chance = Math.Min(chance, 0.9f);
            if (spawn.ApplyDailyLuck)
            {
                chance += (float)player.DailyLuck;
            }

            var chanceModifiers = spawn.ChanceModifiers;
            if (chanceModifiers != null && chanceModifiers.Count > 0)
            {
                chance = Utility.ApplyQuantityModifiers(chance, spawn.ChanceModifiers, spawn.ChanceModifierMode, location);
            }

            return chance;
        }

        return 1f;
    }
}
