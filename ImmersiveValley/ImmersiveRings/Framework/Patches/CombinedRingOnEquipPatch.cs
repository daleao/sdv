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
internal sealed class CombinedRingOnEquipPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CombinedRingOnEquipPatch()
    {
        Target = RequireMethod<CombinedRing>(nameof(CombinedRing.onEquip));
    }

    #region harmony patches

    /// <summary>Add Iridium Band resonance.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnEquipPostfix(CombinedRing __instance, Farmer who)
    {
        if (__instance.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I || __instance.combinedRings.Count < 2) return;

        __instance.get_Resonances().ForEach(pair => pair.Key.OnEquip(pair.Value, who.currentLocation, who));
        __instance.ApplyResonanceGlow(who.currentLocation, who);
        ModEntry.Events.Enable<ResonanceUpdateTickedEvent>();
    }

    #endregion harmony patches
}