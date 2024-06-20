namespace DaLion.Overhaul.Modules.Combat.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuGetForgeCostPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuGetForgeCostPatcher"/> class.</summary>
    internal NewForgeMenuGetForgeCostPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>("GetForgeCost");
    }

    #region harmony patches

    /// <summary>Modify forge cost of Infinity Band.</summary>
    [HarmonyPrefix]
    private static bool NewForgeMenuGetForgeCostPrefix(ref int __result, Item left_item, Item right_item)
    {
        if (!CombatModule.Config.RingsEnchantments.EnableInfinityBand || left_item is not Ring left)
        {
            return true; // run original logic
        }

        if (left.ParentSheetIndex == JsonAssetsIntegration.InfinityBandIndex && right_item is Ring right && right.IsGemRing())
        {
            __result = 10;
            return false; // don't run original logic
        }

        if (left.ParentSheetIndex == ObjectIds.IridiumBand &&
            right_item.ParentSheetIndex == ObjectIds.GalaxySoul)
        {
            __result = 20;
            return false; // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches
}
