namespace DaLion.Redux.Framework.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Redux.Framework.Arsenal.Weapons.Enchantments;
using DaLion.Redux.Framework.Arsenal.Weapons.Extensions;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetNumberOfDescriptionCategoriesPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetNumberOfDescriptionCategoriesPatch"/> class.</summary>
    internal MeleeWeaponGetNumberOfDescriptionCategoriesPatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getNumberOfDescriptionCategories));
    }

    #region harmony patches

    /// <summary>Correct number of description categories.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetNumberOfDescriptionCategoriesPrefix(MeleeWeapon __instance, ref int __result)
    {
        var number = 1;

        var crate = __instance.critChance.Value;
        var baseCrate = __instance.DefaultCritChance();
        if (crate > baseCrate)
        {
            ++number;
        }

        var cpow = __instance.critMultiplier.Value;
        var baseCpow = __instance.DefaultCritPower();
        if (cpow > baseCpow)
        {
            ++number;
        }

        var knockback = __instance.knockback.Value;
        var baseKnockback = __instance.defaultKnockBackForThisType(__instance.type.Value);
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (knockback != baseKnockback)
        {
            ++number;
        }

        if (__instance.speed.Value != 0)
        {
            ++number;
        }

        if (__instance.hasEnchantmentOfType<GarnetEnchantment>())
        {
            ++number;
        }

        if (__instance.addedDefense.Value != 0)
        {
            ++number;
        }

        if (__instance.hasEnchantmentOfType<DiamondEnchantment>())
        {
            ++number;
        }

        __result = number;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
