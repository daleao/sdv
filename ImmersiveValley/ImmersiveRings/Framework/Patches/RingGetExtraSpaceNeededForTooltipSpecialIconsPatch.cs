namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using System;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class RingGetExtraSpaceNeededForTooltipSpecialIconsPatch : Common.Harmony.HarmonyPatch
{
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
        if (__instance is not CombinedRing { ParentSheetIndex: Constants.IRIDIUM_BAND_INDEX_I } iridiumBand ||
            iridiumBand.combinedRings.Count == 0) return;

        __result.Y += (int)(Math.Max(font.MeasureString("TT").Y, 48f) *
                             iridiumBand.combinedRings.Select(r => r.ParentSheetIndex).Distinct().Count());
        if (iridiumBand.IsResonant(out _)) __result.Y += (int)Math.Min(font.MeasureString("TT").Y, 48f);
    }

    #endregion harmony patches
}