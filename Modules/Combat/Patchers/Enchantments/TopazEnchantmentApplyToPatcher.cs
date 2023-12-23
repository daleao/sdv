namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TopazEnchantmentApplyToPatcher"/> class.</summary>
    internal TopazEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<TopazEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchantment for Melee Weapons.</summary>
    [HarmonyPrefix]
    private static bool TopazEnchantmentApplyToPrefix(TopazEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !CombatModule.Config.RingsEnchantments.RebalancedGemstones)
        {
            return true; // run original logic
        }

        weapon.addedDefense.Value += __instance.GetLevel();
        return false; // don't run original logic
    }

    /// <summary>Reset cached stats.</summary>
    [HarmonyPostfix]
    private static void TopazEnchantmentApplyPostfix(Item item)
    {
        switch (item)
        {
            case MeleeWeapon weapon:
                weapon.Invalidate();
                break;
            case Slingshot slingshot:
                slingshot.Invalidate();
                break;
        }
    }

    #endregion harmony patches
}
