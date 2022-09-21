namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodEnchantmentCanApplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodEnchantmentCanApplyToPatch"/> class.</summary>
    internal FishingRodEnchantmentCanApplyToPatch()
    {
        this.Target = this.RequireMethod<FishingRodEnchantment>(nameof(FishingRodEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Master enchantment to other tools.</summary>
    [HarmonyPostfix]
    private static void FishingRodEnchantmentCanApplyTo(FishingRodEnchantment __instance, ref bool __result, Item item)
    {
        if (__instance is MasterEnchantment && item is Axe or Hoe or Pickaxe or WateringCan)
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
