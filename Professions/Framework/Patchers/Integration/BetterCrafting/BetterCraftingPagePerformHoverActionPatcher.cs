namespace DaLion.Professions.Framework.Patchers.Integration.BetterCrafting;

#region using directives

using DaLion.Professions.Framework.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[ModRequirement("leclair.bettercrafting", minimumVersion: "2.18.0")]
internal sealed class BetterCraftingPagePerformHoverActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BetterCraftingPagePerformHoverActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BetterCraftingPagePerformHoverActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = "Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage"
            .ToType()
            .RequireMethod("performHoverAction");
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool BetterCraftingPagePerformHoverActionPrefix()
    {
        return State.TapperCraftingRecipeBeingHovered is null && State.LuremasterCraftingRecipeBeingHovered is null;
    }

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void BetterCraftingPagePerformHoverActionPostfix(IClickableMenu __instance, int x, int y)
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

        var bcIntegration = BetterCraftingIntegration.Instance!;
        bcIntegration.AssertLoaded();

        var recipe = bcIntegration.Menu!.ActiveRecipe;
        if (recipe is null)
        {
            return;
        }

        var vanillaRecipe = recipe.CraftingRecipe;
        var ingredients = recipe.Ingredients;
        if (vanillaRecipe is null || ingredients is null)
        {
            return;
        }

        var reflectedIngredients = bcIntegration.GetIngredientsFromHoveredRecipe();
        if (reflectedIngredients is null || ingredients.Length != reflectedIngredients.Length)
        {
            ThrowHelper.ThrowInvalidDataException("Mismatch between API-provided and reflected ingredients.");
            return;
        }

        if (Game1.player.HasProfession(Profession.Luremaster, true) && recipe.Name == "Bait" && ingredients.Length == 1)
        {
            var inventoryMenu = bcIntegration.GetInventory(__instance);
            if (Config.ModKey.IsDown() && State.LuremasterCraftingRecipeBeingHovered is null)
            {
                State.LuremasterValidIngredientsForSubstitution.Clear();
                var inventory = inventoryMenu.actualInventory;
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

                inventoryMenu.highlightMethod = item => item is SObject @object && @object.IsValidBaitIngredientForLuremaster();
                __instance.exitFunction = () =>
                {
                    State.LuremasterCraftingRecipeBeingHovered = null;
                    State.LuremasterCraftingIngredientSelected = 0;
                    inventoryMenu.highlightMethod = InventoryMenu.highlightAllItems;
                };

                State.LuremasterCraftingRecipeBeingHovered = vanillaRecipe;
                State.OriginalRecipeList = [.. vanillaRecipe.recipeList];
                State.LuremasterCraftingIngredientSelected = 0;
                bcIntegration.HoveredIngredientsCopy = [.. ingredients];
                State.CraftingMenuCursorLockPosition = new(x, y);
                var substituteIngredient = (SObject)bcIntegration.GetInventoryMenu().actualInventory[State.LuremasterCraftingIngredientSelected];
                vanillaRecipe.AlterCraftingRecipeForLuremaster(substituteIngredient);
                return;
            }

            if (Config.ModKey.IsDown() && State.LuremasterCraftingRecipeBeingHovered is not null &&
                bcIntegration.GetItemBeingHovered(__instance) is SObject hoverObject &&
                !hoverObject.IsBait())
            {
                vanillaRecipe.AlterCraftingRecipeForLuremaster(hoverObject);
                return;
            }

            if (!Config.ModKey.IsDown() && State.LuremasterCraftingRecipeBeingHovered == vanillaRecipe)
            {
                inventoryMenu.highlightMethod = InventoryMenu.highlightAllItems;
                State.LuremasterCraftingRecipeBeingHovered = null;
                State.LuremasterCraftingIngredientSelected = 0;
                vanillaRecipe.ResetLuremasterCraftingRecipe();
                bcIntegration.HoveredIngredientsCopy = null;
                return;
            }
        }

        var recipeList = vanillaRecipe.recipeList;
        if (Game1.player.HasProfession(Profession.Tapper, true) && ingredients.Length > 1)
        {
            if (Config.ModKey.IsDown() && State.TapperCraftingRecipeBeingHovered is null &&
                reflectedIngredients.Cast<object>().None(i => bcIntegration.GetIngredientId(i) == "92"))
            {
                State.TapperValidIngredientsForSubstitution.Clear();
                var doesRecipeUseSyrup = reflectedIngredients
                    .Cast<object>()
                    .Any(i => bcIntegration.GetIngredientId(i).IsSyrupId());
                for (var i = 0; i < ingredients.Length; i++)
                {
                    var ingredient = ingredients[i];
                    var reflectedIngredient = reflectedIngredients.GetValue(i);
                    if (reflectedIngredient is null)
                    {
                        ThrowHelper.ThrowInvalidDataException($"Reflected ingredient at index {i} was null!");
                        return;
                    }

                    if (!doesRecipeUseSyrup || bcIntegration.GetIngredientId(reflectedIngredient).IsSyrupId())
                    {
                        State.TapperValidIngredientsForSubstitution.Add(i);
                    }
                }

                if (State.TapperValidIngredientsForSubstitution.Count < 1)
                {
                    return;
                }

                State.TapperCraftingRecipeBeingHovered = vanillaRecipe;
                __instance.exitFunction = () =>
                {
                    State.TapperCraftingRecipeBeingHovered = null;
                    State.TapperCraftingIngredientSelected = 0;
                };

                State.OriginalQuantityPerCraft = vanillaRecipe.numberProducedPerCraft;
                if (doesRecipeUseSyrup)
                {
                    vanillaRecipe.numberProducedPerCraft /= 2;
                }

                bcIntegration.HoveredIngredientsCopy = [.. ingredients];
                State.CraftingMenuCursorLockPosition = new(x, y);
                vanillaRecipe.AlterCraftingRecipeForTapper();
                return;
            }

            if (!Config.ModKey.IsDown() && State.TapperCraftingRecipeBeingHovered == vanillaRecipe)
            {
                State.TapperCraftingRecipeBeingHovered = null;
                State.TapperCraftingIngredientSelected = 0;
                vanillaRecipe.ResetTapperCraftingRecipe();
                bcIntegration.HoveredIngredientsCopy = null;
            }
        }
    }

    #endregion harmony patches
}
