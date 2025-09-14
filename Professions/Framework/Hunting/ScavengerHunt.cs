namespace DaLion.Professions.Framework.Hunting;

#region using directives

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DaLion.Professions.Framework.Buffs;
using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

#endregion using directives

/// <summary>Manages treasure hunt events for Scavenger professions.</summary>
internal sealed class ScavengerHunt : TreasureHunt
{
    private const int BASE_TRIGGER_POOL_THRESHOLD = 3000;
    private const int MIN_DISTANCE_TO_TREASURE = 12;
    private const int POINTS_PER_STEP = 1;
    private const int POINTS_PER_ITEM_FORAGED = 50;
    private const int POINTS_PER_TREES_CHOPPED = 100;

    private readonly ConcurrentDictionary<string, List<(Vector2 Tile, bool Diggable)>> _eligibleTreasureHuntTilesByMap = [];
    private readonly ConcurrentDictionary<string, Task> _treasureTileCacheTaskByMap = [];
    private readonly string[] _artifactsThatCanBeFound =
    [
        QIDs.ChippedAmphora, // chipped amphora
        QIDs.Arrowhead, // arrowhead
        QIDs.AncientDoll, // ancient doll
        QIDs.ElvishJewelry, // elvish jewelry
        QIDs.ChewingStick, // chewing stick
        QIDs.OrnamentalFan, // ornamental fan
        QIDs.AncientSword, // ancient sword
        QIDs.PrehistoricTool, // prehistoric tool
        QIDs.GlassShards, // glass shards
        QIDs.BoneFlute, // bone flute
        QIDs.PrehistoricHandaxe, // prehistoric hand-axe
        QIDs.AncientDrum, // ancient drum
        QIDs.GoldenMask, // golden mask
        QIDs.GoldenRelic, // golden relic
        QIDs.StrangeDoll0, // strange doll
        QIDs.StrangeDoll1, // strange doll
    ];

    /// <summary>Initializes a new instance of the <see cref="ScavengerHunt"/> class.</summary>
    internal ScavengerHunt()
        : base(
            TreasureHuntProfession.Scavenger,
            I18n.Scavenger_HuntStarted(),
            I18n.Scavenger_HuntFailed(),
            new Rectangle(80, 656, 16, 16))
    {
        this.TriggerPool = Data.ReadAs<int>(Game1.player, DataKeys.ScavengerPointPool);
    }

    /// <inheritdoc />
    public override int TimeLimit => Config.ScavengerHuntTimeLimit;

    /// <inheritdoc />
    public override int TriggerPool { get; protected set; }

    /// <inheritdoc />
    protected override int TriggerThreshold => (int)(BASE_TRIGGER_POOL_THRESHOLD * Config.TreasureHuntTriggerModifier);

    /// <inheritdoc />
    public override void Complete()
    {
        if (this.TargetTile is null || this.Location is null)
        {
            this.End(false);
            return;
        }

        if (!this.Location.terrainFeatures.TryGetValue(this.TargetTile.Value, out var feature) ||
            feature is not HoeDirt)
        {
            return;
        }

        DelayedAction.functionAfterDelay(this.BeginFindTreasure, 200);
        Game1.playSound("questcomplete");

        var player = Game1.player;
        var currentStreak = Data.ReadAs<int>(player, DataKeys.CurrentScavengerHuntStreak);
        var longestStreak = Data.ReadAs<int>(player, DataKeys.LongestScavengerHuntStreak);
        if (++currentStreak > longestStreak)
        {
            Data.Write(player, DataKeys.LongestScavengerHuntStreak, currentStreak.ToString());
        }

        Data.Write(player, DataKeys.CurrentScavengerHuntStreak, currentStreak.ToString());
        this.End(true);
    }

    /// <inheritdoc />
    public override void Fail()
    {
        Game1.addHUDMessage(new HuntNotification(this.HuntFailedMessage));
        Data.Write(Game1.player, DataKeys.LongestScavengerHuntStreak, "0");
        this.End(false);
    }

    /// <inheritdoc />
    public override void UpdateTriggerPool(params int[] criteria)
    {
        if (criteria.Length != 3)
        {
            ThrowHelper.ThrowInvalidOperationException("Criteria did not match expected size.");
        }

        if (this.IsActive)
        {
            return;
        }

        var stepsTaken = criteria[0];
        var itemsForaged = criteria[1];
        var treesChopped = criteria[2];
        this.TriggerPool += (stepsTaken * POINTS_PER_STEP) + (itemsForaged * POINTS_PER_ITEM_FORAGED) +
                            (treesChopped * POINTS_PER_TREES_CHOPPED);
        Log.D($"[Scavenger Hunt]: Hunt trigger pool increased to {this.TriggerPool}/{this.TriggerThreshold}.");
        if (this.TriggerPool >= this.TriggerThreshold)
        {
            Log.D("[Scavenger Hunt]: Hunt threshold reached. Begin monitoring for valid hunt location...");
            EventManager.Enable<ScavengerHuntTriggerTimeChangedEvent>();
        }
    }

    /// <inheritdoc />
    internal override void TimeUpdate(uint ticks)
    {
        if (!Game1.game1.ShouldTimePass())
        {
            return;
        }

        if (this.IsActive && ticks % 60 == 0 &&
            (this.Location?.terrainFeatures.TryGetValue(this.TargetTile.Value, out var feature) == true &&
             feature is HoeDirt))
        {
            this.Complete();
        }

#if RELEASE
        if (ticks % 60 == 0 && ++this.Elapsed > this.TimeLimit)
        {
            this.Fail();
        }
#endif
    }

    /// <summary>Launches a background task to cache eligible treasure tiles in the specified <paramref name="location"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    internal void TryCacheEligibleTreasureTiles(GameLocation location)
    {
        if (!this.IsLocationSuitable(location) ||
            this._eligibleTreasureHuntTilesByMap.ContainsKey(location.NameOrUniqueName) ||
            this._treasureTileCacheTaskByMap.ContainsKey(location.NameOrUniqueName))
        {
            return;
        }

        var task = Task.Run(() =>
        {
            this.GetEligibleTreasureTilesTask(location, Game1.player.Tile);
            this._treasureTileCacheTaskByMap.TryRemove(location.NameOrUniqueName, out _);
        });

        this._treasureTileCacheTaskByMap[location.NameOrUniqueName] = task;
    }

    /// <summary>Clears cached data.</summary>
    internal void FlushMapCache()
    {
        foreach (var map in this._eligibleTreasureHuntTilesByMap)
        {
            var location = Game1.getLocationFromName(map.Key);
            if (location is null)
            {
                continue;
            }

            foreach (var tuple in map.Value)
            {
                var (x, y) = tuple.Tile;
                if (location.doesTileHaveProperty((int)x, (int)y, "Diggable", "Back") is null)
                {
                    continue;
                }

                var digSpot = new Location((int)x * Game1.tileSize, (int)y * Game1.tileSize);
                location.Map.GetLayer("Back").PickTile(digSpot, Game1.viewport.Size).Properties["Diggable"] = tuple.Diggable;
            }
        }

        this._eligibleTreasureHuntTilesByMap.Clear();
        this._treasureTileCacheTaskByMap.Clear();
    }

    /// <inheritdoc />
    protected override bool ChooseTreasureTile(GameLocation location)
    {
        if (!this._eligibleTreasureHuntTilesByMap.TryGetValue(location.NameOrUniqueName, out var eligible))
        {
            return false;
        }

        for (var i = eligible.Count() - 1; i >= 0; i--)
        {
            if (!this.IsTileValidForBuriedTreasure(location, eligible[i].Tile))
            {
                eligible.RemoveAt(i);
            }
        }

        Log.D("[Scavenger Hunt]: Starting selection for Scavenger hunt....");
        var chosen = eligible
            .Where(t => Game1.player.SquaredTileDistance(t.Tile) > MIN_DISTANCE_TO_TREASURE * MIN_DISTANCE_TO_TREASURE)
            .Choose(this.Random);
        if (chosen == default)
        {
            Log.D("[Scavenger Hunt]: None of the eligible tiles cleared the distance threshold. The hunt will not begin.");
            return false;
        }

        Log.D($"[Scavenger Hunt]: Chosen target is at {chosen.Tile}.");
        this.TargetTile = chosen.Tile;
        return true;
    }

    /// <inheritdoc />
    protected override bool IsLocationSuitable(GameLocation location)
    {
        return location.IsOutdoors && (!location.IsFarm || Config.AllowScavengerHuntsOnFarm);
    }

    /// <inheritdoc />
    protected override void End(bool success)
    {
        if (!this._eligibleTreasureHuntTilesByMap.TryGetValue(this.Location!.NameOrUniqueName, out var eligible))
        {
            base.End(false);
            return;
        }

        this.TriggerPool = 0;
        foreach (var tuple in eligible)
        {
            if (this.Location.terrainFeatures.TryGetValue(tuple.Tile, out var feature) && feature is HoeDirt)
            {
                continue;
            }

            var (x, y) = tuple.Tile;
            var digSpot = new Location((int)x * Game1.tileSize, (int)y * Game1.tileSize);
            this.Location.Map.GetLayer("Back").PickTile(digSpot, Game1.viewport.Size).Properties["Diggable"] = tuple.Diggable;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer &&
            Game1.player.HasProfession(VanillaProfession.Scavenger, true))
        {
            Broadcaster.MessageHost("false", $"HuntingForTreasure/Scavenger");
        }

        base.End(success);
        Game1.player.buffs.Remove(ScavengerHuntBuff.ID);
    }

    /// <inheritdoc />
    protected override void StartImpl(GameLocation location, Vector2 treasureTile)
    {
        location.EnforceTileDiggable(treasureTile);
        foreach (var tuple in this._eligibleTreasureHuntTilesByMap[location.NameOrUniqueName])
        {
            location.EnforceTileDiggable(tuple.Tile);
        }

        Game1.addHUDMessage(new HuntNotification(this.HuntStartedMessage, this.IconSourceRect));
        if (Game1.player.HasProfession(VanillaProfession.Scavenger, true) &&
            (!Context.IsMultiplayer || Context.IsMainPlayer))
        {
            EventManager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
        }
        else
        {
            Broadcaster.MessageHost(
                $"true/" +
                $"{this.Location!.NameOrUniqueName}/" +
                $"{this.TargetTile!.Value.X.ToString(CultureInfo.InvariantCulture)}/" +
                $"{this.TargetTile!.Value.X.ToString(CultureInfo.InvariantCulture)}",
                "HuntingForTreasure/Scavenger");
        }

        base.StartImpl(location, treasureTile);
        Game1.player.applyBuff(new ScavengerHuntBuff());
    }

    /// <summary>Runs in the background to fetch and cache eligible treasure tiles.</summary>
    private void GetEligibleTreasureTilesTask(GameLocation location, Vector2 origin)
    {
        Log.D($"[Scavenger Hunt]: Starting cache build for treasure tiles in {location.NameOrUniqueName}.");
        var mapWidth = location.Map.Layers[0].LayerWidth;
        var mapHeight = location.Map.Layers[0].LayerHeight;
        var flooded = origin
            .FloodFill(mapWidth, mapHeight, tile => boundary(location, tile), maxSteps: 10000)
            .Select(t => t.Tile);

        var soil = flooded
            .Where(t => this.IsTileValidForBuriedTreasure(location, t))
            .ToList();
        if (soil.Count == 0)
        {
            Log.D("[Scavenger Hunt]: There were no soil tiles that cleared the boundary condition.");
            this._eligibleTreasureHuntTilesByMap[location.NameOrUniqueName] = [];
            return;
        }

        Log.D($"[Scavenger Hunt]: Found {soil.Count()} eligible soil tiles.");
        this._eligibleTreasureHuntTilesByMap[location.NameOrUniqueName] = soil.Select(t =>
        {
            var isDiggable = location.doesTileHaveProperty((int)t.X, (int)t.Y, "Diggable", "Back") is not null;
            return (t, diggable: isDiggable);
        }).ToList();
        return;

        bool boundary(GameLocation loc, Vector2 tile) => loc.isTilePassable(tile) &&
                                                         (!loc.objects.TryGetValue(tile, out var @object) ||
                                                          @object is null) && !loc.IsNoSpawnTile(tile);
    }

    /// <summary>Plays treasure chest found animation.</summary>
    private void BeginFindTreasure()
    {
        Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(
            "LooseSprites\\Cursors",
            new Rectangle(64, 1920, 32, 32),
            500f,
            1,
            0,
            Game1.player.Position + new Vector2(-32f, -160f),
            false,
            false,
            (Game1.player.StandingPixel.Y / 10000f) + 0.001f,
            0f,
            Color.White,
            4f,
            0f,
            0f,
            0f)
        {
            motion = new Vector2(0f, -0.128f),
            timeBasedMotion = true,
            endFunction = this.OpenChestEndFunction,
            extraInfoForEndBehavior = 0,
            alpha = 0f,
            alphaFade = -0.002f,
        });
    }

    /// <summary>Plays open treasure chest animation.</summary>
    /// <param name="extra">Not applicable.</param>
    private void OpenChestEndFunction(int extra)
    {
        Game1.currentLocation.localSound("openChest");
        Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(
            "LooseSprites\\Cursors",
            new Rectangle(64, 1920, 32, 32),
            200f,
            4,
            0,
            Game1.player.Position + new Vector2(-32f, -228f),
            false,
            false,
            (Game1.player.StandingPixel.Y / 10000f) + 0.001f,
            0f,
            Color.White,
            4f,
            0f,
            0f,
            0f) { endFunction = this.OpenTreasureMenuEndFunction, extraInfoForEndBehavior = 0, });
    }

    /// <summary>Opens the treasure chest menu.</summary>
    /// <param name="extra">Not applicable.</param>
    private void OpenTreasureMenuEndFunction(int extra)
    {
        Game1.player.completelyStopAnimatingOrDoingAction();
        var treasures = this.GetTreasureContents();
        Game1.activeClickableMenu = new ItemGrabMenu(treasures).setEssential(true);
        ((ItemGrabMenu)Game1.activeClickableMenu).source = ItemGrabMenu.source_fishingChest;
    }

    /// <summary>Chooses the contents of the treasure chest.</summary>
    /// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
    private List<Item> GetTreasureContents()
    {
        var effectiveLuck = (Game1.player.LuckLevel * 0.1) + Game1.player.DailyLuck;
        List<Item> treasures = [];
        this.AddInitialTreasure(treasures, effectiveLuck);
        this.AddMetals(treasures, effectiveLuck);
        switch (this.Random.Next(3))
        {
            case 0:
                this.AddMinerals(treasures, effectiveLuck);
                break;
            case 1:
                this.AddSeeds(treasures, effectiveLuck);
                break;
            case 2:
                this.AddSyrups(treasures, effectiveLuck);
                break;
        }

        if (this.Random.NextBool(0.65))
        {
            this.AddArtifacts(treasures, effectiveLuck);
        }
        else
        {
            this.AddSpecialTreasure(treasures, effectiveLuck);
        }

        if (treasures.Count <= 3)
        {
            this.AddFiller(treasures, effectiveLuck);
        }

        return treasures.Shuffle().Take(6).ToList();
    }

    private void AddInitialTreasure(List<Item> treasures, double luck)
    {
        if (Game1.currentSeason == "spring" && Game1.currentLocation is not Beach && this.Random.NextBool(0.15 + luck))
        {
            var stack = this.Random.Next(2, 6) + (this.Random.NextBool(0.25) ? 5 : 0);
            treasures.Add(ItemRegistry.Create(QIDs.RiceShoot, stack));
        }

        if (this.Random.NextBool(0.45 + luck) && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        {
            var stack = this.Random.Next(1, 3) + (this.Random.NextBool(0.25) ? 2 : 0);
            treasures.Add(ItemRegistry.Create(QIDs.QiBean, stack));
        }
    }

    private void AddMetals(List<Item> treasures, double luck)
    {
        if (Game1.player.deepestMineLevel > 120 && this.Random.NextBool(0.35 + luck))
        {
            treasures.Add(ItemRegistry.Create(QIDs.IridiumBar, this.RollStack(1, 3, 0.3, 0.9)));
        }

        if (Game1.player.deepestMineLevel > 80 && this.Random.NextBool(0.55 + luck))
        {
            treasures.Add(ItemRegistry.Create(QIDs.GoldBar, this.RollStack(1, 3, 0.35, 0.9)));
        }

        if (Game1.player.deepestMineLevel > 40 && this.Random.NextBool(0.75 + luck))
        {
            treasures.Add(ItemRegistry.Create(QIDs.IronBar, this.RollStack(1, 3, 0.35, 0.9)));
        }

        if (this.Random.NextBool(0.9 + luck))
        {
            treasures.Add(ItemRegistry.Create(QIDs.CopperBar, this.RollStack(1, 3, 0.35, 0.9)));
        }
    }

    private void AddArtifacts(List<Item> treasures, double luck)
    {
        if (this.Random.NextBool(0.1 + luck) && Game1.netWorldState.Value.LostBooksFound < 21 &&
            Game1.player.hasOrWillReceiveMail("lostBookFound"))
        {
            treasures.Add(ItemRegistry.Create(QIDs.LostBook));
        }
        else if (Game1.player.archaeologyFound.Any() && this.Random.NextBool())
        {
            var id = this.Random.NextBool()
                ? this._artifactsThatCanBeFound[this.Random.Next(this._artifactsThatCanBeFound.Length)]
                : this.Random.NextBool()
                    ? QIDs.AncientSeed
                    : QIDs.Geode;
            treasures.Add(ItemRegistry.Create(id));
        }
    }

    private void AddMinerals(List<Item> treasures, double luck)
    {
        if (this.Random.NextBool(0.15 + luck))
        {
            var stack = 1;
            while (this.Random.NextBool(0.3))
            {
                stack++;
            }

            treasures.Add(ItemRegistry.Create(QIDs.Diamond, stack));
        }

        if (this.Random.NextBool(0.35 + luck))
        {
            var stack = 1;
            while (this.Random.NextBool(0.4))
            {
                stack++;
            }

            var id = this.Random.NextBool()
                    ? QIDs.Ruby
                    : QIDs.Emerald;
            treasures.Add(ItemRegistry.Create(id, stack));
        }

        if (this.Random.NextBool(0.35 + luck))
        {
            var stack = 1;
            while (this.Random.NextBool(0.4))
            {
                stack++;
            }

            var id = this.Random.NextBool()
                    ? QIDs.Jade
                    : QIDs.Aquamarine;
            treasures.Add(ItemRegistry.Create(id, stack));
        }

        if (this.Random.NextBool(0.35 + luck))
        {
            var stack = 1;
            while (this.Random.NextBool(0.4))
            {
                stack++;
            }

            var id = this.Random.NextBool()
                    ? QIDs.Amethyst
                    : QIDs.Topaz;
            treasures.Add(ItemRegistry.Create(id, stack));
        }
    }

    private void AddSeeds(List<Item> treasures, double luck)
    {
        if (this.Random.NextBool(0.15 + luck))
        {
            treasures.Add(this.Random.NextBool()
                ? ItemRegistry.Create(QIDs.RareSeed)
                : ItemRegistry.Create(QIDs.AncientSeeds));
        }
        else if (this.Random.NextBool(0.35 + luck))
        {
            treasures.Add(ItemRegistry.Create(this.Random.NextBool()
                ? QIDs.CoffeeBean
                : QIDs.TeaSapling));
        }
        else
        {
            if (this.Random.NextBool(0.65 + luck))
            {
                var stack = 5 + (this.Random.NextBool(0.25 + luck) ? 5 : 0);
                var id = Game1.currentSeason switch
                {
                    "spring" => QIDs.SpringSeeds,
                    "summer" => QIDs.SummerSeeds,
                    "fall" => QIDs.FallSeeds,
                    "winter" => QIDs.WinterSeeds,
                };

                treasures.Add(ItemRegistry.Create(id, stack));
            }
            else
            {
                var stack = 5 + (this.Random.NextBool(0.5 + luck) ? 5 : 0);
                treasures.Add(ItemRegistry.Create(QIDs.MixedSeeds, stack));
            }
        }
    }

    private void AddSyrups(List<Item> treasures, double luck)
    {
        if (this.Random.NextBool(0.55 + luck))
        {
            var id = Game1.random.Next(3) switch
            {
                0 => QIDs.MapleSyrup,
                1 => QIDs.OakResin,
                2 => QIDs.PineTar,
            };

            treasures.Add(ItemRegistry.Create(id));
        }
        else
        {
            var stack = 2.5f;
            while (this.Random.NextBool())
            {
                stack *= this.Random.Next(4);
            }

            var id = Game1.random.Next(3) switch
            {
                0 => QIDs.MapleSeed,
                1 => QIDs.Acorn,
                2 => QIDs.PineCone,
            };

            treasures.Add(ItemRegistry.Create(id, (int)stack));
        }
    }

    private void AddSpecialTreasure(List<Item> treasures, double luck)
    {
        var luckModifier = 1d + luck;
        if (this.Random.NextBool(0.15 * luckModifier))
        {
            treasures.Add(this.Random.NextBool() ? new MeleeWeapon(QIDs.ForestSword) : new MeleeWeapon(QIDs.ElfBlade));
        }
        else if (this.Random.NextBool(0.15 * luckModifier))
        {
            if (this.Random.NextBool(0.25 * luckModifier))
            {
                treasures.Add(ItemRegistry.Create(QIDs.IridiumBand));
            }
            else
            {
                switch (this.Random.Next(3))
                {
                    case 0:
                    {
                        var id = QIDs.SmallGlowRing + (this.Random.NextBool(Game1.player.LuckLevel / 11f)
                            ? 1
                            : 0);
                        treasures.Add(ItemRegistry.Create(id));
                        break;
                    }

                    case 1:
                    {
                        var id = QIDs.SmallMagnetRing +
                                 (this.Random.NextBool(Game1.player.LuckLevel / 11f)
                                     ? 1
                                     : 0);
                        treasures.Add(ItemRegistry.Create(id));
                        break;
                    }

                    // gemstone ring
                    case 2:
                    {
                        var id = this.Random.Next(529, 535);
                        treasures.Add(ItemRegistry.Create($"(O){id}"));
                        break;
                    }
                }
            }
        }
        else if (this.Random.NextBool(0.15 * luckModifier))
        {
            var id = $"(B){this.Random.Next(504, 511)}";
            if (id is QIDs.ThermalBoots or QIDs.TundraBoots && Game1.currentSeason != "winter")
            {
                id = QIDs.CowboyBoots;
            }

            treasures.Add(ItemRegistry.Create(id));
        }
        else if (this.Random.NextBool(0.15 * luckModifier))
        {
            treasures.Add(ItemRegistry.Create(QIDs.PrismaticShard));
        }
        else if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") &&
            this.Random.NextBool(0.35 * luckModifier))
        {
            treasures.Add(ItemRegistry.Create(QIDs.GoldenEgg));
        }
        else if (this.Random.NextBool(0.5 * luckModifier))
        {
            treasures.Add(ItemRegistry.Create(QIDs.Diamond));
        }

        if (this.Random.NextBool(0.05))
        {
            treasures.Add(ItemRegistry.Create(QIDs.StrangeDoll0));
        }
        else if (this.Random.NextBool(0.05))
        {
            treasures.Add(ItemRegistry.Create(QIDs.StrangeDoll1));
        }
    }

    private void AddFiller(List<Item> treasures, double luck)
    {
        if (this.Random.NextBool(0.65 + luck))
        {
            var id = this.Random.Next(5) switch
            {
                0 => QIDs.WarpTotem_Farm,
                1 => QIDs.WarpTotem_Beach,
                2 => QIDs.WarpTotem_Mountains,
                3 when Game1.MasterPlayer.mailReceived.Contains("ccVault") => QIDs.WarpTotem_Desert,
                4 when Game1.MasterPlayer.mailReceived.Contains("willyBoatFixed") => QIDs.WarpTotem_Island,
                _ => QIDs.FieldSnack,
            };

            treasures.Add(ItemRegistry.Create(id));
        }
        else
        {
            if (this.Random.NextBool(0.55 + luck))
            {
                var stack = 5 + (this.Random.NextBool(0.25 + luck) ? 5 : 0);
                treasures.Add(ItemRegistry.Create(QIDs.WildBait, stack));
            }
            else if (this.Random.NextBool())
            {
                var stack = 5 + (this.Random.NextBool(0.5 + luck) ? 5 : 0);
                treasures.Add(ItemRegistry.Create(QIDs.Bait, stack));
            }
        }
    }

    private bool IsTileValidForBuriedTreasure(GameLocation location, Vector2 tile)
    {
        if (location.terrainFeatures.ContainsKey(tile))
        {
            // Log.D("Already hoed!");
            return false;
        }

        if (!location.CanItemBePlacedHere(tile))
        {
            // Log.D("Items can't be placed here!");
            return false;
        }

        if (location.isBehindBush(tile) || location.isBehindTree(tile))
        {
            // Log.D("Is either behind a bush or a tree!");
            return false;
        }

        if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "AlwaysFront") != -1)
        {
            // Log.D("Something's always in front!");
            return false;
        }

        if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Front") != -1)
        {
            // Log.D("Something's in front!");
            return false;
        }

        return location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Type", "Back") is "Dirt" or "Grass";
    }
}
