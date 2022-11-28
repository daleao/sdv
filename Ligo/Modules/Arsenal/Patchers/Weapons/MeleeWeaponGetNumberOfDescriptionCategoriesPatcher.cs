namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Reflection;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetNumberOfDescriptionCategoriesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetNumberOfDescriptionCategoriesPatcher"/> class.</summary>
    internal MeleeWeaponGetNumberOfDescriptionCategoriesPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getNumberOfDescriptionCategories));
    }

    #region harmony patches

    /// <summary>Correct number of description categories.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetNumberOfDescriptionCategoriesPrefix(MeleeWeapon __instance, ref int __result)
    {
        try
        {
            __result = __instance.Count_NonZeroStats();
            if (__instance.hasEnchantmentOfType<DiamondEnchantment>())
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
