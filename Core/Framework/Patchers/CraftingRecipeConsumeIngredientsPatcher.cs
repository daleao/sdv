namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Inventories;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeConsumeIngredientsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CraftingRecipeConsumeIngredientsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CraftingRecipeConsumeIngredientsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.consumeIngredients));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Overrides ingredient consumption to allow non-SObject types.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CraftingRecipeConsumeIngredientsPrefix(
        CraftingRecipe __instance, List<IInventory?>? additionalMaterials)
    {
        if (!__instance.name.Contains("Ring") ||
            !__instance.name.ContainsAnyOf("Glow", "Magnet"))
        {
            return true; // run original logic
        }

        try
        {
            foreach (var (id, required) in __instance.recipeList)
            {
                var remaining = id.IsRingId()
                    ? Game1.player.ConsumeRing(id, required)
                    : Game1.player.ConsumeObject(id, required);
                if (remaining <= 0 || additionalMaterials is null)
                {
                    continue;
                }

                foreach (var items in additionalMaterials)
                {
                    if (items is null)
                    {
                        continue;
                    }

                    remaining = id.IsRingId()
                        ? items.ConsumeRing(id, remaining)
                        : items.ConsumeObject(id, remaining);
                    if (remaining > 0)
                    {
                        continue;
                    }

                    items.RemoveEmptySlots();
                    break;
                }

                if (remaining > 0)
                {
                    throw new Exception("Failed to consume required materials.");
                }
            }

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
