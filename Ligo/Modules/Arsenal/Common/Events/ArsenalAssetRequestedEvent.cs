namespace DaLion.Ligo.Modules.Arsenal.Common.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ArsenalAssetRequestedEvent : AssetRequestedEvent
{
    private static readonly Dictionary<string, (Action<IAssetData> Callback, AssetEditPriority Priority)> AssetEditors =
        new();

    private static readonly Dictionary<string, (Func<string> Provide, AssetLoadPriority Priority)> AssetProviders =
        new();

    /// <summary>Initializes a new instance of the <see cref="ArsenalAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        AssetEditors["Data/ObjectInformation"] =
            (Callback: EditDataObjectInformation, Priority: AssetEditPriority.Default);
        AssetEditors["Strings/Locations"] = (Callback: EditLocationsStrings, Priority: AssetEditPriority.Default);
        AssetEditors["Strings/StringsFromCSFiles"] =
            (Callback: EditStringsFromCsFilesStrings, Priority: AssetEditPriority.Default);
        AssetEditors["TileSheets/Projectiles"] =
            (Callback: EditProjectilesTileSheet, Priority: AssetEditPriority.Default);

        AssetProviders[$"{ModEntry.Manifest.UniqueID}/InfinityCollisionAnimation"] = (Provide: () => "assets/animations/infinity.png",
            Priority: AssetLoadPriority.Medium);
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (AssetEditors.TryGetValue(e.NameWithoutLocale.Name, out var editor))
        {
            e.Edit(editor.Callback, editor.Priority);
        }
        else if (AssetProviders.TryGetValue(e.NameWithoutLocale.Name, out var provider))
        {
            e.LoadFromModFile<Texture2D>(provider.Provide(), provider.Priority);
        }
    }

    #region editor callbacks

    /// <summary>Edits galaxy soul description.</summary>
    private static void EditDataObjectInformation(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;

        // edit galaxy soul description
        var fields = data[Constants.GalaxySoulIndex].Split('/');
        fields[5] = ModEntry.i18n.Get("galaxysoul.desc");
        data[Constants.GalaxySoulIndex] = string.Join('/', fields);
    }

    /// <summary>Edits location string data with custom legendary sword rhyme.</summary>
    private static void EditLocationsStrings(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["Town_DwarfGrave_Translated"] = ModEntry.i18n.Get("locations.Town_DwarfGrave_Translated");
    }

    /// <summary>Edits strings data with custom legendary sword reward prompt.</summary>
    private static void EditStringsFromCsFilesStrings(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["MeleeWeapon.cs.14122"] = ModEntry.i18n.Get("fromcsfiles.MeleeWeapon.cs.14122");
    }

    /// <summary>Edits strings data with custom legendary sword reward prompt.</summary>
    private static void EditProjectilesTileSheet(IAssetData asset)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        var editor = asset.AsImage();
        var srcArea = new Rectangle(0, 0, 16, 16);
        var targetArea = new Rectangle(48, 16, 16, 16);
        editor.PatchImage(
            Textures.ProjectilesTx,
            srcArea,
            targetArea);
    }

    #endregion editor callbacks
}
