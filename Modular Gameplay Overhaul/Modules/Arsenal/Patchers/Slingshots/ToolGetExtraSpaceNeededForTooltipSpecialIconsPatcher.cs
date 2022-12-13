namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Slingshots;

#region using directives

using System.Linq;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolGetExtraSpaceNeededForTooltipSpecialIconsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolGetExtraSpaceNeededForTooltipSpecialIconsPatcher"/> class.</summary>
    internal ToolGetExtraSpaceNeededForTooltipSpecialIconsPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.getExtraSpaceNeededForTooltipSpecialIcons));
    }

    #region harmony patches

    /// <summary>Fix forged Slingshot tooltip box height.</summary>
    [HarmonyPostfix]
    private static void ToolGetExtraSpaceNeededForTooltipSpecialIconsPostfix(
        Tool __instance, ref Point __result, SpriteFont font)
    {
        if (__instance is not Slingshot slingshot || slingshot.enchantments.Count <= 0)
        {
            return;
        }

        if (slingshot.GetTotalForgeLevels() > 0)
        {
            var forgeCount = slingshot.enchantments.Where(e => e.IsForge()).Distinct().Count();
            if (slingshot.hasEnchantmentOfType<DiamondEnchantment>())
            {
                __result.X = (int)Math.Max(
                    __result.X,
                    font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural", __instance.GetMaxForges())).X);
            }

            __result.Y += (int)(Math.Max(font.MeasureString("TT").Y, 48f) * forgeCount);
        }
    }

    #endregion harmony patches
}
