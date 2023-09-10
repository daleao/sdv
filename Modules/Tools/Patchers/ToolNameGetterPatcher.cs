namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ModConflict("spacechase0.MoonMisadventures")]
internal sealed class ToolNameGetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolNameGetterPatcher"/> class.</summary>
    internal ToolNameGetterPatcher()
    {
        this.Target = this.RequirePropertyGetter<Tool>(nameof(Tool.Name));
    }

    #region harmony patches

    /// <summary>Don't change tile index. We will simply patch over it.</summary>
    [HarmonyPrefix]
    private static bool ToolSetNewTileIndexForUpgradeLevelPrefix(Tool __instance, ref string __result)
    {
        if (__instance.UpgradeLevel < 5)
        {
            return true; // run original logic
        }

        var tool = __instance switch
        {
            Axe => "axe",
            Hoe => "hoe",
            Pickaxe => "pick",
            WateringCan => "can",
            _ => string.Empty,
        };

        __result = _I18n.Get("radioactive." + tool);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
