namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Common;
using Common.Extensions;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeConsumeIngredientsPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CraftingRecipeConsumeIngredientsPatch()
    {
        Target = RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.consumeIngredients));
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Overrides ingredient consumption to allow non-SObject types.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CraftingRecipeConsumeIngredientsPrefix(CraftingRecipe __instance, IList<Chest?>? additional_materials)
    {
        if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet") ||
            !ModEntry.Config.CraftableGlowAndMagnetRings && !ModEntry.Config.ImmersiveGlowstoneRecipe) return true; // run original logic

        try
        {
            foreach (var (index, required) in __instance.recipeList)
            {
                var remaining = index.IsRingIndex()
                    ? Game1.player.ConsumeRing(index, required)
                    : Game1.player.ConsumeObject(index, required);
                if (remaining <= 0) continue;

                if (additional_materials is null) throw new("Failed to consume required materials.");

                foreach (var chest in additional_materials)
                {
                    if (chest is null) continue;

                    remaining = index.IsRingIndex()
                        ? chest.ConsumeRing(index, remaining)
                        : chest.ConsumeObject(index, remaining);
                    if (remaining > 0) continue;

                    chest.clearNulls();
                    break;
                }

                if (remaining > 0) throw new("Failed to consume required materials.");
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