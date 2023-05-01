namespace DaLion.Overhaul.Modules.Enchantments.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class EnchantmentsAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="EnchantmentsAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal EnchantmentsAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("TileSheets/BuffsIcons", new AssetEditor(EditBuffsIconsTileSheet));
        //this.Provide(
        //    $"{Manifest.UniqueID}/QuincyCollisionAnimation",
        //    new ModTextureProvider(() => "assets/animations/quincy.png", AssetLoadPriority.Medium));
    }

    #region editor callbacks

    /// <summary>Patches buffs icons with energized buff icon.</summary>
    private static void EditBuffsIconsTileSheet(IAssetData asset)
    {
        if (ProfessionsModule.ShouldEnable)
        {
            return;
        }

        var editor = asset.AsImage();
        editor.ExtendImage(192, 80);

        var sourceArea = new Rectangle(64, 16, 32, 16);
        var targetArea = new Rectangle(64, 64, 32, 16);
        editor.PatchImage(
            ModHelper.ModContent.Load<Texture2D>("assets/sprites/buffs"),
            sourceArea,
            targetArea);
    }

    #endregion editor callbacks
}
