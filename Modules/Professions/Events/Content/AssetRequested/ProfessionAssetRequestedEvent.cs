namespace DaLion.Overhaul.Modules.Professions.Events.Content.AssetRequested;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Content;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProfessionAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("Data/achievements", new AssetEditor(EditAchievementsData));
        this.Edit("Data/FishPondData", new AssetEditor(EditFishPondDataData, AssetEditPriority.Early));
        this.Edit("Data/mail", new AssetEditor(EditMailData));
        this.Edit("LooseSprites/Cursors", new AssetEditor(EditCursorsLooseSprites));
        this.Edit("TileSheets/BuffsIcons", new AssetEditor(EditBuffsIconsTileSheets));

        this.Provide(
            $"{Manifest.UniqueID}/HudPointer",
            new ModTextureProvider(() => "assets/sprites/interface/pointer.png"));
        this.Provide(
            $"{Manifest.UniqueID}/MaxIcon",
            new ModTextureProvider(() => "assets/sprites/interface/max.png"));
        this.Provide(
            $"{Manifest.UniqueID}/PrestigeProgression",
            new ModTextureProvider(() => $"assets/sprites/interface/{ProfessionsModule.Config.ProgressionStyle}.png"));
        this.Provide(
            $"{Manifest.UniqueID}/SkillBars",
            new ModTextureProvider(ProvideSkillBars));
        this.Provide(
            $"{Manifest.UniqueID}/UltimateMeter",
            new ModTextureProvider(ProvideUltimateMeter));
    }

    #region editor callback

    /// <summary>Patches achievements data with prestige achievements.</summary>
    private static void EditAchievementsData(IAssetData asset)
    {
        var data = asset.AsDictionary<int, string>().Data;

        string title =
            _I18n.Get("prestige.achievement.title" +
                              (Game1.player.IsMale ? ".male" : ".female"));
        var desc = I18n.Prestige_Achievement_Desc();

        const string shouldDisplayBeforeEarned = "false";
        const string prerequisite = "-1";
        const string hatIndex = "";

        var newEntry = string.Join("^", new[] { title, desc, shouldDisplayBeforeEarned, prerequisite, hatIndex });
        data[title.GetDeterministicHashCode()] = newEntry;
    }

    /// <summary>Patches fish pond data with legendary fish data.</summary>
    private static void EditFishPondDataData(IAssetData asset)
    {
        var data = (List<FishPondData>)asset.Data;
        var index = data.FindIndex(0, d => d.RequiredTags.Contains("category_fish"));
        data.InsertRange(index, new List<FishPondData>() // legendary fish
        {
            new()
            {
                PopulationGates = null,
                ProducedItems = new List<FishPondReward>
                {
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.1f,
                        ItemID = 378,
                        MinQuantity = 10,
                        MaxQuantity = 15,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.01f,
                        ItemID = 768,
                        MinQuantity = 10,
                        MaxQuantity = 20,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 1.0f,
                        ItemID = 812,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.8f,
                        ItemID = 812,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                },
                RequiredTags = new List<string>
                {
                    "item_angler",
                },
                SpawnTime = 999999,
            },
            new()
            {
                PopulationGates = null,
                ProducedItems = new List<FishPondReward>
                {
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.05f,
                        ItemID = 536,
                        MinQuantity = 5,
                        MaxQuantity = 10,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.075f,
                        ItemID = 84,
                        MinQuantity = 5,
                        MaxQuantity = 10,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.1f,
                        ItemID = 380,
                        MinQuantity = 10,
                        MaxQuantity = 10,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.01f,
                        ItemID = 72,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 1.0f,
                        ItemID = 812,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.8f,
                        ItemID = 812,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                },
                RequiredTags = new List<string>
                {
                    "item_glacierfish",
                },
                SpawnTime = 999999,
            },
            new()
            {
                PopulationGates = null,
                ProducedItems = new List<FishPondReward>
                {
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.05f,
                        ItemID = 537,
                        MinQuantity = 5,
                        MaxQuantity = 10,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.075f,
                        ItemID = 82,
                        MinQuantity = 5,
                        MaxQuantity = 10,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.1f,
                        ItemID = 384,
                        MinQuantity = 10,
                        MaxQuantity = 10,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.033f,
                        ItemID = 286,
                        MinQuantity = 1,
                        MaxQuantity = 3,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.02f,
                        ItemID = 441,
                        MinQuantity = 1,
                        MaxQuantity = 3,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.01f,
                        ItemID = 288,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 1.0f,
                        ItemID = 812,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.8f,
                        ItemID = 812,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                },
                RequiredTags = new List<string>
                {
                    "item_crimsonfish",
                },
                SpawnTime = 999999,
            },
            new()
            {
                PopulationGates = null,
                ProducedItems = new List<FishPondReward>
                {
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.1f,
                        ItemID = 386,
                        MinQuantity = 5,
                        MaxQuantity = 10,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 1.0f,
                        ItemID = 812,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.8f,
                        ItemID = 812,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                },
                RequiredTags = new List<string>
                {
                    "item_legend",
                },
                SpawnTime = 999999,
            },
            new()
            {
                PopulationGates = null,
                ProducedItems = new List<FishPondReward>
                {
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.1f,
                        ItemID = 909,
                        MinQuantity = 5,
                        MaxQuantity = 15,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.02f,
                        ItemID = 910,
                        MinQuantity = 1,
                        MaxQuantity = 5,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 1.0f,
                        ItemID = 812,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                    new()
                    {
                        RequiredPopulation = 0,
                        Chance = 0.8f,
                        ItemID = 812,
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                },
                RequiredTags = new List<string>
                {
                    "item_mutant_carp",
                },
                SpawnTime = 999999,
            },
            new()
            {
                PopulationGates = null,
                ProducedItems = new List<FishPondReward>
                {
                    new()
                    {
                        Chance = 1f,
                        ItemID = 812, // roe
                        MinQuantity = 1,
                        MaxQuantity = 1,
                    },
                },
                RequiredTags = new List<string> { "fish_legendary" },
                SpawnTime = 999999,
            },
        });

        data.Move(d => d.RequiredTags.Contains("item_mutant_carp"), index);
        data.Move(d => d.RequiredTags.Contains("item_legend"), index);
        data.Move(d => d.RequiredTags.Contains("item_crimsonfish"), index);
        data.Move(d => d.RequiredTags.Contains("item_glacierfish"), index);
        data.Move(d => d.RequiredTags.Contains("item_angler"), index);
    }

    /// <summary>Patches mail data with mail from the Ferngill Revenue Service.</summary>
    private static void EditMailData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        var taxBonus =
            Game1.player.Read<float>(DataKeys.ConservationistActiveTaxDeduction);
        var key = taxBonus >= ProfessionsModule.Config.ConservationistTaxDeductionCeiling
            ? "conservationist.mail.max"
            : "conservationist.mail";

        string honorific = _I18n.Get("honorific" + (Game1.player.IsMale ? ".male" : ".female"));
        var farm = Game1.player.farmName;
        var season = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr
            ? _I18n.Get("season." + Game1.currentSeason)
            : Game1.CurrentSeasonDisplayName;

        string message = _I18n.Get(
            key, new { honorific, taxBonus = FormattableString.CurrentCulture($"{taxBonus:0%}"), farm, season });
        data[$"{Manifest.UniqueID}/ConservationistTaxNotice"] = message;
    }

    /// <summary>Patches cursors with modded profession icons.</summary>
    private static void EditCursorsLooseSprites(IAssetData asset)
    {
        var editor = asset.AsImage();
        var sourceTx = ModHelper.ModContent.Load<Texture2D>("assets/sprites/interface/professions");
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
                (Game1.activeClickableMenu is not LevelUpMenu || profession.Skill.CurrentLevel <= 10))
            {
                continue;
            }

            sourceArea = profession.SourceSheetRect;
            sourceArea.Y += 80;
            targetArea = profession.TargetSheetRect;
            editor.PatchImage(sourceTx, sourceArea, targetArea);
        }
    }

    /// <summary>Patches buffs icons with modded profession buff icons.</summary>
    private static void EditBuffsIconsTileSheets(IAssetData asset)
    {
        var editor = asset.AsImage();
        editor.ExtendImage(192, 80);

        var targetArea = new Rectangle(0, 48, 96, 32);
        editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/sprites/interface/buffs"), null, targetArea);
    }

    #endregion editor callbacks

    #region provider callbacks

    /// <summary>Provides the correct skill bars texture path.</summary>
    private static string ProvideSkillBars()
    {
        var path = "assets/sprites/interface/skillbars";
        if (ModHelper.ModRegistry.IsLoaded("ManaKirel.VMI") ||
            ModHelper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2"))
        {
            path += "_vintage";
        }

        return path + ".png";
    }

    /// <summary>Provides the correct ultimate meter texture path.</summary>
    private static string ProvideUltimateMeter()
    {
        var path = "assets/sprites/interface/gauge";
        if (StardewValleyExpandedIntegration.Instance?.IsLoaded == true)
        {
            if (!StardewValleyExpandedIntegration.Instance.DisabeGaldoranTheme &&
                (Game1.currentLocation?.NameOrUniqueName.IsIn(
                     "Custom_CastleVillageOutpost",
                     "Custom_CrimsonBadlands",
                     "Custom_IridiumQuarry",
                     "Custom_TreasureCave") == true ||
                 StardewValleyExpandedIntegration.Instance.UseGaldoranThemeAllTimes))
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
