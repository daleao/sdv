namespace DaLion.Ligo.Modules.Arsenal.Patches;

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
