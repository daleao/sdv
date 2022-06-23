namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class JadeEnchantmentUnpplyToPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal JadeEnchantmentUnpplyToPatch()
    {
        Target = RequireMethod<JadeEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Jade enchant.</summary>
    [HarmonyPostfix]
    private static void JadeEnchantmentUnpplyToPostfix(JadeEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalancedEnchants) return;

        weapon.critMultiplier.Value -= 0.4f * __instance.GetLevel();
    }

    #endregion harmony patches
}