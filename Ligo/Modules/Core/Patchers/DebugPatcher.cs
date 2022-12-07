namespace DaLion.Ligo.Modules.Core.Patchers;

#region using directives

using System.Diagnostics;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using DaLion.Shared.Integrations.BetterCrafting;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class DebugPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DebugPatcher"/> class.</summary>
    internal DebugPatcher()
    {
        Target = this.RequireMethod<InventoryPage>(nameof(InventoryPage.performHoverAction));
    }

    #region harmony patches

    /// <summary>Placeholder patch for debugging.</summary>
    [HarmonyPrefix]
    internal static bool DebugPrefix(object __instance)
    {
        var caller = new StackTrace().GetFrame(1)?.GetMethod()?.GetFullName();
        //Log.D($"{caller} prefix called!");
        return true;
    }

    /// <summary>Placeholder patch for debugging.</summary>
    [HarmonyPostfix]
    internal static void DebugPostfix(InventoryPage __instance, string ___hoverText, string ___hoverTitle)
    {
        var caller = new StackTrace().GetFrame(1)?.GetMethod()?.GetFullName();
        //Log.D($"{caller} postfix called!");
    }

    #endregion harmony patches
}
