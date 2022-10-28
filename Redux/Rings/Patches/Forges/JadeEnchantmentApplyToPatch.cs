namespace DaLion.Redux.Rings.Patches;

#region using directives

using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class JadeEnchantmentApplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="JadeEnchantmentApplyToPatch"/> class.</summary>
    internal JadeEnchantmentApplyToPatch()
    {
        this.Target = this.RequireMethod<JadeEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Jade enchant.</summary>
    [HarmonyPostfix]
    private static void JadeEnchantmentApplyToPostfix(JadeEnchantment __instance, Item item)
    {
    }

    #endregion harmony patches
}
