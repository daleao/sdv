namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

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
    [HarmonyPrefix]
    private static bool EmeraldEnchantmentUnapplyToPrefix(EmeraldEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ModEntry.Config.Arsenal.RebalancedForges)
        {
            return true; // run original logic
        }

        weapon.addedDefense.Value -= __instance.GetLevel();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
