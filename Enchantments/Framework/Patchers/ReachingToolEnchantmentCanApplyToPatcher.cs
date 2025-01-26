namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ReachingToolEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ReachingToolEnchantmentCanApplyToPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ReachingToolEnchantmentCanApplyToPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<ReachingToolEnchantment>(nameof(ReachingToolEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Reaching enchantment to Scythe.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReachingToolEnchantmentCanApplyToPostfix(FishingRodEnchantment __instance, ref bool __result, Item item)
    {
        __result = __result || (item is MeleeWeapon weapon && weapon.isScythe() && weapon.QualifiedItemId == QIDs.IridiumScythe);
    }

    #endregion harmony patches
}
