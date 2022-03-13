namespace DaLion.Stardew.Tweaks.Framework.Patches.Rings;

#region using directives

using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Menus;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;

using Common.Extensions;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class CraftingPageDrawPatch : BasePatch
{
    private static readonly MethodInfo _GetContainerContents = typeof(CraftingPage).MethodNamed("getContainerContents");

    /// <summary>Construct an instance.</summary>
    internal CraftingPageDrawPatch()
    {
        Original = RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.drawRecipeDescription));
    }

    #region harmony patches

    /// <summary>Correctly draws recipes with non-Object types.</summary>
    [HarmonyPrefix]
    private static bool CraftingRecipeDrawRecipeDescriptionPrefix(CraftingRecipe __instance, SpriteBatch b, Vector2 position, int width, IList<Item> additional_crafting_items)
    {
		var lineExpansion = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? 8 : 0;
        b.Draw(Game1.staminaRect,
            new Rectangle((int) (position.X + 8f),
                (int) (position.Y + 32f + Game1.smallFont.MeasureString("Ing!").Y) - (int) (lineExpansion * 1.5f) - 6,
                width - 32, 2), Game1.textColor * 0.35f);

        Utility.drawTextWithShadow(b,
            Game1.content.LoadString(
                PathUtilities.NormalizeAssetName("Strings/StringsFromCSFiles:CraftingRecipe.cs.567")), Game1.smallFont,
            position + new Vector2(8f, 28f), Game1.textColor * 0.75f);
        var i = 0;
        foreach (var (index, required) in __instance.recipeList)
		{
			var foundInBackpack = index.IsRingIndex() ? Game1.player.GetRingItemCount(index) : Game1.player.getItemCount(index, 8);
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
            b.Draw(Game1.mouseCursors, new Rectangle((int) textDrawPosition.X, (int) textDrawPosition.Y + 2, 22, 26),
                new Rectangle(268, 1436, 11, 13), Color.White);
            Utility.drawTextWithShadow(b, (foundInBackpack + foundInContainers).ToString(), Game1.smallFont,
                textDrawPosition -
                new Vector2(Game1.smallFont.MeasureString(foundInBackpack + foundInContainers + " ").X, 0f), drawColor);
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

	#endregion harmony patches
}