namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class PowerfulEnchantmentApplyToPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal PowerfulEnchantmentApplyToPatch()
    {
        Target = RequireMethod<PowerfulEnchantment>("_ApplyTo");
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