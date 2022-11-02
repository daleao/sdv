namespace DaLion.Redux.Framework.Rings.Patches;

#region using directives

using DaLion.Redux.Framework.Rings.VirtualProperties;
using HarmonyLib;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingUpdatePatch"/> class.</summary>
    internal CombinedRingUpdatePatch()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.update));
    }

    #region harmony patches

    /// <summary>Update Infinity Band resonances.</summary>
    [HarmonyPostfix]
    private static void CombinedRingUpdatePostfix(CombinedRing __instance, GameLocation environment, Farmer who)
    {
        __instance.Get_Chord()?.Update(who);
    }

    #endregion harmony patches
}
