namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentApplyToPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BaseEnchantmentApplyToPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<BaseEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Apply Haymaker effect to Scythe.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void BaseEnchantmentApplyToPostfix(BaseEnchantment __instance, Item item)
    {
        if (__instance is ReachingToolEnchantment && item is MeleeWeapon weapon)
        {
            weapon.addedAreaOfEffect.Value++;
        }
    }

    #endregion harmony patches
}
