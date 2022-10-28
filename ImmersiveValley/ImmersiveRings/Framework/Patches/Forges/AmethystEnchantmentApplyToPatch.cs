namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Common.Integrations.ImmersiveRings;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class AmethystEnchantmentApplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="AmethystEnchantmentApplyToPatch"/> class.</summary>
    internal AmethystEnchantmentApplyToPatch()
    {
        this.Target = this.RequireMethod<AmethystEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Amethyst enchant.</summary>
    [HarmonyPostfix]
    private static void AmethystEnchantmentApplyToPostfix(AmethystEnchantment __instance, Item item)
    {
        
    }

    #endregion harmony patches
}
