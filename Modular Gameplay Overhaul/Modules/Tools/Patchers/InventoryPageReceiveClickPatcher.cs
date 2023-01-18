namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class InventoryPageReceiveClickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="InventoryPageReceiveClickPatcher"/> class.</summary>
    internal InventoryPageReceiveClickPatcher()
    {
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<InventoryPage>(nameof(InventoryPage.receiveLeftClick));
        base.ApplyImpl(harmony);

        this.Target = this.RequireMethod<InventoryPage>(nameof(InventoryPage.receiveRightClick));
        base.ApplyImpl(harmony);
    }

    /// <inheritdoc />
    protected override void UnapplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<InventoryPage>(nameof(InventoryPage.receiveLeftClick));
        base.UnapplyImpl(harmony);

        this.Target = this.RequireMethod<InventoryPage>(nameof(InventoryPage.receiveRightClick));
        base.UnapplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Toggle selectable tool.</summary>
    [HarmonyPrefix]
    private static bool InventoryPageReceiveClickPrefix(Item? ___hoveredItem, bool playSound)
    {
        if (!ToolsModule.Config.EnableAutoSelection || !ToolsModule.Config.ModKey.IsDown())
        {
            return true; // run original logic
        }

        if (___hoveredItem is not (Tool tool
            and (Axe or Hoe or Pickaxe or WateringCan or FishingRod or MilkPail or Shears or MeleeWeapon)))
        {
            return true; // run original logic
        }

        if (tool is MeleeWeapon weapon && !weapon.isScythe())
        {
            return true; // run original logic
        }

        if (ToolsModule.State.SelectableTools.Contains(tool))
        {
            ToolsModule.State.SelectableTools.Remove(tool);
            if (playSound)
            {
                Game1.playSound("smallSelect");
            }

            return false; // don't run original logic
        }

        if (ToolsModule.State.SelectableTools.ContainsType(tool.GetType()))
        {
            ToolsModule.State.SelectableTools.RemoveTypes(tool.GetType());
        }

        ToolsModule.State.SelectableTools.Add(tool);
        if (playSound)
        {
            Game1.playSound("smallSelect");
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
