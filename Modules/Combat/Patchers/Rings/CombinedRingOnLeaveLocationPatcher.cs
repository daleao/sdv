namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnLeaveLocationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnLeaveLocationPatcher"/> class.</summary>
    internal CombinedRingOnLeaveLocationPatcher()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onLeaveLocation));
    }

    #region harmony patches

    /// <summary>Remove Infinity Band resonance location effects.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnLeaveLocationPostfix(CombinedRing __instance, GameLocation environment)
    {
        __instance.Get_Chord()?.OnLeaveLocation(environment);
    }

    #endregion harmony patches
}
