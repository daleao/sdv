namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

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
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ToolGetExtraSpaceNeededForTooltipSpecialIconsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.getExtraSpaceNeededForTooltipSpecialIcons));
    }

    #region harmony patches

    /// <summary>Fix enchanted Slingshot tooltip box height.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ToolGetExtraSpaceNeededForTooltipSpecialIconsPostfix(
        Tool __instance, ref Point __result)
    {
        if (__instance is Slingshot slingshot && slingshot.enchantments.Any())
        {
            __result.Y += 4;
        }
    }

    #endregion harmony patches
}
