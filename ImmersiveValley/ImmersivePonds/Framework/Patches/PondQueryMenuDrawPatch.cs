// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;

using Common;
using Common.Data;
using Common.Extensions;
using Common.Extensions.Reflection;
using Common.Harmony;
using Extensions;

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
        .CompileUnboundFieldGetterDelegate<Func<FishPond, FishPondData?>>();

    /// <summary>Construct an instance.</summary>
    internal PondQueryMenuDrawPatch()
    {
        Target = RequireMethod<PondQueryMenu>(nameof(PondQueryMenu.draw), new[] {typeof(SpriteBatch)});
        Prefix!.before = new[] {"DaLion.ImmersiveProfessions"};
    }

    #region harmony patches

    /// <summary>Adjust fish pond query menu for algae.</summary>
    [HarmonyPrefix]
    [HarmonyAfter("DaLion.ImmersiveProfessions")]
    private static bool PondQueryMenuDrawPrefix(PondQueryMenu __instance, float ____age,
        Rectangle ____confirmationBoxRectangle, string ____confirmationText, bool ___confirmingEmpty,
        string ___hoverText, SObject ____fishItem, FishPond ____pond, SpriteBatch b)
    {
        try
        {
            bool isAlgaePond = ____fishItem.IsAlgae(), isLegendaryPond = false, hasExtendedFamily = false;
            var familyCount = 0;
            if (!isAlgaePond)
            {
                isLegendaryPond = ____fishItem.HasContextTag("fish_legendary");
                if (!isLegendaryPond)
                {
                    return true; // run original logic
                }

                familyCount = ModDataIO.ReadDataAs<int>(____pond, "FamilyLivingHere");
                hasExtendedFamily = familyCount > 0;
            }

            var owner = Game1.getFarmerMaybeOffline(____pond.owner.Value) ?? Game1.MasterPlayer;
            var isAquarist = ModEntry.ModHelper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions") &&
                             owner.professions.Contains(Farmer.pirate + 100);

            if (!Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                var hasUnresolvedNeeds = ____pond.neededItem.Value is not null && ____pond.HasUnresolvedNeeds() &&
                                         !____pond.hasCompletedRequest.Value;
                var pondNameText = isAlgaePond
                    ? ModEntry.i18n.Get("algae")
                    : Game1.content.LoadString(
                        PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_Name"),
                        ____fishItem.DisplayName);
                var textSize = Game1.smallFont.MeasureString(pondNameText);
                Game1.DrawBox((int) (Game1.uiViewport.Width / 2 - (textSize.X + 64f) * 0.5f),
                    __instance.yPositionOnScreen - 4 + 128, (int)(textSize.X + 64f), 64);
                Utility.drawTextWithShadow(b, pondNameText, Game1.smallFont,
                    new(Game1.uiViewport.Width / 2 - textSize.X * 0.5f,
                        __instance.yPositionOnScreen - 4 + 160f - textSize.Y * 0.5f), Color.Black);
                var displayedText = _GetDisplayedText(__instance);
                var extraHeight = 0;
                if (hasUnresolvedNeeds)
                    extraHeight += 116;

                var extraTextHeight = _MeasureExtraTextHeight(__instance, displayedText);
                Game1.drawDialogueBox(__instance.xPositionOnScreen, __instance.yPositionOnScreen + 128,
                    PondQueryMenu.width, PondQueryMenu.height - 128 + extraHeight + extraTextHeight, false, true);
                var populationText = Game1.content.LoadString(
                    PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_Population"),
                    string.Concat(____pond.FishCount), ____pond.maxOccupants.Value);
                textSize = Game1.smallFont.MeasureString(populationText);
                Utility.drawTextWithShadow(b, populationText, Game1.smallFont,
                    new(__instance.xPositionOnScreen + PondQueryMenu.width / 2 - textSize.X * 0.5f,
                        __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128),
                    Game1.textColor);

                int x = 0, y = 0;
                var slotsToDraw = ____pond.maxOccupants.Value;
                var slotSpacing = Constants.REGULAR_SLOT_SPACING_F;
                var unlockedMaxPopulation = false;
                if (isAquarist)
                {
                    if (isLegendaryPond)
                    {
                        slotSpacing += 1f;
                    }
                    else
                    {
                        var fishPondData = _GetFishPondData(____pond);
                        var populationGates = fishPondData?.PopulationGates;
                        if (populationGates is null || ____pond.lastUnlockedPopulationGate.Value >= populationGates.Keys.Max())
                        {
                            unlockedMaxPopulation = true;
                            slotSpacing -= 1f;
                        }
                    }
                }

                int seaweedCount = 0, greenAlgaeCount = 0, whiteAlgaeCount = 0;
                SObject? itemToDraw = null;
                if (hasExtendedFamily)
                {
                    itemToDraw = new(Framework.Utils.ExtendedFamilyPairs[____fishItem.ParentSheetIndex], 1);
                }
                else if (isAlgaePond)
                {
                    seaweedCount = ModDataIO.ReadDataAs<int>(____pond, "SeaweedLivingHere");
                    greenAlgaeCount = ModDataIO.ReadDataAs<int>(____pond, "GreenAlgaeLivingHere");
                    whiteAlgaeCount = ModDataIO.ReadDataAs<int>(____pond, "WhiteAlgaeLivingHere");
                }

                for (var i = 0; i < slotsToDraw; ++i)
                {
                    var yOffset = (float) Math.Sin(____age * 1f + x * 0.75f + y * 0.25f) * 2f;
                    var xPos = __instance.xPositionOnScreen - 20 + PondQueryMenu.width / 2 -
                        slotSpacing * Math.Min(slotsToDraw, 5) * 4f * 0.5f + slotSpacing * 4f * x + 12f;
                    var yPos = __instance.yPositionOnScreen + (int) (yOffset * 4f) + y * 4 * slotSpacing + 275.2f;
                    if (unlockedMaxPopulation) xPos -= 24f;
                    else if (isLegendaryPond) xPos += 60f;

                    if (isLegendaryPond)
                    {
                        if (i < ____pond.FishCount - familyCount)
                            ____fishItem.drawInMenu(b, new(xPos, yPos), 0.75f, 1f, 0f, StackDrawType.Hide,
                                Color.White, false);
                        else if (i < ____pond.FishCount)
                            itemToDraw!.drawInMenu(b, new(xPos, yPos), 0.75f, 1f, 0f, StackDrawType.Hide,
                                Color.White, false);
                        else
                            ____fishItem.drawInMenu(b, new(xPos, yPos), 0.75f, 0.35f, 0f, StackDrawType.Hide,
                                Color.Black, false);
                    }
                    else //if (isAlgaePond)
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

                    ++x;
                    if (x != (isLegendaryPond ? 3 : unlockedMaxPopulation ? 6 : 5)) continue;

                    x = 0;
                    ++y;
                }

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
                    _DrawHorizontalPartition(__instance, b, (int) (__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - 48f));
                    Utility.drawWithShadow(
                        b: b,
                        texture: Game1.mouseCursors,
                        position: new(
                            x: __instance.xPositionOnScreen + 60 + 8f * Game1.dialogueButtonScale / 10f,
                            y: __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 28
                        ),
                        sourceRect: new(412, 495, 5, 4),
                        color: Color.White,
                        rotation: (float) Math.PI / 2f,
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
            }

            if (___confirmingEmpty) __instance.drawMouse(b);

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    /// <summary>Draw pond fish quality stars in query menu.</summary>
    [HarmonyPostfix]
    private static void PondQueryMenuDrawPostfix(PondQueryMenu __instance, bool ___confirmingEmpty, float ____age,
        FishPond ____pond, SpriteBatch b)
    {
        if (___confirmingEmpty) return;

        var isLegendaryPond = ____pond.IsLegendaryPond();
        var familyCount = ModDataIO.ReadDataAs<int>(____pond, "FamilyLivingHere");

        var (_, numMedQuality, numHighQuality, numBestQuality) =
            ModDataIO.ReadData(____pond, "FishQualities", $"{____pond.FishCount - familyCount},0,0,0")
                .ParseTuple<int, int, int, int>();
        var (_, numMedFamilyQuality, numHighFamilyQuality, numBestFamilyQuality) =
            ModDataIO.ReadData(____pond, "FamilyQualities", $"{familyCount},0,0,0").ParseTuple<int, int, int, int>();

        if (numBestQuality + numHighQuality + numMedQuality == 0 &&
            (familyCount == 0 || numBestFamilyQuality + numHighFamilyQuality + numMedFamilyQuality == 0))
        {
            __instance.drawMouse(b);
            return;
        }

        var owner = Game1.getFarmerMaybeOffline(____pond.owner.Value) ?? Game1.MasterPlayer;
        var isAquarist = ModEntry.ModHelper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions") &&
                         owner.professions.Contains(Farmer.pirate + 100);
        float SLOT_SPACING_F, xOffset;
        if (isAquarist && ____pond.HasUnlockedFinalPopulationGate() && !isLegendaryPond)
        {
            SLOT_SPACING_F = Constants.AQUARIST_SLOT_SPACING_F;
            xOffset = Constants.AQUARIST_X_OFFSET_F;
        }
        else if (isLegendaryPond)
        {
            SLOT_SPACING_F = Constants.LEGENDARY_SLOT_SPACING_F;
            xOffset = Constants.REGULAR_SLOT_SPACING_F + Constants.LEGENDARY_X_OFFSET_F;
        }
        else
        {
            SLOT_SPACING_F = Constants.REGULAR_SLOT_SPACING_F;
            xOffset = Constants.REGULAR_X_OFFSET_F;
        }

        var totalSlots = ____pond.maxOccupants.Value;
        var slotsToDraw = ____pond.currentOccupants.Value - familyCount;
        int x = 0, y = 0;
        for (var i = 0; i < slotsToDraw; ++i)
        {
            var yOffset = (float) Math.Sin(____age * 1f + x * 0.75f + y * 0.25f) * 2f;
            var xPos = __instance.xPositionOnScreen - 20 + PondQueryMenu.width / 2 -
                SLOT_SPACING_F * Math.Min(totalSlots, 5) * 4f * 0.5f + SLOT_SPACING_F * 4f * x - 12f;
            var yPos = __instance.yPositionOnScreen + (int) (yOffset * 4f) + y * 4 * SLOT_SPACING_F + 275.2f;

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
                if (x == (isAquarist ? isLegendaryPond ? 3 : 6 : 5))
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
                : (float) ((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) +
                            1f) * 0.05f);
            b.Draw(
                texture: Game1.mouseCursors,
                position: new(xPos + xOffset, yPos + yOffset + 50f),
                sourceRectangle: qualityRect,
                color: Color.White,
                rotation: 0f,
                origin: new(4f, 4f),
                scale: 3f * 0.75f * (1f + yOffset),
                effects: SpriteEffects.None,
                layerDepth: 0.9f
            );

            ++x;
            if (x != (isAquarist ? isLegendaryPond ? 3 : 6 : 5)) continue;

            x = 0;
            ++y;
        }

        if (familyCount > 0)
        {
            slotsToDraw = familyCount;
            for (var i = 0; i < slotsToDraw; ++i)
            {
                var yOffset = (float) Math.Sin(____age * 1f + x * 0.75f + y * 0.25f) * 2f;
                var xPos = __instance.xPositionOnScreen - 20 + PondQueryMenu.width / 2 -
                    SLOT_SPACING_F * Math.Min(totalSlots, 5) * 4f * 0.5f + SLOT_SPACING_F * 4f * x - 12f;
                var yPos = __instance.yPositionOnScreen + (int) (yOffset * 4f) + y * 4 * SLOT_SPACING_F +
                           275.2f;

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
                    : (float) ((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) +
                                1f) * 0.05f);
                b.Draw(Game1.mouseCursors, new(xPos + xOffset, yPos + yOffset + 50f), qualityRect, Color.White,
                    0f, new(4f, 4f), 3f * 0.75f * (1f + yOffset), SpriteEffects.None, 0.9f);

                ++x;
                if (x != 3) continue; // at this point we know the player has the Aquarist profession

                x = 0;
                ++y;
            }

        }

        __instance.drawMouse(b);
    }

    #endregion harmony patches
}