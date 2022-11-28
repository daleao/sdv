namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponTransformPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponTransformPatcher"/> class.</summary>
    internal MeleeWeaponTransformPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.transform));
    }

    #region harmony patches

    /// <summary>Convert cursed -> blessed enchantment + galaxysoul -> infinity enchatnment.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponTransformPostfix(MeleeWeapon __instance, int newIndex)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            return;
        }

        switch (newIndex)
        {
            // dark sword -> holy blade
            case Constants.HolyBladeIndex:
                __instance.RemoveEnchantment(__instance.GetEnchantmentOfType<CursedEnchantment>());
                __instance.AddEnchantment(new BlessedEnchantment());
                break;
            // galaxy -> infinity
            case Constants.InfinityBladeIndex:
            case Constants.InfinityDaggerIndex:
            case Constants.InfinityGavelIndex:
                __instance.RemoveEnchantment(__instance.GetEnchantmentOfType<GalaxySoulEnchantment>());
                __instance.AddEnchantment(new InfinityEnchantment());
                __instance.specialItem = true;
                break;
        }
    }

    #endregion harmony patches
}
