namespace DaLion.Redux.Framework.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponLeftClickPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponLeftClickPatch"/> class.</summary>
    internal MeleeWeaponLeftClickPatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.leftClick));
    }

    #region harmony patches

    /// <summary>Eliminate dumb vanilla weapon spam.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponLeftClickPrefix(MeleeWeapon __instance, ref bool ___anotherClick, Farmer who)
    {
        return false;
    }

    #endregion harmony patches
}
