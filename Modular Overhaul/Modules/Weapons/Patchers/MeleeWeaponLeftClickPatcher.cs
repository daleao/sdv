namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponLeftClickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponLeftClickPatcher"/> class.</summary>
    internal MeleeWeaponLeftClickPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.leftClick));
    }

    #region harmony patches

    /// <summary>Eliminate dumb vanilla weapon spam.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponLeftClickPrefix(MeleeWeapon __instance)
    {
        return __instance.type.Value == MeleeWeapon.dagger || !WeaponsModule.Config.EnableComboHits;
    }

    #endregion harmony patches
}
