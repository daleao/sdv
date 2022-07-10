namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
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
        if (__instance.IsResonant(out var resonance)) resonance.OnEquip(who);
    }

    #endregion harmony patches
}