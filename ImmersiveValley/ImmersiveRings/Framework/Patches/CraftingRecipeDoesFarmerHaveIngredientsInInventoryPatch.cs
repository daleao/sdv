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
internal sealed class CraftingRecipeDoesFarmerHaveIngredientsInInventoryPatch : HarmonyPatch
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CraftingRecipeDoesFarmerHaveIngredientsInInventoryPatch"/>
    ///     class.
    /// </summary>
    internal CraftingRecipeDoesFarmerHaveIngredientsInInventoryPatch()
    {
        this.Target = this.RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.doesFarmerHaveIngredientsInInventory));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Overrides ingredient search to allow non-Object types.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CraftingRecipeDoesFarmerHaveIngredientsInInventoryPrefix(
        CraftingRecipe __instance, ref bool __result, IList<Item>? extraToCheck)
    {
        if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet") ||
            (!ModEntry.Config.CraftableGlowAndMagnetRings && !ModEntry.Config.ImmersiveGlowstoneRecipe))
        {
            return true; // run original logic
        }

        try
        {
            foreach (var (index, required) in __instance.recipeList)
            {
                var remaining = required - (index.IsRingIndex()
                    ? Game1.player.GetRingItemCount(index)
                    : Game1.player.getItemCount(index, 5));
                if (remaining <= 0)
                {
                    continue;
                }

                if (extraToCheck is not null)
                {
                    remaining -= index.IsRingIndex()
                        ? Game1.player.GetRingItemCount(index, extraToCheck)
                        : Game1.player.getItemCountInList(extraToCheck, index, 5);
                    if (remaining <= 0)
                    {
                        continue;
                    }
                }

                __result = false;
                return false; // don't run original logic
            }

            __result = true;
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
