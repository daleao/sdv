namespace DaLion.Redux.Framework.Arsenal.Slingshots.Patches;

#region using directives

using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolGetExtraSpaceNeededForTooltipSpecialIconsPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ToolGetExtraSpaceNeededForTooltipSpecialIconsPatch"/> class.</summary>
    internal ToolGetExtraSpaceNeededForTooltipSpecialIconsPatch()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.getExtraSpaceNeededForTooltipSpecialIcons));
    }

    #region harmony patches

    /// <summary>Fix forged Slingshot tooltip box height.</summary>
    [HarmonyPostfix]
    private static void ToolGetExtraSpaceNeededForTooltipSpecialIconsPostfix(
        Tool __instance, ref Point __result, SpriteFont font)
    {
        if (__instance is not Slingshot slingshot)
        {
            return;
        }

        if (slingshot.enchantments.Count > 0)
        {
            if (slingshot.GetTotalForgeLevels() > 0)
            {
                var forgeCount = slingshot.enchantments.Where(e => e.IsForge()).Distinct().Count();
                if (slingshot.hasEnchantmentOfType<DiamondEnchantment>())
                {
                    __result.X = (int)Math.Max(
                        __result.X,
                        font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural", __instance.GetMaxForges())).X);
                }

                __result.Y += (int)(Math.Max(font.MeasureString("TT").Y, 48f) * (forgeCount + 1));
            }

            if (slingshot.enchantments.Any(e => !e.IsForge()))
            {
                __result.Y += 12;
            }
        }
    }

    #endregion harmony patches
}
