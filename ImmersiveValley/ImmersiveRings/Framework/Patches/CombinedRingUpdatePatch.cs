namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingUpdatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CombinedRingUpdatePatch()
    {
        Target = RequireMethod<CombinedRing>(nameof(CombinedRing.update));
    }

    #region harmony patches

    /// <summary>Update Iridium Band resonance.</summary>
    [HarmonyPostfix]
    private static void CombinedRingUpdatePostfix(CombinedRing __instance, GameLocation environment, Farmer who)
    {
        if (__instance.ParentSheetIndex == Constants.IRIDIUM_BAND_INDEX_I && __instance.combinedRings.Count >= 2)
            __instance.UpdateResonanceGlow(environment, who);
    }

    #endregion harmony patches
}