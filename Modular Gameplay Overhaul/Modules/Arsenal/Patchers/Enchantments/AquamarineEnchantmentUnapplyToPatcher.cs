namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AquamarineEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AquamarineEnchantmentUnapplyToPatcher"/> class.</summary>
    internal AquamarineEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<AquamarineEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    ///// <summary>Rebalances Aquamarine enchant.</summary>
    //[HarmonyPrefix]
    //private static bool JadeEnchantmentUnapplyToPrefix(JadeEnchantment __instance, Item item)
    //{
    //    if (item is not MeleeWeapon weapon || !ArsenalModule.Config.RebalancedForges)
    //    {
    //        return true; // run original logic
    //    }

    //    weapon.critChance.Value -= 0.046f * __instance.GetLevel();
    //    return false; // don't run original logic
    //}

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
