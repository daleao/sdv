namespace DaLion.Overhaul.Modules.Core.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Shared.Attributes;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DebugPatcher"/> class.</summary>
    internal DebugPatcher()
    {
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void DebugPrefix(object __instance, object __result)
    {
        Log.D("Debug prefix called!");
    }

    [HarmonyPostfix]
    private static void DebugPostfix(object __instance, object __result)
    {
        Log.D("Debug postfix called!");
    }

    #endregion harmony patches
}
