namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoAnimateSpecialMovePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoAnimateSpecialMovePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponDoAnimateSpecialMovePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
    }

    #region harmony patches

    /// <summary>Disposes certain enchantments.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
    {
        if (__instance.GetEnchantmentOfType<ExplosiveEnchantment>() is { } explosive && explosive.ExplosionRadius >= 1)
        {
            explosive.Explode(__instance);
        }
    }

    #endregion harmony patches
}
