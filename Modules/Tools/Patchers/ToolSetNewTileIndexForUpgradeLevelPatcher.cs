namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModConflict("spacechase0.MoonMisadventures")]
internal sealed class ToolSetNewTileIndexForUpgradeLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolSetNewTileIndexForUpgradeLevelPatcher"/> class.</summary>
    internal ToolSetNewTileIndexForUpgradeLevelPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.setNewTileIndexForUpgradeLevel));
    }

    #region harmony patches

    /// <summary>Don't change tile index. We will simply patch over it.</summary>
    [HarmonyPrefix]
    [SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Bypass property setter.")]
    private static void ToolSetNewTileIndexForUpgradeLevelPrefix(Tool __instance, ref int? __state)
    {
        if (__instance.UpgradeLevel < 5)
        {
            return;
        }

        __state = __instance.UpgradeLevel;
        __instance.upgradeLevel.Value = 4;
    }

    /// <summary>Don't change tile index. We will simply patch over it.</summary>
    [HarmonyPostfix]
    [SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Bypass property setter.")]
    private static void ToolTilesAffectedPostfix(Tool __instance, ref int? __state)
    {
        if (__state.HasValue)
        {
            __instance.upgradeLevel.Value = __state.Value;
        }
    }

    #endregion harmony patches
}
