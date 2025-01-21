namespace DaLion.Professions.Framework.TreasureHunts;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Professions.Framework.Events.Display.RenderedHud;
using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.Events.World.TerrainFeatureListChanged;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

#endregion using directives

/// <summary>Manages treasure hunt events for Scavenger professions.</summary>
internal sealed class ScavengerHunt : TreasureHunt
{
    private readonly string[] _artifactsThatCanBeFound =
    [
        QIDs.ChippedAmphora, // chipped amphora
        QIDs.Arrowhead, // arrowhead
        QIDs.AncientDoll, // ancient doll
        QIDs.ElvishJewelry, // elvish jewelry
        QIDs.ChewingStick, // chewing stick
        QIDs.OrnamentalFan, // ornamental fan
        QIDs.AncientSword, // ancient sword
        QIDs.AncientSeed, // ancient seed
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
    }

    /// <inheritdoc />
    public override bool TryStart(double chance)
    {
        return !ReferenceEquals(this.Location, Game1.currentLocation) && base.TryStart(chance);
    }

    /// <inheritdoc />
    public override void Complete()
    {
        if (this.TreasureTile is null || this.Location is null)
        {
            this.End(false);
            return;
        }

        if (!this.Location.terrainFeatures.TryGetValue(this.TreasureTile.Value, out var feature) ||
            feature is not HoeDirt)
        {
            return;
        }

        DelayedAction.functionAfterDelay(this.BeginFindTreasure, 200);
        Game1.playSound("questcomplete");
        Data.Increment(Game1.player, DataKeys.ScavengerHuntStreak);
        this.End(true);
    }

    /// <inheritdoc />
    public override void Fail()
    {
        Game1.addHUDMessage(new HuntNotification(this.HuntFailedMessage));
        Data.Write(Game1.player, DataKeys.ScavengerHuntStreak, "0");
        this.End(false);
    }

    /// <inheritdoc />
    protected override bool TrySetTreasureTile(GameLocation location)
    {
        var mapWidth = location.Map.Layers[0].LayerWidth;
        var mapHeight = location.Map.Layers[0].LayerHeight;
        var boundary = (Vector2 tile) => !location.IsTileBlockedBy(tile) && location.IsTileValidForTreasure(tile);
        var validTiles = Game1.player.Tile.FloodFill(mapWidth, mapHeight, boundary, 50, 100).ToHashSet();
        this.TreasureTile = validTiles.Count > 0 ? validTiles.Choose() : null;
        return this.TreasureTile is not null;
    }

    /// <inheritdoc />
    protected override uint SetTimeLimit()
    {
#if DEBUG
        this.TimeLimit = uint.MaxValue;
#else
        this.TimeLimit = (uint)(this.Location!.Map.DisplaySize.Area / Math.Pow(Game1.tileSize, 2) / 100 *
                                Config.ScavengerHuntHandicap);
        this.TimeLimit = Math.Max(this.TimeLimit, 30);
#endif

        return this.TimeLimit;
    }

    /// <inheritdoc />
    protected override void End(bool success)
    {
        base.End(success);
        EventManager.Disable<ScavengerHuntRenderedHudEvent>();
        EventManager.Disable<ScavengerHuntUpdateTickedEvent>();
        if (!Context.IsMultiplayer || Context.IsMainPlayer ||
            !Game1.player.HasProfession(VanillaProfession.Scavenger, true))
        {
            return;
        }

        Broadcaster.MessageHost("false", "HuntingForTreasure/Scavenger");
    }

    /// <inheritdoc />
    protected override void StartImpl(GameLocation location, Vector2 treasureTile)
    {
        location.MakeTileDiggable(treasureTile);
        EventManager.Enable(
            Context.IsMainPlayer
                ? typeof(ScavengerHuntTerrainFeatureListChangedEvent)
                : typeof(FarmhandScavengerHuntUpdateTickedEvent),
            typeof(ScavengerHuntRenderedHudEvent),
            typeof(ScavengerHuntUpdateTickedEvent));
        Game1.addHUDMessage(new HuntNotification(this.HuntStartedMessage, this.IconSourceRect));
        if (Game1.player.HasProfession(VanillaProfession.Scavenger, true) && (!Context.IsMultiplayer || Context.IsMainPlayer))
        {
            EventManager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
        }
        else
        {
            Broadcaster.MessageHost("true", "HuntingForTreasure/Scavenger");
        }

        base.StartImpl(location, treasureTile);
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
        List<Item> treasures = [];
        var chance = 1.0;
        var streak = Data.ReadAs<int>(Game1.player, DataKeys.ScavengerHuntStreak);
        var i = 0;
        while (this.Random.NextBool(chance))
        {
            chance *= Math.Pow(0.1, 1d / (streak + 1));
            this.AddInitialTreasureItems(treasures);
            switch (this.Random.Next(4))
            {
                case 0:
                    this.AddOreToTreasures(treasures);
                    break;
                case 1:
                    this.AddBaitToTreasures(treasures);
                    break;
                case 2:
                    this.AddArtifactsToTreasures(treasures);
                    break;
                case 3:
                    switch (this.Random.Next(3))
                    {
                        case 0:
                            this.AddGeodesToTreasures(treasures);
                            break;
                        case 1:
                            this.AddMineralsToTreasures(treasures);
                            break;
                        case 2:
                            this.AddSpecialTreasureItems(treasures);
                            break;
                    }

                    break;
            }

            i++;
        }

        Log.D($"[ScavengerHunt] Produced {i} treasures.");

        if (treasures.Count <= 2)
        {
            this.AddSeedsToTreasures(treasures);
        }

        return treasures;
    }

    private void AddInitialTreasureItems(List<Item> treasures)
    {
        if (Game1.currentSeason == "spring" && Game1.currentLocation is not Beach && this.Random.NextBool(0.1))
        {
            var stack = this.Random.Next(2, 6) + (this.Random.NextBool(0.25) ? 5 : 0);
            treasures.Add(ItemRegistry.Create(QIDs.RiceShoot, stack));
        }

        if (this.Random.NextBool(0.33) && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        {
            var stack = this.Random.Next(1, 3) + (this.Random.NextBool(0.25) ? 2 : 0);
            treasures.Add(ItemRegistry.Create(QIDs.QiBean, stack));
        }
    }

    private void AddOreToTreasures(List<Item> treasures)
    {
        List<string> possibles = [];

        if (this.Random.NextBool(0.4))
        {
            possibles.Add(QIDs.IridiumOre);
        }

        if (possibles.Count == 0 || this.Random.NextBool(0.4))
        {
            possibles.Add(QIDs.GoldOre);
        }

        if (possibles.Count == 0 || this.Random.NextBool(0.4))
        {
            possibles.Add(QIDs.IronOre);
        }

        if (possibles.Count == 0 || this.Random.NextBool(0.4))
        {
            possibles.Add(QIDs.CopperOre);
        }

        if (possibles.Count == 0 || this.Random.NextBool(0.4))
        {
            possibles.Add(QIDs.Wood);
        }

        if (possibles.Count == 0 || this.Random.NextBool(0.4))
        {
            possibles.Add(QIDs.Stone);
        }

        possibles.Add(QIDs.Coal);

        var id = possibles.ElementAt(this.Random.Next(possibles.Count));
        var stack = this.Random.Next(2, 7) *
                    (this.Random.NextBool(0.05 + (Game1.player.LuckLevel * 0.015)) ? 2 : 1);
        treasures.Add(ItemRegistry.Create(id, stack));
        if (this.Random.NextBool(0.05 + (Game1.player.LuckLevel * 0.03)))
        {
            treasures.Last().Stack *= 2;
        }
    }

    private void AddBaitToTreasures(List<Item> treasures)
    {
        if (this.Random.NextBool(0.25) && Game1.player.craftingRecipes.ContainsKey("Wild Bait"))
        {
            var stack = 5 + (this.Random.NextBool(0.25) ? 5 : 0);
            treasures.Add(ItemRegistry.Create(QIDs.WildBait, stack));
        }
        else
        {
            treasures.Add(ItemRegistry.Create(QIDs.Bait, 10));
        }
    }

    private void AddArtifactsToTreasures(List<Item> treasures)
    {
        if (this.Random.NextBool(0.1) && Game1.netWorldState.Value.LostBooksFound < 21 &&
            Game1.player.hasOrWillReceiveMail("lostBookFound"))
        {
            treasures.Add(ItemRegistry.Create(QIDs.LostBook));
        }
        else if (Game1.player.archaeologyFound.Any() && this.Random.NextBool())
        {
            var id = this.Random.NextBool()
                ? this._artifactsThatCanBeFound[this.Random.Next(this._artifactsThatCanBeFound.Length)]
                : this.Random.NextBool(0.25)
                    ? QIDs.AncientSeed
                    : QIDs.Geode;
            treasures.Add(ItemRegistry.Create(id));
        }
        else
        {
            var stack = this.Random.Next(1, 3);
            treasures.Add(ItemRegistry.Create(QIDs.Coal, stack));
        }
    }

    private void AddGeodesToTreasures(List<Item> treasures)
    {
        var id = this.Random.Next(535, 538);
        var stack = this.Random.Next(1, 4);
        treasures.Add(ItemRegistry.Create($"(O){id}", stack));
        if (this.Random.NextBool(0.05 + (Game1.player.LuckLevel * 0.03)))
        {
            treasures[^1].Stack *= 2;
        }
    }

    private void AddMineralsToTreasures(List<Item> treasures)
    {
        switch (this.Random.Next(4))
        {
            case 0:
            {
                var id = this.Random.NextBool(0.3)
                    ? QIDs.FireQuartz
                    : this.Random.NextBool()
                        ? QIDs.Ruby
                        : QIDs.Emerald;
                var stack = this.Random.Next(1, 3);
                treasures.Add(ItemRegistry.Create(id, stack));
                break;
            }

            case 1:
            {
                var id = this.Random.NextBool(0.3)
                    ? QIDs.FrozenTear
                    : this.Random.NextBool()
                        ? QIDs.Jade
                        : QIDs.Aquamarine;
                var stack = this.Random.Next(1, 3);
                treasures.Add(ItemRegistry.Create(id, stack));
                break;
            }

            case 2:
            {
                var id = this.Random.NextBool(0.3)
                    ? QIDs.EarthCrystal
                    : this.Random.NextBool()
                        ? QIDs.Amethyst
                        : QIDs.Topaz;
                var stack = this.Random.Next(1, 3);
                treasures.Add(ItemRegistry.Create(id, stack));
                break;
            }

            case 3:
            {
                treasures.Add(this.Random.NextBool(0.28)
                    ? ItemRegistry.Create(QIDs.Diamond)
                    : ItemRegistry.Create(QIDs.Quartz, this.Random.Next(1, 3)));
                break;
            }
        }

        if (this.Random.NextBool(0.05))
        {
            treasures[^1].Stack *= 2;
        }
    }

    private void AddSpecialTreasureItems(List<Item> treasures)
    {
        var luckModifier = 1.0 + (Game1.player.DailyLuck * 10);
        if (this.Random.NextBool(0.25 * luckModifier))
        {
            if (this.Random.NextBool(0.05 * luckModifier))
            {
                treasures.Add(new MeleeWeapon(QIDs.ForestSword));
            }
        }
        else if (this.Random.NextBool(0.25 * luckModifier))
        {
            if (this.Random.NextBool(0.05 * luckModifier))
            {
                treasures.Add(new MeleeWeapon(QIDs.ElfBlade));
            }
        }

        if (this.Random.NextBool(0.07 * luckModifier))
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

        if (this.Random.NextBool(0.01 * luckModifier))
        {
            treasures.Add(ItemRegistry.Create(QIDs.TreasureChest));
        }

        if (this.Random.NextBool(0.005 * luckModifier))
        {
            treasures.Add(ItemRegistry.Create(QIDs.PrismaticShard));
        }

        if (this.Random.NextBool(0.01 * luckModifier))
        {
            treasures.Add(ItemRegistry.Create(QIDs.StrangeDoll0));
        }

        if (this.Random.NextBool(0.01 * luckModifier))
        {
            treasures.Add(ItemRegistry.Create(QIDs.StrangeDoll1));
        }

        if (this.Random.NextBool(0.01 * luckModifier))
        {
            treasures.Add(ItemRegistry.Create(QIDs.IridiumBand));
        }

        if (this.Random.NextBool(0.01 * luckModifier))
        {
            treasures.Add(ItemRegistry.Create($"(B){this.Random.Next(504, 514)}"));
        }

        if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") &&
            this.Random.NextBool(0.01 * luckModifier))
        {
            treasures.Add(ItemRegistry.Create(QIDs.GoldenEgg));
        }

        if (treasures.Count == 1)
        {
            treasures.Add(ItemRegistry.Create(QIDs.Diamond));
        }
    }

    private void AddSeedsToTreasures(List<Item> treasures)
    {
        if (this.Random.NextBool(0.4))
        {
            switch (Game1.currentSeason)
            {
                case "spring":
                    treasures.Add(ItemRegistry.Create(QIDs.SpringSeeds));
                    break;

                case "summer":
                    treasures.Add(ItemRegistry.Create(QIDs.SummerSeeds));
                    break;

                case "fall":
                    treasures.Add(ItemRegistry.Create(QIDs.FallSeeds));
                    break;

                case "winter":
                    treasures.Add(ItemRegistry.Create(QIDs.WinterSeeds));
                    break;
            }
        }
        else
        {
            var stack = this.Random.Next(1, 4) * 5;
            treasures.Add(ItemRegistry.Create(QIDs.MixedSeeds, stack));
        }
    }
}
