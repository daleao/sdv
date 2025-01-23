namespace DaLion.Arsenal.Framework.Patchers;

#region using directives

using System.Reflection;
using DaLion.Arsenal.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetItemLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetItemLevelPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MeleeWeaponGetItemLevelPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getItemLevel));
    }

    #region harmony patches

    /// <summary>Adjust weapon level.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetItemLevelPrefix(MeleeWeapon __instance, ref int __result)
    {
        try
        {
            var data = Game1.weaponData[__instance.QualifiedItemId];
            var minDamage = data.MinDamage;
            var maxDamage = data.MaxDamage;
            var points = Data.Read(__instance, DataKeys.BaseMaxDamage, maxDamage) * __instance.type.Value switch
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
            points += weapon.addedPrecision.Value;
            points += weapon.addedDefense.Value;
            points += weapon.speed.Value;
            points += weapon.addedAreaOfEffect.Value / 4f;

            holder.Level = (int)Math.Floor(points / 10f);
            if (weapon.isGalaxyWeapon() || weapon.IsInfinityWeapon() || weapon.IsCursedOrBlessed() || weapon.IsLegacyWeapon())
            {
                holder.Level++;
            }

            holder.Level = Math.Clamp(holder.Level, 1, 10);
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
