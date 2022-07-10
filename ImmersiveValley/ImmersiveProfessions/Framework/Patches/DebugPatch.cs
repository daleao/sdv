using DaLion.Common;
using HarmonyLib;
using JetBrains.Annotations;

namespace DaLion.Stardew.Professions.Framework.Patches;

/// <summary>Wildcard prefix patch for on-demand debugging.</summary>
[UsedImplicitly]
internal class DebugPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal DebugPatch()
    {
#if DEBUG
        //Target = RequireMethod<>(nameof(.));
#endif
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