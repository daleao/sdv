namespace DaLion.Ligo.Modules.Arsenal.Common.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponCtorPatcher"/> class.</summary>
    internal MeleeWeaponCtorPatcher()
    {
        this.Target = this.RequireConstructor<MeleeWeapon>(typeof(int));
    }

    #region harmony patches

    /// <summary>Add intrinsic weapon enchantments.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCtorPostfix(MeleeWeapon __instance)
    {
        if (!ModEntry.Config.Arsenal.InfinityPlusOne || __instance.isScythe())
        {
            return;
        }

        // apply unique enchants
        switch (__instance.InitialParentTileIndex)
        {
            case Constants.DarkSwordIndex:
                __instance.enchantments.Add(new CursedEnchantment());
                __instance.specialItem = true;
                break;
            case Constants.HolyBladeIndex:
                __instance.enchantments.Add(new BlessedEnchantment());
                __instance.specialItem = true;
                break;
            case Constants.InfinityBladeIndex:
            case Constants.InfinityDaggerIndex:
            case Constants.InfinityClubIndex:
                __instance.enchantments.Add(new InfinityEnchantment());
                __instance.specialItem = true;
                break;
        }
    }

    #endregion harmony patches
}
