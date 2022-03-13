namespace DaLion.Stardew.Tweaks.Framework.Patches.Rings;

#region using directives

using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class CraftingRecipeDoesFarmerHaveIngredientsInInventoryPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CraftingRecipeDoesFarmerHaveIngredientsInInventoryPatch()
    {
        Original = RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.doesFarmerHaveIngredientsInInventory));
    }

    #region harmony patches

    /// <summary>Overrides ingredient search to allow non-Object types.</summary>
    [HarmonyPrefix]
    private static bool CraftingRecipeDoesFarmerHaveIngredientsInInventoryPrefix(CraftingRecipe __instance,
        // ReSharper disable once RedundantAssignment
        ref bool __result, IList<Item> extraToCheck)
    {
        foreach (var (index, required) in __instance.recipeList)
        {
            var remaining = required - (index.IsRingIndex()
                ? Game1.player.GetRingItemCount(index)
                : Game1.player.getItemCount(index, 5));
            if (remaining <= 0) continue;
            
            if (extraToCheck is not null)
            {
                remaining -= index.IsRingIndex()
                    ? Game1.player.GetRingItemCount(index, extraToCheck)
                    : Game1.player.getItemCountInList(extraToCheck, index, 5);
                if (remaining <= 0) continue;
            }

            __result = false;
            return false; // don't run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}