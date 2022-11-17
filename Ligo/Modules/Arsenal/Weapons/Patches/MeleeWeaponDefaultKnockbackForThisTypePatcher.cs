namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

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
    private static bool MeleeWeaponDefaultKnockbackForThisTypePrefix(ref float __result, int type)
    {
        if (!ModEntry.Config.Arsenal.OverhauledKnockback)
        {
            return true; // run original logic
        }

        switch (type)
        {
            case MeleeWeapon.dagger:
                __result = 0.5f;
                break;
            case MeleeWeapon.stabbingSword:
            case MeleeWeapon.defenseSword:
                __result = 0.75f;
                break;
            case MeleeWeapon.club:
                __result = 1f;
                break;
            default:
                __result = -1f;
                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
