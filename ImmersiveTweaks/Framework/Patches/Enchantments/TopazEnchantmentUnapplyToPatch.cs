namespace DaLion.Stardew.Tweaks.Framework.Patches.Enchantments;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal class TopazEnchantmentUnapplyToPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal TopazEnchantmentUnapplyToPatch()
    {
        Original = RequireMethod<TopazEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchant</summary>
    [HarmonyPrefix]
    private static void TopazEnchantmentUnapplyToPostfix(TopazEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalanceForges) return;

        weapon.addedDefense.Value -= 4 * __instance.GetLevel();
    }

    #endregion harmony patches
}