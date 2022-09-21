namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using DaLion.Stardew.Rings.Framework.VirtualProperties;
using HarmonyLib;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnEquipPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnEquipPatch"/> class.</summary>
    internal CombinedRingOnEquipPatch()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onEquip));
    }

    #region harmony patches

    /// <summary>Add Infinity Band resonance effects.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnEquipPostfix(CombinedRing __instance, Farmer who)
    {
        __instance.Get_Chord()?.OnEquip(who.currentLocation, who);
    }

    #endregion harmony patches
}
