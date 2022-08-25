namespace DaLion.Stardew.Slingshots.Framework.Events;

#region using directives

using Common.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotAssetRequestedEvent : AssetRequestedEvent
{
    private static readonly Dictionary<string, (Action<IAssetData> callback, AssetEditPriority priority)> AssetEditors =
        new();

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        AlwaysEnabled = true;
        AssetEditors["TileSheets/animations"] =
            (callback: EditAnimationsTileSheet, priority: AssetEditPriority.Default);
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (AssetEditors.TryGetValue(e.NameWithoutLocale.Name, out var editor))
            e.Edit(editor.callback, editor.priority);
    }

    #region editor callbacks

    /// <summary>Patches animations with snowball collision.</summary>
    private static void EditAnimationsTileSheet(IAssetData asset)
    {
        var editor = asset.AsImage();
        editor.ExtendImage(640, 3392);
        var srcArea = new Rectangle(0, 0, 640, 64);
        var targetArea = new Rectangle(0, 3328, 640, 64);

        editor.PatchImage(ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/animations/snowball.png"), srcArea,
            targetArea);
    }

    #endregion editor callbacks
}