namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Overhaul.Modules.Rings.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeConsumeIngredientsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CraftingRecipeConsumeIngredientsPatcher"/> class.</summary>
    internal CraftingRecipeConsumeIngredientsPatcher()
    {
        this.Target = this.RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.consumeIngredients));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Overrides ingredient consumption to allow non-SObject types.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CraftingRecipeConsumeIngredientsPrefix(
        CraftingRecipe __instance, IList<Chest?>? additional_materials)
    {
        if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet") ||
            (!RingsModule.Config.CraftableGlowAndMagnetRings && !RingsModule.Config.ImmersiveGlowstoneRecipe))
        {
            return true; // run original logic
        }

        try
        {
            foreach (var (index, required) in __instance.recipeList)
            {
                var remaining = index.IsRingIndex()
                    ? Game1.player.ConsumeRing(index, required)
                    : Game1.player.ConsumeObject(index, required);
                if (remaining <= 0)
                {
                    continue;
                }

                if (additional_materials is null)
                {
                    throw new Exception("Failed to consume required materials.");
                }

                foreach (var chest in additional_materials)
                {
                    if (chest is null)
                    {
                        continue;
                    }

                    remaining = index.IsRingIndex()
                        ? chest.ConsumeRing(index, remaining)
                        : chest.ConsumeObject(index, remaining);
                    if (remaining > 0)
                    {
                        continue;
                    }

                    chest.clearNulls();
                    break;
                }

                if (remaining > 0)
                {
                    throw new Exception("Failed to consume required materials.");
                }
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
