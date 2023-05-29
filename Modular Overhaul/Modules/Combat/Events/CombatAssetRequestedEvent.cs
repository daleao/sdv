namespace DaLion.Overhaul.Modules.Combat.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("TileSheets/Projectiles", new AssetEditor(EditProjectilesTileSheet));
        this.Edit("Strings/StringsFromCSFiles", new AssetEditor(EditStringsFromCsFiles, AssetEditPriority.Late));
    }

    #region editor callbacks

    /// <summary>Edits the Shooter projectile to be more immersive.</summary>
    private static void EditProjectilesTileSheet(IAssetData asset)
    {
        if (!CombatModule.Config.ShadowyShooterProjectile)
        {
            return;
        }

        var editor = asset.AsImage();
        var sourceArea = new Rectangle(16, 0, 16, 16);
        var targetArea = new Rectangle(64, 16, 16, 16);
        editor.PatchImage(
            ModHelper.ModContent.Load<Texture2D>("assets/sprites/projectiles"),
            sourceArea,
            targetArea);
    }

    /// <summary>Adjust Jinxed debuff description.</summary>
    private static void EditStringsFromCsFiles(IAssetData asset)
    {
        if (!CombatModule.Config.OverhauledDefense)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["Buff.cs.465"] = I18n.Ui_Buffs_Jinxed();
    }

    #endregion editor callbacks
}
