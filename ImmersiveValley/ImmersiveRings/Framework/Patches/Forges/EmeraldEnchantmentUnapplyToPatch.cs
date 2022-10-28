namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class EmeraldEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="EmeraldEnchantmentUnapplyToPatch"/> class.</summary>
    internal EmeraldEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<EmeraldEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Emerald enchant.</summary>
    [HarmonyPostfix]
    private static void EmeraldEnchantmentUnapplyToPostfix(EmeraldEnchantment __instance, Item item)
    {

    }

    #endregion harmony patches
}
