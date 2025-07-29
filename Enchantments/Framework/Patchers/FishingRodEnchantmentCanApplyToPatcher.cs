namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodEnchantmentCanApplyToPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishingRodEnchantmentCanApplyToPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FishingRodEnchantment>(nameof(FishingRodEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Master enchantment to other tools.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FishingRodEnchantmentCanApplyTo(FishingRodEnchantment __instance, ref bool __result, Item item)
    {
        if (!__result && __instance is MasterEnchantment)
        {
            __result = item is Axe or Hoe or Pickaxe or WateringCan;
        }
    }

    #endregion harmony patches
}
