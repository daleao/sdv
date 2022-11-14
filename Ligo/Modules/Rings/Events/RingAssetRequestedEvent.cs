namespace DaLion.Ligo.Modules.Rings.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal class RingAssetRequestedEvent : AssetRequestedEvent
{
    private static readonly Dictionary<string, (Action<IAssetData> Edit, AssetEditPriority Priority)> AssetEditors =
        new();

    private static readonly Dictionary<string, (Func<string> Provide, AssetLoadPriority Priority)> AssetProviders =
        new();

    /// <summary>Initializes a new instance of the <see cref="RingAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal RingAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        // editors
        AssetEditors["Data/CraftingRecipes"] = (Edit: EditCraftingRecipesData, Priority: AssetEditPriority.Default);
        AssetEditors["Data/ObjectInformation"] = (Edit: EditObjectInformationData, Priority: AssetEditPriority.Default);
        AssetEditors["Maps/springobjects"] = (Edit: EditSpringObjectsMaps, Priority: AssetEditPriority.Late);
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (AssetEditors.TryGetValue(e.NameWithoutLocale.Name, out var editor))
        {
            e.Edit(editor.Edit, editor.Priority);
        }
        else if (AssetProviders.TryGetValue(e.NameWithoutLocale.Name, out var provider))
        {
            e.LoadFromModFile<Texture2D>(provider.Provide(), provider.Priority);
        }
    }

    #region editor callbacks

    /// <summary>Edits crafting recipes wit new ring recipes.</summary>
    private static void EditCraftingRecipesData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;

        string[] fields;
        if (ModEntry.Config.Rings.ImmersiveGlowstoneRecipe)
        {
            fields = data["Glowstone Ring"].Split('/');
            fields[0] = "517 1 519 1 768 20 769 20";
            data["Glowstone Ring"] = string.Join('/', fields);
        }

        if (ModEntry.Config.Rings.CraftableGlowAndMagnetRings)
        {
            data["Glow Ring"] = "516 2 768 10/Home/517/Ring/Mining 2";
            data["Magnet Ring"] = "518 2 769 10/Home/519/Ring/Mining 2";
        }

        if (ModEntry.Config.Rings.CraftableGemRings)
        {
            data["Emerald Ring"] = "60 1 336 5/Home/533/Ring/Combat 6";
            data["Aquamarine Ring"] = "62 1 335 5/Home/531/Ring/Combat 4";
            data["Ruby Ring"] = "64 1 336 5/Home/534/Ring/Combat 6";
            data["Amethyst Ring"] = "66 1 334 5/Home/529/Ring/Combat 2";
            data["Topaz Ring"] = "68 1 334 5/Home/530/Ring/Combat 2";
            data["Jade Ring"] = "70 1 335 5/Home/532/Ring/Combat 4";
        }

        if (ModEntry.Config.Rings.TheOneInfinityBand)
        {
            fields = data["Iridium Band"].Split('/');
            fields[0] = "337 5 768 100 769 100";
            data["Iridium Band"] = string.Join('/', fields);
        }
    }

    /// <summary>Edits object information with rebalanced ring recipes.</summary>
    private static void EditObjectInformationData(IAssetData asset)
    {
        var data = asset.AsDictionary<int, string>().Data;
        string[] fields;

        if (ModEntry.Config.Rings.RebalancedRings)
        {
            fields = data[Constants.TopazRingIndex].Split('/');
            fields[5] = ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.OverhauledDefense
                ? ModEntry.i18n.Get("rings.topaz.description.resist")
                : ModEntry.i18n.Get("rings.topaz.description.defense");
            data[Constants.TopazRingIndex] = string.Join('/', fields);

            fields = data[Constants.JadeRingIndex].Split('/');
            fields[5] = ModEntry.i18n.Get("rings.jade.description");
            data[Constants.JadeRingIndex] = string.Join('/', fields);
        }

        if (ModEntry.Config.Rings.TheOneInfinityBand)
        {
            fields = data[Constants.IridiumBandIndex].Split('/');
            fields[5] = ModEntry.i18n.Get("rings.iridium.description");
            data[Constants.IridiumBandIndex] = string.Join('/', fields);
        }
    }

    /// <summary>Edits spring objects with new and custom rings.</summary>
    private static void EditSpringObjectsMaps(IAssetData asset)
    {
        var editor = asset.AsImage();
        Rectangle srcArea, targetArea;

        var sourceY = Ligo.Integrations.IsBetterRingsLoaded ? 16 : 0;
        if (ModEntry.Config.Rings.CraftableGemRings)
        {
            srcArea = new Rectangle(16, sourceY, 96, 16);
            targetArea = new Rectangle(16, 352, 96, 16);
            editor.PatchImage(Textures.RingsTx, srcArea, targetArea);
        }

        if (ModEntry.Config.Rings.TheOneInfinityBand)
        {
            srcArea = new Rectangle(0, sourceY, 16, 16);
            targetArea = new Rectangle(368, 336, 16, 16);
            editor.PatchImage(Textures.RingsTx, srcArea, targetArea);
        }
    }

    #endregion editor callbacks
}
