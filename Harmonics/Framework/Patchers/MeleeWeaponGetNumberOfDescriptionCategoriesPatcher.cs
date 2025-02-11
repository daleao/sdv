namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetNumberOfDescriptionCategoriesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetNumberOfDescriptionCategoriesPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponGetNumberOfDescriptionCategoriesPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getNumberOfDescriptionCategories));
    }

    #region harmony patches

    /// <summary>Display cooldown effects in tooltip.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void MeleeWeaponDrawTooltipPostfix(MeleeWeapon __instance, ref int __result)
    {
        if (__instance.hasEnchantmentOfType<GarnetEnchantment>() && __instance.Get_CooldownReduction().Value > 0f)
        {
            __result++;
        }
    }

    #endregion harmony patches
}
