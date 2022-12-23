namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponRecalculateAppliedForgesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponRecalculateAppliedForgesPatcher"/> class.</summary>
    internal MeleeWeaponRecalculateAppliedForgesPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.RecalculateAppliedForges));
    }

    #region harmony patches

    /// <summary>Apply custom stats if necessary.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponRecalculateAppliedForgedPrefix(MeleeWeapon __instance, bool force)
    {
        if (!ArsenalModule.Config.Weapons.EnableRebalance)
        {
            return true; // run original logic
        }

        if (__instance.enchantments.Count == 0 && !force)
        {
            return false; // don't run original logic
        }

        try
        {
            // this should not longer be necessary given that all stats are refreshed and forges don't have unapply effects
            // and even if they did, they shouldn't need to be unapplied anyway
            foreach (var enchantment in __instance.enchantments.Where(e => e.IsForge()))
            {
                enchantment.UnapplyTo(__instance);
            }

            __instance.RefreshStats();

            foreach (var enchantment in __instance.enchantments.Where(e => e.IsForge()))
            {
                enchantment.ApplyTo(__instance);
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
