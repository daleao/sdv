namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Enchantments;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetEnchantmentFromItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentGetEnchantmentFromItemPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BaseEnchantmentGetEnchantmentFromItemPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetEnchantmentFromItem));
    }

    #region harmony patches

    /// <summary>Resonate with chords.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void BaseEnchantmentGetEnchantmentFromItemPostfix(ref BaseEnchantment __result, Item? base_item, Item? item)
    {
        if ((base_item is null || (base_item is MeleeWeapon weapon && !weapon.isScythe()))
            && item?.ItemId == GarnetStoneId)
        {
            __result = new GarnetEnchantment();
        }
    }

    #endregion harmony patches
}
