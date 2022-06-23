namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentUnpplyToPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal TopazEnchantmentUnpplyToPatch()
    {
        Target = RequireMethod<TopazEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchant.</summary>
    [HarmonyPostfix]
    private static void TopazEnchantmentUnpplyToPostfix(TopazEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalancedEnchants) return;

        weapon.addedDefense.Value -= 4 * __instance.GetLevel();
    }

    #endregion harmony patches
}