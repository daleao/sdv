namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class PowerfulEnchantmentApplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="PowerfulEnchantmentApplyToPatch"/> class.</summary>
    internal PowerfulEnchantmentApplyToPatch()
    {
        this.Target = this.RequireMethod<PowerfulEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalance powerful enchantment so it is not redundant.</summary>
    [HarmonyPrefix]
    private static bool PowerfulEnchantmentApplyToPrefix(Item item)
    {
        switch (item)
        {
            case Axe axe:
                axe.additionalPower.Value += 99;
                break;
            case Pickaxe pickaxe:
                pickaxe.additionalPower.Value += 99;
                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
