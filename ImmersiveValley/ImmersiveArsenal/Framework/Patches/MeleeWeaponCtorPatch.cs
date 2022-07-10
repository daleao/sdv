namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common.Data;
using Enchantments;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCtorPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponCtorPatch()
    {
        Target = RequireConstructor<MeleeWeapon>(typeof(int));
    }

    #region harmony patches

    /// <summary>Add Dark Sword mod data</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCtorPostfix(MeleeWeapon __instance)
    {
        if (!ModEntry.Config.InfinityPlusOneWeapons) return;

        switch (__instance.InitialParentTileIndex)
        {
            case Constants.DARK_SWORD_INDEX_I:
                __instance.enchantments.Add(new DemonicEnchantment());
                __instance.specialItem = true;
                ModDataIO.WriteTo(__instance, "EnemiesSlain", 0.ToString());
                break;
            case Constants.HOLY_BLADE_INDEX_I:
                __instance.enchantments.Add(new HolyEnchantment());
                __instance.specialItem = true;
                break;
            case Constants.INFINITY_BLADE_INDEX_I:
            case Constants.INFINITY_DAGGER_INDEX_I:
            case Constants.INFINITY_CLUB_INDEX_I:
                __instance.enchantments.Add(new InfinityEnchantment());
                __instance.specialItem = true;
                break;
        }
    }

    #endregion harmony patches
}