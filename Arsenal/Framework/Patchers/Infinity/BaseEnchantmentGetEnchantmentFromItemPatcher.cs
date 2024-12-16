namespace DaLion.Arsenal.Framework.Patchers.Infinity;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Harmony;
using HarmonyLib;

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

    /// <summary>Allow Hero Soul forge.</summary>
    [HarmonyPostfix]
    private static void BaseEnchantmentGetEnchantmentFromItemPostfix(ref BaseEnchantment? __result, Item item)
    {
        if (__result is null && JsonAssetsIntegration.HeroSoulIndex.HasValue && item.ParentSheetIndex == JsonAssetsIntegration.HeroSoulIndex.Value)
        {
            __result = new InfinityEnchantment();
        }
    }

    #endregion harmony patches
}
