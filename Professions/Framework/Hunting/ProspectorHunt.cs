namespace DaLion.Professions.Framework.Hunting;

#region using directives

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DaLion.Professions.Framework.Buffs;
using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewValley.Locations;
using StardewValley.Tools;

#endregion using directives

/// <summary>Manages treasure hunt events for Prospector profession.</summary>
internal sealed class ProspectorHunt : TreasureHunt
{
    private const int BASE_TRIGGER_POOL_THRESHOLD = 2000;
    private const int MIN_STONES_REQUIRED_FOR_HUNT = 32;
    private const int MIN_STONE_DISTANCE_FROM_PLAYER = 3;
    private const int MAX_STONE_DISTANCE_FROM_PLAYER = 12;
    private const int POINTS_PER_STEP = 1;
    private const int POINTS_PER_ROCK_CRUSHED = 10;
    private const int POINTS_PER_MINE_FLOOR_ADVANCED = 100;

    private static List<double> _linSpace = MathUtils.LinSpace(0d, 1d, 10).ToList();
    private static int[] _pitches = [0, 200, 400, 500, 700, 900, 1100, 1200];

    private readonly ICue _cue = Game1.soundBank.GetCue("SinWave");
    private int _targetsFound;
    private int _fadeStepIndex;

    /// <summary>Initializes a new instance of the <see cref="ProspectorHunt"/> class.</summary>
    internal ProspectorHunt()
        : base(
            TreasureHuntProfession.Prospector,
            I18n.Prospector_HuntStarted(),
            I18n.Prospector_HuntFailed(),
            new Rectangle(48, 672, 16, 16))
    {
        this.TriggerPool = Data.ReadAs<int>(Game1.player, DataKeys.ProspectorPointPool);
    }

    /// <inheritdoc />
    public override int TimeLimit => Config.ProspectorHuntTimeLimit;

    /// <inheritdoc />
    public override int TriggerPool { get; protected set; }

    /// <summary>Gets the stone instance that is currently targeted.</summary>
    public SObject? TreasureStone => (this.Location?.Objects.TryGetValue(this.TargetTile ?? default, out var stone) ?? false) ? stone : null;

    /// <inheritdoc />
    protected override int TriggerThreshold => (int)(BASE_TRIGGER_POOL_THRESHOLD * Config.TreasureHuntTriggerModifier);

    /// <summary>Ends the current iteration of the active hunt successfully.</summary>
    public override void Complete()
    {
        this.Complete(false);
    }

    /// <inheritdoc />
    public override void Fail()
    {
        Game1.addHUDMessage(new HuntNotification(this.HuntFailedMessage));
        Data.Write(Game1.player, DataKeys.CurrentProspectorHuntStreak, "0");
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
        var rocksCrushed = criteria[1];
        var floorAdvanced = criteria[2];
        this.TriggerPool += (stepsTaken * POINTS_PER_STEP) + (rocksCrushed * POINTS_PER_ROCK_CRUSHED) +
                            (floorAdvanced * POINTS_PER_MINE_FLOOR_ADVANCED);
        Log.D($"[Prospector Hunt]: Hunt trigger pool increased to {this.TriggerPool}/{this.TriggerThreshold}.");
    }

    /// <summary>Invoked when the local player destroys a stone.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The <see cref="Vector2"/> tile where the stone was located.</param>
    public void OnStoneDestroyed(GameLocation location, Vector2 tile)
    {
        if (this.TriggerPool >= this.TriggerThreshold && !this.IsActive)
        {
            Log.D("[Prospector Hunt]: Trigger threshold reached. Attempting to start Prospector Hunt.");
            this.TryStart(location);
        }
        else if (this.IsActive && tile == this.TargetTile)
        {
            Log.D("[Prospector Hunt]: Target found! Completing current hunt cycle...");
            this.Complete();
        }
    }

    /// <inheritdoc />
    protected override bool ChooseTreasureTile(GameLocation location)
    {
        Log.D("[Prospector Hunt]: Starting target search...");

        var origin = Game1.player.Tile;
        var mapWidth = location.Map.Layers[0].LayerWidth;
        var mapHeight = location.Map.Layers[0].LayerHeight;

        // find all reachable tiles; this includes empty tiles as well as tiles with stones
        // this is necessary because, if we enforce having a stone as a boundary condition, the
        // search becomes limited to stone clusters instead of the whole mine floor
        var flooded = origin.FloodFill(
            mapWidth,
            mapHeight,
            tile => boundary(location, tile),
            maxSteps: 1000);

        var eligible = flooded
            .Where(t => location.HasStoneAt(t.Tile))
            .ToList();
        if (eligible.Count < MIN_STONES_REQUIRED_FOR_HUNT && !this.IsActive)
        {
            Log.D($"[Prospector Hunt]: Failed due to not enough stones in current location ({eligible.Count}/{MIN_STONES_REQUIRED_FOR_HUNT}).");
            return false;
        }

        Log.D($"[Prospector Hunt]: Found {eligible.Count} eligible stones.");
        var goldilocks = eligible
            .Where(t =>
                t.Distance is >= MIN_STONE_DISTANCE_FROM_PLAYER and <= MAX_STONE_DISTANCE_FROM_PLAYER)
            .Select(t => t.Tile)
            .ToList();
        if (!goldilocks.Any())
        {
            Log.D("[Prospector Hunt]: None of the eligible stones cleared the initial distance filter.");

            // just take whatever is nearest
            var nearest = eligible
                .Where(t => t.Distance >= MIN_STONE_DISTANCE_FROM_PLAYER)
                .ArgMin(t => t.Distance);
            if (nearest == (default, 0))
            {
                Log.D("[Prospector Hunt]: The nearest stone could not be found. How did we get here?.");
                if (this.IsActive)
                {
                    Log.D("[Prospector Hunt]: The hunt will conclude immediately.");
                    this.Complete(force: true);
                }

                Log.D("[Prospector Hunt]: A new target could not be chosen.");
                return false;
            }

            Log.D($"[Prospector Hunt]: Nearest new target was found at {nearest.Tile}.");
            this.TargetTile = nearest.Tile;
            return true;
        }

        var centroid = new Vector2(
            eligible.Average(t => t.Tile.X),
            eligible.Average(t => t.Tile.Y));
        Log.D($"[Prospector Hunt]: The centroid is at {centroid}. The selection will be biased in that direction.");

        var playerTile = Game1.player.Tile;
        var directionToCentroid = centroid - playerTile;
        if (directionToCentroid == default)
        {
            directionToCentroid = Vector2.UnitX.Collect(Vector2.UnitY).Choose();
        }
        else
        {
            directionToCentroid.Normalize();
        }

        var weighted = goldilocks
            .Select(t =>
            {
                var directionToStone = t - playerTile;
                if (directionToStone == default)
                {
                    directionToStone = Vector2.UnitX.Collect(Vector2.UnitY).Choose();
                }
                else
                {
                    directionToStone.Normalize();
                }

                var weight = Vector2.Dot(directionToStone, directionToCentroid) + 1f;
                return (t, weight);
            })
            .ToList();

        var totalWeight = weighted.Sum(t => t.weight);
        var roll = (float)Game1.random.NextDouble() * totalWeight;
        this.TargetTile = weighted[0].t;
        foreach (var stone in weighted)
        {
            if (roll <= stone.weight)
            {
                this.TargetTile = stone.t;
                break;
            }

            roll -= stone.weight;
        }

        if (this.Location is not null)
        {
            this.Location.Objects[this.TargetTile.Value].MinutesUntilReady += this._targetsFound;
        }

        Log.D($"[Prospector Hunt]: New chosen target is at {this.TargetTile.Value}.");
        return true;

        bool boundary(GameLocation loc, Vector2 tile)
        {
            return loc.isTilePassable(tile) || loc.HasStoneAt(tile);
        }
    }

    /// <inheritdoc />
    protected override bool IsLocationSuitable(GameLocation location)
    {
        return ((location is MineShaft shaft && !shaft.IsTreasureOrSafeRoom()) || location is VolcanoDungeon) &&
               location.Objects.Values.Count(o => o?.IsBreakableStone() ?? false) > MIN_STONES_REQUIRED_FOR_HUNT;
    }

    /// <inheritdoc />
    protected override void End(bool success)
    {
        this.TriggerPool = 0;
        if (Context.IsMultiplayer && !Context.IsMainPlayer &&
            Game1.player.HasProfession(VanillaProfession.Prospector, true))
        {
            Broadcaster.MessageHost("false", "HuntingForTreasure/Prospector");
        }

        base.End(success);
        Game1.player.buffs.Remove(ProspectorHuntBuff.ID);
    }

    /// <inheritdoc />
    protected override void StartImpl(GameLocation location, Vector2 treasureTile)
    {
        Game1.addHUDMessage(new HuntNotification(this.HuntStartedMessage, this.IconSourceRect));
        if (Game1.player.HasProfession(VanillaProfession.Prospector, true) && (!Context.IsMultiplayer || Context.IsMainPlayer))
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
                "HuntingForTreasure/Prospector");
        }

        this._targetsFound = 0;
        base.StartImpl(location, treasureTile);
        Game1.player.applyBuff(new ProspectorHuntBuff());
    }

    /// <summary>Ends the active hunt successfully.</summary>
    /// <param name="force">If <see langword="true"/>, skips all pending iterations.</param>
    private void Complete(bool force)
    {
        if (this.TargetTile is null || this.Location is null)
        {
            this.End(false);
            return;
        }

        this.PlayCue();

        if (++this._targetsFound < 8 && !force)
        {
            this.ChooseTreasureTile(this.Location);
            return;
        }

        switch (this.Location)
        {
            case MineShaft shaft:
                this.GetStoneTreasureForMineShaft(shaft.mineLevel);
                if (shaft.shouldCreateLadderOnThisLevel() && !shaft.GetLadderTiles().Any())
                {
                    shaft.createLadderDown((int)this.TargetTile!.Value.X, (int)this.TargetTile!.Value.Y);
                }

                break;
            case VolcanoDungeon:
                this.GetStoneTreasureForVolcanoDungeon();
                break;
        }

        Game1.playSound("questcomplete");

        var player = Game1.player;
        var currentStreak = Data.ReadAs<int>(player, DataKeys.CurrentProspectorHuntStreak);
        var longestStreak = Data.ReadAs<int>(player, DataKeys.LongestProspectorHuntStreak);
        if (++currentStreak > longestStreak)
        {
            Data.Write(player, DataKeys.LongestProspectorHuntStreak, currentStreak.ToString());
        }

        Data.Write(player, DataKeys.CurrentProspectorHuntStreak, currentStreak.ToString());
        this.End(true);
    }

    /// <summary>Spawns hunt spoils as debris. Applies to <see cref="MineShaft"/>.</summary>
    /// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
    private void GetStoneTreasureForMineShaft(int mineLevel)
    {
        var effectiveLuck = (Game1.player.LuckLevel * 0.1) + Game1.player.DailyLuck;
        Dictionary<string, int> treasuresAndQuantities = [];
        this.AddInitialTreasure(treasuresAndQuantities, effectiveLuck);
        this.AddOre(treasuresAndQuantities, effectiveLuck, mineLevel);
        this.AddMinerals(treasuresAndQuantities, effectiveLuck, mineLevel);
        if (this.Random.NextBool(0.65))
        {
            this.AddArtifacts(treasuresAndQuantities, effectiveLuck);
        }
        else
        {
            this.AddSpecialTreasureItems(treasuresAndQuantities, effectiveLuck, mineLevel);
        }

        foreach (var (treasure, quantity) in treasuresAndQuantities)
        {
            if (treasure.StartsWith(ItemRegistry.type_weapon))
            {
                Game1.createItemDebris(
                    ItemRegistry.Create<MeleeWeapon>(treasure),
                    (this.TargetTile!.Value * Game1.tileSize) + new Vector2(32f, 32f),
                    this.Random.Next(4),
                    Game1.currentLocation);
            }
            else
            {
                Game1.createMultipleObjectDebris(
                    treasure,
                    (int)this.TargetTile!.Value.X,
                    (int)this.TargetTile.Value.Y,
                    quantity,
                    Game1.player.UniqueMultiplayerID,
                    Game1.currentLocation);
            }
        }
    }

    /// <summary>Spawns hunt spoils as debris. Applies to <see cref="VolcanoDungeon"/>.</summary>
    /// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
    private void GetStoneTreasureForVolcanoDungeon()
    {
        var effectiveLuck = (Game1.player.LuckLevel * 0.1) + Game1.player.DailyLuck;
        Dictionary<string, int> treasuresAndQuantities = [];
        this.AddInitialTreasure(treasuresAndQuantities, effectiveLuck);
        this.AddOre(treasuresAndQuantities, effectiveLuck);
        this.AddMinerals(treasuresAndQuantities, effectiveLuck);
        if (this.Random.NextBool(0.65))
        {
            this.AddArtifacts(treasuresAndQuantities, effectiveLuck);
        }
        else
        {
            this.AddSpecialTreasureItems(treasuresAndQuantities, effectiveLuck);
        }

        if (treasuresAndQuantities.TryGetValue(QIDs.Coal, out var stack) && this.Random.NextBool())
        {
            treasuresAndQuantities[QIDs.CinderShard] = stack;
            treasuresAndQuantities.Remove(QIDs.Coal);
        }

        foreach (var (treasure, quantity) in treasuresAndQuantities)
        {
            Game1.createMultipleObjectDebris(
                treasure,
                (int)this.TargetTile!.Value.X,
                (int)this.TargetTile.Value.Y,
                quantity,
                Game1.player.UniqueMultiplayerID,
                Game1.currentLocation);
        }
    }

    private void AddInitialTreasure(IDictionary<string, int> treasuresAndQuantities, double luck)
    {
        if (this.Random.NextBool(0.45 + luck) && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        {
            treasuresAndQuantities.AddOrUpdate(
                QIDs.QiBean,
                this.Random.Next(1, 3) + (this.Random.NextBool(0.25) ? 2 : 0),
                (a, b) => a + b);
        }
    }

    private void AddOre(Dictionary<string, int> treasuresAndQuantities, double luck, int mineLevel = -1)
    {
        if (mineLevel > 120 && this.Random.NextBool(0.45 + luck))
        {
            treasuresAndQuantities.AddOrUpdate(QIDs.IridiumOre, this.RollStack(2, 5, 0.3, 0.9), (a, b) => a + b);
        }

        if ((mineLevel < 0 || Game1.mine.GetAdditionalDifficulty() > 0) && this.Random.NextBool(0.45 + luck))
        {
            treasuresAndQuantities.AddOrUpdate(QIDs.RadioactiveOre, this.RollStack(2, 5, 0.3, 0.9), (a, b) => a + b);
        }

        if (mineLevel is < 0 or > 80 && this.Random.NextBool(0.65 + luck))
        {
            treasuresAndQuantities.AddOrUpdate(QIDs.GoldOre, this.RollStack(2, 5, 0.35, 0.9), (a, b) => a + b);
        }

        if (mineLevel is < 0 or > 40 && this.Random.NextBool(0.85 + luck))
        {
            treasuresAndQuantities.AddOrUpdate(QIDs.IronOre, this.RollStack(2, 5, 0.35, 0.9), (a, b) => a + b);
        }

        treasuresAndQuantities.AddOrUpdate(QIDs.CopperOre, this.RollStack(2, 5, 0.35, 0.9), (a, b) => a + b);
        treasuresAndQuantities.AddOrUpdate(QIDs.Coal, this.RollStack(3, 10, 0.4, 0.9), (a, b) => a + b);
    }

    private void AddArtifacts(Dictionary<string, int> treasuresAndQuantities, double luck)
    {
        if (Game1.player.archaeologyFound.Any() && this.Random.NextBool(0.5 + luck))
        {
            treasuresAndQuantities.AddOrUpdate(
                this.Random.NextBool()
                    ? $"(O){this.Random.Next(538, 579)}"
                    : this.Random.NextBool()
                        ? $"(O){this.Random.Next(121, 123)}"
                        : $"(O){this.Random.Next(579, 590)}",
                1,
                (a, b) => a + b);
        }
        else
        {
            treasuresAndQuantities.AddOrUpdate(QIDs.Geode, this.Random.Next(3, 5), (a, b) => a + b);
        }
    }

    private void AddMinerals(Dictionary<string, int> treasuresAndQuantities, double luck, int mineLevel = -1)
    {
        if (mineLevel is < 0 or > 120 && this.Random.NextBool(0.25 + luck))
        {
            var stack = 1;
            while (this.Random.NextBool(0.3))
            {
                stack++;
            }

            treasuresAndQuantities.AddOrUpdate(
                this.Random.NextBool(0.3) ? QIDs.OmniGeode : QIDs.Diamond,
                stack,
                (a, b) => a + b);
        }

        if (mineLevel > 80 && this.Random.NextBool(0.45 + luck))
        {
            var stack = 1;
            while (this.Random.NextBool(0.4))
            {
                stack++;
            }

            treasuresAndQuantities.AddOrUpdate(
                this.Random.NextBool(0.3) ? QIDs.FireQuartz :
                this.Random.NextBool() ? QIDs.Ruby : QIDs.Emerald,
                stack,
                (a, b) => a + b);
        }

        if (mineLevel > 40 && this.Random.NextBool(0.45 + luck))
        {
            var stack = 1;
            while (this.Random.NextBool(0.4))
            {
                stack++;
            }

            treasuresAndQuantities.AddOrUpdate(
                this.Random.NextBool(0.3 + luck) ? QIDs.FrozenTear :
                this.Random.NextBool() ? QIDs.Jade : QIDs.Aquamarine,
                stack,
                (a, b) => a + b);
        }

        if (this.Random.NextBool(0.45 + luck))
        {
            var stack = 1;
            while (this.Random.NextBool(0.4))
            {
                stack++;
            }

            treasuresAndQuantities.AddOrUpdate(
                this.Random.NextBool(0.3) ? QIDs.EarthCrystal :
                this.Random.NextBool() ? QIDs.Amethyst : QIDs.Topaz,
                stack,
                (a, b) => a + b);
        }
    }

    private void AddSpecialTreasureItems(Dictionary<string, int> treasuresAndQuantities, double luck, int mineLevel = -1)
    {
        var luckModifier = Math.Max(0, 1d + (luck * Math.Max(mineLevel / 4, 1)));
        if (mineLevel > 0 && this.Random.NextBool(0.15 * luckModifier))
        {
            treasuresAndQuantities.TryAdd(this.Random.NextBool() ? QIDs.Femur : QIDs.OssifiedBlade, 1);
        }
        else if (this.Location is VolcanoDungeon && this.Random.NextBool(0.65 * luckModifier))
        {
            treasuresAndQuantities.AddOrUpdate(
                QIDs.DragonTooth,
                1,
                (a, b) => a + b);
        }

        if (this.Random.NextBool(0.35 * luckModifier))
        {
            treasuresAndQuantities.AddOrUpdate(QIDs.PrismaticShard, 1, (a, b) => a + b);
        }
        else if (this.Random.NextBool(0.5 * luckModifier))
        {
            treasuresAndQuantities.AddOrUpdate(QIDs.Diamond, 1, (a, b) => a + b);
        }
    }

    /// <summary>Plays the sound cue.</summary>
    private void PlayCue()
    {
        this._cue.SetVariable("Pitch", _pitches[this._targetsFound]);
        this._cue.Play();
        var fadeSteps = 10;
        foreach (var step in Enumerable.Range(0, fadeSteps))
        {
            DelayedAction.functionAfterDelay(
                this.FadeCue,
                500 + (step * 100));
        }
    }

    /// <summary>Fades out the sound cue volume.</summary>
    private void FadeCue()
    {
        if (++this._fadeStepIndex >= _linSpace.Count)
        {
            this._cue.Stop(AudioStopOptions.Immediate);
            this._cue.Volume = 1f;
            this._fadeStepIndex = 0;
            return;
        }

        if ((float)MathUtils.BoundedSCurve(_linSpace[this._fadeStepIndex], 3d) is var newVolume && newVolume < this._cue.Volume)
        {
            this._cue.Volume = newVolume;
        }

        if (this._cue.Volume is > 0f and <= 0.01f)
        {
            this._cue.Stop(AudioStopOptions.Immediate);
        }
    }
}
