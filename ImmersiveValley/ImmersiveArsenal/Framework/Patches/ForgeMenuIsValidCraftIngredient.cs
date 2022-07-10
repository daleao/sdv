namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuIsValidCraftIngredient : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ForgeMenuIsValidCraftIngredient()
    {
        Target = RequireMethod<ForgeMenu>(nameof(ForgeMenu.IsValidCraftIngredient));
    }

    #region harmony patches

    /// <summary>Allow forging with Hero Soul.</summary>
    [HarmonyPostfix]
    private static void ForgeMenuIsValidCraftIngredientPostfix(ref bool __result, Item item)
    {
        if (item.IsHeroSoul()) __result = true;
    }

    #endregion harmony patches
}