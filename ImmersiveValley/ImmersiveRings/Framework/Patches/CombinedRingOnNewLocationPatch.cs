namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnNewLocationPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CombinedRingOnNewLocationPatch()
    {
        Target = RequireMethod<CombinedRing>(nameof(CombinedRing.onNewLocation));
    }

    #region harmony patches

    /// <summary>Add Iridium Band resonance location effects.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnNewLocationPostfix(CombinedRing __instance, Farmer who, GameLocation environment)
    {
        if (__instance.ParentSheetIndex == Constants.IRIDIUM_BAND_INDEX_I && __instance.combinedRings.Count >= 2)
            __instance.ApplyResonanceGlow(environment, who);
    }

    #endregion harmony patches
}