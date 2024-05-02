namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeDoesFarmerHaveIngredientsInInventoryPatcher : HarmonyPatcher
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CraftingRecipeDoesFarmerHaveIngredientsInInventoryPatcher"/>
    ///     class.
    /// </summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CraftingRecipeDoesFarmerHaveIngredientsInInventoryPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.doesFarmerHaveIngredientsInInventory));
    }

    #region harmony patches

    /// <summary>Overrides ingredient search to allow non-Ammo types.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CraftingRecipeDoesFarmerHaveIngredientsInInventoryPrefix(
        CraftingRecipe __instance, ref bool __result, IList<Item>? extraToCheck)
    {
        if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet"))
        {
            return true; // run original logic
        }

        try
        {
            foreach (var (id, required) in __instance.recipeList)
            {
                var remaining = required - (id.IsRingId()
                    ? Game1.player.GetRingItemCount(id)
                    : Game1.player.getItemCount(id));
                if (remaining <= 0)
                {
                    continue;
                }

                if (extraToCheck is not null)
                {
                    remaining -= id.IsRingId()
                        ? Game1.player.GetRingItemCount(id, extraToCheck)
                        : Game1.player.getItemCountInList(extraToCheck, id);
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
