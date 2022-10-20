// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System.Reflection;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Ponds.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley.Buildings;
using StardewValley.Menus;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;
using Utility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class PondQueryMenuDrawPatch : HarmonyPatch
{
    private static readonly Lazy<DrawHorizontalPartitionDelegate> DrawHorizontalPartition = new(() =>
        typeof(PondQueryMenu)
            .RequireMethod("drawHorizontalPartition")
            .CompileUnboundDelegate<DrawHorizontalPartitionDelegate>());

    private static readonly Lazy<Func<PondQueryMenu, string>> GetDisplayedText = new(() =>
        typeof(PondQueryMenu)
            .RequireMethod("getDisplayedText")
            .CompileUnboundDelegate<Func<PondQueryMenu, string>>());

    private static readonly Lazy<Func<PondQueryMenu, string, int>> MeasureExtraTextHeight = new(() =>
        typeof(PondQueryMenu)
            .RequireMethod("measureExtraTextHeight")
            .CompileUnboundDelegate<Func<PondQueryMenu, string, int>>());

    /// <summary>Initializes a new instance of the <see cref="PondQueryMenuDrawPatch"/> class.</summary>
    internal PondQueryMenuDrawPatch()
    {
        this.Target = this.RequireMethod<PondQueryMenu>(nameof(PondQueryMenu.draw), new[] { typeof(SpriteBatch) });
        this.Prefix!.priority = Priority.High;
        this.Prefix!.before = new[] { "DaLion.ImmersiveProfessions" };
    }

    private delegate void DrawHorizontalPartitionDelegate(
        IClickableMenu instance, SpriteBatch b, int yPosition, bool small = false, int red = -1, int green = -1, int blue = -1);

    #region harmony patches

    /// <summary>Adjust fish pond query menu for algae.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyBefore("DaLion.ImmersiveProfessions")]
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
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            var hasUnresolvedNeeds = ____pond.neededItem.Value is not null && ____pond.HasUnresolvedNeeds() &&
                                     !____pond.hasCompletedRequest.Value;
            bool isAlgaePond = ____fishItem.IsAlgae(), isLegendaryPond = ____fishItem.HasContextTag("fish_legendary");
            var pondNameText = isAlgaePond
                ? ModEntry.i18n.Get("algae")
                : Game1.content.LoadString(
                    PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_Name"),
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
            var displayedText = GetDisplayedText.Value(__instance);
            var extraHeight = 0;
            if (hasUnresolvedNeeds)
            {
                extraHeight += 116;
            }

            var extraTextHeight = MeasureExtraTextHeight.Value(__instance, displayedText);
            Game1.drawDialogueBox(
                __instance.xPositionOnScreen,
                __instance.yPositionOnScreen + 128,
                PondQueryMenu.width,
                PondQueryMenu.height - 128 + extraHeight + extraTextHeight,
                false,
                true);
            var populationText = Game1.content.LoadString(
                PathUtilities.NormalizeAssetName(
                    "Strings/UI:PondQuery_Population"),
                string.Concat(____pond.FishCount),
                ____pond.maxOccupants.Value);
            textSize = Game1.smallFont.MeasureString(populationText);
            Utility.drawTextWithShadow(
                b,
                populationText,
                Game1.smallFont,
                new Vector2(
                    __instance.xPositionOnScreen + (PondQueryMenu.width / 2) - (textSize.X * 0.5f),
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128),
                Game1.textColor);

            // draw fish
            int x = 0, y = 0, seaweedCount = 0, greenAlgaeCount = 0, whiteAlgaeCount = 0, familyCount = 0;
            var slotsToDraw = ____pond.maxOccupants.Value;
            var columns = Math.Max((int)Math.Ceiling(slotsToDraw / 2f), 2);
            var slotSpacing = 18 - columns;
            var xOffset = columns switch
            {
                7 => -52,
                6 => -20,
                4 => 36,
                3 => 70,
                2 => 76,
                _ => 0,
            };
            SObject? itemToDraw = null;
            if (isAlgaePond)
            {
                seaweedCount = ____pond.Read<int>(DataFields.SeaweedLivingHere);
                greenAlgaeCount = ____pond.Read<int>(DataFields.GreenAlgaeLivingHere);
                whiteAlgaeCount = ____pond.Read<int>(DataFields.WhiteAlgaeLivingHere);
            }
            else if (isLegendaryPond)
            {
                familyCount = ____pond.Read<int>(DataFields.FamilyLivingHere);
                itemToDraw = ____fishItem;
            }
            else
            {
                itemToDraw = ____fishItem;
            }

            for (var i = 0; i < slotsToDraw; ++i)
            {
                var yOffset = (float)Math.Sin(____age + (x * 0.75f) + (y * 0.25f)) * 2f;
                var yPos = __instance.yPositionOnScreen + (int)(yOffset * 4f) + (y * slotSpacing * 4f) + 275.2f;
                var xPos = __instance.xPositionOnScreen + (PondQueryMenu.width / 2) -
                    (slotSpacing * Math.Min(slotsToDraw, 5) * 2f) + (x * slotSpacing * 4f) - 12f + xOffset;

                if (isAlgaePond)
                {
                    itemToDraw = seaweedCount-- > 0
                        ? new SObject(Constants.SeaweedIndex, 1)
                        : greenAlgaeCount-- > 0
                            ? new SObject(Constants.GreenAlgaeIndex, 1)
                            : whiteAlgaeCount-- > 0
                                ? new SObject(Constants.WhiteAlgaeIndex, 1)
                                : null;

                    if (itemToDraw is not null)
                    {
                        itemToDraw.drawInMenu(
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
                }
                else if (i < ____pond.FishCount)
                {
                    if (isLegendaryPond && familyCount > 0 && i == ____pond.FishCount - familyCount)
                    {
                        itemToDraw = new SObject(Utils.ExtendedFamilyPairs[____fishItem.ParentSheetIndex], 1);
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

                ++x;
                if (x != columns)
                {
                    continue;
                }

                x = 0;
                ++y;
            }

            // draw stars
            if (!isAlgaePond)
            {
                var (_, numMedQuality, numHighQuality, numBestQuality) =
                    ____pond.Read(DataFields.FishQualities, $"{____pond.FishCount - familyCount},0,0,0")
                        .ParseTuple<int, int, int, int>()!.Value;
                if (numBestQuality + numHighQuality + numMedQuality > 0)
                {
                    x = y = 0;
                    for (var i = 0; i < ____pond.FishCount - familyCount; ++i)
                    {
                        var yOffset = (float)Math.Sin(____age + (x * 0.75f) + (y * 0.25f)) * 2f;
                        var yPos = __instance.yPositionOnScreen + (int)(yOffset * 4f) + (y * slotSpacing * 4f) + 270.2f;
                        var xPos = __instance.xPositionOnScreen + (PondQueryMenu.width / 2) -
                            (slotSpacing * Math.Min(slotsToDraw, 5) * 2f) + (x * slotSpacing * 4f) + 4f + xOffset;

                        var quality = numBestQuality-- > 0
                            ? SObject.bestQuality
                            : numHighQuality-- > 0
                                ? SObject.highQuality
                                : numMedQuality-- > 0
                                    ? SObject.medQuality
                                    : SObject.lowQuality;
                        if (quality <= SObject.lowQuality)
                        {
                            ++x;
                            if (x == columns)
                            {
                                x = 0;
                                ++y;
                            }

                            continue;
                        }

                        var qualityRect = quality < SObject.bestQuality
                            ? new Rectangle(338 + ((quality - 1) * 8), 400, 8, 8)
                            : new Rectangle(346, 392, 8, 8);
                        yOffset = quality < SObject.bestQuality
                            ? 0f
                            : (float)((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) +
                                       1f) * 0.05f);
                        b.Draw(
                            Game1.mouseCursors,
                            new Vector2(xPos, yPos + yOffset + 50f),
                            qualityRect,
                            Color.White,
                            0f,
                            new Vector2(4f, 4f),
                            3f * 0.75f * (1f + yOffset),
                            SpriteEffects.None,
                            0.9f);

                        ++x;
                        if (x != columns)
                        {
                            continue;
                        }

                        x = 0;
                        ++y;
                    }
                }

                if (familyCount > 0)
                {
                    var (_, numMedFamilyQuality, numHighFamilyQuality, numBestFamilyQuality) =
                        ____pond.Read(DataFields.FamilyQualities, $"{familyCount},0,0,0").ParseTuple<int, int, int, int>()!
                            .Value;
                    if (numBestFamilyQuality + numHighFamilyQuality + numMedFamilyQuality > 0)
                    {
                        for (var i = ____pond.FishCount - familyCount; i < ____pond.FishCount; ++i)
                        {
                            var yOffset = (float)Math.Sin((____age * 1f) + (x * 0.75f) + (y * 0.25f)) * 2f;
                            var yPos = __instance.yPositionOnScreen + (int)(yOffset * 4f) + (y * 4 * slotSpacing) +
                                       270.2f;
                            var xPos = __instance.xPositionOnScreen + (PondQueryMenu.width / 2) -
                                       (slotSpacing * Math.Min(slotsToDraw, 5) * 4f * 0.5f) + (x * slotSpacing * 4f) +
                                       4f +
                                       xOffset;

                            var quality = numBestFamilyQuality-- > 0
                                ? SObject.bestQuality
                                : numHighFamilyQuality-- > 0
                                    ? SObject.highQuality
                                    : numMedFamilyQuality-- > 0
                                        ? SObject.medQuality
                                        : SObject.lowQuality;
                            if (quality <= SObject.lowQuality)
                            {
                                break;
                            }

                            var qualityRect = quality < SObject.bestQuality
                                ? new Rectangle(338 + ((quality - 1) * 8), 400, 8, 8)
                                : new Rectangle(346, 392, 8, 8);
                            yOffset = quality < SObject.bestQuality
                                ? 0f
                                : (float)((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI /
                                                    512.0) +
                                           1f) * 0.05f);
                            b.Draw(
                                Game1.mouseCursors,
                                new Vector2(xPos, yPos + yOffset + 50f),
                                qualityRect,
                                Color.White,
                                0f,
                                new Vector2(4f, 4f),
                                3f * 0.75f * (1f + yOffset),
                                SpriteEffects.None,
                                0.9f);

                            ++x;
                            if (x != columns)
                            {
                                continue;
                            }

                            x = 0;
                            ++y;
                        }
                    }
                }
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
                DrawHorizontalPartition.Value(
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
                    Game1.content.LoadString(
                        PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_StatusRequest_Bring"));
                textSize = Game1.smallFont.MeasureString(bringText);
                var leftX = __instance.xPositionOnScreen + 88;
                float textX = leftX;
                var iconX = textX + textSize.X + 4f;
                if (LocalizedContentManager.CurrentLanguageCode.IsIn(
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

                b.Draw(
                    Game1.objectSpriteSheet,
                    new Vector2(
                        iconX,
                        __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 4),
                    Game1.getSourceRectForStandardTileSheet(
                        Game1.objectSpriteSheet,
                        ____pond.neededItem.Value?.ParentSheetIndex ?? 0,
                        16,
                        16),
                    Color.Black * 0.4f,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    1f);

                b.Draw(
                    Game1.objectSpriteSheet,
                    new Vector2(
                        iconX + 4f,
                        __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight),
                    Game1.getSourceRectForStandardTileSheet(
                        Game1.objectSpriteSheet,
                        ____pond.neededItem.Value?.ParentSheetIndex ?? 0,
                        16,
                        16),
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

            __instance.okButton.draw(b);
            __instance.emptyButton.draw(b);
            __instance.changeNettingButton.draw(b);
            if (___confirmingEmpty)
            {
                b.Draw(
                    Game1.fadeToBlackRect,
                    Game1.graphics.GraphicsDevice.Viewport.Bounds,
                    Color.Black * 0.75f);

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
