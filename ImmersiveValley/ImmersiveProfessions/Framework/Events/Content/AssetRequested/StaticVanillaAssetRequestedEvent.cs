namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

using Utility;

#endregion using directives

[UsedImplicitly]
internal class StaticVanillaAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Construct an instance.</summary>
    internal StaticVanillaAssetRequestedEvent()
    {
        Enable();
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("TileSheets/BuffsIcons"))
        {
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
        else if (e.NameWithoutLocale.IsEquivalentTo("LooseSprites/Cursors"))
        {
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
}