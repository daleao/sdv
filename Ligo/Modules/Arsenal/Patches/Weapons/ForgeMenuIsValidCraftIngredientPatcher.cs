namespace DaLion.Ligo.Modules.Arsenal.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Extensions;
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
        if (item.IsHeroSoul())
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
