namespace DaLion.Professions.Framework.Events.Content;

#region using directives

using System.Collections.Generic;
using DaLion.Professions.Framework.Integrations;
using DaLion.Shared.Content;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.FarmAnimals;
using StardewValley.GameData.FishPonds;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Objects;
using StardewValley.Menus;
using xTile;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProfessionAssetRequestedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionAssetRequestedEvent(EventManager? manager = null)
    : AssetRequestedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void Initialize()
    {
        this.Edit("Data/achievements", new AssetEditor(EditAchievementsData));
        this.Edit("Data/BigCraftables", new AssetEditor(EditBigCraftablesData));
        this.Edit("Data/Buildings", new AssetEditor(EditBuildingsData));
        this.Edit("Data/FarmAnimals", new AssetEditor(EditFarmAnimalsData));
        this.Edit("Data/FishPondData", new AssetEditor(EditFishPondDataData, AssetEditPriority.Early));
        this.Edit("Data/mail", new AssetEditor(EditMailData));
        this.Edit("Data/Machines", new AssetEditor(EditMachinesData));
        this.Edit("Data/Objects", new AssetEditor(EditObjectsData));
        this.Edit("LooseSprites/Cursors", new AssetEditor(EditCursorsLooseSprites));
        this.Edit("Maps/Barn3", new AssetEditor(EditDeluxeBarnMap));
        this.Edit("Maps/Coop3", new AssetEditor(EditDeluxeCoopMap));
        this.Edit("Maps/SlimeHutch", new AssetEditor(EditSlimeHutchMap));
        this.Edit("TileSheets/BuffsIcons", new AssetEditor(EditBuffsIconsTileSheets));

        this.Provide(
            $"{UniqueId}_AnimalDerivedGoods",
            new ModDictionaryProvider<string, string[]>(() => "assets/data/AnimalDerivedGoods.json"));
        this.Provide(
            $"{UniqueId}_ArtisanMachines",
            new ModDictionaryProvider<string, string[]>(() => "assets/data/ArtisanMachines.json"));
        this.Provide(
            $"{UniqueId}_HudPointer",
            new ModTextureProvider(() => "assets/sprites/pointer.png"));
        this.Provide(
            $"{UniqueId}_MaxIcon",
            new ModTextureProvider(() => "assets/sprites/max.png"));
        this.Provide(
            $"{UniqueId}_PrestigeRibbons",
            new ModTextureProvider(() => "assets/sprites/StackedStars.png"));
        this.Provide(
            $"{UniqueId}_ProfessionIcons",
            new ModTextureProvider(() => $"assets/sprites/professions_{Config.Masteries.PrestigeProfessionIconStyle}.png"));
        this.Provide(
            $"{UniqueId}_MasteredSkillIcons",
            new ModTextureProvider(() => $"assets/sprites/skills_{Config.Masteries.MasteredSkillIconStyle}.png"));
        this.Provide(
            $"{UniqueId}_SkillBars",
            new ModTextureProvider(ProvideSkillBars));
        this.Provide(
            $"{UniqueId}_LimitGauge",
            new ModTextureProvider(ProvideLimitGauge));
        this.Provide(
            $"{UniqueId}_Mayo",
            new ModTextureProvider(() => "assets/sprites/mayo.png"));
    }

    #region editor callback

    /// <summary>Patches achievements data with prestige achievements.</summary>
    private static void EditAchievementsData(IAssetData asset)
    {
        var data = asset.AsDictionary<int, string>().Data;

        string title = _I18n.Get("prestige.achievement.title" + (Game1.player.IsMale ? ".male" : ".female"));
        string desc = _I18n.Get("prestige.achievement.desc" + (Game1.player.IsMale ? ".male" : ".female"));

        const string shouldDisplayBeforeEarned = "false";
        const string prerequisite = "-1";
        const string hatIndex = "";

        var newEntry = string.Join("^", [title, desc, shouldDisplayBeforeEarned, prerequisite, hatIndex]);
        data[title.GetDeterministicHashCode()] = newEntry;
    }

    /// <summary>Removes Heavy Tapper multiplier.</summary>
    private static void EditBigCraftablesData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, BigCraftableData>().Data;
        if (Config.ImmersiveHeavyTapperYield)
        {
            var id = QualifiedBigCraftableIds.HeavyTapper.SplitWithoutAllocation(')')[1].ToString();
            data[id].ContextTags.Remove("tapper_multiplier_2");
        }
    }

    /// <summary>Patches buffs icons with modded profession buff icons.</summary>
    private static void EditBuffsIconsTileSheets(IAssetData asset)
    {
        var editor = asset.AsImage();
        editor.ExtendImage(editor.Data.Width, 80);

        var targetArea = new Rectangle(0, 64, 160, 16);
        editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/sprites/buffs"), null, targetArea);
    }

    /// <summary>Patches Feed Hopper position for upgraded Deluxe Barn map.</summary>
    private static void EditBuildingsData(IAssetData asset)
    {
        if (!Context.IsWorldReady || Game1.player is null)
        {
            return;
        }

        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Breeder, true, true))
        {
            asset.AsDictionary<string, BuildingData>().Data["Deluxe Barn"].IndoorItems[0].Tile.X -= 2;
        }
    }

    /// <summary>Patches cursors with modded profession icons.</summary>
    private static void EditCursorsLooseSprites(IAssetData asset)
    {
        var editor = asset.AsImage();
        var sourceTx = ModHelper.GameContent.Load<Texture2D>($"{UniqueId}_ProfessionIcons");
        var sourceArea = new Rectangle(0, 0, 96, 80);
        var targetArea = new Rectangle(0, 624, 96, 80);
        editor.PatchImage(sourceTx, sourceArea, targetArea);
        if (!Context.IsWorldReady || Game1.player is null)
        {
            return;
        }

        foreach (var profession in Profession.List)
        {
            if (!Game1.player.HasProfession(profession, true) &&
                (Game1.activeClickableMenu is not LevelUpMenu || profession.ParentSkill.CurrentLevel <= 10))
            {
                continue;
            }

            sourceArea = profession.SourceSheetRect;
            sourceArea.Y += 80;
            targetArea = profession.TargetSheetRect;
            editor.PatchImage(sourceTx, sourceArea, targetArea);
        }
    }

    /// <summary>Patches upgraded Deluxe Barn map.</summary>
    private static void EditDeluxeBarnMap(IAssetData asset)
    {
        if (!Context.IsWorldReady || Game1.player is null)
        {
            return;
        }

        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Breeder, true, true))
        {
            asset.AsMap().ReplaceWith(ModHelper.ModContent.Load<Map>("assets/maps/Barn3.tmx"));
        }
    }

    /// <summary>Patches upgraded Deluxe Coop map.</summary>
    private static void EditDeluxeCoopMap(IAssetData asset)
    {
        if (!Context.IsWorldReady || Game1.player is null)
        {
            return;
        }

        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Producer, true, true))
        {
            asset.AsMap().ReplaceWith(ModHelper.ModContent.Load<Map>("assets/maps/Coop3.tmx"));
        }
    }

    /// <summary>Patches upgraded Slime Hutch map.</summary>
    private static void EditSlimeHutchMap(IAssetData asset)
    {
        if (!Context.IsWorldReady || Game1.player is null)
        {
            return;
        }

        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Piper, true, true))
        {
            asset.AsMap().ReplaceWith(ModHelper.ModContent.Load<Map>("assets/maps/SlimeHutch.tmx"));
        }
    }

    /// <summary>Patches animal data for Producer perk.</summary>
    private static void EditFarmAnimalsData(IAssetData asset)
    {
        asset.AsDictionary<string, FarmAnimalData>().Data.ForEach(pair =>
        {
            pair.Value.ProfessionForHappinessBoost = Profession.Rancher;
            pair.Value.ProfessionForFasterProduce = Profession.Producer;
            pair.Value.ProfessionForQualityBoost = -1;
        });
    }

    /// <summary>Patches fish pond data with legendary fish data.</summary>
    private static void EditFishPondDataData(IAssetData asset)
    {
        ((List<FishPondData>)asset.Data).AddRange(
            [
                new FishPondData
                {
                    Id = UniqueId + "/Angler",
                    PopulationGates = null,
                    ProducedItems =
                    [
                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.1f,
                            ItemId = QualifiedObjectIds.CopperOre,
                            MinStack = 10,
                            MaxStack = 15,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.01f,
                            ItemId = QualifiedObjectIds.SolarEssence,
                            MinStack = 10,
                            MaxStack = 20,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 1.0f,
                            ItemId = QualifiedObjectIds.Roe,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.8f,
                            ItemId = QualifiedObjectIds.Roe,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                    ],
                    RequiredTags =
                    [
                        "item_angler",
                    ],
                    SpawnTime = -1,
                    Precedence = 0,
                },
                new FishPondData
                {
                    Id = UniqueId + "/Glacierfish",
                    PopulationGates = null,
                    ProducedItems =
                    [
                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.05f,
                            ItemId = QualifiedObjectIds.FrozenGeode,
                            MinStack = 5,
                            MaxStack = 10,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.075f,
                            ItemId = QualifiedObjectIds.FrozenTear,
                            MinStack = 5,
                            MaxStack = 10,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.1f,
                            ItemId = QualifiedObjectIds.IronOre,
                            MinStack = 10,
                            MaxStack = 10,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.01f,
                            ItemId = QualifiedObjectIds.Diamond,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 1.0f,
                            ItemId = QualifiedObjectIds.Roe,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.8f,
                            ItemId = QualifiedObjectIds.Roe,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                    ],
                    RequiredTags =
                    [
                        "item_glacierfish",
                    ],
                    SpawnTime = -1,
                    Precedence = 0,
                },
                new FishPondData
                {
                    Id = UniqueId + "/Crimsonfish",
                    PopulationGates = null,
                    ProducedItems =
                    [
                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.05f,
                            ItemId = QualifiedObjectIds.MagmaGeode,
                            MinStack = 5,
                            MaxStack = 10,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.075f,
                            ItemId = QualifiedObjectIds.FireQuartz,
                            MinStack = 5,
                            MaxStack = 10,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.1f,
                            ItemId = QualifiedObjectIds.GoldOre,
                            MinStack = 10,
                            MaxStack = 10,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.033f,
                            ItemId = QualifiedObjectIds.CherryBomb,
                            MinStack = 1,
                            MaxStack = 3,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.02f,
                            ItemId = QualifiedObjectIds.ExplosiveAmmo,
                            MinStack = 1,
                            MaxStack = 3,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.01f,
                            ItemId = QualifiedObjectIds.MegaBomb,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 1.0f,
                            ItemId = QualifiedObjectIds.Roe,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.8f,
                            ItemId = QualifiedObjectIds.Roe,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                    ],
                    RequiredTags =
                    [
                        "item_crimsonfish",
                    ],
                    SpawnTime = -1,
                    Precedence = 0,
                },
                new FishPondData
                {
                    Id = UniqueId + "/Legend",
                    PopulationGates = null,
                    ProducedItems =
                    [
                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.1f,
                            ItemId = QualifiedObjectIds.IridiumOre,
                            MinStack = 5,
                            MaxStack = 10,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 1.0f,
                            ItemId = QualifiedObjectIds.Roe,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.8f,
                            ItemId = QualifiedObjectIds.Roe,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                    ],
                    RequiredTags =
                    [
                        "item_legend",
                    ],
                    SpawnTime = -1,
                    Precedence = 0,
                },
                new FishPondData
                {
                    Id = UniqueId + "/MutantCarp",
                    PopulationGates = null,
                    ProducedItems =
                    [
                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.1f,
                            ItemId = QualifiedObjectIds.RadioactiveOre,
                            MinStack = 5,
                            MaxStack = 15,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 1.0f,
                            ItemId = QualifiedObjectIds.Roe,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.8f,
                            ItemId = QualifiedObjectIds.Roe,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                    ],
                    RequiredTags =
                    [
                        "item_mutant_carp",
                    ],
                    SpawnTime = -1,
                    Precedence = 0,
                },
                new FishPondData
                {
                    Id = UniqueId + "/Tui",
                    PopulationGates = null,
                    ProducedItems =
                    [
                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 1f,
                            ItemId = QualifiedObjectIds.SolarEssence,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.8f,
                            ItemId = QualifiedObjectIds.SolarEssence,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                    ],
                    RequiredTags =
                    [
                        "item_tui",
                    ],
                    SpawnTime = -1,
                    Precedence = 0,
                },
                new FishPondData
                {
                    Id = UniqueId + "/La",
                    PopulationGates = null,
                    ProducedItems =
                    [
                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 1f,
                            ItemId = QualifiedObjectIds.VoidEssence,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                        new FishPondReward
                        {
                            RequiredPopulation = 0,
                            Chance = 0.8f,
                            ItemId = QualifiedObjectIds.VoidEssence,
                            MinStack = 1,
                            MaxStack = 1,
                        },

                    ],
                    RequiredTags =
                    [
                        "item_la",
                    ],
                    SpawnTime = -1,
                    Precedence = 0,
                },
                new FishPondData
                {
                    PopulationGates = null,
                    ProducedItems =
                    [
                        new FishPondReward
                        {
                            Chance = 1f, ItemId = QualifiedObjectIds.Roe, MinStack = 1, MaxStack = 1,
                        },

                    ],
                    RequiredTags = ["fish_legendary"],
                    SpawnTime = -1,
                    Precedence = 100,
                },
            ]);
    }

    /// <summary>Patches mail data with mail from the Ferngill Revenue Service.</summary>
    private static void EditMailData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        var taxBonus =
            Data.ReadAs<float>(Game1.player, DataKeys.ConservationistActiveTaxDeduction);
        var key = taxBonus >= Config.ConservationistTaxDeductionCeiling
            ? "conservationist.mail.max"
            : "conservationist.mail";

        string honorific = _I18n.Get("honorific" + (Game1.player.IsMale ? ".male" : ".female"));
        var farm = Game1.player.farmName;
        var season = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr
            ? _I18n.Get("season." + Game1.currentSeason)
            : Game1.CurrentSeasonDisplayName;

        string message = _I18n.Get(
            key, new { honorific, taxBonus = FormattableString.CurrentCulture($"{taxBonus:0%}"), farm, season });
        data[$"{UniqueId}_ConservationistTaxNotice"] = message;
    }

    /// <summary>Patches machine rules for new mayo objects.</summary>
    private static void EditMachinesData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, MachineData>().Data;

        if (Config.EnableGoldenOstrichMayo)
        {
            var rule = data[QualifiedBigCraftableIds.MayonnaiseMachine]
                .OutputRules
                .First(r => r.Id == "Default_OstrichEgg");
            var output = rule.OutputItem.Single();
            output.Id = $"(O){OstrichMayoId}";
            output.ItemId = $"(O){OstrichMayoId}";
            output.MinStack = -1;
            output.CopyQuality = false;

            rule = data[QualifiedBigCraftableIds.MayonnaiseMachine]
                .OutputRules
                .First(r => r.Id == "Default_GoldenEgg");
            output = rule.OutputItem.Single();
            output.Id = $"(O){GoldenMayoId}";
            output.ItemId = $"(O){GoldenMayoId}";
            output.MinStack = -1;
            output.Quality = -1;
        }

        if (Config.ImmersiveDairyYield)
        {
            var rule = data[QualifiedBigCraftableIds.MayonnaiseMachine]
                .OutputRules
                .First(rule => rule.Id == "Default_LargeEgg");
            var output = rule.OutputItem.Single();
            output.Quality = -1;
            output.MinStack = 2;

            rule = data[QualifiedBigCraftableIds.CheesePress]
                .OutputRules
                .First(rule => rule.Id == "Default_LargeMilk");
            output = rule.OutputItem.Single();
            output.Quality = -1;
            output.MinStack = 2;

            rule = data[QualifiedBigCraftableIds.CheesePress]
                .OutputRules
                .First(rule => rule.Id == "Default_LargeGoatMilk");
            output = rule.OutputItem.Single();
            output.Quality = -1;
            output.MinStack = 2;
        }
    }

    /// <summary>Patches new mayo objects.</summary>
    private static void EditObjectsData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, ObjectData>().Data;
        if (Config.EnableGoldenOstrichMayo)
        {
            data[$"{OstrichMayoId}"] = new ObjectData
            {
                Name = "Delight Mayonnaise",
                DisplayName = I18n.Objects_Ostrichmayo_Name(),
                Description = I18n.Objects_Ostrichmayo_Desc(),
                Type = "Basic",
                Category = (int)ObjectCategory.ArtisanGoods,
                Price = 2000,
                Texture = $"{UniqueId}_Mayo",
                SpriteIndex = 1,
                Edibility = 50,
                IsDrink = true,
                ContextTags =
                [
                    "color_white",
                    "mayo_item",
                ],
            };

            data[$"{GoldenMayoId}"] = new ObjectData
            {
                Name = "Shiny Mayonnaise",
                DisplayName = I18n.Objects_Goldenmayo_Name(),
                Description = I18n.Objects_Goldenmayo_Desc(),
                Type = "Basic",
                Category = (int)ObjectCategory.ArtisanGoods,
                Price = 2500,
                Texture = $"{UniqueId}_Mayo",
                SpriteIndex = 0,
                Edibility = 20,
                IsDrink = true,
                ContextTags =
                [
                    "color_gold",
                    "mayo_item",
                ],
                ExcludeFromShippingCollection = true,
            };
        }

        if (!Context.IsWorldReady || !Game1.player.HasProfession(Profession.Aquarist))
        {
            return;
        }

        foreach (var value in data.Values)
        {
            if (value.ContextTags?.Contains("fish_legendary") == true)
            {
                value.ContextTags.Remove("fish_pond_ignore");
            }
        }
    }

    #endregion editor callbacks

    #region provider callbacks

    /// <summary>Provides the correct skill bars texture path.</summary>
    private static string ProvideSkillBars()
    {
        var path = "assets/sprites/skillbars";
        if (ModHelper.ModRegistry.IsLoaded("ManaKirel.VMI") ||
            ModHelper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2"))
        {
            path += "_vintage";
        }

        return path + ".png";
    }

    /// <summary>Provides the correct <see cref="Limits.LimitGauge"/> texture path.</summary>
    private static string ProvideLimitGauge()
    {
        var path = "assets/sprites/gauge";
        if (SveIntegration.Instance?.IsLoaded == true)
        {
            if (!SveIntegration.Instance.DisabeGaldoranTheme &&
                (Game1.currentLocation?.NameOrUniqueName.IsAnyOf(
                     "Custom_CastleVillageOutpost",
                     "Custom_CrimsonBadlands",
                     "Custom_IridiumQuarry",
                     "Custom_TreasureCave") == true ||
                 SveIntegration.Instance.UseGaldoranThemeAllTimes))
            {
                return path + "_galdora.png";
            }
        }

        if (ModHelper.ModRegistry.IsLoaded("ManaKirel.VMI"))
        {
            path += "_vintage_pink";
        }
        else if (ModHelper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2"))
        {
            path += "_vintage_brown";
        }

        return path + ".png";
    }

    #endregion provider callbacks
}
