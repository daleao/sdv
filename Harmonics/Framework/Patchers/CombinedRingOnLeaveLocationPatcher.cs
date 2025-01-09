namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnLeaveLocationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnLeaveLocationPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CombinedRingOnLeaveLocationPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onLeaveLocation));
    }

    #region harmony patches

    /// <summary>Remove Infinity Band resonance location effects.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CombinedRingOnLeaveLocationPostfix(CombinedRing __instance, GameLocation environment)
    {
        __instance.Get_Chord()?.OnLeaveLocation(environment);
    }

    #endregion harmony patches
}
