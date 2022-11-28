namespace DaLion.Ligo.Modules.Core.Patchers;

#region using directives

using System.Diagnostics;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
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
    internal static void DebugPrefix(object __instance)
    {
        var caller = new StackTrace().GetFrame(1)?.GetMethod()?.GetFullName();
        Log.D($"{caller} prefix called!");
    }

    /// <summary>Placeholder patch for debugging.</summary>
    [HarmonyPostfix]
    internal static void DebugPostfix(object __instance)
    {
        var caller = new StackTrace().GetFrame(1)?.GetMethod()?.GetFullName();
        Log.D($"{caller} postfix called!");
    }

    #endregion harmony patches
}
