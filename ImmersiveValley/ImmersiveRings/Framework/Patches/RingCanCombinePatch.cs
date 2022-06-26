namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingCanCombinePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal RingCanCombinePatch()
    {
        Target = RequireMethod<Ring>(nameof(Ring.CanCombine));
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Allows feeding up to four gemstone rings into iridium bands.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingCanCombinePrefix(Ring __instance, ref bool __result, Ring ring)
    {
        if (!ModEntry.Config.TheOneIridiumBand) return true; // run original logic

        if (__instance.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I)
            return ring.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I;

        __result = ring.IsGemRing() &&
                   (__instance is not CombinedRing combined || combined.combinedRings.Count < 4);
        return false; // don't run original logic

    }

    #endregion harmony patches
}