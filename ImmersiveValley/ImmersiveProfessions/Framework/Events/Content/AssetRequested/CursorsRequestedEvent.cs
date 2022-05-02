namespace DaLion.Stardew.Professions.Framework.Events.Content.AssetRequested;

#region using directives

using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

using GameLoop;
using Utility;

#endregion using directives

[UsedImplicitly]
internal class CursorsRequestedEvent : AssetRequestedEvent
{
    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("LooseSprites/Cursors")) return;

        e.Edit(asset =>
        {
            // patch modded profession icons
            var editor = asset.AsImage();
            var srcArea = new Rectangle(0, 0, 96, 80);
            var targetArea = new Rectangle(0, 624, 96, 80);

            editor.PatchImage(Textures.Spritesheet, srcArea, targetArea);
        });
    }
}