using DaLion.Stardew.Rings.Extensions;

namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using System;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class RingGetExtraSpaceNeededForTooltipSpecialIconsPatch : Common.Harmony.HarmonyPatch
{
    internal static int MaxWidth { private get; set; }

    /// <summary>Construct an instance.</summary>
    internal RingGetExtraSpaceNeededForTooltipSpecialIconsPatch()
    {
        Target = RequireMethod<Ring>(nameof(Ring.getExtraSpaceNeededForTooltipSpecialIcons));
    }

    #region harmony patches

    /// <summary>Fix combined Iridium Band tooltip box height.</summary>
    [HarmonyPostfix]
    private static void RingGetExtraSpaceNeededForTooltipSpecialIconsPostfix(Ring __instance, ref Point __result, SpriteFont font)
    {
        if (!__instance.IsCombinedIridiumBand(out var combined)) return;

        __result.X = Math.Max(__result.X, MaxWidth + 86);
        __result.Y += (int)(Math.Max(font.MeasureString("TT").Y, 48f) *
                             combined.combinedRings.Select(r => r.ParentSheetIndex).Distinct().Count());
    }

    #endregion harmony patches
}