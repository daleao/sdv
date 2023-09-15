namespace DaLion.Overhaul.Modules.Tools.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Tools.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidCraftPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidCraftPatcher"/> class.</summary>
    internal NewForgeMenuIsValidCraftPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.IsValidCraft));
    }

    #region harmony patches

    /// <summary>Allow forge upgrades.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidCraftPostfix(ref bool __result, Item left_item, Item right_item)
    {
        if (!ToolsModule.Config.EnableForgeUpgrading)
        {
            return;
        }

        if (left_item is not (Tool tool and (Axe or Hoe or Pickaxe or WateringCan)))
        {
            return;
        }

        var maxToolUpgrade = MoonMisadventuresIntegration.Instance?.IsLoaded == true ? 6 : 5;
        if (tool.UpgradeLevel >= maxToolUpgrade)
        {
            return;
        }

        var upgradeItemIndex = tool.UpgradeLevel switch
        {
            0 => ObjectIds.CopperBar,
            1 => ObjectIds.IronBar,
            2 => ObjectIds.GoldBar,
            3 => ObjectIds.IridiumBar,
            4 => ObjectIds.RadioactiveBar,
            5 => "spacechase0.MoonMisadventures/Mythicite Bar".GetDeterministicHashCode(),
            _ => SObject.prismaticShardIndex,
        };

        if (right_item.ParentSheetIndex == upgradeItemIndex && right_item.Stack >= 5)
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
