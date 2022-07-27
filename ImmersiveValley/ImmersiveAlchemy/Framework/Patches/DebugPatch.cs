#if DEBUG
using DaLion.Common;
using DaLion.Common.Attributes;
using HarmonyLib;
using JetBrains.Annotations;

namespace DaLion.Stardew.Alchemy.Framework.Patches;

/// <summary>Wildcard prefix patch for on-demand debugging.</summary>
[UsedImplicitly, DebugOnly]
internal class DebugPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
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
#endif