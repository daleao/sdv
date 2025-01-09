namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnNewLocationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnNewLocationPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CombinedRingOnNewLocationPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onNewLocation));
    }

    #region harmony patches

    /// <summary>Add Infinity Band resonance location effects.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CombinedRingOnNewLocationPostfix(CombinedRing __instance, GameLocation environment)
    {
        __instance.Get_Chord()?.OnNewLocation(environment);
    }

    #endregion harmony patches
}
