namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetMaxForgesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetMaxForgesPatcher"/> class.</summary>
    internal MeleeWeaponGetMaxForgesPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.GetMaxForges));
    }

    #region harmony patches

    /// <summary>Custom forge slots for weapons + extra slot for Infinity enchant.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetMaxForgesPrefix(MeleeWeapon __instance, ref int __result)
    {
        if (!ArsenalModule.Config.Weapons.EnableRebalance)
        {
            return true; // run original logic
        }

        try
        {
            __result = __instance.getItemLevel() switch
            {
                >= 6 => 3,
                >= 4 => 2,
                >= 2 => 1,
                _ => 0,
            };

            if (__instance.hasEnchantmentOfType<InfinityEnchantment>())
            {
                __result++;
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
