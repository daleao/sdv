namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentApplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="TopazEnchantmentApplyToPatch"/> class.</summary>
    internal TopazEnchantmentApplyToPatch()
    {
        this.Target = this.RequireMethod<TopazEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchant.</summary>
    [HarmonyPrefix]
    private static bool TopazEnchantmentApplyToPrefix(TopazEnchantment __instance, Item item)
    {
        switch (item)
        {
            case MeleeWeapon weapon when ModEntry.Config.RebalancedForges:
                weapon.addedDefense.Value += (ModEntry.Config.RebalancedForges ? 5 : 1) * __instance.GetLevel();
                break;
            case Slingshot:
                Game1.player.resilience += (ModEntry.Config.RebalancedForges ? 5 : 1) * __instance.GetLevel();
                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
