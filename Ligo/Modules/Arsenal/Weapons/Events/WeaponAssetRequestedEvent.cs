namespace DaLion.Ligo.Modules.Arsenal.Weapons.Events;

#region using directives

using System.Collections.Generic;
using System.Globalization;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class WeaponAssetRequestedEvent : AssetRequestedEvent
{
    private static readonly Dictionary<string, (Action<IAssetData> Callback, AssetEditPriority Priority)> AssetEditors =
        new();

    /// <summary>Initializes a new instance of the <see cref="WeaponAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        AssetEditors["TileSheets/BuffsIcons"] =
            (Callback: EditBuffsIconsTileSheet, Priority: AssetEditPriority.Default);
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (AssetEditors.TryGetValue(e.NameWithoutLocale.Name, out var editor))
        {
            e.Edit(editor.Callback, editor.Priority);
        }
    }

    #region editor callbacks

    /// <summary>Patches buffs icons with energized buff icon.</summary>
    private static void EditBuffsIconsTileSheet(IAssetData asset)
    {
        var editor = asset.AsImage();
        editor.ExtendImage(192, 64);

        var srcArea = new Rectangle(64, 16, 16, 16);
        var targetArea = new Rectangle(96, 48, 16, 16);
        editor.PatchImage(
            Textures.BuffsSheetTx,
            srcArea,
            targetArea);
    }

    #endregion editor callbacks
}
