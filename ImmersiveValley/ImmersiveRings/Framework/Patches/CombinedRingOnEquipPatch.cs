namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Common.Extensions.Collections;
using Extensions;
using HarmonyLib;
using StardewValley.Objects;

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
        __instance.CheckResonances().ForEach(r => r.OnEquip(who));
    }

    #endregion harmony patches
}