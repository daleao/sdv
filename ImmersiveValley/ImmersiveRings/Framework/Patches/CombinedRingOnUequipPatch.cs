using DaLion.Stardew.Rings.Framework.Events;

namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Common.Extensions.Collections;
using Extensions;
using HarmonyLib;
using StardewValley.Objects;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnUnequipPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CombinedRingOnUnequipPatch()
    {
        Target = RequireMethod<CombinedRing>(nameof(CombinedRing.onUnequip));
    }

    #region harmony patches

    /// <summary>Remove Iridium Band resonance.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnUnequipPostfix(CombinedRing __instance, Farmer who)
    {
        if (__instance.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I || __instance.combinedRings.Count < 2) return;

        __instance.get_Resonances().ForEach(pair => pair.Key.OnUnequip(pair.Value, who.currentLocation, who));
        __instance.UnapplyResonanceGlow(who.currentLocation);
        if (!who.leftRing.Value.IsCombinedIridiumBand(out _) && !who.rightRing.Value.IsCombinedIridiumBand(out _))
            ModEntry.Events.Disable<ResonanceUpdateTickedEvent>();
    }

    #endregion harmony patches
}