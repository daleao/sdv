namespace DaLion.Stardew.Rings.Framework.Events;

#region using directives

using Common.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;

#endregion using directives

[UsedImplicitly]
internal class RingAssetRequestedEvent : AssetRequestedEvent
{
    private static readonly Dictionary<string, (Action<IAssetData> edit, AssetEditPriority priority)> AssetEditors =
        new();

    private static readonly Dictionary<string, (Func<string> provide, AssetLoadPriority priority)> AssetProviders = new();

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal RingAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        AssetEditors["Data/CraftingRecipes"] = (edit: EditCraftingRecipesData, priority: AssetEditPriority.Default);
        AssetEditors["Data/ObjectInformation"] = (edit: EditObjectInformationData, priority: AssetEditPriority.Default);
        AssetEditors["Maps/springobjects"] = (edit: EditSpringObjectsMaps, priority: AssetEditPriority.Late);

        AssetProviders[$"{ModEntry.Manifest.UniqueID}/Gemstones"] = (() =>
            "assets/gemstones" + (ModEntry.IsBetterRingsLoaded ? "_better" : string.Empty) + ".png",
            AssetLoadPriority.Medium);
        AssetProviders[$"{ModEntry.Manifest.UniqueID}/Rings"] = (() =>
            "assets/rings" + (ModEntry.IsBetterRingsLoaded ? "_better" : string.Empty) + ".png",
            AssetLoadPriority.Medium);
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (AssetEditors.TryGetValue(e.NameWithoutLocale.Name, out var editor))
            e.Edit(editor.edit, editor.priority);
        else if (AssetProviders.TryGetValue(e.NameWithoutLocale.Name, out var provider))
            e.LoadFromModFile<Texture2D>(provider.provide(), provider.priority);
    }

    #region editor callbacks

    /// <summary>Edits crafting recipes wit new ring recipes.</summary>
    private static void EditCraftingRecipesData(IAssetData asset)
    {
        var data = asset.AsDictionary<string, string>().Data;

        string[] fields;
        if (ModEntry.Config.ImmersiveGlowstoneRecipe)
        {
            fields = data["Glowstone Ring"].Split('/');
            fields[0] = "517 1 519 1 768 20 769 20";
            data["Glowstone Ring"] = string.Join('/', fields);
        }

        if (ModEntry.Config.CraftableGlowAndMagnetRings)
        {
            data["Glow Ring"] = "516 2 768 10/Home/517/Ring/Mining 2";
            data["Magnet Ring"] = "518 2 769 10/Home/519/Ring/Mining 2";
        }

        if (ModEntry.Config.CraftableGemRings)
        {
            data["Emerald Ring"] = "60 1 336 5/Home/533/Ring/Combat 6";
            data["Aquamarine Ring"] = "62 1 335 5/Home/531/Ring/Combat 4";
            data["Ruby Ring"] = "64 1 336 5/Home/534/Ring/Combat 6";
            data["Amethyst Ring"] = "66 1 334 5/Home/529/Ring/Combat 2";
            data["Topaz Ring"] = "68 1 334 5/Home/530/Ring/Combat 2";
            data["Jade Ring"] = "70 1 335 5/Home/532/Ring/Combat 4";
        }

        if (ModEntry.Config.TheOneIridiumBand)
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

        if (ModEntry.Config.RebalancedRings)
        {
            fields = data[Constants.TOPAZ_RING_INDEX_I].Split('/');
            fields[5] = ModEntry.i18n.Get("rings.topaz.description");
            data[Constants.TOPAZ_RING_INDEX_I] = string.Join('/', fields);

            fields = data[Constants.JADE_RING_INDEX_I].Split('/');
            fields[5] = ModEntry.i18n.Get("rings.jade.description");
            data[Constants.JADE_RING_INDEX_I] = string.Join('/', fields);
        }

        if (ModEntry.Config.TheOneIridiumBand)
        {
            fields = data[Constants.IRIDIUM_BAND_INDEX_I].Split('/');
            fields[5] = ModEntry.i18n.Get("rings.iridium.description");
            data[Constants.IRIDIUM_BAND_INDEX_I] = string.Join('/', fields);
        }
    }

    /// <summary>Edits spring objects with new and custom rings.</summary>
    private static void EditSpringObjectsMaps(IAssetData asset)
    {
        var editor = asset.AsImage();
        Rectangle srcArea, targetArea;

        var ringsTx = ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/Rings");
        if (ModEntry.Config.CraftableGemRings)
        {
            srcArea = new(16, 0, 96, 16);
            targetArea = new(16, 352, 96, 16);
            editor.PatchImage(ringsTx, srcArea, targetArea);
        }

        if (ModEntry.Config.TheOneIridiumBand)
        {
            srcArea = new(0, 0, 16, 16);
            targetArea = new(368, 336, 16, 16);
            editor.PatchImage(ringsTx, srcArea, targetArea);
        }
    }

    #endregion editor callbacks
}