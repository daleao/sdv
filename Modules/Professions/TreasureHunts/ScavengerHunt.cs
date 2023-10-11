namespace DaLion.Overhaul.Modules.Professions.TreasureHunts;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Overhaul.Modules.Professions.Events.Display.RenderedHud;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Events.World.TerrainFeatureListChanged;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Networking;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

#endregion using directives

/// <summary>Manages treasure hunt events for Scavenger professions.</summary>
internal sealed class ScavengerHunt : TreasureHunt
{
    private readonly int[] _artifactsThatCanBeFound =
    {
        ObjectIds.ChippedAmphora, // chipped amphora
        ObjectIds.Arrowhead, // arrowhead
        ObjectIds.AncientDoll, // ancient doll
        ObjectIds.ElvishJewelry, // elvish jewelry
        ObjectIds.ChewingStick, // chewing stick
        ObjectIds.OrnamentalFan, // ornamental fan
        ObjectIds.AncientSword, // ancient sword
        ObjectIds.AncientSeed, // ancient seed
        ObjectIds.PrehistoricTool, // prehistoric tool
        ObjectIds.GlassShards, // glass shards
        ObjectIds.BoneFlute, // bone flute
        ObjectIds.PrehistoricHandaxe, // prehistoric hand-axe
        ObjectIds.AncientDrum, // ancient drum
        ObjectIds.GoldenMask, // golden mask
        ObjectIds.GoldenRelic, // golden relic
        ObjectIds.StrangeDoll0, // strange doll
        ObjectIds.StrangeDoll1, // strange doll
    };

    /// <summary>Initializes a new instance of the <see cref="ScavengerHunt"/> class.</summary>
    internal ScavengerHunt()
        : base(
            TreasureHuntType.Scavenger,
            I18n.Scavenger_HuntStarted(),
            I18n.Scavenger_HuntFailed(),
            new Rectangle(80, 656, 16, 16))
    {
    }

    /// <inheritdoc />
    public override bool TryStart(GameLocation location)
    {
        if (ReferenceEquals(this.Location, location) || !this.TryStart())
        {
            return false;
        }

        this.TreasureTile = this.ChooseTreasureTile(location);
        if (this.TreasureTile is null)
        {
            return false;
        }

        this.Location = location;
        this.Location.MakeTileDiggable(this.TreasureTile.Value);
#if DEBUG
        this.TimeLimit = int.MaxValue;
#elif RELEASE
        this.TimeLimit = (uint)(location.Map.DisplaySize.Area / Math.Pow(Game1.tileSize, 2) / 100 *
                                ProfessionsModule.Config.ScavengerHuntHandicap);
        this.TimeLimit = Math.Max(this.TimeLimit, 30);
#endif
        this.Elapsed = 0;
        EventManager.Enable(
            typeof(ScavengerHuntTerrainFeatureListChangedEvent),
            typeof(ScavengerHuntRenderedHudEvent),
            typeof(ScavengerHuntUpdateTickedEvent));
        HudPointer.Instance.Value.ShouldBob = true;
        Game1.addHUDMessage(new HuntNotification(this.HuntStartedMessage, this.IconSourceRect));
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat($"{Game1.player.Name} is hunting for treasure.");

            if (Game1.player.HasProfession(Profession.Scavenger, true))
            {
                Game1.player.Get_IsHuntingTreasure().Value = true;
                if (!Context.IsMainPlayer)
                {
                    ModEntry.Broadcaster.MessagePeer("HuntIsOn", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
                }
                else
                {
                    EventManager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
                }
            }
        }

        this.OnStarted();
        return true;
    }

    /// <inheritdoc />
    public override void ForceStart(GameLocation location, Vector2 target)
    {
        this.ForceStart();
        this.TreasureTile = target;
        this.Location = location;
        this.Location.MakeTileDiggable(this.TreasureTile.Value);
        this.TimeLimit = (uint)(location.Map.DisplaySize.Area / Math.Pow(Game1.tileSize, 2) / 100 *
                                ProfessionsModule.Config.ScavengerHuntHandicap);
        this.TimeLimit = Math.Max(this.TimeLimit, 30);
        this.Elapsed = 0;
        EventManager.Enable(
            typeof(ScavengerHuntTerrainFeatureListChangedEvent),
            typeof(ScavengerHuntRenderedHudEvent),
            typeof(ScavengerHuntUpdateTickedEvent));
        HudPointer.Instance.Value.ShouldBob = true;
        Game1.addHUDMessage(new HuntNotification(this.HuntStartedMessage, this.IconSourceRect));
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat($"{Game1.player.Name} is hunting for treasure.");

            if (Game1.player.HasProfession(Profession.Scavenger, true))
            {
                Game1.player.Get_IsHuntingTreasure().Value = true;
                if (!Context.IsMainPlayer)
                {
                    ModEntry.Broadcaster.MessagePeer("HuntIsOn", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
                }
                else
                {
                    EventManager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
                }
            }
        }

        this.OnStarted();
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

        var getTreasure = new DelayedAction(200, this.BeginFindTreasure);
        Game1.delayedActions.Add(getTreasure);
        Game1.player.Increment(DataKeys.ScavengerHuntStreak);
        this.End(true);
    }

    /// <inheritdoc />
    public override void Fail()
    {
        Game1.addHUDMessage(new HuntNotification(this.HuntFailedMessage));
        Game1.player.Write(DataKeys.ScavengerHuntStreak, "0");
        this.End(false);
    }

    /// <inheritdoc />
    protected override Vector2? ChooseTreasureTile(GameLocation location)
    {
        Vector2 tile;
        var failSafe = 0;
        do
        {
            if (failSafe > 69)
            {
                return null;
            }

            var x = this.Random.Next(location.Map.DisplayWidth / Game1.tileSize);
            var y = this.Random.Next(location.Map.DisplayHeight / Game1.tileSize);
            tile = location.getRandomTile();
            failSafe++;
        }
        while (!location.IsTileValidForTreasure(tile));

        return tile;
    }

    /// <inheritdoc />
    protected override void End(bool found)
    {
        Game1.player.Get_IsHuntingTreasure().Value = false;
        EventManager.Disable<ScavengerHuntRenderedHudEvent>();
        EventManager.Disable<ScavengerHuntUpdateTickedEvent>();
        HudPointer.Instance.Value.ShouldBob = false;
        this.TreasureTile = null;
        if (!Context.IsMultiplayer || Context.IsMainPlayer ||
            !Game1.player.HasProfession(Profession.Scavenger, true))
        {
            return;
        }

        Broadcaster.SendPublicChat(found
            ? $"{Game1.player.Name} has found the treasure!"
            : $"{Game1.player.Name} failed to find the treasure.");
        ModEntry.Broadcaster.MessagePeer("HuntIsOff", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);

        this.OnEnded(found);
    }

    /// <summary>Plays treasure chest found animation.</summary>
    private void BeginFindTreasure()
    {
        Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(
            PathUtilities.NormalizeAssetName("LooseSprites/Cursors"),
            new Rectangle(64, 1920, 32, 32),
            500f,
            1,
            0,
            Game1.player.Position + new Vector2(-32f, -160f),
            false,
            false,
            (Game1.player.getStandingY() / 10000f) + 0.001f,
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
            PathUtilities.NormalizeAssetName("LooseSprites/Cursors"),
            new Rectangle(64, 1920, 32, 32),
            200f,
            4,
            0,
            Game1.player.Position + new Vector2(-32f, -228f),
            false,
            false,
            (Game1.player.getStandingY() / 10000f) + 0.001f,
            0f,
            Color.White,
            4f,
            0f,
            0f,
            0f)
        {
            endFunction = this.OpenTreasureMenuEndFunction, extraInfoForEndBehavior = 0,
        });
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
        List<Item> treasures = new();
        var chance = 1.0;
        var streak = Game1.player.Read<int>(DataKeys.ScavengerHuntStreak);
        var i = 0;
        while (this.Random.NextDouble() < chance)
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

#if DEBUG
        if (CombatModule.ShouldEnable && CombatModule.Config.DwarvenLegacy &&
            JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue)
        {
            if (!Game1.player.Read(Combat.DataKeys.BlueprintsFound).ParseList<int>()
                    .ContainsAll(WeaponIds.ElfBlade, WeaponIds.ForestSword))
            {
                treasures.Add(new SObject(JsonAssetsIntegration.DwarvishBlueprintIndex.Value, 1));
            }
            else if (JsonAssetsIntegration.ElderwoodIndex.HasValue)
            {
                treasures.Add(new SObject(JsonAssetsIntegration.ElderwoodIndex.Value, 1));
            }
        }
        else
        {
            treasures.Add(new MeleeWeapon(WeaponIds.ElfBlade));
        }
#endif

        return treasures;
    }

    private void AddInitialTreasureItems(List<Item> treasures)
    {
        if (Game1.currentSeason == "spring" && Game1.currentLocation is not Beach && this.Random.NextDouble() < 0.1)
        {
            var stack = this.Random.Next(2, 6) + (this.Random.NextDouble() < 0.25 ? 5 : 0);
            treasures.Add(new SObject(ObjectIds.RiceShoot, stack));
        }

        if (this.Random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        {
            var stack = this.Random.Next(1, 3) + (this.Random.NextDouble() < 0.25 ? 2 : 0);
            treasures.Add(new SObject(ObjectIds.QiBean, stack));
        }
    }

    private void AddOreToTreasures(List<Item> treasures)
    {
        List<int> possibles = new();

        if (this.Random.NextDouble() < 0.4)
        {
            possibles.Add(ObjectIds.IridiumOre);
        }

        if (possibles.Count == 0 || this.Random.NextDouble() < 0.4)
        {
            possibles.Add(ObjectIds.GoldOre);
        }

        if (possibles.Count == 0 || this.Random.NextDouble() < 0.4)
        {
            possibles.Add(ObjectIds.IronOre);
        }

        if (possibles.Count == 0 || this.Random.NextDouble() < 0.4)
        {
            possibles.Add(ObjectIds.CopperOre);
        }

        if (possibles.Count == 0 || this.Random.NextDouble() < 0.4)
        {
            possibles.Add(ObjectIds.Wood);
        }

        if (possibles.Count == 0 || this.Random.NextDouble() < 0.4)
        {
            possibles.Add(ObjectIds.Stone);
        }

        possibles.Add(ObjectIds.Coal);

        var index = possibles.ElementAt(this.Random.Next(possibles.Count));
        var stack = this.Random.Next(2, 7) *
                    (this.Random.NextDouble() < 0.05 + (Game1.player.LuckLevel * 0.015) ? 2 : 1);
        treasures.Add(new SObject(index, stack));
        if (this.Random.NextDouble() < 0.05 + (Game1.player.LuckLevel * 0.03))
        {
            treasures.Last().Stack *= 2;
        }
    }

    private void AddBaitToTreasures(List<Item> treasures)
    {
        if (this.Random.NextDouble() < 0.25 && Game1.player.craftingRecipes.ContainsKey("Wild Bait"))
        {
            var stack = 5 + (this.Random.NextDouble() < 0.25 ? 5 : 0);
            treasures.Add(new SObject(ObjectIds.WildBait, stack));
        }
        else
        {
            treasures.Add(new SObject(ObjectIds.Bait, 10));
        }
    }

    private void AddArtifactsToTreasures(List<Item> treasures)
    {
        if (this.Random.NextDouble() < 0.1 && Game1.netWorldState.Value.LostBooksFound.Value < 21 &&
            Game1.player.hasOrWillReceiveMail("lostBookFound"))
        {
            treasures.Add(new SObject(ObjectIds.LostBook, 1));
        }
        else if (Game1.player.archaeologyFound.Any() && this.Random.NextDouble() < 0.5)
        {
            var index = this.Random.NextDouble() < 0.5
                ? this._artifactsThatCanBeFound[this.Random.Next(this._artifactsThatCanBeFound.Length)]
                : this.Random.NextDouble() < 0.25
                    ? 114
                    : 535;
            treasures.Add(new SObject(index, 1));
        }
        else
        {
            var stack = this.Random.Next(1, 3);
            treasures.Add(new SObject(ObjectIds.Coal, stack));
        }
    }

    private void AddGeodesToTreasures(List<Item> treasures)
    {
        var index = this.Random.Next(535, 538);
        var stack = this.Random.Next(1, 4);
        treasures.Add(new SObject(index, stack));
        if (this.Random.NextDouble() < 0.05 + (Game1.player.LuckLevel * 0.03))
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
                var index = this.Random.NextDouble() < 0.3
                    ? ObjectIds.FireQuartz
                    : this.Random.NextDouble() < 0.5
                        ? ObjectIds.Ruby
                        : ObjectIds.Emerald;
                var stack = this.Random.Next(1, 3);
                treasures.Add(new SObject(index, stack));
                break;
            }

            case 1:
            {
                var index = this.Random.NextDouble() < 0.3
                    ? ObjectIds.FrozenTear
                    : this.Random.NextDouble() < 0.5
                        ? ObjectIds.Jade
                        : ObjectIds.Aquamarine;
                var stack = this.Random.Next(1, 3);
                treasures.Add(new SObject(index, stack));
                break;
            }

            case 2:
            {
                var index = this.Random.NextDouble() < 0.3
                    ? ObjectIds.EarthCrystal
                    : this.Random.NextDouble() < 0.5
                        ? ObjectIds.Amethyst
                        : ObjectIds.Topaz;
                var stack = this.Random.Next(1, 3);
                treasures.Add(new SObject(index, stack));
                break;
            }

            case 3:
            {
                treasures.Add(this.Random.NextDouble() < 0.28
                    ? new SObject(ObjectIds.Diamond, 1)
                    : new SObject(ObjectIds.Quartz, this.Random.Next(1, 3)));
                break;
            }
        }

        if (this.Random.NextDouble() < 0.05)
        {
            treasures[^1].Stack *= 2;
        }
    }

    private void AddSpecialTreasureItems(List<Item> treasures)
    {
        var luckModifier = 1.0 + (Game1.player.DailyLuck * 10);
        var streak = Game1.player.Read<uint>(DataKeys.ScavengerHuntStreak);

        if (this.Random.NextDouble() < 0.25 * luckModifier)
        {
            if (CombatModule.ShouldEnable && CombatModule.Config.DwarvenLegacy &&
                JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue)
            {
                if (!Game1.player.Read(Combat.DataKeys.BlueprintsFound).ParseList<int>()
                        .Contains(WeaponIds.ForestSword))
                {
                    treasures.Add(new SObject(JsonAssetsIntegration.DwarvishBlueprintIndex.Value, 1));
                }
                else if (JsonAssetsIntegration.ElderwoodIndex.HasValue)
                {
                    treasures.Add(new SObject(JsonAssetsIntegration.ElderwoodIndex.Value, 1));
                }
            }
            else if (this.Random.NextDouble() < 0.05 * luckModifier * streak)
            {
                treasures.Add(new MeleeWeapon(WeaponIds.ForestSword));
            }
        }
        else if (this.Random.NextDouble() < 0.25 * luckModifier)
        {
            if (CombatModule.ShouldEnable && CombatModule.Config.DwarvenLegacy &&
                JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue)
            {
                if (!Game1.player.Read(Combat.DataKeys.BlueprintsFound).ParseList<int>()
                        .Contains(WeaponIds.ElfBlade))
                {
                    treasures.Add(new SObject(JsonAssetsIntegration.DwarvishBlueprintIndex.Value, 1));
                }
                else if (JsonAssetsIntegration.ElderwoodIndex.HasValue)
                {
                    treasures.Add(new SObject(JsonAssetsIntegration.ElderwoodIndex.Value, 1));
                }
            }
            else if (this.Random.NextDouble() < 0.05 * luckModifier * streak)
            {
                treasures.Add(new MeleeWeapon(WeaponIds.ElfBlade));
            }
        }

        if (this.Random.NextDouble() < 0.07 * luckModifier)
        {
            switch (this.Random.Next(3))
            {
                case 0:
                {
                    var index = ObjectIds.SmallGlowRing + (this.Random.NextDouble() < Game1.player.LuckLevel / 11f
                        ? 1
                        : 0);
                    treasures.Add(new Ring(index));
                    break;
                }

                case 1:
                {
                    var index = ObjectIds.SmallMagnetRing + (this.Random.NextDouble() < Game1.player.LuckLevel / 11f
                        ? 1
                        : 0);
                    treasures.Add(new Ring(index));
                    break;
                }

                // gemstone ring
                case 2:
                {
                    var index = this.Random.Next(529, 535);
                    treasures.Add(new Ring(index));
                    break;
                }
            }
        }

        if (this.Random.NextDouble() < 0.01 * luckModifier * Math.Pow(2, streak))
        {
            treasures.Add(new SObject(ObjectIds.TreasureChest, 1));
        }

        if (this.Random.NextDouble() < 0.005 * luckModifier * Math.Pow(2, streak))
        {
            treasures.Add(new SObject(ObjectIds.PrismaticShard, 1));
        }

        if (this.Random.NextDouble() < 0.01 * luckModifier)
        {
            treasures.Add(new SObject(ObjectIds.StrangeDoll0, 1));
        }

        if (this.Random.NextDouble() < 0.01 * luckModifier)
        {
            treasures.Add(new SObject(ObjectIds.StrangeDoll1, 1));
        }

        if (this.Random.NextDouble() < 0.01 * luckModifier)
        {
            treasures.Add(new Ring(ObjectIds.IridiumBand));
        }

        if (this.Random.NextDouble() < 0.01 * luckModifier)
        {
            treasures.Add(new Boots(this.Random.Next(504, 514))); // boots
        }

        if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") &&
            this.Random.NextDouble() < 0.01 * luckModifier)
        {
            treasures.Add(new SObject(ObjectIds.GoldenEgg, 1));
        }

        if (treasures.Count == 1)
        {
            treasures.Add(new SObject(ObjectIds.Diamond, 1));
        }
    }

    private void AddSeedsToTreasures(List<Item> treasures)
    {
        if (this.Random.NextDouble() < 0.4)
        {
            switch (Game1.currentSeason)
            {
                case "spring":
                    treasures.Add(new SObject(ObjectIds.SpringSeeds, 1));
                    break;

                case "summer":
                    treasures.Add(new SObject(ObjectIds.SummerSeeds, 1));
                    break;

                case "fall":
                    treasures.Add(new SObject(ObjectIds.FallSeeds, 1));
                    break;

                case "winter":
                    treasures.Add(new SObject(ObjectIds.WinterSeeds, 1));
                    break;
            }
        }
        else
        {
            var stack = this.Random.Next(1, 4) * 5;
            treasures.Add(new SObject(ObjectIds.MixedSeeds, stack));
        }
    }
}
