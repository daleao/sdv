namespace DaLion.Redux.Rings.Patches;

#region using directives

using DaLion.Redux.Rings.Extensions;
using HarmonyLib;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

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
        if (!ModEntry.Config.Rings.TheOneIridiumBand)
        {
            return true; // run original logic
        }

        if (__instance.ParentSheetIndex == Constants.IridiumBandIndex ||
            ring.ParentSheetIndex == Constants.IridiumBandIndex)
        {
            return false;
        }

        if (__instance.ParentSheetIndex != Globals.InfinityBandIndex)
        {
            return ring.ParentSheetIndex != Globals.InfinityBandIndex;
        }

        __result = ring.IsGemRing() &&
                   (__instance is not CombinedRing combined || combined.combinedRings.Count < 4);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
