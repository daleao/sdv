namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using System;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolGetExtraSpaceNeededForTooltipSpecialIconsPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ToolGetExtraSpaceNeededForTooltipSpecialIconsPatch()
    {
        Target = RequireMethod<Tool>(nameof(Tool.getExtraSpaceNeededForTooltipSpecialIcons));
    }

    #region harmony patches

    /// <summary>Fix forged Slingshot tooltip box height.</summary>
    [HarmonyPostfix]
    private static void ToolGetExtraSpaceNeededForTooltipSpecialIconsPostfix(Tool __instance, ref Point __result, SpriteFont font)
    {
        if (__instance is not Slingshot slingshot || slingshot.GetTotalForgeLevels() <= 0)
            return;

        if (__instance.hasEnchantmentOfType<DiamondEnchantment>())
            __result.X = (int)Math.Max(__result.X,
                font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural",
                    __instance.GetMaxForges())).X);

        __result.Y += (int)(Math.Max(font.MeasureString("TT").Y, 48f) *
                             (__instance.enchantments.Where(e => e.IsForge()).Distinct().Count() + 1));
    }

    #endregion harmony patches
}