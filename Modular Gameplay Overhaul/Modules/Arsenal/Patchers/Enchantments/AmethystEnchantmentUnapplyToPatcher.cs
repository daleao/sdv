namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AmethystEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AmethystEnchantmentUnapplyToPatcher"/> class.</summary>
    internal AmethystEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<AmethystEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Amethyst enchant.</summary>
    [HarmonyPrefix]
    private static bool AmethystEnchantmentUnapplyToPrefix(AmethystEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ArsenalModule.Config.RebalancedForges || !ArsenalModule.Config.KnockbackDamage)
        {
            return true; // run original logic
        }

        weapon.knockback.Value -= __instance.GetLevel() * 0.1f;
        return false; // don't run original logic
    }

    /// <summary>Reset cached stats.</summary>
    [HarmonyPostfix]
    private static void AquamarineEnchantmentUnapplyPostfix(Item item)
    {
        if (item is Tool tool and (MeleeWeapon or Slingshot))
        {
            tool.Invalidate();
        }
    }

    #endregion harmony patches
}
