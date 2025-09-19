namespace DaLion.Professions.Framework.Events.Content;

#region using directives

using System.Collections.Generic;
using DaLion.Professions.Framework.Integrations;
using DaLion.Shared.Content;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI.Events;
using StardewValley.GameData;
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
        this.Edit("Data/CraftingRecipes", new AssetEditor(EditCraftingRecipesData, AssetEditPriority.Early));
        this.Edit("Data/FarmAnimals", new AssetEditor(EditFarmAnimalsData, AssetEditPriority.Late));
        this.Edit("Data/FishPondData", new AssetEditor(EditFishPondDataData, AssetEditPriority.Early));
        this.Edit("Data/mail", new AssetEditor(EditMailData));
        this.Edit("Data/Machines", new AssetEditor(EditMachinesData, (AssetEditPriority)int.MaxValue));
        this.Edit("Data/NPCGiftTastes", new AssetEditor(EditNPCGiftTastesData));
        this.Edit("Data/Objects", new AssetEditor(EditObjectsData, AssetEditPriority.Early));
        this.Edit("LooseSprites/Cursors", new AssetEditor(EditCursorsLooseSprites));
        this.Edit("Maps/Barn3", new AssetEditor(EditDeluxeBarnMap, AssetEditPriority.Late));
        this.Edit("Maps/Coop3", new AssetEditor(EditDeluxeCoopMap, AssetEditPriority.Late));
        this.Edit("Maps/SVE_PremiumBarn", new AssetEditor(EditPremiumBarnMap, AssetEditPriority.Late));
        this.Edit("Maps/SVE_PremiumCoop", new AssetEditor(EditPremiumCoopMap, AssetEditPriority.Late));
        this.Edit("Maps/SlimeHutch", new AssetEditor(EditSlimeHutchMap));
        this.Edit("TileSheets/BuffsIcons", new AssetEditor(EditBuffsIconsTileSheets));

        this.Provide(
            $"{UniqueId}_AnimalDerivedGoods", new DictionaryProvider<string, string[]>(ProvideAnimalDerivedGoods));
        this.Provide(
            $"{UniqueId}_ArtisanMachines", new DictionaryProvider<string, string[]>(ProvideArtisanMachines));
        this.Provide(
            $"{UniqueId}_LegendaryFishPondData", new DictionaryProvider<string, List<FishPondData>>(ProvideLegendaryFishPondData));
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
            new ModTextureProvider(() => $"assets/sprites/professions_{Config.Masteries.GoldSpritePalette}.png"));
        this.Provide(
            $"{UniqueId}_MasteredSkillIcons",
            new ModTextureProvider(() => $"assets/sprites/skills_{Config.Masteries.GoldSpritePalette}.png"));
        this.Provide(
            $"{UniqueId}_SkillBars",
            new ModTextureProvider(ProvideSkillBars));
        this.Provide(
            $"{UniqueId}_LimitGauge",
            new ModTextureProvider(ProvideLimitGauge));
        this.Provide(
            $"{UniqueId}_Mayo",
            new ModTextureProvider(() => "assets/sprites/mayo.png"));
        this.Provide(
            $"{UniqueId}_SlimeGoods",
            new ModTextureProvider(() => "assets/sprites/slimegoods.png"));
        this.Provide(
            $"{UniqueId}_GoldSlime",
            new ModTextureProvider(() => $"assets/sprites/slime_{Config.Masteries.GoldSpritePalette}.png"));
        this.Provide(
            $"{UniqueId}_DirtArrow",
            new ModTextureProvider(() => $"assets/sprites/dirtarrow_{(ModHelper.ModRegistry.IsLoaded("Acerbicon.Recolor") ? "Wittily" : "Vanilla")}.png"));
        this.Provide(
            $"{UniqueId}_Highlight",
            new ModTextureProvider(() => "assets/sprites/highlight.png"));
        this.Provide(
            $"{UniqueId}_Minion",
            new ModTextureProvider(() => "assets/sprites/minion.png"));
        this.Provide(
            $"{UniqueId}_Brushes",
            new ModTextureProvider(() => "assets/sprites/PaintBrush.png"));
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
            var id = QIDs.HeavyTapper.SplitWithoutAllocation(')')[1].ToString();
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

    /// <summary>Patches Tapper recipes for Foraging professions.</summary>
    private static void EditCraftingRecipesData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;

        var redBrushRecipe =
            $"{QIDs.Wood} 1 {QIDs.Fiber} 1 {QIDs.RedSlimeEgg} 1/Field/{RedBrushId}/false/none/";
        var greenBrushRecipe =
            $"{QIDs.Wood} 1 {QIDs.Fiber} 1 {QIDs.GreenSlimeEgg} 1/Field/{GreenBrushId}/false/none/";
        var blueBrushRecipe =
            $"{QIDs.Wood} 1 {QIDs.Fiber} 1 {QIDs.BlueSlimeEgg} 1/Field/{BlueBrushId}/false/none/";
        var purpleBrushRecipe =
            $"{QIDs.Wood} 1 {QIDs.Fiber} 1 {QIDs.PurpleSlimeEgg} 1/Field/{PurpleBrushId}/false/none/";
        var prismaticBrushRecipe =
            $"{QIDs.Wood} 1 {QIDs.Fiber} 1 {QIDs.PrismaticJelly} 1/Field/{PrismaticBrushId}/false/none/";

        data["Red Paintbrush"] = redBrushRecipe;
        data["Green Paintbrush"] = greenBrushRecipe;
        data["Blue Paintbrush"] = blueBrushRecipe;
        data["Purple Paintbrush"] = purpleBrushRecipe;
        data["Prismatic Paintbrush"] = prismaticBrushRecipe;
        if (!Context.IsWorldReady || (!Game1.player?.HasProfession(Profession.Tapper) ?? false))
        {
            return;
        }

        string[] fields;
        if (ModHelper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP") &&
            (ModHelper.ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP")?.Value<bool?>("BalancedCrafting") ?? false))
        {
            fields = data["Tapper"].Split('/');
            fields[0] = $"{QIDs.Hardwood} 2 {QIDs.CopperBar} 1";
            data["Tapper"] = string.Join('/', fields);
            goto heavyTapper;
        }

        fields = data["Tapper"].Split('/');
        fields[0] = $"{QIDs.Wood} 20 {QIDs.CopperBar} 1";
        data["Tapper"] = string.Join('/', fields);

    heavyTapper:
        fields = data["Heavy Tapper"].Split('/');
        fields[0] = $"{QIDs.Hardwood} 15 {QIDs.RadioactiveBar} 1";
        data["Heavy Tapper"] = string.Join('/', fields);
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

    /// <summary>Patches upgraded Deluxe Barn map.</summary>
    private static void EditPremiumBarnMap(IAssetData asset)
    {
        if (!Context.IsWorldReady || Game1.player is null)
        {
            return;
        }

        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Breeder, true, true))
        {
            asset.AsMap().ReplaceWith(ModHelper.ModContent.Load<Map>("assets/maps/PremiumBarn.tmx"));
        }
    }

    /// <summary>Patches upgraded Deluxe Coop map.</summary>
    private static void EditPremiumCoopMap(IAssetData asset)
    {
        if (!Context.IsWorldReady || Game1.player is null)
        {
            return;
        }

        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Producer, true, true))
        {
            asset.AsMap().ReplaceWith(ModHelper.ModContent.Load<Map>("assets/maps/PremiumCoop.tmx"));
        }
    }

    /// <summary>Patches upgraded Slime Hutch map.</summary>
    private static void EditSlimeHutchMap(IAssetData asset)
    {
        if (!Context.IsWorldReady || Game1.player is null)
        {
            return;
        }

        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Piper, true))
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
        var assetData = (List<FishPondData>)asset.Data;
        var modData =
            ModHelper.GameContent.Load<Dictionary<string, List<FishPondData>>>($"{UniqueId}_LegendaryFishPondData")[
                "LegendaryFishPondData"];
        assetData.AddRange(modData);
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

    /// <summary>Patches machine rules for new mayo and other objects.</summary>
    private static void EditMachinesData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, MachineData>().Data;

        if (Config.EnableGoldenOstrichMayo)
        {
            var rule = data[QIDs.MayonnaiseMachine]
                .OutputRules
                .First(r => r.Id == "Default_OstrichEgg");
            var output = rule.OutputItem.Single();
            output.Id = $"(O){OstrichMayoId}";
            output.ItemId = $"(O){OstrichMayoId}";
            output.MinStack = -1;
            output.CopyQuality = false;

            rule = data[QIDs.MayonnaiseMachine]
                .OutputRules
                .First(r => r.Id == "Default_GoldenEgg");
            output = rule.OutputItem.Single();
            output.Id = $"(O){GoldenMayoId}";
            output.ItemId = $"(O){GoldenMayoId}";
            output.MinStack = -1;
            output.Quality = -1;
        }

        if (Config.EnableSlimeGoods)
        {
            data[QIDs.MayonnaiseMachine]
                .OutputRules
                .Add(new MachineOutputRule()
                    {
                        Id = $"{UniqueId}_Slime",
                        Triggers =
                        [
                            new MachineOutputTriggerRule
                            {
                                Id = "ItemPlacedInMachine",
                                Trigger = MachineOutputTrigger.ItemPlacedInMachine,
                                RequiredItemId = QIDs.Slime,
                                RequiredCount = 10,
                            },
                        ],
                        OutputItem =
                        [
                            new MachineItemOutput { Id = $"(O){SlimeMayoId}", ItemId = $"(O){SlimeMayoId}", }
                        ],
                        MinutesUntilReady = 180,
                    });

            data[QIDs.CheesePress]
                .OutputRules
                .Add(new MachineOutputRule()
                    {
                        Id = $"{UniqueId}_Slime",
                        Triggers =
                        [
                            new MachineOutputTriggerRule
                            {
                                Id = "ItemPlacedInMachine",
                                Trigger = MachineOutputTrigger.ItemPlacedInMachine,
                                RequiredItemId = QIDs.Slime,
                                RequiredCount = 25,
                            },
                        ],
                        OutputItem =
                        [
                            new MachineItemOutput { Id = $"(O){SlimeCheeseId}", ItemId = $"(O){SlimeCheeseId}", }
                        ],
                        MinutesUntilReady = 200,
                    });
        }

        if (Config.ImmersiveDairyYield)
        {
            foreach (var (_, machine) in data)
            {
                if (machine.OutputRules is null)
                {
                    continue;
                }

                Func<MachineOutputRule, bool> appliesToLargeDairyItem = r =>
                    (r.Id is not null && r.Id.Contains("Large") && r.Id.ContainsAnyOf("Egg", "Milk")) ||
                    (r.Triggers?.FirstOrDefault() is { RequiredTags: not null } first &&
                     first.RequiredTags.ContainsAny("large_milk_item", "large_egg_item"));
                Func<MachineOutputRule, bool> appliesToEggItem = r =>
                    (r.Id is not null && r.Id.ContainsAnyOf("Egg")) ||
                    (r.Triggers?.FirstOrDefault() is { RequiredTags: not null } first &&
                     first.RequiredTags.ContainsAny("egg_item"));
                Func<MachineOutputRule, bool> appliesToMilkItem = r =>
                    (r.Id is not null && r.Id.ContainsAnyOf("Milk")) ||
                    (r.Triggers?.FirstOrDefault() is { RequiredTags: not null } first &&
                     first.RequiredTags.ContainsAny("milk_item"));
                foreach (var rule in machine.OutputRules)
                {
                    if (appliesToLargeDairyItem(rule))
                    {
                        foreach (var output in rule.OutputItem)
                        {
                            if (output.Quality > 0)
                            {
                                output.Quality = -1;
                            }

                            output.StackModifiers ??= [];
                            output.StackModifiers.Add(new QuantityModifier()
                            {
                                Id = $"{UniqueId}_ImmersiveDairyYield",
                                Modification = QuantityModifier.ModificationType.Multiply,
                                Amount = 2f,
                            });
                        }
                    }
                    else if (appliesToEggItem(rule))
                    {
                        foreach (var output in rule.OutputItem)
                        {
                            if (output.Quality > 0)
                            {
                                output.Quality = -1;
                            }

                            output.StackModifiers ??= [];
                            output.StackModifiers.Add(new QuantityModifier()
                            {
                                Id = $"{UniqueId}_ImmersiveDairyYield",
                                Modification = QuantityModifier.ModificationType.Multiply,
                                Amount = 2f,
                                Condition = "ITEM_CONTEXT_TAG Input large_egg_item",
                            });
                        }
                    }
                    else if (appliesToMilkItem(rule))
                    {
                        foreach (var output in rule.OutputItem)
                        {
                            if (output.Quality > 0)
                            {
                                output.Quality = -1;
                            }

                            output.StackModifiers ??= [];
                            output.StackModifiers.Add(new QuantityModifier()
                            {
                                Id = $"{UniqueId}_ImmersiveDairyYield",
                                Modification = QuantityModifier.ModificationType.Multiply,
                                Amount = 2f,
                                Condition = "ITEM_CONTEXT_TAG Input large_milk_item",
                            });
                        }
                    }
                }
            }
        }
    }

    /// <summary>Patches NPC gift tastes for new mayo and other objects.</summary>
    private static void EditNPCGiftTastesData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        if (Config.EnableGoldenOstrichMayo)
        {
            foreach (var (key, value) in data)
            {
                if (!value.ContainsAllOf("306", "307"))
                {
                    continue;
                }

                var split = value.Split("307");
                data[key] = split[0] + "307 " + OstrichMayoId + ' ' + GoldenMayoId + split[1];
            }
        }

        if (Config.EnableSlimeGoods)
        {
            data["Universal_Hate"] += ' ' + SlimeCheeseId + ' ' + SlimeMayoId;

            var split = data["Krobus"].Split('/');
            split[5] += ' ' + SlimeCheeseId + ' ' + SlimeMayoId;
            data["Krobus"] = string.Join('/', split);

            split = data["Wizard"].Split('/');
            split[5] += ' ' + SlimeCheeseId + ' ' + SlimeMayoId;
            data["Wizard"] = string.Join('/', split);
        }

        data["Universal_Neutral"] += " slime_paint_item";
        var loved = "Leah".Collect("Emily", "Krobus", "Jas", "Vincent", "Leo");
        foreach (var name in loved)
        {
            var split = data[name].Split('/');
            split[1] += " slime_paint_item";
            data[name] = string.Join('/', split);
        }

        var liked = "Penny".Collect("Robin");
        foreach (var name in liked)
        {
            var split = data[name].Split('/');
            split[3] += " slime_paint_item";
            data[name] = string.Join('/', split);
        }

        var disliked = "Haley".Collect("Pam", "Sebastian");
        foreach (var name in disliked)
        {
            var split = data[name].Split('/');
            split[1] += " slime_paint_item";
            data[name] = string.Join('/', split);
        }
    }

    /// <summary>Patches new mayo and other objects.</summary>
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

        if (Config.EnableSlimeGoods)
        {
            data[$"{SlimeMayoId}"] = new ObjectData
            {
                Name = "Slime Mayonnaise",
                DisplayName = I18n.Objects_Slimemayo_Name(),
                Description = I18n.Objects_Slimemayo_Desc(),
                Type = "Basic",
                Category = (int)ObjectCategory.ArtisanGoods,
                Price = 190,
                Texture = $"{UniqueId}_SlimeGoods",
                SpriteIndex = 0,
                Edibility = -30,
                IsDrink = true,
                ContextTags =
                [
                    "color_green",
                    "mayo_item",
                ],
            };

            data[$"{SlimeCheeseId}"] = new ObjectData
            {
                Name = "Slime Cheese",
                DisplayName = I18n.Objects_Slimecheese_Name(),
                Description = I18n.Objects_Slimecheese_Desc(),
                Type = "Basic",
                Category = (int)ObjectCategory.ArtisanGoods,
                Price = 230,
                Texture = $"{UniqueId}_SlimeGoods",
                SpriteIndex = 1,
                Edibility = -30,
                IsDrink = true,
                ContextTags =
                [
                    "color_green",
                    "cheese_item",
                ],
            };
        }

        data[$"{RedBrushId}"] = new ObjectData
        {
            Name = "Red Paintbrush",
            DisplayName = I18n.Objects_Redbrush_Name(),
            Description = I18n.Objects_Redbrush_Desc(),
            Type = "Basic",
            Category = (int)ObjectCategory.ArtisanGoods,
            Price = 250,
            Texture = $"{UniqueId}_Brushes",
            SpriteIndex = 0,
            Edibility = -300,
            ContextTags =
            [
                "color_red",
                "slime_painter_item",
            ],
            ExcludeFromShippingCollection = true,
        };

        data[$"{GreenBrushId}"] = new ObjectData
        {
            Name = "Green Paintbrush",
            DisplayName = I18n.Objects_Greenbrush_Name(),
            Description = I18n.Objects_Greenbrush_Desc(),
            Type = "Basic",
            Category = (int)ObjectCategory.ArtisanGoods,
            Price = 100,
            Texture = $"{UniqueId}_Brushes",
            SpriteIndex = 1,
            Edibility = -300,
            ContextTags =
            [
                "color_green",
                "slime_painter_item",
            ],
            ExcludeFromShippingCollection = true,
        };

        data[$"{BlueBrushId}"] = new ObjectData
        {
            Name = "Blue Paintbrush",
            DisplayName = I18n.Objects_Bluebrush_Name(),
            Description = I18n.Objects_Bluebrush_Desc(),
            Type = "Basic",
            Category = (int)ObjectCategory.ArtisanGoods,
            Price = 175,
            Texture = $"{UniqueId}_Brushes",
            SpriteIndex = 2,
            Edibility = -300,
            ContextTags =
            [
                "color_blue",
                "slime_painter_item",
            ],
            ExcludeFromShippingCollection = true,
        };

        data[$"{PurpleBrushId}"] = new ObjectData
        {
            Name = "Purple Paintbrush",
            DisplayName = I18n.Objects_Purplebrush_Name(),
            Description = I18n.Objects_Purplebrush_Desc(),
            Type = "Basic",
            Category = (int)ObjectCategory.ArtisanGoods,
            Price = 500,
            Texture = $"{UniqueId}_Brushes",
            SpriteIndex = 3,
            Edibility = -300,
            ContextTags =
            [
                "color_purple",
                "slime_painter_item",
            ],
            ExcludeFromShippingCollection = true,
        };

        data[$"{PrismaticBrushId}"] = new ObjectData
        {
            Name = "Prismatic Paintbrush",
            DisplayName = I18n.Objects_Prismaticbrush_Name(),
            Description = I18n.Objects_Prismaticbrush_Desc(),
            Type = "Basic",
            Category = (int)ObjectCategory.ArtisanGoods,
            Price = 1000,
            Texture = $"{UniqueId}_Brushes",
            SpriteIndex = 4,
            Edibility = -300,
            ContextTags =
            [
                "color_white",
                "color_prismatic",
                "slime_painter_item",
            ],
            ExcludeFromShippingCollection = true,
        };

        // data["898"].ContextTags.Add("item_crimsonfish");
        // data["899"].ContextTags.Add("item_angler");
        // data["900"].ContextTags.Add("item_legend");
        // data["901"].ContextTags.Add("item_mutant_carp");
        // data["902"].ContextTags.Add("item_glacierfish");
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
        if (SveIntegration.Instance?.IsLoaded ?? false)
        {
            if (!SveIntegration.Instance.DisabeGaldoranTheme &&
                ((Game1.currentLocation?.NameOrUniqueName.IsAnyOf(
                     "Custom_CastleVillageOutpost",
                     "Custom_CrimsonBadlands",
                     "Custom_IridiumQuarry",
                     "Custom_TreasureCave") ?? false) ||
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

    private static Dictionary<string, string[]> ProvideAnimalDerivedGoods()
    {
        var path = Path.Combine(ModHelper.DirectoryPath, "assets", "data");
        HashSet<string> animalGoods = [];

        try
        {
            var files = Directory.GetFiles(path, "*.AnimalDerivedGoods.json");
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);
                if (parsed?.TryGetValue("AnimalDerivedGoods", out var goods) ?? false)
                {
                    animalGoods.UnionWith(goods);
                }
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed loading Animal Derived Goods data.\n{ex}");
        }

        return new Dictionary<string, string[]> { ["AnimalDerivedGoods"] = animalGoods.ToArray() };
    }

    private static Dictionary<string, string[]> ProvideArtisanMachines()
    {
        var path = Path.Combine(ModHelper.DirectoryPath, "assets", "data");
        HashSet<string> artisanMachines = [];

        try
        {
            var files = Directory.GetFiles(path, "*.ArtisanMachines.json");
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);
                if (parsed?.TryGetValue("ArtisanMachines", out var machines) ?? false)
                {
                    artisanMachines.UnionWith(machines);
                }
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed loading Artisan Machines data.\n{ex}");
        }

        return new Dictionary<string, string[]> { ["ArtisanMachines"] = artisanMachines.ToArray() };
    }

    private static Dictionary<string, List<FishPondData>> ProvideLegendaryFishPondData()
    {
        var file = Path.Combine(ModHelper.DirectoryPath, "assets", "data", "LegendaryFishPondData.json");

        try
        {
            var json = File.ReadAllText(file);
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, List<FishPondData>>>(json);
            return (parsed?.TryGetValue("LegendaryFishPondData", out _) ?? false) ? parsed : [];
        }
        catch (Exception ex)
        {
            Log.E($"Failed loading Legendary Fish Pond data.\n{ex}");
            return [];
        }
    }

    #endregion provider callbacks
}
