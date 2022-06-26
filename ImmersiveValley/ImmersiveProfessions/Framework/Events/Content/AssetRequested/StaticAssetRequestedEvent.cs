namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.FishPond;

using Common.Data;
using Common.Events;
using Common.Extensions;
using Textures;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticAssetRequestedEvent : AssetRequestedEvent
{
    private static readonly Dictionary<string, (Action<IAssetData> edit, AssetEditPriority priority)> AssetEditors =
        new();

    private static readonly Dictionary<string, (Func<string> provide, AssetLoadPriority priority)> AssetProviders =
        new();

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticAssetRequestedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        AlwaysHooked = true;

        AssetEditors["Data/achievements"] = (edit: EditAchievementsData, priority: AssetEditPriority.Default);
        AssetEditors["Data/FishPondData"] = (edit: EditFishPondDataData, priority: AssetEditPriority.Late);
        AssetEditors["Data/mail"] = (edit: EditMailData, priority: AssetEditPriority.Default);
        AssetEditors["LooseSprites/Cursors"] = (edit: EditCursorsLooseSprites, priority: AssetEditPriority.Default);
        AssetEditors["TileSheets/BuffsIcons"] = (edit: EditBuffsIconsTileSheets, priority: AssetEditPriority.Default);

        AssetProviders[$"{ModEntry.Manifest.UniqueID}/HudPointer"] = (provide: () => "assets/hud/pointer.png", priority: AssetLoadPriority.Medium);
        AssetProviders[$"{ModEntry.Manifest.UniqueID}/MaxFishSizeIcon"] = (provide: () => "assets/menus/max.png", priority: AssetLoadPriority.Medium);
        AssetProviders[$"{ModEntry.Manifest.UniqueID}/PrestigeProgression"] = (provide: () => $"assets/sprites/{ModEntry.Config.PrestigeProgressionStyle}.png", priority: AssetLoadPriority.Medium);
        AssetProviders[$"{ModEntry.Manifest.UniqueID}/SkillBars"] = (provide: ProvideSkillBars, priority: AssetLoadPriority.Medium);
        AssetProviders[$"{ModEntry.Manifest.UniqueID}/SpriteSheet"] = (provide: () => "assets/sprites/spritesheet.png", priority: AssetLoadPriority.Medium);
        AssetProviders[$"{ModEntry.Manifest.UniqueID}/UltimateMeter"] = (provide: ProvideUltimateMeter, priority: AssetLoadPriority.Medium);
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (AssetEditors.TryGetValue(e.NameWithoutLocale.Name, out var editor))
            e.Edit(editor.edit, editor.priority);
        else if (AssetProviders.TryGetValue(e.NameWithoutLocale.Name, out var provider))
            e.LoadFromModFile<Texture2D>(provider.provide.Invoke(), provider.priority);
    }

    #region editor callback

    /// <summary>Patches achievements data with prestige achievements.</summary>
    private static void EditAchievementsData(IAssetData asset)
    {
        var data = asset.AsDictionary<int, string>().Data;

        string name =
            ModEntry.i18n.Get("prestige.achievement.name" +
                              (Game1.player.IsMale ? ".male" : ".female"));
        var desc = ModEntry.i18n.Get("prestige.achievement.desc");

        const string SHOULD_DISPLAY_BEFORE_EARNED_S = "false";
        const string PREREQUISITE_S = "-1";
        const string HAT_INDEX_S = "";

        var newEntry = string.Join("^", name, desc, SHOULD_DISPLAY_BEFORE_EARNED_S, PREREQUISITE_S, HAT_INDEX_S);
        data[name.GetDeterministicHashCode()] = newEntry;
    }

    /// <summary>Patches fish pond data with legendary fish data.</summary>
    private static void EditFishPondDataData(IAssetData asset)
    {
        var data = (List<FishPondData>) asset.Data;
        data.InsertRange(data.Count - 2, new List<FishPondData>
        {
            new() // legendary fish
            {
                PopulationGates = null,
                ProducedItems = new()
                {
                    new()
                    {
                        Chance = 1f,
                        ItemID = 812, // roe
                        MinQuantity = 1,
                        MaxQuantity = 1
                    }
                },
                RequiredTags = new() {"fish_legendary"},
                SpawnTime = int.MaxValue
            }
        });
    }

    /// <summary>Patches mail data with mail from the Ferngill Revenue Service.</summary>
    private static void EditMailData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;
        var taxBonus =
            ModDataIO.ReadDataAs<float>(Game1.player, ModData.ConservationistActiveTaxBonusPct.ToString());
        var key = taxBonus >= ModEntry.Config.ConservationistTaxBonusCeiling
            ? "conservationist.mail.max"
            : "conservationist.mail";
        var honorific = ModEntry.i18n.Get("honorific" + (Game1.player.IsMale ? ".male" : ".female"));
        var farm = Game1.getFarm().Name;
        var season = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr
            ? ModEntry.i18n.Get("season." + Game1.currentSeason)
            : Game1.CurrentSeasonDisplayName;

        string message = ModEntry.i18n.Get(key,
            new {honorific, taxBonus = FormattableString.CurrentCulture($"{taxBonus:p0}"), farm, season});
        data[$"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice"] = message;
    }

    /// <summary>Patches cursors with modded profession icons.</summary>
    private static void EditCursorsLooseSprites(IAssetData asset)
    {
        var editor = asset.AsImage();
        var srcArea = new Rectangle(0, 0, 96, 80);
        var targetArea = new Rectangle(0, 624, 96, 80);

        editor.PatchImage(Textures.SpriteTx, srcArea, targetArea);
    }

    /// <summary>Patches buffs icons with modded profession buff icons.</summary>
    private static void EditBuffsIconsTileSheets(IAssetData asset)
    {
        var editor = asset.AsImage();
        editor.ExtendImage(192, 80);
        var srcArea = new Rectangle(0, 80, 96, 32);
        var targetArea = new Rectangle(0, 48, 96, 32);

        editor.PatchImage(Textures.SpriteTx, srcArea, targetArea);
    }

    #endregion editor callbacks

    #region provider callbacks

    /// <summary>Provides the correct skill bars texture path.</summary>
    private static string ProvideSkillBars()
    {
        var path = "assets/menus/";

        if (ModEntry.Config.VintageInterfaceSupport != ModConfig.VintageInterfaceStyle.Off)
        {
            var vintage = "off";
            if (ModEntry.Config.VintageInterfaceSupport == ModConfig.VintageInterfaceStyle.Automatic)
            {
                if (ModEntry.ModHelper.ModRegistry.IsLoaded("ManaKirel.VMI") ||
                    ModEntry.ModHelper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2"))
                    vintage = "on";
            }
            else
            {
                vintage = "on";
            }

            if (vintage != "off")
                return path + "skillbars_vintage.png";
        }

        return path + "skillbars.png";
    }

    /// <summary>Provides the correct ultimate meter texture path.</summary>
    private static string ProvideUltimateMeter()
    {
        var path = "assets/hud/";

        if (ModEntry.SVEConfig is not null)
        {
            if (ModEntry.SVEConfig.Value<bool?>("DisableGaldoranTheme") == false &&
                (Game1.currentLocation.NameOrUniqueName.IsIn("Custom_CastleVillageOutpost", "Custom_CrimsonBadlands",
                     "Custom_IridiumQuarry", "Custom_TreasureCave") ||
                 ModEntry.SVEConfig.Value<bool?>("UseGaldoranThemeAllTimes") == true))
            {
                return path + "gauge_galdora.png";
            }
        }

        if (ModEntry.Config.VintageInterfaceSupport != ModConfig.VintageInterfaceStyle.Off)
        {
            var vintage = "off";
            if (ModEntry.Config.VintageInterfaceSupport == ModConfig.VintageInterfaceStyle.Automatic)
            {
                if (ModEntry.ModHelper.ModRegistry.IsLoaded("ManaKirel.VMI"))
                    vintage = "pink";
                else if (ModEntry.ModHelper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2"))
                    vintage = "brown";
            }
            else
            {
                vintage = ModEntry.Config.VintageInterfaceSupport.ToString().ToLowerInvariant();
            }

            if (vintage != "off")
                return path + $"gauge_vintage_{vintage}.png";
        }
        
        return path + "gauge.png";
    }

    #endregion provider callbacks
}