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
internal sealed class ForgeMenuIsValidCraftPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuIsValidCraftPatcher"/> class.</summary>
    internal ForgeMenuIsValidCraftPatcher()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.IsValidCraft));
    }

    #region harmony patches

    /// <summary>Allow forge upgrades.</summary>
    [HarmonyPostfix]
    private static void ForgeMenuIsValidCraftPostfix(ref bool __result, Item left_item, Item right_item)
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

        switch (tool.UpgradeLevel)
        {
            case < 5:
            {
                var upgradeItemIndex = tool.UpgradeLevel switch
                {
                    0 => ObjectIds.CopperBar,
                    1 => ObjectIds.IronBar,
                    2 => ObjectIds.GoldBar,
                    3 => ObjectIds.IridiumBar,
                    4 => ObjectIds.RadioactiveBar,
                };

                if (right_item.ParentSheetIndex == upgradeItemIndex && right_item.Stack >= 5)
                {
                    __result = true;
                }

                break;
            }

            case 5 when right_item.ParentSheetIndex == 1720 &&
                        Reflector.GetUnboundPropertyGetter<object, string>(right_item, "FullId").Invoke(right_item) ==
                        "spacechase0.MoonMisadventures/Mythicite Bar" && right_item.Stack >= 5:
                __result = true;
                break;
        }
    }

    #endregion harmony patches
}
