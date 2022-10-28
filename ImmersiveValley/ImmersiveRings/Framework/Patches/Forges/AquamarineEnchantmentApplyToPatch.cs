namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class AquamarineEnchantmentApplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="AquamarineEnchantmentApplyToPatch"/> class.</summary>
    internal AquamarineEnchantmentApplyToPatch()
    {
        this.Target = this.RequireMethod<AquamarineEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Aquamarine enchant.</summary>
    [HarmonyPostfix]
    private static void AquamarineEnchantmentApplyToPostfix(AquamarineEnchantment __instance, Item item)
    {
        
    }

    #endregion harmony patches
}
