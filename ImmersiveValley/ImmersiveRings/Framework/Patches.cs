namespace DaLion.Stardew.Rings.Framework;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

using Common.Extensions;
using Common.Extensions.Reflection;
using Common.Harmony;
using Extensions;

#endregion using directives

/// <summary>Patches the game code to implement modded ring behavior.</summary>
[UsedImplicitly]
internal static class Patches
{
    #region harmony patches 

    [HarmonyPatch(typeof(CraftingRecipe), nameof(CraftingRecipe.consumeIngredients))]
    internal class CraftingRecipeConsumeIngredientsPatch
    {
        /// <summary>Overrides ingredient consumption to allow non-SObject types.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(CraftingRecipe __instance, IList<Chest> additional_materials)
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
    }

    [HarmonyPatch(typeof(CraftingRecipe), nameof(CraftingRecipe.doesFarmerHaveIngredientsInInventory))]
    internal class CraftingRecipeDoesFarmerHaveIngredientsInInventory
    {
        /// <summary>Overrides ingredient search to allow non-Object types.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(CraftingRecipe __instance, ref bool __result, IList<Item> extraToCheck)
        {
            if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet") ||
                !ModEntry.Config.CraftableGlowAndMagnetRings && !ModEntry.Config.ImmersiveGlowstoneRecipe) return true; // run original logic

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

    [HarmonyPatch(typeof(CraftingRecipe), nameof(CraftingRecipe.drawRecipeDescription))]
    internal class CraftingRecipeDrawRecipeDescription
    {
        /// <summary>Correctly draws recipes with non-Object types.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(CraftingRecipe __instance, SpriteBatch b, Vector2 position, int width, IList<Item> additional_crafting_items)
        {
            if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet") ||
                !ModEntry.Config.CraftableGlowAndMagnetRings && !ModEntry.Config.ImmersiveGlowstoneRecipe) return true; // run original logic

            try
            {
                var lineExpansion = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko
                    ? 8
                    : 0;
                b.Draw(Game1.staminaRect,
                    new Rectangle((int)(position.X + 8f),
                        (int)(position.Y + 32f + Game1.smallFont.MeasureString("Ing!").Y) - (int)(lineExpansion * 1.5f) -
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
                    new Rectangle((int)position.X + 8,
                        (int)position.Y + lineExpansion + 64 + 4 + __instance.recipeList.Count * 36, width - 32, 2),
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

    [HarmonyPatch(typeof(CraftingRecipe), nameof(CraftingRecipe.getCraftableCount), typeof(IList<Item>))]
    internal class CraftingRecipeGetCraftableCountPatch
    {
        /// <summary>Overrides craftable count for non-SObject types.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(CraftingRecipe __instance, ref int __result, IList<Item> additional_materials)
        {
            if (!__instance.name.Contains("Ring") || !__instance.name.ContainsAnyOf("Glow", "Magnet") ||
                !ModEntry.Config.CraftableGlowAndMagnetRings && !ModEntry.Config.ImmersiveGlowstoneRecipe) return true; // run original logic

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

    [HarmonyPatch(typeof(Ring), nameof(Ring.onEquip))]
    internal class RingOnEquipPatch
    {
        /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(Ring __instance, Farmer who)
        {
            if (ModEntry.Config.ForgeableIridiumBand &&
                __instance.indexInTileSheet.Value == Constants.IRIDIUM_BAND_INDEX_I) return false; // don't run original logic

            if (!ModEntry.Config.RebalancedRings) return true; // run original logic
            
            switch (__instance.indexInTileSheet.Value)
            {
                case Constants.TOPAZ_RING_INDEX_I: // topaz to give +3 defense
                    who.resilience += 3;
                    return false; // don't run original logic
                case Constants.JADE_RING_INDEX_I: // jade ring to give +30% crit. power
                    who.critPowerModifier += 0.3f;
                    return false; // don't run original logic
                case Constants.CRAB_RING_INDEX_I: // crab ring to give +12 defense
                    who.resilience += 12;
                    return false; // don't run original logic
                default:
                    return true; // run original logic
            }
        }
    }

    [HarmonyPatch(typeof(Ring), nameof(Ring.onUnequip))]
    internal class RingOnUnequipPatch
    {
        /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(Ring __instance, Farmer who)
        {
            if (ModEntry.Config.ForgeableIridiumBand &&
                __instance.indexInTileSheet.Value == Constants.IRIDIUM_BAND_INDEX_I) return false; // don't run original logic

            if (!ModEntry.Config.RebalancedRings) return true; // run original logic

            switch (__instance.indexInTileSheet.Value)
            {
                case Constants.TOPAZ_RING_INDEX_I: // topaz to give +3 defense
                    who.resilience -= 3;
                    return false; // don't run original logic
                case Constants.JADE_RING_INDEX_I: // jade ring to give +30% crit. power
                    who.critPowerModifier -= 0.3f;
                    return false; // don't run original logic
                case Constants.CRAB_RING_INDEX_I: // crab ring to give +12 defense
                    who.resilience -= 12;
                    return false; // don't run original logic
                default:
                    return true; // run original logic
            }
        }
    }

    [HarmonyPatch(typeof(Ring), nameof(Ring.onNewLocation))]
    internal class RingOnNewLocationPatch
    {
        /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(Ring __instance, Farmer who)
        {
            return !ModEntry.Config.ForgeableIridiumBand || __instance.indexInTileSheet.Value != Constants.IRIDIUM_BAND_INDEX_I;
        }
    }

    [HarmonyPatch(typeof(Ring), nameof(Ring.onLeaveLocation))]
    internal class RingOnLeaveLocationPatch
    {
        /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(Ring __instance, Farmer who)
        {
            return !ModEntry.Config.ForgeableIridiumBand || __instance.indexInTileSheet.Value != Constants.IRIDIUM_BAND_INDEX_I;
        }
    }

    [HarmonyPatch(typeof(Ring), nameof(Ring.drawTooltip))]
    internal class RingDrawTooltipPatch
    {
        /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
        [HarmonyTranspiler]
        protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            var displayVanillaEffect = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Ldc_I4_5)
                    )
                    .AddLabels(displayVanillaEffect)
                    .Insert(
                        new CodeInstruction(OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(OpCodes.Call,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.RebalancedRings))),
                        new CodeInstruction(OpCodes.Brfalse_S, displayVanillaEffect),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 12),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution)
                    )
                    .Advance()
                    .AddLabels(resumeExecution);
            }
            catch (Exception ex)
            {
                Log.E($"Failed injecting custom crabshell tooltip.\nHelper returned {ex}");
                return null;
            }

            return helper.Flush();
        }
    }

    [HarmonyPatch(typeof(Ring), nameof(Ring.CanCombine))]
    internal class RingCanCombinePatch
    {
        /// <summary>Allows feeding up to four gemstone rings into iridium bands.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(Ring __instance, ref bool __result, Ring ring)
        {
            if (!ModEntry.Config.ForgeableIridiumBand) return true; // run original logic

            if (__instance.ParentSheetIndex == Constants.IRIDIUM_BAND_INDEX_I)
            {
                __result = ring.IsGemRing() &&
                           (__instance is not CombinedRing combined || combined.combinedRings.Count < 4);
                return false; // don't run original logic
            }

            if (ring.ParentSheetIndex == Constants.IRIDIUM_BAND_INDEX_I)
                return false; // don't run original logic

            return true; // run original logic
        }
    }

    [HarmonyPatch(typeof(Ring), nameof(Ring.Combine))]
    internal class RingCombine
    {
        /// <summary>Changes combined ring to iridium band when combining.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(Ring __instance, ref Ring __result, Ring ring)
        {
            if (!ModEntry.Config.ForgeableIridiumBand || __instance.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I)
                return true; // run original logic

            try
            {
                var toCombine = new List<Ring>();
                if (__instance is CombinedRing combined)
                {
                    if (combined.combinedRings.Count >= 4)
                        throw new InvalidOperationException("Unexpected number of combined rings.");

                    toCombine.AddRange(combined.combinedRings);
                }

                toCombine.Add(ring);
                var combinedRing = new CombinedRing(880);
                combinedRing.combinedRings.AddRange(toCombine);
                combinedRing.ParentSheetIndex = Constants.IRIDIUM_BAND_INDEX_I;
                ModEntry.ModHelper.Reflection.GetField<NetInt>(combinedRing, nameof(Ring.indexInTileSheet)).GetValue()
                    .Set(Constants.IRIDIUM_BAND_INDEX_I);
                combinedRing.UpdateDescription();
                __result = combinedRing;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
                return true; // default to original logic
            }
        }
    }

    [HarmonyPatch(typeof(CombinedRing), nameof(CombinedRing._GetOneFrom))]
    internal class CombinedRingGetOneFromPatch
    {
        /// <summary>Changes combined ring to iridium band when getting one.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(CombinedRing __instance, Item source)
        {
            if (!ModEntry.Config.ForgeableIridiumBand || source.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I)
                return true; // run original logic

            __instance.ParentSheetIndex = Constants.IRIDIUM_BAND_INDEX_I;
            ModEntry.ModHelper.Reflection.GetField<NetInt>(__instance, nameof(Ring.indexInTileSheet)).GetValue()
                .Set(Constants.IRIDIUM_BAND_INDEX_I);
            return true; // run original logic
        }
    }

    [HarmonyPatch(typeof(CombinedRing), "loadDisplayFields")]
    internal class CombinedRingsLoadDisplayFieldsPatch
    {
        /// <summary>Iridium description is always first, and gemstone descriptions are grouped together.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(CombinedRing __instance, ref bool __result)
        {
            if (!ModEntry.Config.ForgeableIridiumBand || __instance.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I)
                return true; // don't run original logic

            if (Game1.objectInformation is null || __instance.indexInTileSheet is null)
            {
                __result = false;
                return false; // don't run original logic
            }

            var data = Game1.objectInformation[__instance.indexInTileSheet.Value].Split('/');
            __instance.displayName = data[4];
            __instance.description = data[5];

            int addedKnockback = 0, addedPrecision = 0, addedCritChance = 0, addedCritPower = 0, addedSwingSpeed = 0, addedDamage = 0, addedDefense = 0;
            foreach (var ring in __instance.combinedRings)
                switch (ring.ParentSheetIndex)
                {
                    case Constants.AMETHYSTR_RING_INDEX_I:
                        addedKnockback += 10;
                        break;
                    case Constants.TOPAZ_RING_INDEX_I:
                        if (ModEntry.Config.RebalancedRings) addedDefense += 3;
                        else addedPrecision += 10;
                        break;
                    case Constants.AQUAMARINE_RING_INDEX_I:
                        addedCritChance += 10;
                        break;
                    case Constants.JADE_RING_INDEX_I:
                        addedCritPower += ModEntry.Config.RebalancedRings ? 30 : 10;
                        break;
                    case Constants.EMERALD_RING_INDEX_I:
                        addedSwingSpeed += 10;
                        break;
                    case Constants.RUBY_RING_INDEX_I:
                        addedDamage += 10;
                        break;
                }

            if (addedKnockback > 0)
            {
                data = Game1.objectInformation[Constants.AMETHYSTR_RING_INDEX_I].Split('/');
                var description = Regex.Replace(data[5], @"\d{2}", addedKnockback.ToString());
                __instance.description += "\n\n" + description;
            }

            if (addedPrecision > 0)
            {
                data = Game1.objectInformation[Constants.TOPAZ_RING_INDEX_I].Split('/');
                var description = Regex.Replace(data[5], @"\d{2}", addedPrecision.ToString());
                __instance.description += "\n\n" + description;
            }

            if (addedCritChance > 0)
            {
                data = Game1.objectInformation[Constants.AQUAMARINE_RING_INDEX_I].Split('/');
                var description = Regex.Replace(data[5], @"\d{2}", addedCritChance.ToString());
                __instance.description += "\n\n" + description;
            }

            if (addedCritPower > 0)
            {
                data = Game1.objectInformation[Constants.JADE_RING_INDEX_I].Split('/');
                var description = Regex.Replace(data[5], @"\d{2}", addedCritPower.ToString());
                __instance.description += "\n\n" + description;
            }

            if (addedSwingSpeed > 0)
            {
                data = Game1.objectInformation[Constants.EMERALD_RING_INDEX_I].Split('/');
                var description = Regex.Replace(data[5], @"\d{2}", addedSwingSpeed.ToString());
                __instance.description += "\n\n" + description;
            }

            if (addedDamage > 0)
            {
                data = Game1.objectInformation[Constants.RUBY_RING_INDEX_I].Split('/');
                var description = Regex.Replace(data[5], @"\d{2}", addedDamage.ToString());
                __instance.description += "\n\n" + description;
            }

            if (addedDefense > 0)
            {
                var description = ModEntry.ModHelper.Translation.Get("rings.topaz").ToString();
                description = Regex.Replace(description, @"\d{1}", addedDefense.ToString());
                __instance.description += "\n\n" + description;
            }

            __instance.description = __instance.description.Trim();
            __result = true;
            return false; // don't run original logic
        }
    }

    [HarmonyPatch(typeof(CombinedRing), nameof(CombinedRing.drawInMenu))]
    internal class CombinedRingDrawInMenuPatch
    {
        /// <summary>Draw gemstones on combined iridium band.</summary>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.HigherThanNormal)]
        protected static bool Prefix(CombinedRing __instance, SpriteBatch spriteBatch, Vector2 location,
            float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color,
            bool drawShadow)
        {
            if (!ModEntry.Config.ForgeableIridiumBand || __instance.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I)
                return true; // run original logic

            try
            {
                var count = __instance.combinedRings.Count;
                if (count is < 1 or > 4)
                    throw new InvalidOperationException("Unexpected number of combined rings.");

                var oldScaleSize = scaleSize;
                scaleSize = 1f;
                location.Y -= (oldScaleSize - 1f) * 32f;
                
                // draw left half
                var src = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, __instance.indexInTileSheet.Value, 16, 16);
                src.X += 5;
                src.Y += 7;
                src.Width = 4;
                src.Height = 6;
                spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(51f, 51f) * scaleSize + new Vector2(-12f, 8f) * scaleSize, src, color * transparency, 0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
                src.X++;
                src.Y += 4;
                src.Width = 3;
                src.Height = 1;
                spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(51f, 51f) * scaleSize + new Vector2(-8f, 4f) * scaleSize, src, color * transparency, 0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);

                // draw right half
                src = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, __instance.indexInTileSheet.Value, 16, 16);
                src.X += 9;
                src.Y += 7;
                src.Width = 4;
                src.Height = 6;
                spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(51f, 51f) * scaleSize + new Vector2(4f, 8f) * scaleSize, src, color * transparency, 0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
                src.Y += 4;
                src.Width = 3;
                src.Height = 1;
                spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(51f, 51f) * scaleSize + new Vector2(4f, 4f) * scaleSize, src, color * transparency, 0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);

                RingDrawInMenuPatch.Reverse(__instance, spriteBatch, location + new Vector2(-5f, -1f), scaleSize, transparency,
                    layerDepth, drawStackNumber, color, drawShadow);

                Vector2 offset;

                // draw top gem
                color = Utils.ColorByGemstone[__instance.combinedRings[0].ParentSheetIndex] * transparency;
                offset = ModEntry.HasBetterRings ? new Vector2(19f, 3f) : new(23f, 11f);
                spriteBatch.Draw(Textures.GemstonesTx, location + offset * scaleSize,
                    new Rectangle(0, 0, 4, 4), color, 0f, Vector2.Zero, scaleSize * 4f, SpriteEffects.None, layerDepth);

                if (count > 1)
                {
                    // draw bottom gem (or left, in case of better rings)
                    color = Utils.ColorByGemstone[__instance.combinedRings[1].ParentSheetIndex] * transparency;
                    offset = ModEntry.HasBetterRings ? new Vector2(23f, 19f) : new(23f, 43f);
                    spriteBatch.Draw(Textures.GemstonesTx, location + offset * scaleSize,
                        new Rectangle(4, 0, 4, 4), color, 0, Vector2.Zero, scaleSize * 4f, SpriteEffects.None,
                        layerDepth);
                }

                if (count > 2)
                {
                    // draw left gem (or right, in case of better rings)
                    color = Utils.ColorByGemstone[__instance.combinedRings[2].ParentSheetIndex] * transparency;
                    offset = ModEntry.HasBetterRings ? new Vector2(35f, 7f) : new(7f, 27f);
                    spriteBatch.Draw(Textures.GemstonesTx, location + offset * scaleSize,
                        new Rectangle(8, 0, 4, 4), color, 0f, Vector2.Zero, scaleSize * 4f, SpriteEffects.None, layerDepth);
                }

                if (count > 3)
                {
                    // draw right gem (or bottom, in case of better rings)
                    color = Utils.ColorByGemstone[__instance.combinedRings[3].ParentSheetIndex] * transparency;
                    offset = ModEntry.HasBetterRings ? new Vector2(39f, 23f) : new(39f, 27f);
                    spriteBatch.Draw(Textures.GemstonesTx, location + offset * scaleSize,
                        new Rectangle(12, 0, 4, 4), color, 0f, Vector2.Zero, scaleSize * 4f, SpriteEffects.None, layerDepth);
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
                return true; // default to original logic
            }
        }
    }

    [HarmonyPatch(typeof(Ring), nameof(Ring.drawInMenu))]
    internal class RingDrawInMenuPatch
    {
        /// <summary>Stub for base Ring.drawInMenu</summary>
        [HarmonyReversePatch]
        internal static void Reverse(object instance, SpriteBatch spriteBatch, Vector2 location,
            float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color,
            bool drawShadow)
        {
            // its a stub so it has no initial content
            throw new NotImplementedException("It's a stub.");
        }
    }

    #endregion harmony patches
}