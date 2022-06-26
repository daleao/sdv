namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

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

using DaLion.Common;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;
using SUtility = StardewValley.Utility;

#endregion using directives

// ReSharper disable PossibleLossOfFraction
[UsedImplicitly]
internal sealed class PondQueryMenuDrawPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private const int PADDING_I = 16;
    private const float AQUARIST_SLOT_SPACING_F = 12f,
        LEGENDARY_SLOT_SPACING_F = 14f,
        LEGENDARY_X_OFFSET_F = 88f;

    private delegate void DrawHorizontalPartitionDelegate(IClickableMenu instance, SpriteBatch b, int yPosition,
        bool small = false, int red = -1, int green = -1, int blue = -1);

    private static Func<PondQueryMenu, string>? _GetDisplayedText;

    private static Func<PondQueryMenu, string, int>? _MeasureExtraTextHeight;

    private static DrawHorizontalPartitionDelegate? _DrawHorizontalPartition;

    private static Func<FishPond, FishPondData?>? _GetFishPondData;

    /// <summary>Construct an instance.</summary>
    internal PondQueryMenuDrawPatch()
    {
        Target = RequireMethod<PondQueryMenu>(nameof(PondQueryMenu.draw), new[] {typeof(SpriteBatch)});
        Prefix!.after = new[] {"DaLion.ImmersivePonds"};
    }

    #region harmony patches

    /// <summary>Patch to adjust fish pond query menu for Aquarist increased max capacity.</summary>
    [HarmonyPrefix]
    [HarmonyAfter("DaLion.ImmersivePonds")]
    private static bool PondQueryMenuDrawPrefix(PondQueryMenu __instance, float ____age,
        Rectangle ____confirmationBoxRectangle, string ____confirmationText, bool ___confirmingEmpty,
        string ___hoverText, SObject ____fishItem, FishPond ____pond, SpriteBatch b)
    {
        try
        {
            var owner = Game1.getFarmerMaybeOffline(____pond.owner.Value) ?? Game1.MasterPlayer;
            if (!owner.HasProfession(Profession.Aquarist)) return true; // run original logic

            _GetFishPondData ??= typeof(FishPond).RequireField("_fishPondData")
                .CompileUnboundFieldGetterDelegate<Func<FishPond, FishPondData?>>();
            var fishPondData = _GetFishPondData(____pond);
            var populationGates = fishPondData?.PopulationGates;
            var isLegendaryPond = ____fishItem.HasContextTag("fish_legendary");
            if (!isLegendaryPond && populationGates is not null &&
                ____pond.lastUnlockedPopulationGate.Value < populationGates.Keys.Max())
                return true; // run original logic

            if (!Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                var hasUnresolvedNeeds = ____pond.neededItem.Value is not null && ____pond.HasUnresolvedNeeds() &&
                                         !____pond.hasCompletedRequest.Value;
                var pondNameText = Game1.content.LoadString(
                    PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_Name"),
                    ____fishItem.DisplayName);
                var textSize = Game1.smallFont.MeasureString(pondNameText);
                Game1.DrawBox(
                    x: (int) (Game1.uiViewport.Width / 2 - (textSize.X + 64f) * 0.5f),
                    y: __instance.yPositionOnScreen - 4 + 128, (int) (textSize.X + 64f), 64
                );
                SUtility.drawTextWithShadow(
                    b: b,
                    text: pondNameText,
                    font: Game1.smallFont,
                    position: new(
                        x: Game1.uiViewport.Width / 2 - textSize.X * 0.5f,
                        y: __instance.yPositionOnScreen - 4 + 160f - textSize.Y * 0.5f
                    ),
                    color: Color.Black
                );
                _GetDisplayedText ??= typeof(PondQueryMenu).RequireMethod("getDisplayedText")
                    .CompileUnboundDelegate<Func<PondQueryMenu, string>>();
                var displayedText = _GetDisplayedText(__instance);
                var extraHeight = 0;
                if (hasUnresolvedNeeds)
                    extraHeight += 116;

                _MeasureExtraTextHeight ??= typeof(PondQueryMenu).RequireMethod("measureExtraTextHeight")
                    .CompileUnboundDelegate<Func<PondQueryMenu, string, int>>();
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
                SUtility.drawTextWithShadow(
                    b: b,
                    text: populationText,
                    font: Game1.smallFont,
                    position: new(
                        x: __instance.xPositionOnScreen + PondQueryMenu.width / 2 - textSize.X * 0.5f,
                        y: __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128
                    ),
                    color: Game1.textColor
                );
                
                int x = 0, y = 0;
                var slotsToDraw = ____pond.maxOccupants.Value;
                var slotSpacing = isLegendaryPond ? LEGENDARY_SLOT_SPACING_F : AQUARIST_SLOT_SPACING_F;
                for (var i = 0; i < slotsToDraw; ++i)
                {
                    var yOffset = (float) Math.Sin(____age * 1f + x * 0.75f + y * 0.25f) * 2f;
                    var xPos = __instance.xPositionOnScreen - 20 + PondQueryMenu.width / 2 -
                        slotSpacing * Math.Min(slotsToDraw, 5) * 4f * 0.5f + slotSpacing * 4f * x - 12f;
                    if (isLegendaryPond) xPos += LEGENDARY_X_OFFSET_F;
                    var yPos = __instance.yPositionOnScreen + (int) (yOffset * 4f) + y * 4 * slotSpacing + 275.2f;

                    if (i < ____pond.FishCount)
                        ____fishItem.drawInMenu(b, new(xPos, yPos), 0.75f, 1f, 0f, StackDrawType.Hide, Color.White,
                            false);
                    else
                        ____fishItem.drawInMenu(b, new(xPos, yPos), 0.75f, 0.35f, 0f, StackDrawType.Hide, Color.Black,
                            false);

                    ++x;
                    if (x != (isLegendaryPond ? 3 : 6)) continue;

                    x = 0;
                    ++y;
                }

                textSize = Game1.smallFont.MeasureString(displayedText);
                SUtility.drawTextWithShadow(
                    b: b, displayedText,
                    font: Game1.smallFont,
                    position: new(
                        x: __instance.xPositionOnScreen + PondQueryMenu.width / 2 - textSize.X * 0.5f,
                        y: __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight -
                           (hasUnresolvedNeeds ? 32 : 48) - textSize.Y
                    ),
                    color: Game1.textColor
                );
                if (hasUnresolvedNeeds)
                {
                    _DrawHorizontalPartition ??= typeof(PondQueryMenu).RequireMethod("drawHorizontalPartition")
                        .CompileUnboundDelegate<DrawHorizontalPartitionDelegate>();
                    _DrawHorizontalPartition(__instance, b,
                        (int) (__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - 48f));
                    SUtility.drawWithShadow(
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

                    SUtility.drawTextWithShadow(
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
                        origin: Vector2.Zero,
                        scale: 4f,
                        effects: SpriteEffects.None,
                        layerDepth: 1f
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
                        SUtility.drawTinyDigits(
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
                    ____confirmationBoxRectangle.Width += PADDING_I;
                    ____confirmationBoxRectangle.Height += PADDING_I;
                    ____confirmationBoxRectangle.X -= PADDING_I / 2;
                    ____confirmationBoxRectangle.Y -= PADDING_I / 2;
                    Game1.DrawBox(
                        ____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y,
                        ____confirmationBoxRectangle.Width, ____confirmationBoxRectangle.Height
                    );
                    ____confirmationBoxRectangle.Width -= PADDING_I;
                    ____confirmationBoxRectangle.Height -= PADDING_I;
                    ____confirmationBoxRectangle.X += PADDING_I / 2;
                    ____confirmationBoxRectangle.Y += PADDING_I / 2;
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