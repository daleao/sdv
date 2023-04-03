namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

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
        this.Prefix!.before = new[] { OverhaulModule.Tools.Namespace };
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<Toolbar>(nameof(Toolbar.receiveLeftClick));
        if (!base.ApplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireMethod<Toolbar>(nameof(Toolbar.receiveRightClick));
        return base.ApplyImpl(harmony);
    }

    /// <inheritdoc />
    protected override bool UnapplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<Toolbar>(nameof(Toolbar.receiveLeftClick));
        if (!base.UnapplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireMethod<Toolbar>(nameof(Toolbar.receiveRightClick));
        return base.UnapplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Toggle selectable tool.</summary>
    [HarmonyPrefix]
    [HarmonyBefore("DaLion.Overhaul.Modules.Tools")]
    private static bool ToolbarReceiveClickPrefix(Item? ___hoverItem, bool playSound)
    {
        if (!SlingshotsModule.Config.EnableAutoSelection || !SlingshotsModule.Config.SelectionKey.IsDown())
        {
            return true; // run original logic
        }

        if (___hoverItem is not Slingshot slingshot)
        {
            return true;  // run original logic
        }

        if (SlingshotsModule.State.AutoSelectableSlingshot == slingshot)
        {
            SlingshotsModule.State.AutoSelectableSlingshot = null;
            if (playSound)
            {
                Game1.playSound("smallSelect");
            }

            return false; // don't run original logic
        }

        SlingshotsModule.State.AutoSelectableSlingshot = slingshot;
        if (playSound)
        {
            Game1.playSound("smallSelect");
        }

        return ToolsModule.ShouldEnable;
    }

    #endregion harmony patches
}
