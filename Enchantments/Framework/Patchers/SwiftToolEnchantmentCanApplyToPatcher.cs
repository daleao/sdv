namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SwiftToolEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SwiftToolEnchantmentCanApplyToPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal SwiftToolEnchantmentCanApplyToPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SwiftToolEnchantment>(nameof(SwiftToolEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Swift enchantment to Watering Can.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ReachingToolEnchantmentCanApplyToPostfix(FishingRodEnchantment __instance, ref bool __result, Item item)
    {
        __result = __result || item is WateringCan;
    }

    #endregion harmony patches
}
