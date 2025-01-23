namespace DaLion.Arsenal.Framework.Patchers.Infinity;

using DaLion.Arsenal.Framework.Integrations;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuIsValidCraftIngredientPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuIsValidCraftIngredientPatcher"/> class.</summary>
    internal ForgeMenuIsValidCraftIngredientPatcher()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.IsValidCraftIngredient));
    }

    #region harmony patches

    /// <summary>Allow forging with Hero Soul.</summary>
    [HarmonyPostfix]
    private static void ForgeMenuIsValidCraftIngredientPostfix(ref bool __result, Item item)
    {
        if (JsonAssetsIntegration.HeroSoulIndex.HasValue &&
            item.ParentSheetIndex == JsonAssetsIntegration.HeroSoulIndex.Value)
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
