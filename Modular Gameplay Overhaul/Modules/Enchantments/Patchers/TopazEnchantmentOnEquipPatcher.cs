namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentOnEquipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TopazEnchantmentOnEquipPatcher"/> class.</summary>
    internal TopazEnchantmentOnEquipPatcher()
    {
        this.Target = this.RequireMethod<TopazEnchantment>("_OnEquip");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchantment for Slingshots.</summary>
    [HarmonyPrefix]
    private static bool TopazEnchantmentOnEquipPrefix(TopazEnchantment __instance, Item item)
    {
        if (item is not Slingshot || !EnchantmentsModule.Config.RebalancedForges || CombatModule.IsEnabled)
        {
            return true; // run original logic
        }

        Game1.player.resilience += __instance.GetLevel();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
