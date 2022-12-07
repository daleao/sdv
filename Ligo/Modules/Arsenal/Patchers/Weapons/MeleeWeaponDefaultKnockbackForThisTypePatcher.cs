namespace DaLion.Ligo.Modules.Arsenal.Patchers.Weapons;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDefaultKnockbackForThisTypePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDefaultKnockbackForThisTypePatcher"/> class.</summary>
    internal MeleeWeaponDefaultKnockbackForThisTypePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.defaultKnockBackForThisType));
    }

    #region harmony patches

    /// <summary>Rebalance knockback for all weapons.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponDefaultKnockbackForThisTypePrefix(MeleeWeapon __instance, ref float __result, int type)
    {
        if (!ModEntry.Config.Arsenal.Weapons.RebalancedWeapons)
        {
            return true; // run original logic
        }

        if (__instance.Name == "Diamond Wand")
        {
            __result = 31f;
            return false; // don't run original logic
        }


        __result = type switch
        {
            MeleeWeapon.dagger => 0.5f,
            MeleeWeapon.stabbingSword or MeleeWeapon.defenseSword=> 0.75f,
            MeleeWeapon.club => 1f,
            _ => -1f,
        };

        return false; // don't run original logic
    }

    #endregion harmony patches
}
