// ReSharper disable PossibleLossOfFraction
namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Shared.Extensions.Stardew;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponSalePricePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponSalePricePatcher"/> class.</summary>
    internal MeleeWeaponSalePricePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.salePrice));
    }

    #region harmony patches

    /// <summary>Adjust weapon level.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetItemLevelPrefix(MeleeWeapon __instance, ref int __result)
    {
        if (!ArsenalModule.Config.Weapons.EnableRebalance)
        {
            return true; // run original logic
        }

        try
        {
            var tier = WeaponTier.GetFor(__instance);
            if (tier == WeaponTier.Untiered)
            {
                return true; // run original logic
            }

            if (tier == WeaponTier.Masterwork)
            {
                __result = __instance.Name.StartsWith("Dragon")
                    ? (int)(tier.Price * 1.5)
                    : __instance.Name.StartsWith("Elvish")
                        ? (int)(tier.Price * 0.75)
                        : tier.Price;
                __result *= 2;
                return false; // don't run original logic
            }

            __result = tier.Price * 2; // x2 because this number will be halved later

            // bonus points if has intrinsic enchantment
            if (__instance.enchantments.FirstOrDefault(e => !e.IsForge() && e.IsSecondaryEnchantment()) is not null)
            {
                __result += 2000;
            }

            // bonus points if has non-forge enchantment
            if (__instance.enchantments.FirstOrDefault(e => !e.IsForge() && !e.IsSecondaryEnchantment()) is not null)
            {
                __result += 2000;
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
