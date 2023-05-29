namespace DaLion.Overhaul.Modules.Rings.Events;

#region using directives

using DaLion.Overhaul.Modules.Rings.Integrations;
using DaLion.Shared.Content;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal class RingAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RingAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal RingAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("Data/CraftingRecipes", new AssetEditor(EditCraftingRecipesData));
        this.Edit("Data/ObjectInformation", new AssetEditor(EditObjectInformationData));
        this.Edit("Maps/springobjects", new AssetEditor(EditSpringObjectsMaps, AssetEditPriority.Late));
    }

    #region editor callbacks

    /// <summary>Edits crafting recipes with new ring recipes.</summary>
    private static void EditCraftingRecipesData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;

        string[] fields;
        if (RingsModule.Config.RebalancedRings)
        {
            data["Ring of Yoba"] = "336 5 335 5 72 1 768 20/Home/524/false/Combat 8";
        }

        if (RingsModule.Config.BetterGlowstoneProgression)
        {
            data["Small Glow Ring"] = "336 2 768 5/Home/516/Ring/Mining 2";
            data["Small Magnet Ring"] = "335 2 769 5/Home/518/Ring/Mining 2";
            data["Glow Ring"] = "516 2 768 10/Home/517/Ring/Mining 4";
            data["Magnet Ring"] = "518 2 769 10/Home/519/Ring/Mining 4";
            data["Glowstone Ring"] = "517 1 519 1 768 20 769 20/Home/888/Ring/Mining 6";
        }

        if (RingsModule.Config.CraftableGemRings)
        {
            data["Emerald Ring"] = "60 1 336 5/Home/533/Ring/Combat 6";
            data["Aquamarine Ring"] = "62 1 335 5/Home/531/Ring/Combat 4";
            data["Ruby Ring"] = "64 1 336 5/Home/534/Ring/Combat 6";
            data["Amethyst Ring"] = "66 1 334 5/Home/529/Ring/Combat 2";
            data["Topaz Ring"] = "68 1 334 5/Home/530/Ring/Combat 2";
            data["Jade Ring"] = "70 1 335 5/Home/532/Ring/Combat 4";
        }

        if (RingsModule.Config.TheOneInfinityBand)
        {
            fields = data["Iridium Band"].Split('/');
            fields[0] = "337 5 768 100 769 100";
            data["Iridium Band"] = string.Join('/', fields);
        }
    }

    /// <summary>Edits object information with rebalanced ring descriptions.</summary>
    private static void EditObjectInformationData(IAssetData asset)
    {
        var data = asset.AsDictionary<int, string>().Data;
        string[] fields;

        if (RingsModule.Config.RebalancedRings)
        {
            fields = data[ItemIDs.TopazRing].Split('/');
            fields[5] = CombatModule.ShouldEnable && CombatModule.Config.OverhauledDefense
                ? I18n.Rings_Topaz_Desc_Resist()
                : I18n.Rings_Topaz_Desc_Defense();
            data[ItemIDs.TopazRing] = string.Join('/', fields);

            fields = data[ItemIDs.JadeRing].Split('/');
            fields[5] = I18n.Rings_Jade_Desc();
            data[ItemIDs.JadeRing] = string.Join('/', fields);

            fields = data[ItemIDs.WarriorRing].Split('/');
            fields[5] = I18n.Rings_Warrior_Desc();
            data[ItemIDs.WarriorRing] = string.Join('/', fields);

            fields = data[ItemIDs.YobaRing].Split('/');
            fields[5] = I18n.Rings_Yoba_Desc();
            data[ItemIDs.YobaRing] = string.Join('/', fields);

            fields = data[ItemIDs.ImmunityRing].Split('/');
            fields[5] += I18n.Rings_Immunity_Extradesc();
            data[ItemIDs.ImmunityRing] = string.Join('/', fields);

            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
            {
                fields = data[ItemIDs.ThornsRing].Split('/');
                if (!ModHelper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage"))
                {
                    fields[0] = "Ring of Thorns";
                }

                data[ItemIDs.ThornsRing] = string.Join('/', fields);
            }
        }

        if (RingsModule.Config.TheOneInfinityBand)
        {
            fields = data[ItemIDs.IridiumBand].Split('/');
            fields[5] = I18n.Rings_Iridium_Desc();
            data[ItemIDs.IridiumBand] = string.Join('/', fields);
        }
    }

    /// <summary>Edits spring objects with new and custom rings.</summary>
    private static void EditSpringObjectsMaps(IAssetData asset)
    {
        var editor = asset.AsImage();
        Rectangle sourceArea, targetArea;

        var sourceY = VanillaTweaksIntegration.Instance?.RingsCategoryEnabled == true
            ? 32 : BetterRingsIntegration.Instance?.IsLoaded == true
                ? 16 : 0;
        if (RingsModule.Config.CraftableGemRings)
        {
            sourceArea = new Rectangle(16, sourceY, 96, 16);
            targetArea = new Rectangle(16, 352, 96, 16);
            editor.PatchImage(Textures.RingsTx, sourceArea, targetArea);
        }

        if (RingsModule.Config.TheOneInfinityBand)
        {
            sourceArea = new Rectangle(0, sourceY, 16, 16);
            targetArea = new Rectangle(368, 336, 16, 16);
            editor.PatchImage(Textures.RingsTx, sourceArea, targetArea);
        }
    }

    #endregion editor callbacks
}
