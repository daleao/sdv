namespace DaLion.Professions.Framework.TreasureHunts;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Professions.Framework.Events.Display.RenderedHud;
using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.Events.World.ObjectListChanged;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Tools;

#endregion using directives

/// <summary>Manages treasure hunt events for Prospector profession.</summary>
internal sealed class ProspectorHunt : TreasureHunt
{
    /// <summary>Initializes a new instance of the <see cref="ProspectorHunt"/> class.</summary>
    internal ProspectorHunt()
        : base(
            TreasureHuntProfession.Prospector,
            I18n.Prospector_HuntStarted(),
            I18n.Prospector_HuntFailed(),
            new Rectangle(48, 672, 16, 16))
    {
    }

    /// <inheritdoc />
    public override bool TryStart(double chance)
    {
        return Game1.currentLocation.Objects.Any() && base.TryStart(chance);
    }

    /// <inheritdoc />
    public override void Complete()
    {
        if (this.TreasureTile is null || this.Location is null)
        {
            this.End(false);
            return;
        }

        if (Game1.currentLocation.Objects.ContainsKey(this.TreasureTile.Value))
        {
            return;
        }

        switch (this.Location)
        {
            case MineShaft shaft:
                this.GetStoneTreasureForMineShaft(shaft.mineLevel);
                if (shaft.shouldCreateLadderOnThisLevel() && !shaft.GetLadderTiles().Any())
                {
                    shaft.createLadderDown((int)this.TreasureTile!.Value.X, (int)this.TreasureTile!.Value.Y);
                }

                break;
            case VolcanoDungeon:
                this.GetStoneTreasureForVolcanoDungeon();
                break;
        }

        Game1.playSound("questcomplete");
        Data.Increment(Game1.player, DataKeys.ProspectorHuntStreak);
        this.End(true);
    }

    /// <inheritdoc />
    public override void Fail()
    {
        Game1.addHUDMessage(new HuntNotification(this.HuntFailedMessage));
        Data.Write(Game1.player, DataKeys.ProspectorHuntStreak, "0");
        this.End(false);
    }

    /// <inheritdoc />
    protected override bool TrySetTreasureTile(GameLocation location)
    {
        var mapWidth = location.Map.DisplaySize.Width;
        var mapHeight = location.Map.DisplaySize.Height;
        var boundary = (Vector2 tile) => !location.isTilePassable(tile) && location.IsTileValidForTreasure(tile);
        var validTiles = Game1.player.Tile.FloodFill(mapWidth, mapHeight, boundary, 15, 50).Where(tile =>
            location.Objects.TryGetValue(tile, out var @object) && @object.IsStone()).ToHashSet();
        this.TreasureTile = validTiles.Count > 0 ? validTiles.Choose() : null;
        return this.TreasureTile is not null;
    }

    /// <inheritdoc />
    protected override uint SetTimeLimit()
    {
#if DEBUG
        this.TimeLimit = int.MaxValue;
#else
        this.TimeLimit = (uint)(this.Location!.Objects.Count() * Config.ProspectorHuntHandicap);
#endif

        return this.TimeLimit;
    }

    /// <inheritdoc />
    protected override void End(bool success)
    {
        base.End(success);
        EventManager.Disable(
            typeof(ProspectorHuntRenderedHudEvent),
            typeof(ProspectorHuntUpdateTickedEvent));
        if (!Context.IsMultiplayer || Context.IsMainPlayer ||
            !Game1.player.HasProfession(VanillaProfession.Prospector, true))
        {
            return;
        }

        Broadcaster.MessageHost("false", "HuntingForTreasure/Prospector");
    }

    /// <inheritdoc />
    protected override void StartImpl(GameLocation location, Vector2 treasureTile)
    {
        EventManager.Enable(
            Context.IsMainPlayer
                ? typeof(ProspectorHuntObjectListChangedEvent)
                : typeof(FarmhandProspectorHuntUpdateTickedEvent),
            typeof(ProspectorHuntUpdateTickedEvent));
        if (Config.UseLegacyProspectorHunt)
        {
            EventManager.Enable<ProspectorHuntRenderedHudEvent>();
        }

        Game1.addHUDMessage(new HuntNotification(this.HuntStartedMessage, this.IconSourceRect));
        if (Game1.player.HasProfession(VanillaProfession.Prospector, true) && (!Context.IsMultiplayer || Context.IsMainPlayer))
        {
            EventManager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
        }
        else
        {
            Broadcaster.MessageHost("true", "HuntingForTreasure/Prospector");
        }

        base.StartImpl(location, treasureTile);
    }

    /// <summary>Spawns hunt spoils as debris. Applies to <see cref="MineShaft"/>.</summary>
    /// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
    private void GetStoneTreasureForMineShaft(int mineLevel)
    {
        Dictionary<string, int> treasuresAndQuantities = [];
        var chance = 1.0;
        var streak = Data.ReadAs<int>(Game1.player, DataKeys.ProspectorHuntStreak);
        var i = 0;
        while (this.Random.NextBool(chance))
        {
            chance *= Math.Pow(0.1, 1d / (streak + 1));
            this.AddInitialTreasure(treasuresAndQuantities);
            switch (this.Random.Next(3))
            {
                case 0:
                    this.AddOreToTreasure(treasuresAndQuantities, mineLevel);
                    break;
                case 1:
                    this.AddArtifactsToTreasures(treasuresAndQuantities);
                    break;
                case 2:
                    switch (this.Random.Next(3))
                    {
                        case 0:
                            this.AddGeodesToTreasures(treasuresAndQuantities, mineLevel);
                            break;
                        case 1:
                            this.AddMineralsToTreasures(treasuresAndQuantities, mineLevel);
                            break;
                        case 2:
                            this.AddSpecialTreasureItems(treasuresAndQuantities, mineLevel);
                            break;
                    }

                    break;
            }

            i++;
        }

        Log.D($"[ProspectorHunt] Produced {i} treasures.");

        foreach (var (treasure, quantity) in treasuresAndQuantities)
        {
            if (treasure.StartsWith(ItemRegistry.type_weapon))
            {
                Game1.createItemDebris(
                    ItemRegistry.Create<MeleeWeapon>(treasure),
                    (this.TreasureTile!.Value * Game1.tileSize) + new Vector2(32f, 32f),
                    this.Random.Next(4),
                    Game1.currentLocation);
            }
            else
            {
                Game1.createMultipleObjectDebris(
                    treasure,
                    (int)this.TreasureTile!.Value.X,
                    (int)this.TreasureTile.Value.Y,
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
        Dictionary<string, int> treasuresAndQuantities = [];
        var chance = 1.0;
        var streak = Data.ReadAs<int>(Game1.player, DataKeys.ProspectorHuntStreak);
        while (this.Random.NextBool(chance))
        {
            chance *= Math.Pow(0.1, 1d / (streak + 1));
            this.AddInitialTreasure(treasuresAndQuantities);
            switch (this.Random.Next(3))
            {
                case 0:
                    this.AddOreToTreasure(treasuresAndQuantities);
                    break;
                case 1:
                    this.AddArtifactsToTreasures(treasuresAndQuantities);
                    break;
                case 2:
                    switch (this.Random.Next(3))
                    {
                        case 0:
                            this.AddGeodesToTreasures(treasuresAndQuantities);
                            break;
                        case 1:
                            this.AddMineralsToTreasures(treasuresAndQuantities);
                            break;
                        case 2:
                            this.AddSpecialTreasureItems(treasuresAndQuantities);
                            break;
                    }

                    break;
            }
        }

        if (treasuresAndQuantities.TryGetValue(QualifiedObjectIds.Coal, out var stack) && this.Location is VolcanoDungeon &&
            this.Random.NextBool())
        {
            treasuresAndQuantities[QualifiedObjectIds.CinderShard] = stack;
            treasuresAndQuantities.Remove(QualifiedObjectIds.Coal);
        }

        foreach (var (treasure, quantity) in treasuresAndQuantities)
        {
            Game1.createMultipleObjectDebris(
                treasure,
                (int)this.TreasureTile!.Value.X,
                (int)this.TreasureTile.Value.Y,
                quantity,
                Game1.player.UniqueMultiplayerID,
                Game1.currentLocation);
        }
    }

    private void AddInitialTreasure(IDictionary<string, int> treasuresAndQuantities)
    {
        if (this.Random.NextBool(0.33) && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        {
            treasuresAndQuantities.AddOrUpdate(
                QualifiedObjectIds.QiBean,
                this.Random.Next(1, 3) + (this.Random.NextBool(0.25) ? 2 : 0),
                (a, b) => a + b);
        }
    }

    private void AddOreToTreasure(Dictionary<string, int> treasuresAndQuantities, int mineLevel = -1)
    {
        if (mineLevel > 120 && this.Random.NextBool(0.06))
        {
            treasuresAndQuantities.AddOrUpdate(QualifiedObjectIds.IridiumOre, this.Random.Next(1, 3), (a, b) => a + b);
        }

        List<string> possibles = [];
        if (mineLevel < 0 || Game1.mine.GetAdditionalDifficulty() > 0)
        {
            possibles.Add(QualifiedObjectIds.RadioactiveOre);
        }

        if (mineLevel is < 0 or > 80)
        {
            possibles.Add(QualifiedObjectIds.GoldOre);
        }

        if (mineLevel is < 0 or > 40 && (possibles.Count == 0 || this.Random.NextBool(0.6)))
        {
            possibles.Add(QualifiedObjectIds.IronOre);
        }

        if (possibles.Count == 0 || this.Random.NextBool(0.6))
        {
            possibles.Add(QualifiedObjectIds.CopperOre);
        }

        possibles.Add(QualifiedObjectIds.Coal);
        treasuresAndQuantities.AddOrUpdate(
            possibles.ElementAt(this.Random.Next(possibles.Count)),
            this.Random.Next(2, 7) * (this.Random.NextBool(0.05 + (Game1.player.LuckLevel * 0.015))
                ? 2
                : 1),
            (a, b) => a + b);
        if (this.Random.NextBool(0.05 + (Game1.player.LuckLevel * 0.03)))
        {
            var key = treasuresAndQuantities.Keys.Last();
            treasuresAndQuantities[key] *= 2;
        }
    }

    private void AddArtifactsToTreasures(Dictionary<string, int> treasuresAndQuantities)
    {
        if (Game1.player.archaeologyFound.Any() && this.Random.NextBool())
        {
            treasuresAndQuantities.AddOrUpdate(
                this.Random.NextBool(0.35)
                    ? $"(O){this.Random.Next(538, 579)}"
                    : this.Random.NextBool(0.15)
                        ? $"(O){this.Random.Next(121, 123)}"
                        : $"(O){this.Random.Next(579, 590)}",
                1,
                (a, b) => a + b);
        }
        else
        {
            treasuresAndQuantities.AddOrUpdate(QualifiedObjectIds.Coal, this.Random.Next(1, 4), (a, b) => a + b);
        }
    }

    private void AddGeodesToTreasures(Dictionary<string, int> treasuresAndQuantities, int mineLevel = -1)
    {
        switch (mineLevel)
        {
            case < 0:
                if (this.Random.NextBool(0.1))
                {
                    treasuresAndQuantities.AddOrUpdate(
                        QualifiedObjectIds.OmniGeode,
                        1,
                        (a, b) => a + b);
                }
                else
                {
                    treasuresAndQuantities.AddOrUpdate(
                        $"(O){this.Random.Next(535, 538)}",
                        this.Random.Next(1, 5),
                        (a, b) => a + b);
                }

                break;

            case > 80:
                treasuresAndQuantities.AddOrUpdate(
                    $"(O){537 + (this.Random.NextBool(0.4) ? this.Random.Next(-2, 0) : 0)}", // magma geode or worse
                    this.Random.Next(1, 4),
                    (a, b) => a + b);
                break;

            case > 40:
                treasuresAndQuantities.AddOrUpdate(
                    $"(O){536 + (this.Random.NextBool(0.4) ? -1 : 0)}", // frozen geode or worse
                    this.Random.Next(1, 4),
                    (a, b) => a + b);
                break;

            default:
                treasuresAndQuantities.AddOrUpdate(QualifiedObjectIds.Geode, this.Random.Next(1, 4), (a, b) => a + b);
                break;
        }

        if (this.Random.NextDouble() > 0.05 + (Game1.player.LuckLevel * 0.03))
        {
            return;
        }

        var key = treasuresAndQuantities.Keys.Last();
        treasuresAndQuantities[key] *= 2;
    }

    private void AddMineralsToTreasures(Dictionary<string, int> treasuresAndQuantities, int mineLevel = -1)
    {
        if (mineLevel.IsIn(1..19))
        {
            treasuresAndQuantities.AddOrUpdate(QualifiedObjectIds.Coal, this.Random.Next(1, 4), (a, b) => a + b);
            return;
        }

        switch (mineLevel)
        {
            case < 0:
                treasuresAndQuantities.AddOrUpdate(
                    this.Random.NextBool(0.3) ? QualifiedObjectIds.FireQuartz :
                    $"(O){60 + (this.Random.Next(6) * 2)}", // gemstones
                    this.Random.Next(1, 4),
                    (a, b) => a + b);
                break;

            case > 80:
                treasuresAndQuantities.AddOrUpdate(
                    this.Random.NextBool(0.3) ? QualifiedObjectIds.FireQuartz :
                    this.Random.NextBool() ? QualifiedObjectIds.Ruby : QualifiedObjectIds.Emerald,
                    this.Random.Next(1, 3),
                    (a, b) => a + b);
                break;

            case > 40:
                treasuresAndQuantities.AddOrUpdate(
                    this.Random.NextBool(0.3) ? QualifiedObjectIds.FrozenTear :
                    this.Random.NextBool() ? QualifiedObjectIds.Jade : QualifiedObjectIds.Aquamarine,
                    this.Random.Next(1, 3),
                    (a, b) => a + b);
                break;

            default:
                treasuresAndQuantities.AddOrUpdate(
                    this.Random.NextBool(0.3) ? QualifiedObjectIds.EarthCrystal :
                    this.Random.NextBool() ? QualifiedObjectIds.Amethyst : QualifiedObjectIds.Topaz,
                    this.Random.Next(1, 3),
                    (a, b) => a + b);
                break;
        }

        if (this.Random.NextBool(0.028 * mineLevel / 12))
        {
            treasuresAndQuantities.AddOrUpdate(QualifiedObjectIds.Diamond, 1, (a, b) => a + b);
        }
        else
        {
            treasuresAndQuantities.AddOrUpdate(QualifiedObjectIds.Quartz, this.Random.Next(1, 3), (a, b) => a + b);
        }
    }

    private void AddSpecialTreasureItems(Dictionary<string, int> treasuresAndQuantities, int mineLevel = -1)
    {
        var luckModifier = Math.Max(0, 1.0 + (Game1.player.DailyLuck * Math.Max(mineLevel / 4, 1)));
        if (mineLevel > 0)
        {
            if (this.Random.NextBool(0.025 * luckModifier))
            {
                treasuresAndQuantities.TryAdd("(W)31", 1); // femur
            }
            else if (this.Random.NextBool(0.01 * luckModifier))
            {
                treasuresAndQuantities.TryAdd("(W)60", 1); // ossified blade
            }
        }
        else if (this.Random.NextBool(0.25 * luckModifier))
        {
            treasuresAndQuantities.AddOrUpdate(
                QualifiedObjectIds.DragonTooth,
                1,
                (a, b) => a + b);
        }

        if (this.Random.NextBool(0.01 * luckModifier))
        {
            treasuresAndQuantities.AddOrUpdate(QualifiedObjectIds.PrismaticShard, 1, (a, b) => a + b);
        }

        if (treasuresAndQuantities.Count == 0)
        {
            treasuresAndQuantities.AddOrUpdate(QualifiedObjectIds.Diamond, 1, (a, b) => a + b);
        }
    }
}
