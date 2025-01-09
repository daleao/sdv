namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CombinedRingUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.update));
    }

    #region harmony patches

    /// <summary>Update Infinity Band resonances.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CombinedRingUpdatePostfix(CombinedRing __instance, Farmer who)
    {
        __instance.Get_Chord()?.Update(who);
    }

    #endregion harmony patches
}
