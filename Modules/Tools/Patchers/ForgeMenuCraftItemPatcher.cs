namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Tools.Integrations;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuCraftItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuCraftItemPatcher"/> class.</summary>
    internal ForgeMenuCraftItemPatcher()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.CraftItem));
    }

    #region harmony patches

    /// <summary>Allow forge upgrades.</summary>
    [HarmonyPostfix]
    private static void ForgeMenuCraftItemPostfix(ref Item __result, Item left_item, Item right_item)
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
            ((Tool)left_item).UpgradeLevel++;
        }

        __result = left_item;
    }

    #endregion harmony patches
}
