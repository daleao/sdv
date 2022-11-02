namespace DaLion.Redux.Framework.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Redux.Framework.Arsenal.Weapons.Enchantments;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCanAddEnchantmentPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponCanAddEnchantmentPatch"/> class.</summary>
    internal MeleeWeaponCanAddEnchantmentPatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.CanAddEnchantment));
    }

    #region harmony patches

    /// <summary>Allow forge galaxy with infinity.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponCanAddEnchantmentPrefix(
        MeleeWeapon __instance, ref bool __result, BaseEnchantment enchantment)
    {
        if (enchantment is not InfinityEnchantment || !__instance.isGalaxyWeapon() ||
            __instance.GetEnchantmentLevel<GalaxySoulEnchantment>() < 3)
        {
            return true; // run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
