namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="TopazEnchantmentUnapplyToPatch"/> class.</summary>
    internal TopazEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<TopazEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchant.</summary>
    [HarmonyPostfix]
    private static void TopazEnchantmentUnapplyToPostfix(TopazEnchantment __instance, Item item)
    {
        
    }

    #endregion harmony patches
}
