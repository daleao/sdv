namespace DaLion.Stardew.Tweaks.Framework.Patches.Enchantments;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal class JadeEnchantmentApplyToPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal JadeEnchantmentApplyToPatch()
    {
        Original = RequireMethod<JadeEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Jade enchant.</summary>
    [HarmonyPrefix]
    private static void JadeEnchantmentApplyToPostfix(JadeEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalanceForges) return;

        weapon.critMultiplier.Value += 0.4f * __instance.GetLevel();
    }

    #endregion harmony patches
}