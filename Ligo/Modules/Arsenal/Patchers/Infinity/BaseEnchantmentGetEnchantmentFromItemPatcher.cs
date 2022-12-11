namespace DaLion.Ligo.Modules.Arsenal.Patchers.Infinity;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetEnchantmentFromItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentGetEnchantmentFromItemPatcher"/> class.</summary>
    internal BaseEnchantmentGetEnchantmentFromItemPatcher()
    {
        this.Target = this.RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetEnchantmentFromItem));
    }

    #region harmony patches

    /// <summary>Allow Hero Soul forge + allow Galaxy Soul forge into Galaxy Slingshot.</summary>
    [HarmonyPostfix]
    private static void BaseEnchantmentGetEnchantmentFromItemPostfix(ref BaseEnchantment? __result, Item? base_item, Item item)
    {
        if (__result is not null)
        {
            return;
        }

        if (item.ParentSheetIndex == Globals.HeroSoulIndex)
        {
            __result = new InfinityEnchantment();
        }
        else if (base_item is Slingshot { InitialParentTileIndex: Constants.GalaxySlingshotIndex } &&
               Utility.IsNormalObjectAtParentSheetIndex(item, 896))
        {
            __result = new GalaxySoulEnchantment();
        }
    }

    #endregion harmony patches
}
