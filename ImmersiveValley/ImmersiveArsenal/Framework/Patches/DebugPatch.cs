using DaLion.Common.Attributes;
using HarmonyLib;

namespace DaLion.Stardew.Arsenal.Framework.Patches;

/// <summary>Wildcard prefix patch for on-demand debugging.</summary>
[UsedImplicitly, DebugOnly]
internal class DebugPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal DebugPatch()
    {
        Target = RequireMethod<Farmer>("set_CanMove");
    }

    #region harmony patches

    [HarmonyPrefix]
    private static bool DebugPrefix(Farmer __instance, bool value)
    {
        return true; // don't run original logic
    }

    #endregion harmony patches
}