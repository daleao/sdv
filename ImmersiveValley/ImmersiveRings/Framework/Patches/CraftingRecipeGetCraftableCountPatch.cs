namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Common.Extensions;
using DaLion.Stardew.Rings.Extensions;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeGetCraftableCountPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CraftingRecipeGetCraftableCountPatch"/> class.</summary>
    internal CraftingRecipeGetCraftableCountPatch()
    {
        this.Target =
            this.RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.getCraftableCount), new[] { typeof(IList<Item>) });
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Overrides craftable count for non-SObject types.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CraftingRecipeGetCraftableCountPrefix(
        CraftingRecipe __instance, ref int __result, IList<Item> additional_materials)
    {
        if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet") ||
            (!ModEntry.Config.CraftableGlowAndMagnetRings && !ModEntry.Config.ImmersiveGlowstoneRecipe))
        {
            return true; // run original logic
        }

        try
        {
            var craftableOverall = -1;
            foreach (var (index, required) in __instance.recipeList)
            {
                var found = index.IsRingIndex()
                    ? Game1.player.GetRingItemCount(index)
                    : Game1.player.getItemCount(index);
                if (additional_materials is not null)
                {
                    found = index.IsRingIndex()
                        ? Game1.player.GetRingItemCount(index, additional_materials)
                        : Game1.player.getItemCountInList(additional_materials, index);
                }

                var craftableWithThisIngredient = found / required;
                if (craftableWithThisIngredient < craftableOverall || craftableOverall == -1)
                {
                    craftableOverall = craftableWithThisIngredient;
                }
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
