namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using DaLion.Stardew.Slingshots.Extensions;
using DaLion.Stardew.Slingshots.Framework.VirtualProperties;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1DrawToolPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="Game1DrawToolPatch"/> class.</summary>
    internal Game1DrawToolPatch()
    {
        this.Target = this.RequireMethod<Game1>(nameof(Game1.drawTool), new[] { typeof(Farmer), typeof(int) });
    }

    #region harmony patches

    /// <summary>Draw slingshot during stunning slam.</summary>
    [HarmonyPrefix]
    private static bool Game1DrawToolPrefix(Farmer f)
    {
        if (f.CurrentTool is not Slingshot slingshot || !slingshot.Get_IsOnSpecial())
        {
            return true; // run original logic
        }

        var position = f.getLocalPosition(Game1.viewport) + f.jitter + f.armOffset;
        var sourceRect = Game1.getSourceRectForStandardTileSheet(
            Tool.weaponsTexture,
            slingshot.IndexOfMenuItemView,
            16,
            16);
        slingshot.DrawDuringUse(
            f.FarmerSprite.currentAnimationIndex,
            f.FacingDirection,
            Game1.spriteBatch,
            position,
            f,
            sourceRect);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
