namespace DaLion.Enchantments.Framework.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="EnchantmentsAssetRequestedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class EnchantmentsAssetRequestedEvent(EventManager? manager = null)
    : AssetRequestedEvent(manager ?? EnchantmentsMod.EventManager)
{
    /// <inheritdoc />
    protected override void Initialize()
    {
        this.Edit("TileSheets/BuffsIcons", new AssetEditor(EditBuffsIconsTileSheet));
        this.Edit("TileSheets/Projectiles", new AssetEditor(EditProjectilesTileSheet));

        this.Provide(
            $"{Manifest.UniqueID}_GemstoneSockets",
            new ModTextureProvider(ProvideGemSockets));
        this.Provide(
            $"{Manifest.UniqueID}_Shield",
            new ModTextureProvider(() => "assets/sprites/shield.png"));
    }

    #region editor callbacks

    /// <summary>Patches buffs icons with energized and explosive buff icons.</summary>
    private static void EditBuffsIconsTileSheet(IAssetData asset)
    {
        var editor = asset.AsImage();
        editor.ExtendImage(editor.Data.Width, 80);

        var targetArea = new Rectangle(160, 64, 32, 16);
        editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/sprites/buffs"), null, targetArea);
    }

    /// <summary>Patches in quincy projectiles.</summary>
    private static void EditProjectilesTileSheet(IAssetData asset)
    {
        var editor = asset.AsImage();
        var targetArea = new Rectangle(64, 32, 32, 16);
        editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/sprites/quincy"), null, targetArea);
    }

    #endregion editor callbacks

    #region provider callbacks

    /// <summary>Provides the correct gemstone socket texture path.</summary>
    private static string ProvideGemSockets()
    {
        var path = "assets/sprites/GemSocket_" + Config.GemstoneSocketStyle;
        if (ModHelper.ModRegistry.IsLoaded("ManaKirel.VMI") ||
            ModHelper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2"))
        {
            path += "_Vintage";
        }

        return path + ".png";
    }

    #endregion provider callbacks
}
