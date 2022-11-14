namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Shared.Attributes;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Deprecated]
internal sealed class MeleeWeaponDefaultKnockbackForThisTypePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDefaultKnockbackForThisTypePatch"/> class.</summary>
    internal MeleeWeaponDefaultKnockbackForThisTypePatch()
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
