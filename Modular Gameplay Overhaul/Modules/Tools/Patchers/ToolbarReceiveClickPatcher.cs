namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using NetFabric.Hyperlinq;
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
            return true; // run original logic
        }

        if (tool is MeleeWeapon weapon && !weapon.isScythe())
        {
            return true; // run original logic
        }

        var toolIndex = Game1.player.Items.IndexOf(tool);
        var type = tool.GetType();
        if (type.Name.Contains("Pan") && type != typeof(Pan))
        {
            type = typeof(Pan);
        }
        else if (type.Name.Contains("Pail") && type != typeof(MilkPail))
        {
            type = typeof(MilkPail);
        }
        else if (type.Name.Contains("Shears") && type != typeof(Shears))
        {
            type = typeof(Shears);
        }

        ToolsModule.State.SelectableToolByType[type] =
            ToolsModule.State.SelectableToolByType.TryGetValue(type, out var selectable) &&
            selectable?.Tool == tool
                ? null
                : new SelectableTool(tool, toolIndex);

        if (playSound)
        {
            Game1.playSound("smallSelect");
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
