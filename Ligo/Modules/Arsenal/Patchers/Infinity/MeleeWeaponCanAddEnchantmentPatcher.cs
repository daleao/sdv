namespace DaLion.Ligo.Modules.Arsenal.Patchers.Infinity;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCanAddEnchantmentPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponCanAddEnchantmentPatcher"/> class.</summary>
    internal MeleeWeaponCanAddEnchantmentPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.CanAddEnchantment));
    }

    #region harmony patches

    /// <summary>Allow forge Galaxy with Infinity.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponCanAddEnchantmentPrefix(
        MeleeWeapon __instance, ref bool __result, BaseEnchantment enchantment)
    {
        if (enchantment is not InfinityEnchantment)
        {
            return true; // run original logic
        }

        __result = __instance.isGalaxyWeapon() && __instance.hasEnchantmentOfType<GalaxySoulEnchantment>() &&
                   __instance.GetEnchantmentLevel<GalaxySoulEnchantment>() >= 3;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
