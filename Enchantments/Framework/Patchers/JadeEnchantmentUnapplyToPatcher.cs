namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class JadeEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="JadeEnchantmentUnapplyToPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal JadeEnchantmentUnapplyToPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<JadeEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalance Jade enchant.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool JadeEnchantmentUnapplyToPrefix(JadeEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon)
        {
            return true; // run original logic
        }

        weapon.critMultiplier.Value -= 0.5f * __instance.GetLevel();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
