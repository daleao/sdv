namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal class TopazEnchantmentApplyToPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal TopazEnchantmentApplyToPatch()
    {
        Original = RequireMethod<TopazEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Patch to rebalance Topaz Enchant.</summary>
    [HarmonyPrefix]
    private static void TopazEnchantmentApplyToPostfix(TopazEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalanceForges) return;

        weapon.addedDefense.Value += 4 * __instance.GetLevel();
    }

    #endregion harmony patches
}