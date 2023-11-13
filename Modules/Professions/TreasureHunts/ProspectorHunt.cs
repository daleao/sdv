namespace DaLion.Overhaul.Modules.Professions.TreasureHunts;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Overhaul.Modules.Professions.Events.Display.RenderedHud;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Events.World.ObjectListChanged;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Networking;
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
            TreasureHuntType.Prospector,
            I18n.Prospector_HuntStarted(),
            I18n.Prospector_HuntFailed(),
            new Rectangle(48, 672, 16, 16))
    {
    }

    /// <inheritdoc />
    public override bool TryStart(GameLocation location)
    {
        if (!location.Objects.Any() || !this.TryStart(location))
        {
            return false;
        }

#if DEBUG
        this.TimeLimit = int.MaxValue;
#elif RELEASE
        this.TimeLimit = (uint)(location.Objects.Count() * ProfessionsModule.Config.ProspectorHuntHandicap);
        this.TimeLimit = Math.Max(this.TimeLimit, 30);
#endif
        EventManager.Enable(
            typeof(ProspectorHuntObjectListChangedEvent),
            typeof(ProspectorHuntRenderedHudEvent),
            typeof(ProspectorHuntUpdateTickedEvent));
        Game1.addHUDMessage(new HuntNotification(this.HuntStartedMessage, this.IconSourceRect));
        if (!Game1.player.HasProfession(Profession.Prospector, true))
        {
            return true;
        }

        if (!Context.IsMultiplayer || Context.IsMainPlayer)
        {
            EventManager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
        }
        else
        {
            ModEntry.Broadcaster.MessageHost("true", OverhaulModule.Professions.Namespace + "/HuntingForTreasure");
        }

        return true;
    }

    /// <inheritdoc />
    public override void ForceStart(GameLocation location, Vector2 target)
    {
        base.ForceStart(location, target);
        this.TimeLimit = (uint)(location.Objects.Count() * ProfessionsModule.Config.ProspectorHuntHandicap);
        EventManager.Enable(
            typeof(ProspectorHuntObjectListChangedEvent),
            typeof(ProspectorHuntRenderedHudEvent),
            typeof(ProspectorHuntUpdateTickedEvent));
        Game1.addHUDMessage(new HuntNotification(this.HuntStartedMessage, this.IconSourceRect));
        if (!Game1.player.HasProfession(Profession.Prospector, true))
        {
            return;
        }

        if (!Context.IsMultiplayer || Context.IsMainPlayer)
        {
            EventManager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
        }
        else
        {
            ModEntry.Broadcaster.MessageHost("true", OverhaulModule.Professions.Namespace + "/HuntingForTreasure");
        }
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
        Game1.player.Increment(DataKeys.ProspectorHuntStreak);
        this.End(true);
    }

    /// <inheritdoc />
    public override void Fail()
    {
        Game1.addHUDMessage(new HuntNotification(this.HuntFailedMessage));
        Game1.player.Write(DataKeys.ProspectorHuntStreak, "0");
        this.End(false);
    }

    /// <inheritdoc />
    protected override Vector2? ChooseTreasureTile(GameLocation location)
    {
        Vector2 v;
        var failSafe = 0;
        do
        {
            if (failSafe > 69) // nice
            {
                return null;
            }

            v = location.Objects.Keys.ElementAtOrDefault(this.Random.Next(location.Objects.Keys.Count()));
            failSafe++;
        }
        while (!location.Objects.TryGetValue(v, out var obj) || !obj.IsStone() || obj.IsResourceNode());

        return v;
    }

    /// <inheritdoc />
    protected override void End(bool success)
    {
        base.End(success);
        EventManager.Disable<ProspectorHuntRenderedHudEvent>();
        EventManager.Disable<ProspectorHuntUpdateTickedEvent>();
        if (!Context.IsMultiplayer || Context.IsMainPlayer ||
            !Game1.player.HasProfession(Profession.Prospector, true))
        {
            return;
        }

        ModEntry.Broadcaster.MessageHost("false", OverhaulModule.Professions.Namespace + "/HuntingForTreasure");
    }

    /// <summary>Spawns hunt spoils as debris. Applies to <see cref="MineShaft"/>.</summary>
    /// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
    private void GetStoneTreasureForMineShaft(int mineLevel)
    {
        Dictionary<int, int> treasuresAndQuantities = new();
        var chance = 1.0;
        var streak = Game1.player.Read<int>(DataKeys.ProspectorHuntStreak);
        var i = 0;
        while (this.Random.NextDouble() < chance)
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
            switch (treasure)
            {
                case -1:
                    Game1.createItemDebris(
                        new MeleeWeapon(31),
                        (this.TreasureTile!.Value * Game1.tileSize) + new Vector2(32f, 32f),
                        this.Random.Next(4),
                        Game1.currentLocation);
                    break;

                case -2:
                    Game1.createItemDebris(
                        new MeleeWeapon(60),
                        (this.TreasureTile!.Value * Game1.tileSize) + new Vector2(32f, 32f),
                        this.Random.Next(4),
                        Game1.currentLocation);
                    break;

                default:
                    Game1.createMultipleObjectDebris(
                        treasure,
                        (int)this.TreasureTile!.Value.X,
                        (int)this.TreasureTile.Value.Y,
                        quantity,
                        Game1.player.UniqueMultiplayerID,
                        Game1.currentLocation);
                    break;
            }
        }
    }

    /// <summary>Spawns hunt spoils as debris. Applies to <see cref="VolcanoDungeon"/>.</summary>
    /// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
    private void GetStoneTreasureForVolcanoDungeon()
    {
        Dictionary<int, int> treasuresAndQuantities = new();
        var chance = 1.0;
        var streak = Game1.player.Read<int>(DataKeys.ProspectorHuntStreak);
        while (this.Random.NextDouble() < chance)
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

        if (treasuresAndQuantities.TryGetValue(ObjectIds.Coal, out var stack) && this.Location is VolcanoDungeon &&
            this.Random.NextDouble() < 0.5)
        {
            treasuresAndQuantities[ObjectIds.CinderShard] = stack;
            treasuresAndQuantities.Remove(ObjectIds.Coal);
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

    private void AddInitialTreasure(IDictionary<int, int> treasuresAndQuantities)
    {
        if (this.Random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        {
            treasuresAndQuantities.AddOrUpdate(
                ObjectIds.QiBean,
                this.Random.Next(1, 3) + (this.Random.NextDouble() < 0.25 ? 2 : 0),
                (a, b) => a + b);
        }
    }

    private void AddOreToTreasure(Dictionary<int, int> treasuresAndQuantities, int mineLevel = -1)
    {
        if (mineLevel > 120 && this.Random.NextDouble() < 0.06)
        {
            treasuresAndQuantities.AddOrUpdate(ObjectIds.IridiumOre, this.Random.Next(1, 3), (a, b) => a + b);
        }

        List<int> possibles = new();
        if (mineLevel < 0 || Game1.mine.GetAdditionalDifficulty() > 0)
        {
            possibles.Add(ObjectIds.RadioactiveOre);
        }

        if (mineLevel is < 0 or > 80)
        {
            possibles.Add(ObjectIds.GoldOre);
        }

        if (mineLevel is < 0 or > 40 && (possibles.Count == 0 || this.Random.NextDouble() < 0.6))
        {
            possibles.Add(ObjectIds.IronOre);
        }

        if (possibles.Count == 0 || this.Random.NextDouble() < 0.6)
        {
            possibles.Add(ObjectIds.CopperOre);
        }

        possibles.Add(ObjectIds.Coal);
        treasuresAndQuantities.AddOrUpdate(
            possibles.ElementAt(this.Random.Next(possibles.Count)),
            this.Random.Next(2, 7) * (this.Random.NextDouble() < 0.05 + (Game1.player.LuckLevel * 0.015)
                ? 2
                : 1),
            (a, b) => a + b);
        if (this.Random.NextDouble() < 0.05 + (Game1.player.LuckLevel * 0.03))
        {
            var key = treasuresAndQuantities.Keys.Last();
            treasuresAndQuantities[key] *= 2;
        }
    }

    private void AddArtifactsToTreasures(Dictionary<int, int> treasuresAndQuantities)
    {
        if (Game1.player.archaeologyFound.Any() && this.Random.NextDouble() < 0.5)
        {
            treasuresAndQuantities.AddOrUpdate(
                this.Random.NextDouble() > 0.35
                    ? this.Random.Next(538, 579)
                    : this.Random.Next(579, 590),
                1,
                (a, b) => a + b);
        }
        else
        {
            treasuresAndQuantities.AddOrUpdate(ObjectIds.Coal, this.Random.Next(1, 4), (a, b) => a + b);
        }
    }

    private void AddGeodesToTreasures(Dictionary<int, int> treasuresAndQuantities, int mineLevel = -1)
    {
        switch (mineLevel)
        {
            case < 0:
                if (this.Random.NextDouble() < 0.1)
                {
                    treasuresAndQuantities.AddOrUpdate(
                        ObjectIds.OmniGeode,
                        1,
                        (a, b) => a + b);
                }
                else
                {
                    treasuresAndQuantities.AddOrUpdate(
                        this.Random.Next(535, 538),
                        this.Random.Next(1, 5),
                        (a, b) => a + b);
                }

                break;

            case > 80:
                treasuresAndQuantities.AddOrUpdate(
                    ObjectIds.MagmaGeode +
                    (this.Random.NextDouble() < 0.4 ? this.Random.Next(-2, 0) : 0),
                    this.Random.Next(1, 4),
                    (a, b) => a + b);
                break;

            case > 40:
                treasuresAndQuantities.AddOrUpdate(
                    ObjectIds.FrozenGeode + (this.Random.NextDouble() < 0.4 ? -1 : 0),
                    this.Random.Next(1, 4),
                    (a, b) => a + b);
                break;

            default:
                treasuresAndQuantities.AddOrUpdate(ObjectIds.Geode, this.Random.Next(1, 4), (a, b) => a + b);
                break;
        }

        if (this.Random.NextDouble() > 0.05 + (Game1.player.LuckLevel * 0.03))
        {
            return;
        }

        var key = treasuresAndQuantities.Keys.Last();
        treasuresAndQuantities[key] *= 2;
    }

    private void AddMineralsToTreasures(Dictionary<int, int> treasuresAndQuantities, int mineLevel = -1)
    {
        if (mineLevel is > 0 and < 20)
        {
            treasuresAndQuantities.AddOrUpdate(ObjectIds.Coal, this.Random.Next(1, 4), (a, b) => a + b);
            return;
        }

        switch (mineLevel)
        {
            case < 0:
                treasuresAndQuantities.AddOrUpdate(
                    this.Random.NextDouble() < 0.3 ? ObjectIds.FireQuartz :
                    60 + (this.Random.Next(6) * 2),
                    this.Random.Next(1, 4),
                    (a, b) => a + b);
                break;

            case > 80:
                treasuresAndQuantities.AddOrUpdate(
                    this.Random.NextDouble() < 0.3 ? ObjectIds.FireQuartz :
                    this.Random.NextDouble() < 0.5 ? ObjectIds.Ruby : ObjectIds.Emerald,
                    this.Random.Next(1, 3),
                    (a, b) => a + b);
                break;

            case > 40:
                treasuresAndQuantities.AddOrUpdate(
                    this.Random.NextDouble() < 0.3 ? ObjectIds.FrozenTear :
                    this.Random.NextDouble() < 0.5 ? ObjectIds.Jade : ObjectIds.Aquamarine,
                    this.Random.Next(1, 3),
                    (a, b) => a + b);
                break;

            default:
                treasuresAndQuantities.AddOrUpdate(
                    this.Random.NextDouble() < 0.3 ? ObjectIds.EarthCrystal :
                    this.Random.NextDouble() < 0.5 ? ObjectIds.Amethyst : ObjectIds.Topaz,
                    this.Random.Next(1, 3),
                    (a, b) => a + b);
                break;
        }

        if (this.Random.NextDouble() < 0.028 * mineLevel / 12)
        {
            treasuresAndQuantities.AddOrUpdate(ObjectIds.Diamond, 1, (a, b) => a + b);
        }
        else
        {
            treasuresAndQuantities.AddOrUpdate(ObjectIds.Quartz, this.Random.Next(1, 3), (a, b) => a + b);
        }
    }

    private void AddSpecialTreasureItems(Dictionary<int, int> treasuresAndQuantities, int mineLevel = -1)
    {
        var luckModifier = Math.Max(0, 1.0 + (Game1.player.DailyLuck * Math.Max(mineLevel / 4, 1)));
        var streak = Game1.player.Read<uint>(DataKeys.ProspectorHuntStreak);
        if (mineLevel > 0)
        {
            if (this.Random.NextDouble() < 0.025 * luckModifier * streak &&
                !Game1.player.specialItems.Contains(31))
            {
                treasuresAndQuantities.TryAdd(-1, 1); // femur
            }

            if (this.Random.NextDouble() < 0.01 * luckModifier * streak &&
                !Game1.player.specialItems.Contains(60))
            {
                treasuresAndQuantities.TryAdd(-2, 1); // ossified blade
            }
        }
        else if (this.Random.NextDouble() < 0.25 * luckModifier)
        {
            treasuresAndQuantities.AddOrUpdate(
                CombatModule.ShouldEnable && CombatModule.Config.DwarvenLegacy &&
                Combat.Integrations.JsonAssetsIntegration.DwarvenScrapIndex is { } dwarvenScrapIndex &&
                this.Random.NextDouble() < 0.4
                    ? dwarvenScrapIndex
                    : ObjectIds.DragonTooth,
                1,
                (a, b) => a + b);
        }

        if (this.Random.NextDouble() < 0.01 * luckModifier * Math.Pow(2, streak))
        {
            treasuresAndQuantities.AddOrUpdate(ObjectIds.PrismaticShard, 1, (a, b) => a + b);
        }

        if (treasuresAndQuantities.Count == 0)
        {
            treasuresAndQuantities.AddOrUpdate(ObjectIds.Diamond, 1, (a, b) => a + b);
        }
    }
}
