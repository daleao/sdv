namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal class JadeEnchantmentUnapplyToPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal JadeEnchantmentUnapplyToPatch()
    {
        Original = RequireMethod<JadeEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Patch to rebalance Jade Enchant.</summary>
    [HarmonyPrefix]
    private static void JadeEnchantmentUnapplyToPostfix(JadeEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalanceForges) return;

        weapon.critMultiplier.Value -= 0.4f * __instance.GetLevel();
    }

    #endregion harmony patches
}