namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
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
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal RingGetExtraSpaceNeededForTooltipSpecialIconsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.getExtraSpaceNeededForTooltipSpecialIcons));
    }

    internal static int MinWidth { private get; set; }

    #region harmony patches

    /// <summary>Fix combined Infinity Band tooltip box height.</summary>
    [HarmonyPrefix]
    private static bool RingGetExtraSpaceNeededForTooltipSpecialIconsPostfix(Ring __instance, ref Point __result, SpriteFont font, int horizontalBuffer, int startingHeight)
    {
        if (__instance is not CombinedRing combined || combined.QualifiedItemId != $"(O){InfinityBandId}")
        {
            return true; // run original logic
        }

        __result.X = (int)Math.Max(
            MinWidth,
            font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", 9999)).X +
            horizontalBuffer);
        __result.Y = startingHeight + (int)(Math.Max(font.MeasureString("TT").Y, 48f) * combined.Get_StatBuffer().Count());
        if (combined.Get_Chord()?.Root is not null)
        {
            __result.Y += (int)font.MeasureString("T").Y;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
