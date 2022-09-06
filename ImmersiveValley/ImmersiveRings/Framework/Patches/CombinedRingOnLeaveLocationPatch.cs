namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnLeaveLocationPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CombinedRingOnLeaveLocationPatch()
    {
        Target = RequireMethod<CombinedRing>(nameof(CombinedRing.onLeaveLocation));
    }

    #region harmony patches

    /// <summary>Remove Iridium Band resonance location effects.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnLeaveLocationPostfix(CombinedRing __instance, GameLocation environment)
    {
        if (__instance.ParentSheetIndex == Constants.IRIDIUM_BAND_INDEX_I && __instance.combinedRings.Count >= 2)
            __instance.UnapplyResonanceGlow(environment);
    }

    #endregion harmony patches
}