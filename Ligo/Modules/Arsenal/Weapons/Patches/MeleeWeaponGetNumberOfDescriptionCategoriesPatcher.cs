namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using DaLion.Ligo.Modules.Arsenal.Weapons.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetNumberOfDescriptionCategoriesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetNumberOfDescriptionCategoriesPatcher"/> class.</summary>
    internal MeleeWeaponGetNumberOfDescriptionCategoriesPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getNumberOfDescriptionCategories));
    }

    #region harmony patches

    /// <summary>Correct number of description categories.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetNumberOfDescriptionCategoriesPrefix(MeleeWeapon __instance, ref int __result)
    {
        var number = 1;

        var crate = __instance.critChance.Value * (1f + __instance.Read<float>(DataFields.ResonantCritChance));
        var baseCrate = __instance.DefaultCritChance();
        if (crate > baseCrate)
        {
            ++number;
        }

        var cpow = __instance.critMultiplier.Value * (1f + __instance.Read<float>(DataFields.ResonantCritPower));
        var baseCpow = __instance.DefaultCritPower();
        if (cpow > baseCpow)
        {
            ++number;
        }

        var knockback = __instance.knockback.Value * (1f + __instance.Read<float>(DataFields.ResonantKnockback));
        var baseKnockback = __instance.defaultKnockBackForThisType(__instance.type.Value);
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (knockback != baseKnockback)
        {
            ++number;
        }

        var speed = __instance.speed.Value + __instance.Read<float>(DataFields.ResonantSpeed);
        if (speed != 0)
        {
            ++number;
        }

        if (__instance.hasEnchantmentOfType<GarnetEnchantment>())
        {
            ++number;
        }

        var defense = __instance.addedDefense.Value + __instance.Read<float>(DataFields.ResonantDefense);
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
