namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentApplyToPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal TopazEnchantmentApplyToPatch()
    {
        Target = RequireMethod<TopazEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchant.</summary>
    [HarmonyPostfix]
    private static void TopazEnchantmentApplyToPostfix(TopazEnchantment __instance, Item item)
    {
        switch (item)
        {
            case MeleeWeapon weapon when ModEntry.Config.RebalancedEnchants:
                weapon.addedDefense.Value += 4 * __instance.GetLevel();
                break;
            case Slingshot:
                Game1.player.resilience += ModEntry.Config.RebalancedEnchants ? 5 : 1;
                break;
        }
    }

    #endregion harmony patches
}