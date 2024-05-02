namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeGetCraftableCountPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CraftingRecipeGetCraftableCountPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CraftingRecipeGetCraftableCountPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target =
            this.RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.getCraftableCount), [typeof(IList<Item>)]);
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Overrides craftable count for non-SObject types.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CraftingRecipeGetCraftableCountPrefix(
        CraftingRecipe __instance, ref int __result, IList<Item> additional_materials)
    {
        if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet"))
        {
            return true; // run original logic
        }

        try
        {
            var craftableOverall = -1;
            foreach (var (id, required) in __instance.recipeList)
            {
                var found = id.IsRingId()
                    ? Game1.player.GetRingItemCount(id)
                    : Game1.player.getItemCount(id);
                if (additional_materials is not null)
                {
                    found = id.IsRingId()
                        ? Game1.player.GetRingItemCount(id, additional_materials)
                        : Game1.player.getItemCountInList(additional_materials, id);
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
