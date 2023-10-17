namespace DaLion.Overhaul.Modules.Core.Patchers;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;

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
    private static void DebugPrefix()
    {
        Log.D("Debug prefix called!");
    }

    [HarmonyPostfix]
    private static void DebugPostfix()
    {
        Log.D("Debug postfix called!");
    }

    #endregion harmony patches
}
