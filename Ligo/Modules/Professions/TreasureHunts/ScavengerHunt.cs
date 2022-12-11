namespace DaLion.Ligo.Modules.Professions.TreasureHunts;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Ligo.Modules.Professions.Events.Display;
using DaLion.Ligo.Modules.Professions.Events.GameLoop;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions;
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
        100, // chipped amphora
        101, // arrowhead
        103, // ancient doll
        104, // elvish jewelry
        105, // chewing stick
        106, // ornamental fan
        109, // ancient sword
        114, // ancient seed
        115, // prehistoric tool
        118, // glass shards
        119, // bone flute
        120, // prehistoric hand-axe
        123, // ancient drum
        124, // golden mask
        125, // golden relic
        126, // strange doll
        127, // strange doll
    };

    /// <summary>Initializes a new instance of the <see cref="ScavengerHunt"/> class.</summary>
    internal ScavengerHunt()
        : base(
            TreasureHuntType.Scavenger,
            i18n.Get("scavenger.huntstarted"),
            i18n.Get("scavenger.huntfailed"),
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
        this.TimeLimit = (uint)(location.Map.DisplaySize.Area / Math.Pow(Game1.tileSize, 2) / 100 *
                                ProfessionsModule.Config.ScavengerHuntHandicap);
        this.TimeLimit = Math.Max(this.TimeLimit, 30);
        this.Elapsed = 0;
        EventManager.Enable<PointerUpdateTickedEvent>();
        EventManager.Enable<ScavengerHuntRenderedHudEvent>();
        EventManager.Enable<ScavengerHuntUpdateTickedEvent>();
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
        EventManager.Enable<PointerUpdateTickedEvent>();
        EventManager.Enable<ScavengerHuntRenderedHudEvent>();
        EventManager.Enable<ScavengerHuntUpdateTickedEvent>();
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
    public override void Fail()
    {
        Game1.addHUDMessage(new HuntNotification(this.HuntFailedMessage));
        Game1.player.Write(DataFields.ScavengerHuntStreak, "0");
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
    protected override void CheckForCompletion()
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
        Game1.player.Increment(DataFields.ScavengerHuntStreak);
        this.End(true);
    }

    /// <inheritdoc />
    protected override void End(bool found)
    {
        Game1.player.Get_IsHuntingTreasure().Value = false;
        EventManager.Disable<ScavengerHuntRenderedHudEvent>();
        EventManager.Disable<ScavengerHuntUpdateTickedEvent>();
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
        while (this.Random.NextDouble() <= chance)
        {
            chance *= 0.4f;
            this.AddInitialTreasureItems(treasures);

            switch (this.Random.Next(4))
            {
                case 0:
                {
                    this.AddOreToTreasures(treasures);
                    break;
                }

                case 1:
                {
                    this.AddBaitToTreasures(treasures);
                    break;
                }

                case 2:
                {
                    this.AddArtifactsToTreasures(treasures);
                    break;
                }

                case 3:
                {
                    switch (this.Random.Next(3))
                    {
                        case 0:
                            this.AddGeodesToTreasures(treasures);
                            break;
                        case 1:
                            this.AddGemstonesToTreasures(treasures);
                            break;
                        case 2:
                            this.AddSpecialTreasureItems(treasures);
                            break;
                    }

                    break;
                }
            }
        }

        this.AddSeedsToTreasures(treasures);
        return treasures;
    }

    private void AddInitialTreasureItems(List<Item> treasures)
    {
        // rice shoot
        if (Game1.currentSeason == "spring" && Game1.currentLocation is not Beach && this.Random.NextDouble() < 0.1)
        {
            var stack = this.Random.Next(2, 6) + (this.Random.NextDouble() < 0.25 ? 5 : 0);
            treasures.Add(new SObject(273, stack));
        }

        // qi beans
        if (this.Random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        {
            var stack = this.Random.Next(1, 3) + (this.Random.NextDouble() < 0.25 ? 2 : 0);
            treasures.Add(new SObject(890, stack));
        }
    }

    private void AddOreToTreasures(List<Item> treasures)
    {
        List<int> possibles = new();

        // iridium ore
        if (this.Random.NextDouble() < 0.4)
        {
            possibles.Add(386);
        }

        // gold ore
        if (possibles.Count == 0 || this.Random.NextDouble() < 0.4)
        {
            possibles.Add(384);
        }

        // iron ore
        if (possibles.Count == 0 || this.Random.NextDouble() < 0.4)
        {
            possibles.Add(380);
        }

        // copper ore
        if (possibles.Count == 0 || this.Random.NextDouble() < 0.4)
        {
            possibles.Add(378);
        }

        // wood
        if (possibles.Count == 0 || this.Random.NextDouble() < 0.4)
        {
            possibles.Add(388);
        }

        // stone
        if (possibles.Count == 0 || this.Random.NextDouble() < 0.4)
        {
            possibles.Add(390);
        }

        // coal
        possibles.Add(382);

        var index = possibles.ElementAt(this.Random.Next(possibles.Count));
        var stack = this.Random.Next(2, 7) *
                    (!(this.Random.NextDouble() < 0.05 + (Game1.player.LuckLevel * 0.015)) ? 1 : 2);
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
            // wild bait
            var stack = 5 + (this.Random.NextDouble() < 0.25 ? 5 : 0);
            treasures.Add(new SObject(774, stack));
        }
        else
        {
            // bait
            treasures.Add(new SObject(685, 10));
        }
    }

    private void AddArtifactsToTreasures(List<Item> treasures)
    {
        if (this.Random.NextDouble() < 0.1 && Game1.netWorldState.Value.LostBooksFound.Value < 21 &&
            Game1.player.hasOrWillReceiveMail("lostBookFound"))
        {
            // lost book
            treasures.Add(new SObject(102, 1));
        }
        else if (Game1.player.archaeologyFound.Any() && this.Random.NextDouble() < 0.5)
        {
            // artifacts
            var index = this.Random.NextDouble() < 0.5
                ? this._artifactsThatCanBeFound[this.Random.Next(this._artifactsThatCanBeFound.Length)]
                : this.Random.NextDouble() < 0.25
                    ? 114
                    : 535;
            treasures.Add(new SObject(index, 1));
        }
        else
        {
            // coal
            var stack = this.Random.Next(1, 3);
            treasures.Add(new SObject(382, stack));
        }
    }

    private void AddGeodesToTreasures(List<Item> treasures)
    {
        var index = this.Random.Next(535, 538);
        var stack = this.Random.Next(1, 4);
        treasures.Add(new SObject(index, stack)); // geodes
        if (this.Random.NextDouble() < 0.05 + (Game1.player.LuckLevel * 0.03))
        {
            treasures[^1].Stack *= 2;
        }
    }

    private void AddGemstonesToTreasures(List<Item> treasures)
    {
        switch (this.Random.Next(4))
        {
            // fire quartz else ruby or emerald
            case 0:
            {
                var index = this.Random.NextDouble() < 0.3
                    ? 82
                    : this.Random.NextDouble() < 0.5
                        ? 64
                        : 60;
                var stack = this.Random.Next(1, 3);
                treasures.Add(new SObject(index, stack));
                break;
            }

            // frozen tear else jade or aquamarine
            case 1:
            {
                var index = this.Random.NextDouble() < 0.3
                    ? 84
                    : this.Random.NextDouble() < 0.5
                        ? 70
                        : 62;
                var stack = this.Random.Next(1, 3);
                treasures.Add(new SObject(index, stack));
                break;
            }

            // earth crystal else amethyst or topaz
            case 2:
            {
                var index = this.Random.NextDouble() < 0.3
                    ? 86
                    : this.Random.NextDouble() < 0.5
                        ? 66
                        : 68;
                var stack = this.Random.Next(1, 3);
                treasures.Add(new SObject(index, stack));
                break;
            }

            case 3:
            {
                treasures.Add(this.Random.NextDouble() < 0.28
                    ? new SObject(72, 1) // diamond
                    : new SObject(80, this.Random.Next(1, 3))); // quartz
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
        var streak = Game1.player.Read<uint>(DataFields.ScavengerHuntStreak);

        // forest sword
        if (this.Random.NextDouble() < 0.25 * luckModifier)
        {
            if (Config.EnableArsenal && ArsenalModule.Config.DwarvishCrafting && Globals.DwarvishBlueprintIndex.HasValue)
            {
                if (!Game1.player.Read(DataFields.BlueprintsFound).ParseList<int>()
                        .Contains(Constants.ForestSwordIndex))
                {
                    treasures.Add(new SObject(Globals.DwarvishBlueprintIndex.Value, 1));
                }
                else if (Globals.ElderwoodIndex.HasValue)
                {
                    treasures.Add(new SObject(Globals.ElderwoodIndex.Value, 1));
                }
            }
            else if (this.Random.NextDouble() < 0.05 * luckModifier * streak)
            {
                treasures.Add(new MeleeWeapon(Constants.ForestSwordIndex));
            }
        }
        else
        // elf blade
        if (this.Random.NextDouble() < 0.25 * luckModifier)
        {
            if (Config.EnableArsenal && ArsenalModule.Config.DwarvishCrafting && Globals.DwarvishBlueprintIndex.HasValue)
            {
                if (!Game1.player.Read(DataFields.BlueprintsFound).ParseList<int>()
                        .Contains(Constants.ElfBladeIndex))
                {
                    treasures.Add(new SObject(Globals.DwarvishBlueprintIndex.Value, 1));
                }
                else if (Globals.ElderwoodIndex.HasValue)
                {
                    treasures.Add(new SObject(Globals.ElderwoodIndex.Value, 1));
                }
            }
            else if (this.Random.NextDouble() < 0.05 * luckModifier * streak)
            {
                treasures.Add(new MeleeWeapon(Constants.ElfBladeIndex));
            }
        }

        if (this.Random.NextDouble() < 0.07 * luckModifier)
        {
            switch (this.Random.Next(3))
            {
                // (small) glow ring
                case 0:
                {
                    var index = 516 + (this.Random.NextDouble() < Game1.player.LuckLevel / 11f
                        ? 1
                        : 0);
                    treasures.Add(new Ring(index));
                    break;
                }

                // (small) magnet ring
                case 1:
                {
                    var index = 518 + (this.Random.NextDouble() < Game1.player.LuckLevel / 11f
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

        // treasure chest
        if (this.Random.NextDouble() < 0.01 * luckModifier * Math.Pow(2, streak))
        {
            treasures.Add(new SObject(166, 1));
        }

        // prismatic shard
        if (this.Random.NextDouble() < 0.005 * luckModifier * Math.Pow(2, streak))
        {
            treasures.Add(new SObject(74, 1));
        }

        // strange doll
        if (this.Random.NextDouble() < 0.01 * luckModifier)
        {
            treasures.Add(new SObject(126, 1));
        }

        // strange doll
        if (this.Random.NextDouble() < 0.01 * luckModifier)
        {
            treasures.Add(new SObject(127, 1));
        }

        // iridium band
        if (this.Random.NextDouble() < 0.01 * luckModifier)
        {
            treasures.Add(new Ring(527));
        }

        // boots
        if (this.Random.NextDouble() < 0.01 * luckModifier)
        {
            treasures.Add(new Boots(this.Random.Next(504, 514)));
        }

        if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") &&
            this.Random.NextDouble() < 0.01 * luckModifier)
        {
            treasures.Add(new SObject(928, 1)); // golden egg
        }

        if (treasures.Count == 1)
        {
            treasures.Add(new SObject(72, 1)); // consolation diamond
        }
    }

    private void AddSeedsToTreasures(List<Item> treasures)
    {
        if (this.Random.NextDouble() < 0.4)
        {
            // forage seeds
            switch (Game1.currentSeason)
            {
                case "spring":
                    treasures.Add(new SObject(495, 1));
                    break;

                case "summer":
                    treasures.Add(new SObject(496, 1));
                    break;

                case "fall":
                    treasures.Add(new SObject(497, 1));
                    break;

                case "winter":
                    treasures.Add(new SObject(498, 1));
                    break;
            }
        }
        else
        {
            // wild seeds
            var stack = this.Random.Next(1, 4) * 5;
            treasures.Add(new SObject(770, stack));
        }
    }
}
