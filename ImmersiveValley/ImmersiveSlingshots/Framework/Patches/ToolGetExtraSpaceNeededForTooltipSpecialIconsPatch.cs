namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using HarmonyLib;
using LinqFasterer;
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
        if (__instance is not Slingshot slingshot) return;

        if (slingshot.enchantments.Count > 0)
        {
            if (slingshot.GetTotalForgeLevels() > 0)
            {
                var forgeCount = slingshot.enchantments.Where(e => e.IsForge()).Distinct().Count();
                if (slingshot.hasEnchantmentOfType<DiamondEnchantment>())
                    __result.X = (int)Math.Max(__result.X,
                        font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural",
                            __instance.GetMaxForges())).X);

                __result.Y += (int)(Math.Max(font.MeasureString("TT").Y, 48f) * (forgeCount + 1));
            }

            if (slingshot.enchantments.AnyF(e => !e.IsForge())) __result.Y += 12;
        }
    }

    #endregion harmony patches
}