namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Weapons.Extensions;
using HarmonyLib;
using StardewValley.Menus;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuIsValidCraftIngredientPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuIsValidCraftIngredientPatch"/> class.</summary>
    internal ForgeMenuIsValidCraftIngredientPatch()
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
