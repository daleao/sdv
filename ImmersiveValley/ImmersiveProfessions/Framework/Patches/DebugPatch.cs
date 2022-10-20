namespace DaLion.Stardew.Professions.Framework.Patches;

using DaLion.Common.Attributes;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

/// <summary>Wildcard prefix patch for on-demand debugging.</summary>
[UsedImplicitly]
[DebugOnly]
internal class DebugPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="DebugPatch"/> class.</summary>
    internal DebugPatch()
    {
        //Target = RequireMethod<>(nameof(.));
    }

    #region harmony patches

    [HarmonyPrefix]
    private static bool DebugPrefix(object __instance)
    {
        Log.D("DebugPatch called!");

        return false; // don't run original logic
    }

    #endregion harmony patches
}
