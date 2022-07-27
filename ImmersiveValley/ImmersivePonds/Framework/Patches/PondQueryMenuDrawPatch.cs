// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Extensions;
using Common.Extensions.Reflection;
using Common.ModData;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using System;
using System.Reflection;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class PondQueryMenuDrawPatch : Common.Harmony.HarmonyPatch
{
    private delegate void DrawHorizontalPartitionDelegate(IClickableMenu instance, SpriteBatch b, int yPosition,
        bool small = false, int red = -1, int green = -1, int blue = -1);

    private static readonly Func<PondQueryMenu, string> _GetDisplayedText = typeof(PondQueryMenu)
        .RequireMethod("getDisplayedText").CompileUnboundDelegate<Func<PondQueryMenu, string>>();

    private static readonly Func<PondQueryMenu, string, int> _MeasureExtraTextHeight = typeof(PondQueryMenu)
        .RequireMethod("measureExtraTextHeight").CompileUnboundDelegate<Func<PondQueryMenu, string, int>>();

    private static readonly DrawHorizontalPartitionDelegate _DrawHorizontalPartition = typeof(PondQueryMenu)
        .RequireMethod("drawHorizontalPartition").CompileUnboundDelegate<DrawHorizontalPartitionDelegate>();

    private static readonly Func<FishPond, FishPondData?> _GetFishPondData = typeof(FishPond).RequireField("_fishPondData")
        .CompileUnboundFieldGetterDelegate<FishPond, FishPondData?>();

    /// <summary>Construct an instance.</summary>
    internal PondQueryMenuDrawPatch()
    {
        Target = RequireMethod<PondQueryMenu>(nameof(PondQueryMenu.draw), new[] { typeof(SpriteBatch) });
        Prefix!.priority = Priority.High;
        Prefix!.before = new[] { "DaLion.ImmersiveProfessions" };
    }

    #region harmony patches

    /// <summary>Adjust fish pond query menu for algae.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyBefore("DaLion.ImmersiveProfessions")]
    private static bool PondQueryMenuDrawPrefix(PondQueryMenu __instance, float ____age,
        Rectangle ____confirmationBoxRectangle, string ____confirmationText, bool ___confirmingEmpty,
        string ___hoverText, SObject ____fishItem, FishPond ____pond, SpriteBatch b)
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
                x: (int)(Game1.uiViewport.Width / 2 - (textSize.X + 64f) * 0.5f),
                y: __instance.yPositionOnScreen - 4 + 128, (int)(textSize.X + 64f), 64
            );
            Utility.drawTextWithShadow(
                b: b,
                text: pondNameText,
                font: Game1.smallFont,
                position: new(
                    x: Game1.uiViewport.Width / 2 - textSize.X * 0.5f,
                    y: __instance.yPositionOnScreen - 4 + 160f - textSize.Y * 0.5f), Color.Black
            );
            var displayedText = _GetDisplayedText(__instance);
            var extraHeight = 0;
            if (hasUnresolvedNeeds) extraHeight += 116;

            var extraTextHeight = _MeasureExtraTextHeight(__instance, displayedText);
            Game1.drawDialogueBox(
                x: __instance.xPositionOnScreen,
                y: __instance.yPositionOnScreen + 128,
                width: PondQueryMenu.width,
                height: PondQueryMenu.height - 128 + extraHeight + extraTextHeight,
                speaker: false,
                drawOnlyBox: true
            );
            var populationText = Game1.content.LoadString(
                PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_Population"),
                string.Concat(____pond.FishCount), ____pond.maxOccupants.Value);
            textSize = Game1.smallFont.MeasureString(populationText);
            Utility.drawTextWithShadow(
                b: b, populationText,
                font: Game1.smallFont,
                position: new(
                    x: __instance.xPositionOnScreen + PondQueryMenu.width / 2 - textSize.X * 0.5f,
                    y: __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128
                ),
                color: Game1.textColor
            );

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
                _ => 0
            };
            SObject? itemToDraw = null;
            if (isAlgaePond)
            {
                seaweedCount = ModDataIO.Read<int>(____pond, "SeaweedLivingHere");
                greenAlgaeCount = ModDataIO.Read<int>(____pond, "GreenAlgaeLivingHere");
                whiteAlgaeCount = ModDataIO.Read<int>(____pond, "WhiteAlgaeLivingHere");
            }
            else if (isLegendaryPond)
            {
                familyCount = ModDataIO.Read<int>(____pond, "FamilyLivingHere");
                itemToDraw = ____fishItem;
            }
            else
            {
                itemToDraw = ____fishItem;
            }

            for (var i = 0; i < slotsToDraw; ++i)
            {
                var yOffset = (float)Math.Sin(____age + x * 0.75f + y * 0.25f) * 2f;
                var yPos = __instance.yPositionOnScreen + (int)(yOffset * 4f) + y * slotSpacing * 4f + 275.2f;
                var xPos = __instance.xPositionOnScreen + PondQueryMenu.width / 2 -
                    slotSpacing * Math.Min(slotsToDraw, 5) * 2f + x * slotSpacing * 4f - 12f + xOffset;

                if (isAlgaePond)
                {
                    itemToDraw = seaweedCount-- > 0
                        ? new(Constants.SEAWEED_INDEX_I, 1)
                        : greenAlgaeCount-- > 0
                            ? new(Constants.GREEN_ALGAE_INDEX_I, 1)
                            : whiteAlgaeCount-- > 0
                                ? new(Constants.WHITE_ALGAE_INDEX_I, 1)
                                : null;

                    if (itemToDraw is not null)
                        itemToDraw.drawInMenu(b, new(xPos, yPos), 0.75f, 1f, 0f, StackDrawType.Hide,
                            Color.White,
                            false);
                    else
                        ____fishItem.drawInMenu(b, new(xPos, yPos), 0.75f, 0.35f, 0f, StackDrawType.Hide,
                            Color.Black,
                            false);
                }
                else if (i < ____pond.FishCount)
                {
                    if (isLegendaryPond && familyCount > 0 && i == ____pond.FishCount - familyCount)
                        itemToDraw = new(Utils.ExtendedFamilyPairs[____fishItem.ParentSheetIndex], 1);

                    itemToDraw!.drawInMenu(b, new(xPos, yPos), 0.75f, 1f, 0f, StackDrawType.Hide,
                        Color.White, false);
                }
                else
                {
                    ____fishItem.drawInMenu(b, new(xPos, yPos), 0.75f, 0.35f, 0f, StackDrawType.Hide,
                            Color.Black, false);
                }

                ++x;
                if (x != columns) continue;

                x = 0;
                ++y;
            }

            // draw stars
            if (!isAlgaePond)
            {
                var (_, numMedQuality, numHighQuality, numBestQuality) =
                    ModDataIO.Read(____pond, "FishQualities", $"{____pond.FishCount - familyCount},0,0,0")
                        .ParseTuple<int, int, int, int>()!.Value;
                if (numBestQuality + numHighQuality + numMedQuality > 0)
                {
                    x = y = 0;
                    for (var i = 0; i < ____pond.FishCount - familyCount; ++i)
                    {
                        var yOffset = (float)Math.Sin(____age + x * 0.75f + y * 0.25f) * 2f;
                        var yPos = __instance.yPositionOnScreen + (int)(yOffset * 4f) + y * slotSpacing * 4f + 270.2f;
                        var xPos = __instance.xPositionOnScreen + PondQueryMenu.width / 2 -
                            slotSpacing * Math.Min(slotsToDraw, 5) * 2f + x * slotSpacing * 4f + 4f + xOffset;

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

                        Rectangle qualityRect = quality < SObject.bestQuality
                            ? new(338 + (quality - 1) * 8, 400, 8, 8)
                            : new(346, 392, 8, 8);
                        yOffset = quality < SObject.bestQuality
                            ? 0f
                            : (float)((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) +
                                        1f) * 0.05f);
                        b.Draw(
                            texture: Game1.mouseCursors,
                            position: new(xPos, yPos + yOffset + 50f),
                            sourceRectangle: qualityRect,
                            color: Color.White,
                            rotation: 0f,
                            origin: new(4f, 4f),
                            scale: 3f * 0.75f * (1f + yOffset),
                            effects: SpriteEffects.None,
                            layerDepth: 0.9f
                        );

                        ++x;
                        if (x != columns) continue;

                        x = 0;
                        ++y;
                    }
                }

                if (familyCount > 0)
                {
                    var (_, numMedFamilyQuality, numHighFamilyQuality, numBestFamilyQuality) =
                        ModDataIO.Read(____pond, "FamilyQualities", $"{familyCount},0,0,0").ParseTuple<int, int, int, int>()!.Value;
                    if (numBestFamilyQuality + numHighFamilyQuality + numMedFamilyQuality > 0)
                    {
                        for (var i = ____pond.FishCount - familyCount; i < ____pond.FishCount; ++i)
                        {
                            var yOffset = (float)Math.Sin(____age * 1f + x * 0.75f + y * 0.25f) * 2f;
                            var yPos = __instance.yPositionOnScreen + (int)(yOffset * 4f) + y * 4 * slotSpacing +
                                       270.2f;
                            var xPos = __instance.xPositionOnScreen + PondQueryMenu.width / 2 -
                                       slotSpacing * Math.Min(slotsToDraw, 5) * 4f * 0.5f + x * slotSpacing * 4f + 4f +
                                       xOffset;

                            var quality = numBestFamilyQuality-- > 0
                                ? SObject.bestQuality
                                : numHighFamilyQuality-- > 0
                                    ? SObject.highQuality
                                    : numMedFamilyQuality-- > 0
                                        ? SObject.medQuality
                                        : SObject.lowQuality;
                            if (quality <= SObject.lowQuality) break;

                            Rectangle qualityRect = quality < SObject.bestQuality
                                ? new(338 + (quality - 1) * 8, 400, 8, 8)
                                : new(346, 392, 8, 8);
                            yOffset = quality < SObject.bestQuality
                                ? 0f
                                : (float)((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI /
                                                     512.0) +
                                            1f) * 0.05f);
                            b.Draw(
                                texture: Game1.mouseCursors,
                                position: new(xPos, yPos + yOffset + 50f),
                                sourceRectangle: qualityRect,
                                color: Color.White,
                                rotation: 0f,
                                origin: new(4f, 4f), 3f * 0.75f * (1f + yOffset),
                                effects: SpriteEffects.None,
                                layerDepth: 0.9f
                            );

                            ++x;
                            if (x != columns) continue;

                            x = 0;
                            ++y;
                        }
                    }
                }
            }

            // draw more stuff
            textSize = Game1.smallFont.MeasureString(displayedText);
            Utility.drawTextWithShadow(
                b: b,
                text: displayedText,
                font: Game1.smallFont,
                position: new(
                    x: __instance.xPositionOnScreen + PondQueryMenu.width / 2 - textSize.X * 0.5f,
                    y: __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - (hasUnresolvedNeeds ? 32 : 48) - textSize.Y
                ),
                color: Game1.textColor
            );

            if (hasUnresolvedNeeds)
            {
                _DrawHorizontalPartition(__instance, b, (int)(__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - 48f));
                Utility.drawWithShadow(
                    b: b,
                    texture: Game1.mouseCursors,
                    position: new(
                        x: __instance.xPositionOnScreen + 60 + 8f * Game1.dialogueButtonScale / 10f,
                        y: __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 28
                    ),
                    sourceRect: new(412, 495, 5, 4),
                    color: Color.White,
                    rotation: (float)Math.PI / 2f,
                    origin: Vector2.Zero
                );

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
                        LocalizedContentManager.LanguageCode.tr)
                )
                {
                    iconX = leftX - 8;
                    textX = leftX + 76;
                }

                Utility.drawTextWithShadow(
                    b: b,
                    text: bringText,
                    font: Game1.smallFont,
                    position: new(
                        x: textX,
                        y: __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 24
                    ),
                    color: Game1.textColor
                );

                b.Draw(
                    texture: Game1.objectSpriteSheet,
                    position: new(
                        x: iconX,
                        y: __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 4
                    ),
                    sourceRectangle: Game1.getSourceRectForStandardTileSheet(
                        tileSheet: Game1.objectSpriteSheet,
                        tilePosition: ____pond.neededItem.Value?.ParentSheetIndex ?? 0,
                        width: 16,
                        height: 16
                    ),
                    color: Color.Black * 0.4f,
                    rotation: 0f,
                    origin: Vector2.Zero, 4f, SpriteEffects.None, 1f
                );

                b.Draw(
                    texture: Game1.objectSpriteSheet,
                    position: new(
                        x: iconX + 4f,
                        y: __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight
                    ),
                    sourceRectangle: Game1.getSourceRectForStandardTileSheet(
                        tileSheet: Game1.objectSpriteSheet,
                        tilePosition: ____pond.neededItem.Value?.ParentSheetIndex ?? 0,
                        width: 16,
                        height: 16
                    ),
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 4f,
                    effects: SpriteEffects.None,
                    layerDepth: 1f
                );

                if (____pond.neededItemCount.Value > 1)
                    Utility.drawTinyDigits(
                        toDraw: ____pond.neededItemCount.Value,
                        b: b,
                        position: new(
                            x: iconX + 48f,
                            y: __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 48
                        ),
                        scale: 3f,
                        layerDepth: 1f,
                        c: Color.White
                    );
            }

            __instance.okButton.draw(b);
            __instance.emptyButton.draw(b);
            __instance.changeNettingButton.draw(b);
            if (___confirmingEmpty)
            {
                b.Draw(
                    texture: Game1.fadeToBlackRect,
                    destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds,
                    color: Color.Black * 0.75f
                );

                const int padding = 16;
                ____confirmationBoxRectangle.Width += padding;
                ____confirmationBoxRectangle.Height += padding;
                ____confirmationBoxRectangle.X -= padding / 2;
                ____confirmationBoxRectangle.Y -= padding / 2;
                Game1.DrawBox(
                    ____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y,
                    ____confirmationBoxRectangle.Width, ____confirmationBoxRectangle.Height
                );

                ____confirmationBoxRectangle.Width -= padding;
                ____confirmationBoxRectangle.Height -= padding;
                ____confirmationBoxRectangle.X += padding / 2;
                ____confirmationBoxRectangle.Y += padding / 2;
                b.DrawString(
                    spriteFont: Game1.smallFont,
                    text: ____confirmationText,
                    position: new(____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y),
                    color: Game1.textColor
                );

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