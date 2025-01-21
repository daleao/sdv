namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnUnequipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnUnequipPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CombinedRingOnUnequipPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onUnequip));
    }

    #region harmony patches

    /// <summary>Remove Infinity Band resonance effects.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CombinedRingOnUnequipPostfix(CombinedRing __instance, Farmer who)
    {
        __instance.Get_Chord()?.Quench(who);
    }

    #endregion harmony patches
}
