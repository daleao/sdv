namespace DaLion.Overhaul.Modules.Tools.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Tools.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuCraftItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuCraftItemPatcher"/> class.</summary>
    internal NewForgeMenuCraftItemPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.CraftItem));
    }

    #region harmony patches

    /// <summary>Allow forge upgrades.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuCraftItemPostfix(ref Item __result, Item left_item, Item right_item)
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
                    _ => -1,
                };

                if (upgradeItemIndex < 0)
                {
                    Log.E("Go home game. You're drunk.");
                }

                if (right_item.ParentSheetIndex == upgradeItemIndex && right_item.Stack >= 5)
                {
                    ((Tool)left_item).UpgradeLevel++;
                }

                break;
            }

            case 5 when right_item.ParentSheetIndex == 1720 &&
                        Reflector.GetUnboundPropertyGetter<object, string>(right_item, "FullId").Invoke(right_item) ==
                        "spacechase0.MoonMisadventures/Mythicite Bar":
                ((Tool)left_item).UpgradeLevel++;
                break;
        }

        __result = left_item;
    }

    #endregion harmony patches
}
