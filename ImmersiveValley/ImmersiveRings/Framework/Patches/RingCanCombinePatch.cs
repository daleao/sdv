namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using DaLion.Stardew.Rings.Extensions;
using HarmonyLib;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RingCanCombinePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RingCanCombinePatch"/> class.</summary>
    internal RingCanCombinePatch()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.CanCombine));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Allows feeding up to four gemstone rings into an Infinity Band.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingCanCombinePrefix(Ring __instance, ref bool __result, Ring ring)
    {
        if (!ModEntry.Config.TheOneIridiumBand)
        {
            return true; // run original logic
        }

        if (__instance.ParentSheetIndex != Constants.IridiumBandIndex)
        {
            return ring.ParentSheetIndex != Constants.IridiumBandIndex;
        }

        __result = ring.IsGemRing() &&
                   (__instance is not CombinedRing combined || combined.combinedRings.Count < 4);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
