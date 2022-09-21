namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Arsenal.Framework.Enchantments;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponTransformPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponTransformPatch"/> class.</summary>
    internal MeleeWeaponTransformPatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.transform));
    }

    #region harmony patches

    /// <summary>Add Dark Sword mod data.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponTransformPostfix(MeleeWeapon __instance, int newIndex)
    {
        if (!ModEntry.Config.InfinityPlusOneWeapons)
        {
            return;
        }

        switch (newIndex)
        {
            // dark sword -> holy blade
            case Constants.HolyBladeIndex:
                __instance.Write("EnemiesSlain", null);
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
