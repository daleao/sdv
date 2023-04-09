namespace DaLion.Overhaul.Modules.Slingshots.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class SlingshotAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("TileSheets/weapons", new AssetEditor(EditWeaponsTileSheet, AssetEditPriority.Early));
        this.Provide(
            $"{Manifest.UniqueID}/SnowballCollisionAnimation",
            new ModTextureProvider(() => "assets/animations/snowball.png"));
    }

    #region editor callbacks

    /// <summary>Edits weapons tilesheet with touched up textures.</summary>
    private static void EditWeaponsTileSheet(IAssetData asset)
    {
        if (!SlingshotsModule.Config.EnableInfinitySlingshot)
        {
            return;
        }

        var editor = asset.AsImage();
        var targetArea = new Rectangle(16, 128, 16, 16);
        editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/sprites/InfinitySlingshot"), targetArea: targetArea);
    }

    #endregion editor callbacks
}
