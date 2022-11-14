namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using DaLion.Ligo.Modules.Rings.VirtualProperties;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnNewLocationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnNewLocationPatcher"/> class.</summary>
    internal CombinedRingOnNewLocationPatcher()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onNewLocation));
    }

    #region harmony patches

    /// <summary>Add Infinity Band resonance location effects.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnNewLocationPostfix(CombinedRing __instance, GameLocation environment)
    {
        __instance.Get_Chord()?.OnNewLocation(environment);
    }

    #endregion harmony patches
}
