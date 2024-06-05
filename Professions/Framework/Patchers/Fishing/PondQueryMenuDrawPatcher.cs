// ReSharper disable PossibleLossOfFraction
namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using DaLion.Shared.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Extensions.Stardew;
using StardewValley.Buildings;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class PondQueryMenuDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PondQueryMenuDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal PondQueryMenuDrawPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<PondQueryMenu>(nameof(PondQueryMenu.draw), [typeof(SpriteBatch)]);
    }

    private delegate void DrawHorizontalPartitionDelegate(
        IClickableMenu instance, SpriteBatch b, int yPosition, bool small = false, int red = -1, int green = -1, int blue = -1);

    #region harmony patches

    /// <summary>Adjust fish pond query menu for family members.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool PondQueryMenuDrawPrefix(
        PondQueryMenu __instance,
        float ____age,
        Rectangle ____confirmationBoxRectangle,
        string ____confirmationText,
        bool ___confirmingEmpty,
        string ___hoverText,
        SObject ____fishItem,
        FishPond ____pond,
        SpriteBatch b)
    {
        try
        {
            if (Game1.globalFade)
            {
                __instance.drawMouse(b);
                return false; // don't run original logic
            }

            // draw stuff
            if (!Game1.options.showClearBackgrounds)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            }

            var hasUnresolvedNeeds = ____pond.neededItem.Value is not null && ____pond.HasUnresolvedNeeds() &&
                                     !____pond.hasCompletedRequest.Value;
            var isLegendaryPond = ____fishItem.IsBossFish();
            var pondNameText = Game1.content.LoadString(
                    "Strings\\UI:PondQuery_Name",
                    ____fishItem.DisplayName);
            var textSize = Game1.smallFont.MeasureString(pondNameText);
            Game1.DrawBox(
                (int)((Game1.uiViewport.Width / 2) - ((textSize.X + 64f) * 0.5f)),
                __instance.yPositionOnScreen - 4 + 128,
                (int)(textSize.X + 64f),
                64);

            Utility.drawTextWithShadow(
                b,
                pondNameText,
                Game1.smallFont,
                new Vector2(
                    (Game1.uiViewport.Width / 2) - (textSize.X * 0.5f),
                    __instance.yPositionOnScreen - 4 + 160f - (textSize.Y * 0.5f)),
                Color.Black);
            var displayedText = Reflector
                .GetUnboundMethodDelegate<Func<PondQueryMenu, string>>(__instance, "getDisplayedText")
                .Invoke(__instance);
            var extraHeight = 0;
            if (hasUnresolvedNeeds)
            {
                extraHeight += 116;
            }

            var extraTextHeight = Reflector
                .GetUnboundMethodDelegate<Func<PondQueryMenu, string, int>>(__instance, "measureExtraTextHeight")
                .Invoke(__instance, displayedText);
            Game1.drawDialogueBox(
                __instance.xPositionOnScreen,
                __instance.yPositionOnScreen + 128,
                PondQueryMenu.width,
                PondQueryMenu.height - 128 + extraHeight + extraTextHeight,
                false,
                true);
            var populationText = Game1.content.LoadString(
                "Strings\\UI:PondQuery_Population",
                string.Concat(____pond.FishCount),
                ____pond.maxOccupants.Value);
            textSize = Game1.smallFont.MeasureString(populationText);
            Utility.drawTextWithShadow(
                b,
                populationText,
                Game1.smallFont,
                new Vector2(
                    ____pond.goldenAnimalCracker.Value
                        ? __instance.xPositionOnScreen + IClickableMenu.borderWidth + 4
                        : __instance.xPositionOnScreen + (PondQueryMenu.width / 2) - (textSize.X * 0.5f),
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128),
                Game1.textColor);

            // draw fish
            int x = 0, y = 0, familyCount = 0;
            var slotsToDraw = ____pond.maxOccupants.Value;
            var columns = Math.Min(slotsToDraw, 5);
            const int slotSpacing = 13;
            SObject? itemToDraw, familyItemToDraw;
            if (isLegendaryPond)
            {
                familyCount = Data.ReadAs<int>(____pond, DataKeys.FamilyLivingHere);
                familyItemToDraw = Lookups.FamilyPairs.TryGetValue(____fishItem.QualifiedItemId, out var pairId)
                    ? ItemRegistry.Create<SObject>(pairId)
                    : ____fishItem;
            }
            else
            {
                familyItemToDraw = ____fishItem;
            }

            itemToDraw = ____fishItem;
            for (var i = 0; i < slotsToDraw; ++i)
            {
                var yOffset = (float)Math.Sin(____age + (x * 0.75f) + (y * 0.25f)) * 2f;
                var yPos = __instance.yPositionOnScreen + (int)(yOffset * 4f) + (y * slotSpacing * 4f) + 275.2f;
                var xPos = __instance.xPositionOnScreen + (PondQueryMenu.width / 2) -
                           (columns * slotSpacing * 2f) + (x * slotSpacing * 4f);
                if (i < ____pond.FishCount)
                {
                    if (isLegendaryPond && familyCount > 0 && i == ____pond.FishCount - familyCount)
                    {
                        itemToDraw = familyItemToDraw;
                    }

                    itemToDraw!.drawInMenu(
                        b,
                        new Vector2(xPos, yPos),
                        0.75f,
                        1f,
                        0f,
                        StackDrawType.Hide,
                        Color.White,
                        false);
                }
                else
                {
                    ____fishItem.drawInMenu(
                        b,
                        new Vector2(xPos, yPos),
                        0.75f,
                        0.35f,
                        0f,
                        StackDrawType.Hide,
                        Color.Black,
                        false);
                }

                if (++x != columns)
                {
                    continue;
                }

                x = 0;
                y++;
            }

            // draw more stuff
            textSize = Game1.smallFont.MeasureString(displayedText);
            Utility.drawTextWithShadow(
                b,
                displayedText,
                Game1.smallFont,
                new Vector2(
                    __instance.xPositionOnScreen + (PondQueryMenu.width / 2) - (textSize.X * 0.5f),
                    __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - (hasUnresolvedNeeds ? 32 : 48) - textSize.Y),
                Game1.textColor);

            if (hasUnresolvedNeeds)
            {
                Reflector.GetUnboundMethodDelegate<DrawHorizontalPartitionDelegate>(__instance, "drawHorizontalPartition").Invoke(
                    __instance,
                    b,
                    (int)(__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - 48f));
                Utility.drawWithShadow(
                    b,
                    Game1.mouseCursors,
                    new Vector2(
                        __instance.xPositionOnScreen + 60 + (8f * Game1.dialogueButtonScale / 10f),
                        __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 28),
                    new Rectangle(412, 495, 5, 4),
                    Color.White,
                    (float)Math.PI / 2f,
                    Vector2.Zero);

                var bringText =
                    Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequest_Bring");
                textSize = Game1.smallFont.MeasureString(bringText);
                var leftX = __instance.xPositionOnScreen + 88;
                float textX = leftX;
                var iconX = textX + textSize.X + 4f;
                if (LocalizedContentManager.CurrentLanguageCode.IsAnyOf(
                        LocalizedContentManager.LanguageCode.ja,
                        LocalizedContentManager.LanguageCode.ko,
                        LocalizedContentManager.LanguageCode.tr))
                {
                    iconX = leftX - 8;
                    textX = leftX + 76;
                }

                Utility.drawTextWithShadow(
                    b,
                    bringText,
                    Game1.smallFont,
                    new Vector2(
                        textX,
                        __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 24),
                    Game1.textColor);

                var dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(____pond.neededItem.Value?.QualifiedItemId);
                var texture = dataOrErrorItem.GetTexture();
                var sourceRect = dataOrErrorItem.GetSourceRect();
                b.Draw(
                    texture,
                    new Vector2(
                        iconX,
                        __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 4),
                    sourceRect,
                    Color.Black * 0.4f,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    1f);

                b.Draw(
                    texture,
                    new Vector2(
                        iconX + 4f,
                        __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight),
                    sourceRect,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    1f);

                if (____pond.neededItemCount.Value > 1)
                {
                    Utility.drawTinyDigits(
                        ____pond.neededItemCount.Value,
                        b,
                        new Vector2(
                            iconX + 48f,
                            __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 48),
                        3f,
                        1f,
                        Color.White);
                }
            }

            if (____pond.goldenAnimalCracker.Value && Game1.objectSpriteSheet_2 is not null)
            {
                Utility.drawWithShadow(
                    b,
                    Game1.objectSpriteSheet_2,
                    new Vector2(
                        __instance.xPositionOnScreen + PondQueryMenu.width - 105.6f,
                        __instance.yPositionOnScreen + 224f),
                    new Rectangle(16, 240, 16, 16),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flipped: false,
                    0.89f);
            }

            __instance.okButton.draw(b);
            __instance.emptyButton.draw(b);
            __instance.changeNettingButton.draw(b);
            if (___confirmingEmpty)
            {
                if (!Game1.options.showClearBackgrounds)
                {
                    b.Draw(
                        Game1.fadeToBlackRect,
                        Game1.graphics.GraphicsDevice.Viewport.Bounds,
                        Color.Black * 0.75f);
                }

                const int padding = 16;
                ____confirmationBoxRectangle.Width += padding;
                ____confirmationBoxRectangle.Height += padding;
                ____confirmationBoxRectangle.X -= padding / 2;
                ____confirmationBoxRectangle.Y -= padding / 2;
                Game1.DrawBox(
                    ____confirmationBoxRectangle.X,
                    ____confirmationBoxRectangle.Y,
                    ____confirmationBoxRectangle.Width,
                    ____confirmationBoxRectangle.Height);

                ____confirmationBoxRectangle.Width -= padding;
                ____confirmationBoxRectangle.Height -= padding;
                ____confirmationBoxRectangle.X += padding / 2;
                ____confirmationBoxRectangle.Y += padding / 2;
                b.DrawString(
                    Game1.smallFont,
                    ____confirmationText,
                    new Vector2(____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y),
                    Game1.textColor);

                __instance.yesButton.draw(b);
                __instance.noButton.draw(b);
            }
            else if (!string.IsNullOrEmpty(___hoverText))
            {
                IClickableMenu.drawHoverText(b, ___hoverText, Game1.smallFont);
            }

            __instance.drawMouse(b);

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
