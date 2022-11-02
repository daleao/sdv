namespace DaLion.Redux.Framework.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class EmeraldEnchantmentApplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="EmeraldEnchantmentApplyToPatch"/> class.</summary>
    internal EmeraldEnchantmentApplyToPatch()
    {
        this.Target = this.RequireMethod<EmeraldEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Emerald enchant.</summary>
    [HarmonyPrefix]
    private static bool EmeraldEnchantmentApplyToPrefix(EmeraldEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ModEntry.Config.Arsenal.RebalancedForges)
        {
            return true; // run original logic
        }

        weapon.speed.Value += __instance.GetLevel();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
