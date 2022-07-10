namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Enchantments;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCanAddEnchantmentPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponCanAddEnchantmentPatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.CanAddEnchantment));
    }

    #region harmony patches

    /// <summary>Allow forge galaxy with infinity.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponCanAddEnchantmentPrefix(MeleeWeapon __instance, ref bool __result, BaseEnchantment enchantment)
    {
        if (enchantment is not InfinityEnchantment || !__instance.isGalaxyWeapon() ||
            __instance.GetEnchantmentLevel<GalaxySoulEnchantment>() < 3) return true; // run original logic

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}