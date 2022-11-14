namespace DaLion.Ligo.Modules.Arsenal.Common.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using HarmonyLib;
using Shared.Harmony;
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

    /// <summary>Add Dark Sword mod data.</summary>
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
                __instance.enchantments.Remove(__instance.GetEnchantmentOfType<CursedEnchantment>());
                __instance.enchantments.Add(new BlessedEnchantment());
                break;
            // galaxy -> infinity
            case Constants.InfinityBladeIndex:
            case Constants.InfinityDaggerIndex:
            case Constants.InfinityClubIndex:
                __instance.enchantments.Remove(__instance.GetEnchantmentOfType<GalaxySoulEnchantment>());
                __instance.enchantments.Add(new InfinityEnchantment());
                __instance.specialItem = true;
                break;
        }
    }

    #endregion harmony patches
}
