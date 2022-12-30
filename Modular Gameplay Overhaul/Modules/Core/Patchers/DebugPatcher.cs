namespace DaLion.Overhaul.Modules.Core.Patchers;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;

#endregion using directives

[UsedImplicitly]
[Debug]
[ImplicitIgnore]
internal sealed class DebugPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DebugPatcher"/> class.</summary>
    internal DebugPatcher()
    {
    }

    #region harmony patches

    /// <summary>Placeholder patch for debugging.</summary>
    [HarmonyPrefix]
    private static bool DebugPrefix(object __instance)
    {
        //var caller = new StackTrace().GetFrame(1)?.GetMethod()?.GetFullName();
        //Log.D($"{caller} prefix called!");
        return true;
    }

    /// <summary>Placeholder patch for debugging.</summary>
    [HarmonyPostfix]
    private static void DebugPostfix(object __instance)
    {
        //var caller = new StackTrace().GetFrame(1)?.GetMethod()?.GetFullName();
        //Log.D($"{caller} postfix called!");
    }

    #endregion harmony patches
}
