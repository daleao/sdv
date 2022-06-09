namespace DaLion.Stardew.Alchemy.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;

#endregion using directives

/// <summary>Wildcard prefix patch for on-demand debugging.</summary>
[UsedImplicitly]
internal class DebugPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal DebugPatch()
    {
#if DEBUG
        //Original = RequireMethod<>(nameof(.));
#endif
    }

    #region harmony patches

    [HarmonyPrefix]
    private static bool DebugPrefix(object __instance)
    {
        Log.D("DebugPatch called!");

        return true; // run original logic
    }

    #endregion harmony patches
}