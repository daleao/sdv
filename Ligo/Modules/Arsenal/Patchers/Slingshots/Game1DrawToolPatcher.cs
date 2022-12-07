namespace DaLion.Ligo.Modules.Arsenal.Patchers.Slingshots;

#region using directives

using System.Reflection;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1DrawToolPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1DrawToolPatcher"/> class.</summary>
    internal Game1DrawToolPatcher()
    {
        this.Target = this.RequireMethod<Game1>(nameof(Game1.drawTool), new[] { typeof(Farmer), typeof(int) });
    }

    #region harmony patches

    /// <summary>Draw slingshot during stunning slam.</summary>
    [HarmonyPrefix]
    private static bool Game1DrawToolPrefix(Farmer f)
    {
        try
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
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
