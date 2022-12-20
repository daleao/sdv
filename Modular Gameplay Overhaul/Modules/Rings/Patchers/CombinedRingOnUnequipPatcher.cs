namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnUnequipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnUnequipPatcher"/> class.</summary>
    internal CombinedRingOnUnequipPatcher()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onUnequip));
    }

    #region harmony patches

    /// <summary>Remove Infinity Band resonance effects.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnUnequipPostfix(CombinedRing __instance, Farmer who)
    {
        var chord = __instance.Get_Chord();
        if (chord is null)
        {
            return;
        }

        chord.Unapply(who.currentLocation, who);
        if (ArsenalModule.IsEnabled && who.CurrentTool is { } tool && chord.Root is not null &&
            tool.Get_ResonatingChord(chord.Root.EnchantmentType) == chord)
        {
            tool.UnsetResonatingChord(chord.Root.EnchantmentType);
        }
    }

    #endregion harmony patches
}
