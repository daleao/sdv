namespace DaLion.Overhaul.Modules.Enchantments.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class EnchantmentAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="EnchantmentAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal EnchantmentAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("TileSheets/BuffsIcons", new AssetEditor(EditBuffsIconsTileSheet));
        this.Provide(
            $"{Manifest.UniqueID}/GemstoneSockets",
            new ModTextureProvider(ProvideGemSockets));
        //this.Provide(
        //    $"{Manifest.UniqueID}/QuincyCollisionAnimation",
        //    new ModTextureProvider(() => "assets/vfx/quincy.png"));
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

    #region provider callbacks

    /// <summary>Provides the correct gemstone socket texture path.</summary>
    private static string ProvideGemSockets()
    {
        var path = "assets/menus/GemSocket_" + EnchantmentsModule.Config.SocketStyle;
        if (ModHelper.ModRegistry.IsLoaded("ManaKirel.VMI") ||
            ModHelper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2"))
        {
            path += "_Vintage";
        }

        return path + ".png";
    }

    #endregion provider callbacks
}
