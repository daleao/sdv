namespace DaLion.Ligo.Modules.Rings.Patchers;

#region using directives

using DaLion.Ligo.Modules.Rings.Extensions;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingGetExtraSpaceNeededForTooltipSpecialIconsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingGetExtraSpaceNeededForTooltipSpecialIconsPatcher"/> class.</summary>
    internal RingGetExtraSpaceNeededForTooltipSpecialIconsPatcher()
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
        __result.Y += (int)(Math.Max(font.MeasureString("TT").Y, 48f) *
                            combined.Get_StatBuffer().Count());
        if (combined.Get_Chord()?.Root is not null)
        {
            __result.Y += (int)font.MeasureString("T").Y;
        }
    }

    #endregion harmony patches
}
