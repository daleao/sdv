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
        this.Edit("Data/weapons", new AssetEditor(EditWeaponsData));
        this.Provide(
            $"{Manifest.UniqueID}/SnowballCollisionAnimation",
            new ModTextureProvider(() => "assets/vfx/snowball.png"));
    }

    #region editor callbacks

    /// <summary>Edits weapons data with Infinity Slingshot.</summary>
    private static void EditWeaponsData(IAssetData asset)
    {
        if (!SlingshotsModule.Config.EnableInfinitySlingshot)
        {
            return;
        }

        var data = asset.AsDictionary<int, string>().Data;
        data[ItemIDs.InfinitySlingshot] = string.Format(
            "Infinity Slingshot/{0}/1/3/1/308/0/0/4/-1/-1/0/.02/3/{1}",
            I18n.Slingshots_Infinity_Desc(),
            I18n.Slingshots_Infinity_Name());
    }

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
