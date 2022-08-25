namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using StardewValley.Tools;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1DrawToolPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal Game1DrawToolPatch()
    {
        Target = RequireMethod<Game1>(nameof(Game1.drawTool), new[] { typeof(Farmer), typeof(int) });
    }

    #region harmony patches

    /// <summary>Draw slingshot during stunning slam.</summary>
    [HarmonyPrefix]
    private static bool Game1DrawToolPrefix(Farmer f)
    {
        if (f.CurrentTool is not Slingshot slingshot || !slingshot.get_IsOnSpecial()) return true; // run original logic

        var position = f.getLocalPosition(Game1.viewport) + f.jitter + f.armOffset;
        var sourceRect =
            Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, slingshot.IndexOfMenuItemView, 16, 16);
        slingshot.DrawDuringUse(((FarmerSprite)f.Sprite).currentAnimationIndex, f.FacingDirection, Game1.spriteBatch,
            position, f, sourceRect);
        return false; // don't run original logic
    }

    #endregion harmony patches
}