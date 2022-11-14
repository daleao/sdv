namespace DaLion.Ligo.Modules.Tools.Patches;

#region using directives

using HarmonyLib;
using Shared.Harmony;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class PowerfulEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PowerfulEnchantmentApplyToPatcher"/> class.</summary>
    internal PowerfulEnchantmentApplyToPatcher()
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
