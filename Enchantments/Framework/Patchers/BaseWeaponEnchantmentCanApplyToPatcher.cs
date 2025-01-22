namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseWeaponEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseWeaponEnchantmentCanApplyToPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BaseWeaponEnchantmentCanApplyToPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<BaseWeaponEnchantment>(nameof(BaseWeaponEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Haymaker as a Scythe enchantment.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool BaseEnchantmentGetAvailableEnchantmentsPrefix(
        BaseWeaponEnchantment __instance, ref bool __result, Item item)
    {
        if (__instance is not HaymakerEnchantment)
        {
            return true; // run original logic
        }

        __result = item is MeleeWeapon weapon && weapon.isScythe() && weapon.QualifiedItemId == QIDs.IridiumScythe;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
