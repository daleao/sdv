namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using System;
using DaLion.Stardew.Rings.Extensions;
using DaLion.Stardew.Rings.Framework.VirtualProperties;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RingGetExtraSpaceNeededForTooltipSpecialIconsPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RingGetExtraSpaceNeededForTooltipSpecialIconsPatch"/> class.</summary>
    internal RingGetExtraSpaceNeededForTooltipSpecialIconsPatch()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.getExtraSpaceNeededForTooltipSpecialIcons));
    }

    internal static int MaxWidth { private get; set; }

    #region harmony patches

    /// <summary>Fix combined Infinity Band tooltip box height.</summary>
    [HarmonyPostfix]
    private static void RingGetExtraSpaceNeededForTooltipSpecialIconsPostfix(
        Ring __instance, ref Point __result, SpriteFont font)
    {
        if (!__instance.IsCombinedInfinityBand(out var combined))
        {
            return;
        }

        __result.X = Math.Max(__result.X, MaxWidth + 86);

        //__result.Y += (int)(Math.Max(font.MeasureString("TT").Y, 48f) *
        //                    combined.Get_StatBuffer().Select(r => r.ParentSheetIndex).Distinct().Count());
    }

    #endregion harmony patches
}
