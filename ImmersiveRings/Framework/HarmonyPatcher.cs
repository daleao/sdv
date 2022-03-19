namespace DaLion.Stardew.Rings.Framework;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

using Common.Extensions;
using Extensions;

#endregion using directives

/// <summary>Patches the game code to implement modded tool behavior.</summary>
[UsedImplicitly]
internal static class HarmonyPatcher
{
    #region harmony patches 

    /// <summary>Overrides ingredient consumption to allow non-SObject types.</summary>
    [HarmonyPatch(typeof(CraftingRecipe), nameof(CraftingRecipe.consumeIngredients))]
    internal class CraftingRecipeConsumeIngredientsPatch
    {
        [HarmonyPrefix]
        protected static bool Prefix(CraftingRecipe __instance, IList<Chest> additional_materials)
        {
            if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet") ||
                !ModEntry.Config.NewGlowAndMagnetRecipes && !ModEntry.Config.BetterGlowstoneRecipe) return true; // run original logic

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
    }

    /// <summary>Overrides ingredient search to allow non-Object types.</summary>
    [HarmonyPatch(typeof(CraftingRecipe), nameof(CraftingRecipe.doesFarmerHaveIngredientsInInventory))]
    internal class CraftingRecipeDoesFarmerHaveIngredientsInInventory
    {
        [HarmonyPrefix]
        protected static bool Prefix(CraftingRecipe __instance, ref bool __result, IList<Item> extraToCheck)
        {
            if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet") ||
                !ModEntry.Config.NewGlowAndMagnetRecipes && !ModEntry.Config.BetterGlowstoneRecipe) return true; // run original logic

            try
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
            catch (Exception ex)
            {
                Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
                return true; // default to original logic
            }
        }
    }

    /// <summary>Correctly draws recipes with non-Object types.</summary>
    [HarmonyPatch(typeof(CraftingRecipe), nameof(CraftingRecipe.drawRecipeDescription))]
    internal class CraftingRecipeDrawRecipeDescription
    {
        [HarmonyPrefix]
        protected static bool Prefix(CraftingRecipe __instance, SpriteBatch b, Vector2 position, int width, IList<Item> additional_crafting_items)
        {
            if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet") ||
                !ModEntry.Config.NewGlowAndMagnetRecipes && !ModEntry.Config.BetterGlowstoneRecipe) return true; // run original logic

            try
            {
                var lineExpansion = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko
                    ? 8
                    : 0;
                b.Draw(Game1.staminaRect,
                    new Rectangle((int) (position.X + 8f),
                        (int) (position.Y + 32f + Game1.smallFont.MeasureString("Ing!").Y) - (int)(lineExpansion * 1.5f) -
                        6,
                        width - 32, 2), Game1.textColor * 0.35f);

                Utility.drawTextWithShadow(b,
                    Game1.content.LoadString(
                        PathUtilities.NormalizeAssetName("Strings/StringsFromCSFiles:CraftingRecipe.cs.567")),
                    Game1.smallFont,
                    position + new Vector2(8f, 28f), Game1.textColor * 0.75f);
                var i = 0;
                foreach (var (index, required) in __instance.recipeList)
                {
                    var foundInBackpack = index.IsRingIndex()
                        ? Game1.player.GetRingItemCount(index)
                        : Game1.player.getItemCount(index, 8);
                    var remaining = required - foundInBackpack;

                    var foundInContainers = 0;
                    if (additional_crafting_items != null)
                    {
                        foundInContainers = index.IsRingIndex()
                            ? Game1.player.GetRingItemCount(index, additional_crafting_items)
                            : Game1.player.getItemCountInList(additional_crafting_items, index, 8);
                        if (remaining > 0) remaining -= foundInContainers;
                    }

                    var ingredientNameText = __instance.getNameFromIndex(index);
                    var drawColor = remaining <= 0 ? Game1.textColor : Color.Red;
                    b.Draw(Game1.objectSpriteSheet, new(position.X, position.Y + 64f + i * 36f),
                        Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                            __instance.getSpriteIndexFromRawIndex(index), 16, 16), Color.White, 0f, Vector2.Zero, 2f,
                        SpriteEffects.None, 0.86f);
                    Utility.drawTinyDigits(required, b,
                        new(position.X + 32f - Game1.tinyFont.MeasureString(required.ToString()).X,
                            position.Y + i * 36f + 85f), 2f, 0.87f, Color.AntiqueWhite);
                    var textDrawPosition = new Vector2(position.X + 32f + 8f, position.Y + i * 36f + 68f);
                    Utility.drawTextWithShadow(b, ingredientNameText, Game1.smallFont, textDrawPosition, drawColor);
                    if (!Game1.options.showAdvancedCraftingInformation)
                    {
                        ++i;
                        continue;
                    }

                    textDrawPosition.X = position.X + width - 40f;
                    b.Draw(Game1.mouseCursors,
                        new Rectangle((int)textDrawPosition.X, (int)textDrawPosition.Y + 2, 22, 26),
                        new Rectangle(268, 1436, 11, 13), Color.White);
                    Utility.drawTextWithShadow(b, (foundInBackpack + foundInContainers).ToString(), Game1.smallFont,
                        textDrawPosition -
                        new Vector2(Game1.smallFont.MeasureString(foundInBackpack + foundInContainers + " ").X, 0f),
                        drawColor);
                    ++i;
                }

                b.Draw(Game1.staminaRect,
                    new Rectangle((int) position.X + 8,
                        (int) position.Y + lineExpansion + 64 + 4 + __instance.recipeList.Count * 36, width - 32, 2),
                    Game1.textColor * 0.35f);
                Utility.drawTextWithShadow(b, Game1.parseText(__instance.description, Game1.smallFont, width - 8),
                    Game1.smallFont, position + new Vector2(0f, __instance.recipeList.Count * 36f + lineExpansion + 76f),
                    Game1.textColor * 0.75f);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
                return true; // default to original logic
            }
        }
    }

    /// <summary>Overrides craftable count for non-SObject types.</summary>
    [HarmonyPatch(typeof(CraftingRecipe), nameof(CraftingRecipe.getCraftableCount), typeof(IList<Item>))]
    internal class CraftingRecipeGetCraftableCountPatch
    {
        [HarmonyPrefix]
        protected static bool Prefix(CraftingRecipe __instance, ref int __result, IList<Item> additional_materials)
        {
            if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet") ||
                !ModEntry.Config.NewGlowAndMagnetRecipes && !ModEntry.Config.BetterGlowstoneRecipe) return true; // run original logic

            try
            {
                var craftableOverall = -1;
                foreach (var (index, required) in __instance.recipeList)
                {
                    var found = index.IsRingIndex() ? Game1.player.GetRingItemCount(index) : Game1.player.getItemCount(index);
                    if (additional_materials is not null)
                        found = index.IsRingIndex()
                            ? Game1.player.GetRingItemCount(index, additional_materials)
                            : Game1.player.getItemCountInList(additional_materials, index);

                    var craftableWithThisIngredient = found / required;
                    if (craftableWithThisIngredient < craftableOverall || craftableOverall == -1)
                        craftableOverall = craftableWithThisIngredient;
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
    }

    /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
    [HarmonyPatch(typeof(Ring), nameof(Ring.onEquip))]
    internal class RingOnEquipPatch
    {
        [HarmonyPostfix]
        protected static void Postfix(Ring __instance, Farmer who)
        {
            if (!ModEntry.Config.RebalancedRings) return;

            switch (__instance.indexInTileSheet.Value)
            {
                case Constants.TOPAZ_RING_INDEX_I: // topaz to give +3 defense
                    who.weaponPrecisionModifier -= 0.1f;
                    who.resilience += 3;
                    break;
                case Constants.JADE_RING_INDEX_I: // jade ring to give +30% crit. power
                    who.critPowerModifier += 0.2f;
                    break;
                case Constants.CRAB_RING_INDEX_I: // crab ring to give +8 defense
                    who.resilience += 3;
                    break;
                default:
                    return;
            }
        }
    }

    /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
    [HarmonyPatch(typeof(Ring), nameof(Ring.onUnequip))]
    internal class RingOnUnequipPatch
    {
        [HarmonyPostfix]
        protected static void Postfix(Ring __instance, Farmer who)
        {
            if (!ModEntry.Config.RebalancedRings) return;

            switch (__instance.indexInTileSheet.Value)
            {
                case Constants.TOPAZ_RING_INDEX_I: // topaz to give +3 defense
                    who.weaponPrecisionModifier += 0.1f;
                    who.resilience -= 3;
                    break;
                case Constants.JADE_RING_INDEX_I: // jade ring to give +30% crit. power
                    who.critPowerModifier -= 0.2f;
                    break;
                case Constants.CRAB_RING_INDEX_I: // crab ring to give +8 defense
                    who.resilience -= 3;
                    break;
                default:
                    return;
            }
        }
    }

    #endregion harmony patches
}