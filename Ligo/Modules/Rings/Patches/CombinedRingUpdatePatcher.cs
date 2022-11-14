namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using DaLion.Ligo.Modules.Rings.VirtualProperties;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingUpdatePatcher"/> class.</summary>
    internal CombinedRingUpdatePatcher()
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
