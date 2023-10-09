namespace DaLion.Overhaul.Modules.Tools.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.MoonMisadventures", "Moon Misadventure")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
internal sealed class GetBlacksmithUpgradeStockPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GetBlacksmithUpgradeStockPatcher"/> class.</summary>
    internal GetBlacksmithUpgradeStockPatcher()
    {
        this.Target = this.RequireMethod<Utility>(nameof(Utility.getBlacksmithUpgradeStock));
    }

    #region harmony patches

    /// <summary>Prevents Radioactive upgrades at Clint's.</summary>
    [HarmonyPostfix]
    private static void UtilityGetShopStockPostfix(object __instance, Dictionary<ISalable, int[]> __result)
    {
        if (!ToolsModule.Config.EnableForgeUpgrading)
        {
            return;
        }

        for (var i = __result.Count - 1; i >= 0; i--)
        {
            var salable = __result.ElementAt(i).Key;
            if (salable is Tool { UpgradeLevel: >4 })
            {
                __result.Remove(salable);
            }
        }
    }

    #endregion harmony patches
}
