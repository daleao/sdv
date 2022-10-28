namespace DaLion.Redux.Rings.Patches;

#region using directives

using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RubyEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RubyEnchantmentUnapplyToPatch"/> class.</summary>
    internal RubyEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<RubyEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Ruby enchant.</summary>
    [HarmonyPostfix]
    private static void RubyEnchantmentUnapplyToPostfix(RubyEnchantment __instance, Item item)
    {
    }

    #endregion harmony patches
}
