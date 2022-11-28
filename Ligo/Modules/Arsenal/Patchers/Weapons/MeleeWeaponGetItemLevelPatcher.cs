// ReSharper disable PossibleLossOfFraction
namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Reflection;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetItemLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetItemLevelPatcher"/> class.</summary>
    internal MeleeWeaponGetItemLevelPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getItemLevel));
    }

    #region harmony patches

    /// <summary>Adjust weapon level.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetItemLevelPrefix(MeleeWeapon __instance, ref int __result)
    {
        if (!ModEntry.Config.Arsenal.Weapons.RebalancedWeapons)
        {
            return true; // run original logic
        }

        try
        {
            var points = __instance.Read<int>(DataFields.InitialMaxDamage) * __instance.type.Value switch
            {
                MeleeWeapon.stabbingSword or MeleeWeapon.defenseSword => 0.5f,
                MeleeWeapon.dagger => 0.75f,
                MeleeWeapon.club => 0.3f,
                _ => 0f,
            };

            points += (__instance.knockback.Value - __instance.defaultKnockBackForThisType(__instance.type.Value)) *
                      10f;
            points += ((__instance.critChance.Value / __instance.DefaultCritChance()) - 1f) * 10f;
            points += ((__instance.critMultiplier.Value / __instance.DefaultCritPower()) - 1f) * 10f;
            points += __instance.addedPrecision.Value;
            points += __instance.addedDefense.Value;
            points += __instance.speed.Value;
            points += __instance.addedAreaOfEffect.Value / 4;

            __result = (int)Math.Floor(points / 10);
            if (__instance.IsUnique() || __instance.CanBeCrafted())
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
