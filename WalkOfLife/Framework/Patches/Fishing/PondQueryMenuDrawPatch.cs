using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using System;
using System.Linq;
using SObject = StardewValley.Object;
using SUtility = StardewValley.Utility;

namespace TheLion.AwesomeProfessions
{
	internal class PondQueryMenuDrawPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(PondQueryMenu), nameof(PondQueryMenu.draw), new[] { typeof(SpriteBatch) }),
				prefix: new HarmonyMethod(GetType(), nameof(PondQueryMenuDrawPrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch to adjust fish pond UI for Aquarist increased max capacity.</summary>
		private static bool PondQueryMenuDrawPrefix(ref PondQueryMenu __instance, ref float ____age, ref Rectangle ____confirmationBoxRectangle, ref string ____confirmationText, ref SObject ____fishItem, ref FishPond ____pond, ref bool ___confirmingEmpty, ref string ___hoverText, SpriteBatch b)
		{
			try
			{
				var owner = Game1.getFarmer(____pond.owner.Value);
				if (!Utility.SpecificPlayerHasProfession("Aquarist", owner) || ____pond.lastUnlockedPopulationGate.Value < AwesomeProfessions.Reflection.GetField<FishPondData>(____pond, name: "_fishPondData").GetValue().PopulationGates.Keys.Max()) return true; // run original logic;

				if (!Game1.globalFade)
				{
					b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
					var hasUnresolvedNeeds = ____pond.neededItem.Value != null && ____pond.HasUnresolvedNeeds() && !____pond.hasCompletedRequest.Value;
					var pondNameText = Game1.content.LoadString("Strings\\UI:PondQuery_Name", ____fishItem.DisplayName);
					var textSize = Game1.smallFont.MeasureString(pondNameText);
					Game1.DrawBox((int)((Game1.uiViewport.Width / 2) - (textSize.X + 64f) * 0.5f), __instance.yPositionOnScreen - 4 + 128, (int)(textSize.X + 64f), 64);
					SUtility.drawTextWithShadow(b, pondNameText, Game1.smallFont, new Vector2((Game1.uiViewport.Width / 2) - textSize.X * 0.5f, __instance.yPositionOnScreen - 4 + 160f - textSize.Y * 0.5f), Color.Black);
					var displayedText = AwesomeProfessions.Reflection.GetMethod(__instance, name: "getDisplayedText").Invoke<string>();
					var extraHeight = 0;
					if (hasUnresolvedNeeds)
						extraHeight += 116;

					var extraTextHeight = AwesomeProfessions.Reflection.GetMethod(__instance, name: "measureExtraTextHeight").Invoke<int>(displayedText);
					Game1.drawDialogueBox(__instance.xPositionOnScreen, __instance.yPositionOnScreen + 128, PondQueryMenu.width, PondQueryMenu.height - 128 + extraHeight + extraTextHeight, speaker: false, drawOnlyBox: true);
					var populationText = Game1.content.LoadString("Strings\\UI:PondQuery_Population", string.Concat(____pond.FishCount), ____pond.maxOccupants.Value);
					textSize = Game1.smallFont.MeasureString(populationText);
					SUtility.drawTextWithShadow(b, populationText, Game1.smallFont, new Vector2((__instance.xPositionOnScreen + PondQueryMenu.width / 2) - textSize.X * 0.5f, __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128), Game1.textColor);
					var slotsToDraw = ____pond.maxOccupants.Value;
					var slotSpacing = 11f;
					var x = 0;
					var y = 0;
					for (var i = 0; i < slotsToDraw; ++i)
					{
						var yOffset = (float)Math.Sin(____age * 1f + x * 0.75f + y * 0.25f) * 2f;
						if (i < ____pond.FishCount)
							____fishItem.drawInMenu(b, new Vector2((__instance.xPositionOnScreen - 20 + PondQueryMenu.width / 2) - slotSpacing * Math.Min(slotsToDraw, 5) * 4f * 0.5f + slotSpacing * 4f * x - 12f, (__instance.yPositionOnScreen + (int)(yOffset * 4f)) + (y * 4) * slotSpacing + 275.2f), 0.75f, 1f, 0f, StackDrawType.Hide, Color.White, drawShadow: false);
						else
							____fishItem.drawInMenu(b, new Vector2((__instance.xPositionOnScreen - 20 + PondQueryMenu.width / 2) - slotSpacing * Math.Min(slotsToDraw, 5) * 4f * 0.5f + slotSpacing * 4f * x - 12f, (__instance.yPositionOnScreen + (int)(yOffset * 4f)) + (y * 4) * slotSpacing + 275.2f), 0.75f, 0.35f, 0f, StackDrawType.Hide, Color.Black, drawShadow: false);

						++x;
						if (x != 6) continue;

						x = 0;
						++y;
					}

					textSize = Game1.smallFont.MeasureString(displayedText);
					SUtility.drawTextWithShadow(b, displayedText, Game1.smallFont, new Vector2((__instance.xPositionOnScreen + PondQueryMenu.width / 2) - textSize.X * 0.5f, (__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - (hasUnresolvedNeeds ? 32 : 48)) - textSize.Y), Game1.textColor);
					if (hasUnresolvedNeeds)
					{
						AwesomeProfessions.Reflection.GetMethod(__instance, name: "drawHorizontalPartition").Invoke(b, (int)((__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight) - 48f));
						SUtility.drawWithShadow(b, Game1.mouseCursors, new Vector2((__instance.xPositionOnScreen + 60) + 8f * Game1.dialogueButtonScale / 10f, __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 28), new Rectangle(412, 495, 5, 4), Color.White, (float)Math.PI / 2f, Vector2.Zero);
						var bringText = Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequest_Bring");
						textSize = Game1.smallFont.MeasureString(bringText);
						var leftX = __instance.xPositionOnScreen + 88;
						float textX = leftX;
						var iconX = textX + textSize.X + 4f;
						if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr)
						{
							iconX = leftX - 8;
							textX = leftX + 76;
						}

						SUtility.drawTextWithShadow(b, bringText, Game1.smallFont, new Vector2(textX, __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 24), Game1.textColor);
						b.Draw(Game1.objectSpriteSheet, new Vector2(iconX, __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 4), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ____pond.neededItem.Value.ParentSheetIndex, 16, 16), Color.Black * 0.4f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
						b.Draw(Game1.objectSpriteSheet, new Vector2(iconX + 4f, __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ____pond.neededItem.Value.ParentSheetIndex, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
						if (____pond.neededItemCount.Value > 1)
							SUtility.drawTinyDigits(____pond.neededItemCount.Value, b, new Vector2(iconX + 48f, __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 48), 3f, 1f, Color.White);
					}

					__instance.okButton.draw(b);
					__instance.emptyButton.draw(b);
					__instance.changeNettingButton.draw(b);
					if (___confirmingEmpty)
					{
						b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
						var padding = 16;
						____confirmationBoxRectangle.Width += padding;
						____confirmationBoxRectangle.Height += padding;
						____confirmationBoxRectangle.X -= padding / 2;
						____confirmationBoxRectangle.Y -= padding / 2;
						Game1.DrawBox(____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y, ____confirmationBoxRectangle.Width, ____confirmationBoxRectangle.Height);
						____confirmationBoxRectangle.Width -= padding;
						____confirmationBoxRectangle.Height -= padding;
						____confirmationBoxRectangle.X += padding / 2;
						____confirmationBoxRectangle.Y += padding / 2;
						b.DrawString(Game1.smallFont, ____confirmationText, new Vector2(____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y), Game1.textColor);
						__instance.yesButton.draw(b);
						__instance.noButton.draw(b);
					}
					else if (!string.IsNullOrEmpty(___hoverText))
						IClickableMenu.drawHoverText(b, ___hoverText, Game1.smallFont);
				}

				__instance.drawMouse(b);
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(PondQueryMenuDrawPrefix)}:\n{ex}");
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}