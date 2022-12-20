namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Enchantments;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class JadeEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="JadeEnchantmentApplyToPatcher"/> class.</summary>
    internal JadeEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<JadeEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Jade enchant.</summary>
    [HarmonyPrefix]
    private static bool JadeEnchantmentApplyToPrefix(JadeEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ArsenalModule.Config.RebalancedForges)
        {
            return true; // run original logic
        }

        weapon.critMultiplier.Value += 0.5f * __instance.GetLevel();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
