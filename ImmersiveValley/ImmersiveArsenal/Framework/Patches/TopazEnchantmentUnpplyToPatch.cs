namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentUnpplyToPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal TopazEnchantmentUnpplyToPatch()
    {
        Target = RequireMethod<TopazEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchant.</summary>
    [HarmonyPrefix]
    private static bool TopazEnchantmentUnpplyToPrefix(TopazEnchantment __instance, Item item)
    {
        if (ModEntry.Config.TopazPerk != ModConfig.Perk.Defense) return false; // don't run original logic

        switch (item)
        {
            case MeleeWeapon weapon when ModEntry.Config.RebalancedEnchants:
                weapon.addedDefense.Value -= (ModEntry.Config.RebalancedEnchants ? 5 : 1) * __instance.GetLevel();
                break;
            case Slingshot:
                Game1.player.resilience -= (ModEntry.Config.RebalancedEnchants ? 5 : 1) * __instance.GetLevel();
                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}