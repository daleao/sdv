namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolbarReceiveClickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolbarReceiveClickPatcher"/> class.</summary>
    internal ToolbarReceiveClickPatcher()
    {
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<Toolbar>(nameof(Toolbar.receiveLeftClick));
        base.ApplyImpl(harmony);

        this.Target = this.RequireMethod<Toolbar>(nameof(Toolbar.receiveRightClick));
        base.ApplyImpl(harmony);
    }

    /// <inheritdoc />
    protected override void UnapplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<Toolbar>(nameof(Toolbar.receiveLeftClick));
        base.UnapplyImpl(harmony);

        this.Target = this.RequireMethod<Toolbar>(nameof(Toolbar.receiveRightClick));
        base.UnapplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Toggle selectable tool.</summary>
    [HarmonyPrefix]
    private static bool ToolbarReceiveClickPrefix(Item? ___hoverItem, bool playSound)
    {
        if (!ToolsModule.Config.EnableAutoSelection || !ToolsModule.Config.ModKey.IsDown())
        {
            return true; // run original logic
        }

        if (___hoverItem is not (Tool tool
            and (Axe or Hoe or Pickaxe or WateringCan or FishingRod or MilkPail or Shears or MeleeWeapon)))
        {
            return false; // don't run original logic
        }

        if (tool is MeleeWeapon weapon && !weapon.isScythe())
        {
            return false; // don't run original logic
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
