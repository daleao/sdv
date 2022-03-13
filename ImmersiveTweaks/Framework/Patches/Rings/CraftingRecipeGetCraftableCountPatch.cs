namespace DaLion.Stardew.Tweaks.Framework.Patches.Rings;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class CraftingRecipeGetCraftableCountPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CraftingRecipeGetCraftableCountPatch()
    {
        Original = RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.getCraftableCount), new[] {typeof(IList<Item>)});
    }

    #region harmony patches

    /// <summary>Overrides craftable count for non-SObject types.</summary>
    [HarmonyPrefix]
    private static bool CraftingRecipeGetCraftableCountPrefix(CraftingRecipe __instance, ref int __result, IList<Item> additional_materials)
    {
        try
        {
            var craftableOverall = -1;
            foreach (var (index, required) in __instance.recipeList)
            {
                var found = index.IsRingIndex() ? Game1.player.GetRingItemCount(index) : Game1.player.getItemCount(index);
                if (additional_materials is not null)
                    found = index.IsRingIndex()
                        ? Game1.player.GetRingItemCount(index, additional_materials)
                        : Game1.player.getItemCountInList(additional_materials, index);
                
                var craftableWithThisIngredient = found / required;
                if (craftableWithThisIngredient < craftableOverall || craftableOverall == -1)
                    craftableOverall = craftableWithThisIngredient;
            }
            
            __result = craftableOverall;
            return false; // don't run original logic

        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}