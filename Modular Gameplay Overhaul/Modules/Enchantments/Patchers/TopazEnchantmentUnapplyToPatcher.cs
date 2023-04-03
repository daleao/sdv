namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TopazEnchantmentUnapplyToPatcher"/> class.</summary>
    internal TopazEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<TopazEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchantment for Melee Weapons.</summary>
    [HarmonyPrefix]
    private static bool TopazEnchantmentUnapplyToPrefix(TopazEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !EnchantmentsModule.Config.RebalancedForges)
        {
            return true; // run original logic
        }

        weapon.addedDefense.Value -= __instance.GetLevel();
        return false; // don't run original logic
    }

    /// <summary>Reset cached stats.</summary>
    [HarmonyPostfix]
    private static void TopazEnchantmentUnapplyPostfix(Item item)
    {
        switch (item)
        {
            case MeleeWeapon weapon when WeaponsModule.ShouldEnable:
                weapon.Invalidate();
                break;
            case Slingshot slingshot when SlingshotsModule.ShouldEnable:
                slingshot.Invalidate();
                break;
        }
    }

    #endregion harmony patches
}
