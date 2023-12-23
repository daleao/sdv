namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

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
        this.Prefix!.before = new[] { OverhaulModule.Tools.Namespace };
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<InventoryPage>(nameof(InventoryPage.receiveLeftClick));
        if (!base.ApplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireMethod<InventoryPage>(nameof(InventoryPage.receiveRightClick));
        return base.ApplyImpl(harmony);
    }

    /// <inheritdoc />
    protected override bool UnapplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<InventoryPage>(nameof(InventoryPage.receiveLeftClick));
        if (!base.UnapplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireMethod<InventoryPage>(nameof(InventoryPage.receiveRightClick));
        return base.UnapplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Toggle selectable tool.</summary>
    [HarmonyPrefix]
    [HarmonyBefore("DaLion.Overhaul.Modules.Tools")]
    private static bool InventoryPageReceiveClickPrefix(Item? ___hoveredItem, bool playSound)
    {
        if (!CombatModule.Config.ControlsUi.EnableAutoSelection || !CombatModule.Config.ControlsUi.SelectionKey.IsDown())
        {
            return true; // run original logic
        }

        switch (___hoveredItem)
        {
            case MeleeWeapon weapon when !weapon.isScythe():
                if (CombatModule.State.AutoSelectableMelee == weapon)
                {
                    CombatModule.State.AutoSelectableMelee = null;
                    if (playSound)
                    {
                        Game1.playSound("smallSelect");
                    }

                    return false; // don't run original logic
                }

                CombatModule.State.AutoSelectableMelee = weapon;
                if (playSound)
                {
                    Game1.playSound("smallSelect");
                }

                return false; // don't run original logic
            case Slingshot slingshot:
                if (CombatModule.State.AutoSelectableRanged == slingshot)
                {
                    CombatModule.State.AutoSelectableRanged = null;
                    if (playSound)
                    {
                        Game1.playSound("smallSelect");
                    }

                    return false; // don't run original logic
                }

                CombatModule.State.AutoSelectableRanged = slingshot;
                if (playSound)
                {
                    Game1.playSound("smallSelect");
                }

                return false; // don't run original logic
            default:
                return true; // run original logic
        }
    }

    #endregion harmony patches
}
