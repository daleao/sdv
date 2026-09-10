namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingPagePerformHoverActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CraftingPagePerformHoverActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CraftingPagePerformHoverActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<CraftingPage>(nameof(CraftingPage.performHoverAction));
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool CraftingPagePerformHoverActionPrefix()
    {
        return State.TapperCraftingRecipeBeingHovered is null && State.LuremasterCraftingRecipeBeingHovered is null;
    }

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CraftingPagePerformHoverActionPostfix(CraftingPage __instance, int x, int y)
    {
        if (Config.ModKey.HasKeybind(new(SButton.LeftShift)))
        {
            var ctrling = Game1.oldKBState.IsKeyDown(Keys.LeftControl) && __instance.GetChildMenu() is null;
            if (ctrling)
            {
                return;
            }
        }
        else if (Config.ModKey.HasKeybind(new(SButton.LeftControl)))
        {
            var shifting = Game1.oldKBState.IsKeyDown(Keys.LeftShift) && __instance.GetChildMenu() is null;
            if (shifting)
            {
                return;
            }
        }

        var recipe = __instance.hoverRecipe;
        if (recipe is null)
        {
            return;
        }

        var recipeList = recipe.recipeList;
        if (Game1.player.HasProfession(Profession.Luremaster, true) && recipe.itemToProduce is ["685"] && recipeList.Count == 1)
        {
            if (Config.ModKey.IsDown() && State.LuremasterCraftingRecipeBeingHovered is null)
            {
                State.LuremasterValidIngredientsForSubstitution.Clear();
                var inventory = __instance.inventory.actualInventory;
                for (var i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i] is SObject @object && @object.IsValidBaitIngredientForLuremaster())
                    {
                        State.LuremasterValidIngredientsForSubstitution.Add(i);
                    }
                }

                if (State.LuremasterValidIngredientsForSubstitution.Count < 1)
                {
                    return;
                }

                __instance.inventory.highlightMethod = item => item is SObject @object && @object.IsValidBaitIngredientForLuremaster();
                __instance.exitFunction = () =>
                {
                    State.LuremasterCraftingRecipeBeingHovered = null;
                    State.LuremasterCraftingIngredientSelected = 0;
                    __instance.inventory.highlightMethod = InventoryMenu.highlightAllItems;
                };

                State.LuremasterCraftingRecipeBeingHovered = recipe;
                State.OriginalRecipeList = [.. recipeList];
                State.OriginalQuantityPerCraft = recipe.numberProducedPerCraft;
                State.LuremasterCraftingIngredientSelected = 0;
                State.CraftingMenuCursorLockPosition = new(x, y);
                var substituteIngredient = (SObject)__instance.inventory.actualInventory[State.LuremasterCraftingIngredientSelected];
                recipe.AlterCraftingRecipeForLuremaster(substituteIngredient);
                return;
            }

            if (Config.ModKey.IsDown() && State.LuremasterCraftingRecipeBeingHovered is not null &&
                __instance.hoverItem is SObject hoverObject && !hoverObject.IsBait())
            {
                recipe.AlterCraftingRecipeForLuremaster(hoverObject);
                return;
            }

            if (!Config.ModKey.IsDown() && State.LuremasterCraftingRecipeBeingHovered == recipe)
            {
                __instance.inventory.highlightMethod = InventoryMenu.highlightAllItems;
                State.LuremasterCraftingRecipeBeingHovered = null;
                State.LuremasterCraftingIngredientSelected = 0;
                recipe.ResetLuremasterCraftingRecipe();
                return;
            }
        }

        if (Game1.player.HasProfession(Profession.Tapper, true) && recipeList.Count > 1)
        {
            if (Config.ModKey.IsDown() && State.TapperCraftingRecipeBeingHovered is null && recipeList.None(i => i.Key == "92"))
            {
                State.TapperValidIngredientsForSubstitution.Clear();
                var doesRecipeUseSyrup = recipeList.Any(i => i.Key.IsSyrupId());
                for (var i = 0; i < recipeList.Count; i++)
                {
                    var ingredient = recipeList.ElementAt(i).Key;
                    if (!doesRecipeUseSyrup || ingredient.IsSyrupId())
                    {
                        State.TapperValidIngredientsForSubstitution.Add(i);
                    }
                }

                if (State.TapperValidIngredientsForSubstitution.Count < 1)
                {
                    return;
                }

                __instance.exitFunction = () =>
                {
                    State.TapperCraftingRecipeBeingHovered = null;
                    State.TapperCraftingIngredientSelected = 0;
                };

                State.TapperCraftingRecipeBeingHovered = recipe;
                State.OriginalRecipeList = [.. recipeList];
                State.OriginalQuantityPerCraft = recipe.numberProducedPerCraft;
                if (doesRecipeUseSyrup)
                {
                    recipe.numberProducedPerCraft /= 2;
                }

                State.TapperCraftingIngredientSelected = State.TapperValidIngredientsForSubstitution[0];
                State.CraftingMenuCursorLockPosition = new(x, y);
                recipe.AlterCraftingRecipeForTapper();
                return;
            }

            if (!Config.ModKey.IsDown() && State.TapperCraftingRecipeBeingHovered == recipe)
            {
                State.TapperCraftingRecipeBeingHovered = null;
                State.TapperCraftingIngredientSelected = 0;
                recipe.ResetTapperCraftingRecipe();
                return;
            }
        }
    }

    #endregion harmony patches
}
