namespace DaLion.Stardew.Professions.Framework.Events.Content.AssetRequested;

#region using directives

using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

using GameLoop;
using Utility;

#endregion using directives

[UsedImplicitly]
internal class BuffsIconsRequestedEvent : AssetRequestedEvent
{
    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("TileSheets/BuffsIcons")) return;

        e.Edit(asset =>
        {
            // patch modded profession buff icons
            var editor = asset.AsImage();
            editor.ExtendImage(192, 80);
            var srcArea = new Rectangle(0, 80, 96, 32);
            var targetArea = new Rectangle(0, 48, 96, 32);

            editor.PatchImage(Textures.Spritesheet, srcArea, targetArea);
        });
    }
}