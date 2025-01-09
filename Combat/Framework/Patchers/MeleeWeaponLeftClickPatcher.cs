namespace DaLion.Combat.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponLeftClickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponLeftClickPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MeleeWeaponLeftClickPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.leftClick));
    }

    #region harmony patches

    /// <summary>Eliminate dumb vanilla weapon spam.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool MeleeWeaponLeftClickPrefix(MeleeWeapon __instance)
    {
        return !Config.EnableComboHits || __instance.IsDagger();
    }

    #endregion harmony patches
}
