// ReSharper disable PossibleLossOfFraction
#nullable enable
namespace DaLion.Stardew.FishPonds.Framework;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using StardewValley.Tools;

using Common.Extensions;
using Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;
using SUtility = StardewValley.Utility;

#endregion using directives

/// <summary>Patches the game code to implement modded tool behavior.</summary>
[UsedImplicitly]
internal static class HarmonyPatcher
{
    private static readonly FieldInfo _FishPondData = typeof(FishPond).Field("_fishPondData");
    private static readonly MethodInfo _CalculateBobberTile = typeof(FishingRod).MethodNamed("calculateBobberTile");
    private static readonly MethodInfo _GetDisplayedText = typeof(PondQueryMenu).MethodNamed("getDisplayedText");
    private static readonly MethodInfo _MeasureExtraTextHeight = typeof(PondQueryMenu).MethodNamed("measureExtraTextHeight");
    private static readonly MethodInfo _DrawHorizontalPartition = typeof(PondQueryMenu).MethodNamed("drawHorizontalPartition");

    #region harmony patches 

    /// <summary>Decrement total Fish Pond quality rating.</summary>
    [HarmonyPatch(typeof(FishingRod), nameof(FishingRod.pullFishFromWater))]
    internal class FishingRodPullFishFromWaterPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(FishingRod __instance, ref int whichFish, ref int fishQuality, bool fromFishPond)
        {
            if (!fromFishPond || whichFish.IsTrash()) return true; // run original logic

            var (x, y) = (Vector2) _CalculateBobberTile.Invoke(__instance, null)!;
            var pond = Game1.getFarm().buildings.OfType<FishPond>().FirstOrDefault(p =>
                x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
                y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
            if (pond is null) return true; // run original logic

            var qualityRating = pond.ReadDataAs<int>("QualityRating");
            var lowestQuality = pond.GetLowestFishQuality(qualityRating);
            if (pond.IsLegendaryPond())
            {
                var familyCount = pond.ReadDataAs<int>("FamilyLivingHere");
                if (familyCount > 0)
                {
                    var familyQualityRating = pond.ReadDataAs<int>("FamilyQualityRating");
                    var lowestFamilyQuality = pond.GetLowestFishQuality(familyQualityRating, true);
                    if (lowestFamilyQuality < lowestQuality)
                    {
                        familyQualityRating -= (int) Math.Pow(16, lowestFamilyQuality == SObject.bestQuality ? lowestFamilyQuality - 1 : lowestFamilyQuality);
                        fishQuality = lowestFamilyQuality;
                        whichFish = Utility.ExtendedFamilyPairs[whichFish];
                        pond.WriteData("FamilyQualityRating", familyQualityRating.ToString());
                        pond.IncrementData("FamilyLivingHere", -1);
                    }
                    else
                    {
                        qualityRating -= (int) Math.Pow(16, lowestQuality == SObject.bestQuality ? lowestQuality - 1 : lowestQuality);
                        pond.WriteData("QualityRating", qualityRating.ToString());
                        fishQuality = lowestQuality;
                    }
                }
                else
                {
                    qualityRating -= (int) Math.Pow(16, lowestQuality == SObject.bestQuality ? lowestQuality - 1 : lowestQuality);
                    pond.WriteData("QualityRating", qualityRating.ToString());
                    fishQuality = lowestQuality;
                }
            }
            else
            {
                qualityRating -= (int)Math.Pow(16, lowestQuality == SObject.bestQuality ? lowestQuality - 1 : lowestQuality);
                pond.WriteData("QualityRating", qualityRating.ToString());
                fishQuality = lowestQuality;
            }

            return true; // run original logic
        }
    }

    [HarmonyPatch(typeof(FishPond), "addFishToPond")]
    internal class FishPondAddFishToPond
    {
        /// <summary>Distinguish extended family pairs + increment total Fish Pond quality rating.</summary>
        [HarmonyPostfix]
        private static void Postfix(FishPond __instance, SObject fish)
        {
            if (fish.HasContextTag("fish_legendary") && fish.ParentSheetIndex != __instance.fishType.Value)
            {
                __instance.IncrementData<int>("FamilyLivingHere");

                var familyQualityRating = __instance.ReadDataAs<int>("FamilyQualityRating");
                familyQualityRating += (int) Math.Pow(16, fish.Quality == SObject.bestQuality ? fish.Quality - 1 : fish.Quality);
                __instance.WriteData("FamilyQualityRating", familyQualityRating.ToString());
            }
            else if (fish.IsAlgae())
            {
                switch (fish.ParentSheetIndex)
                {
                    case Constants.SEAWEED_INDEX_I:
                        __instance.IncrementData<int>("SeaweedLivingHere");
                        break;
                    case Constants.GREEN_ALGAE_INDEX_I:
                        __instance.IncrementData<int>("GreenAlgaeLivingHere");
                        break;
                    case Constants.WHITE_ALGAE_INDEX_I:
                        __instance.IncrementData<int>("WhiteAlgaeLivingHere");
                        break;
                }
            }
            else
            {
                var qualityRating = __instance.ReadDataAs<int>("QualityRating");
                qualityRating += (int) Math.Pow(16, fish.Quality == SObject.bestQuality ? fish.Quality - 1 : fish.Quality);
                __instance.WriteData("QualityRating", qualityRating.ToString());
            }
        }
    }

    [HarmonyPatch(typeof(FishPond), nameof(FishPond.dayUpdate))]
    internal class FishPondDayUpdate
    {
        /// <summary>Boost roe production for everybody.</summary>
        [HarmonyPostfix]
        private static void Postfix(FishPond __instance, ref FishPondData? ____fishPondData)
        {
            if (__instance.currentOccupants.Value == 0)
            {
                __instance.IncrementData<int>("DaysEmpty");
                if (__instance.ReadDataAs<int>("DaysEmpty") < 3) return;

                var r = new Random(Guid.NewGuid().GetHashCode());
                var spawned = r.NextDouble() > 0.25 ? r.Next(152, 154) : 157;
                __instance.fishType.Value = spawned;
                ____fishPondData = null;
                __instance.UpdateMaximumOccupancy();
                ++__instance.currentOccupants.Value;

                switch (spawned)
                {
                    case Constants.SEAWEED_INDEX_I:
                        __instance.IncrementData<int>("SeaweedLivingHere");
                        break;
                    case Constants.GREEN_ALGAE_INDEX_I:
                        __instance.IncrementData<int>("GreenAlgaeLivingHere");
                        break;
                    case Constants.WHITE_ALGAE_INDEX_I:
                        __instance.IncrementData<int>("WhiteAlgaeLivingHere");
                        break;
                }

                __instance.WriteData("DaysEmpty", 0.ToString());
            }
            else
            {
                __instance.GetIndividualFishProduce();
            }
        }

        /// <summary>Replaces single production with individual fish production.</summary>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            /// From: Random r = {...} output.Value = GetFishProduce(r);
            /// To: this.GetIndividualFishProduce();

            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.stats)))
                    )
                    .RemoveUntil(
                        new CodeInstruction(OpCodes.Callvirt, typeof(NetRef<Item>).PropertySetter(nameof(NetRef<Item>.Value)))
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call,
                            typeof(FishPondExtensions).MethodNamed(nameof(FishPondExtensions.GetIndividualFishProduce)))
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed while adding individual fish produce.\nHelper returned {ex}");
#pragma warning disable CS8603
                return null;
#pragma warning restore CS8603
            }

            return helper.Flush();
        }
    }

    [HarmonyPatch(typeof(FishPond), nameof(FishPond.doAction))]
    internal class FishPondDoActionPatch
    {
        /// <summary>Inject ItemGrabMenu + allow legendary fish to share a pond with their extended families.</summary>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            /// From: if (output.Value != null) {...} return true;
            /// To: if (output.Value != null) return this.OpenChumBucketMenu();

            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).Field(nameof(FishPond.output))),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(NetRef<Item>).PropertyGetter(nameof(NetRef<Item>.Value))),
                        new CodeInstruction(OpCodes.Stloc_1)
                    )
                    .Retreat()
                    .SetOpCode(OpCodes.Brfalse_S)
                    .Advance()
                    .RemoveUntil(
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Ret)
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call,
                            typeof(FishPondExtensions).MethodNamed(nameof(FishPondExtensions.OpenChumBucketMenu)))
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed while adding chum bucket menu.\nHelper returned {ex}");
#pragma warning disable CS8603
                return null;
#pragma warning restore CS8603
            }

            /// From: if (who.ActiveObject.ParentSheetIndex != (int) fishType)
            /// To: if (who.ActiveObject.ParentSheetIndex != (int) fishType && !IsExtendedFamily(who.ActiveObject.ParentSheetIndex, (int) fishType)

            try
            {
                helper
                    .FindNext(
                        new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).Field(nameof(FishPond.fishType))),
                        new CodeInstruction(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).MethodNamed("op_Implicit")),
                        new CodeInstruction(OpCodes.Beq)
                    )
                    .RetreatUntil(
                        new CodeInstruction(OpCodes.Ldloc_0)
                    )
                    .GetInstructionsUntil(out var got, true, true,
                        new CodeInstruction(OpCodes.Beq)
                    )
                    .Insert(got)
                    .Retreat()
                    .Insert(
                        new CodeInstruction(OpCodes.Call,
                            typeof(Utility).MethodNamed(nameof(Utility.IsExtendedFamilyMember)))
                    )
                    .SetOpCode(OpCodes.Brtrue_S);
            }
            catch (Exception ex)
            {
                Log.E($"Failed while adding family ties to legendary fish in ponds.\nHelper returned {ex}");
#pragma warning disable CS8603
                return null;
#pragma warning restore CS8603
            }

            return helper.Flush();
        }
    }

    [HarmonyPatch(typeof(FishPond), "doFishSpecificWaterColoring")]
    internal class FishPondDoFishSpecificWaterColoring
    {
        /// <summary>Recolor for Mutant/Radioactive Carps and Algae/Seaweed.</summary>
        [HarmonyPostfix]
        private static void Postfix(FishPond __instance)
        {
            if (__instance.fishType.Value.IsAlgae() && __instance.currentOccupants.Value > 2)
                __instance.overrideWaterColor.Value = new(142, 168, 48);
        }
    }

    [HarmonyPatch(typeof(FishPond), nameof(FishPond.JumpFish))]
    internal class FishPondJumpFishPatch
    {
        /// <summary>Prevent jumping algae.</summary>
        [HarmonyPrefix]
        private static bool Prefix(FishPond __instance, ref bool __result)
        {
            if (!__instance.fishType.Value.IsAlgae()) return true; // run original logic

            __result = false;
            return false; // don't run original logic
        }
    }

    [HarmonyPatch(typeof(FishPond), nameof(FishPond.OnFishTypeChanged))]
    internal class FishPondOnFishTypeChangedPatch
    {
        /// <summary>Reset total Fish Pond quality rating.</summary>
        [HarmonyPostfix]
        private static void Postfix(FishPond __instance)
        {
            __instance.WriteData("QualityRating", null);
            __instance.WriteData("FamilyQualityRating", null);
            __instance.WriteData("FamilyLivingHere", null);
            __instance.WriteData("DaysEmpty", 0.ToString());
            __instance.WriteData("SeaweedLivingHere", null);
            __instance.WriteData("GreenAlgaeLivingHere", null);
            __instance.WriteData("WhiteAlgaeLivingHere", null);
        }
    }

    [HarmonyPatch(typeof(FishPond), nameof(FishPond.SpawnFish))]
    internal class FishPondSpawnFishPatch
    {
        /// <summary>Set the quality of newborn fishes.</summary>
        [HarmonyPostfix]
        private static void Postfix(FishPond __instance)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            if (__instance.fishType.Value.IsAlgae())
            {
                var spawned = r.NextDouble() > 0.25 ? r.Next(Constants.SEAWEED_INDEX_I, Constants.GREEN_ALGAE_INDEX_I + 1) : 157;
                switch (spawned)
                {
                    case Constants.SEAWEED_INDEX_I:
                        __instance.IncrementData<int>("SeaweedLivingHere");
                        break;
                    case Constants.GREEN_ALGAE_INDEX_I:
                        __instance.IncrementData<int>("GreenAlgaeLivingHere");
                        break;
                    case Constants.WHITE_ALGAE_INDEX_I:
                        __instance.IncrementData<int>("WhiteAlgaeLivingHere");
                        break;
                }
                return;
            }

            var forFamily = false;
            if (__instance.IsLegendaryPond())
            {
                var familyCount = __instance.ReadDataAs<int>("FamilyLivingHere");
                if (familyCount > 0 && Game1.random.NextDouble() < (double)familyCount / __instance.FishCount)
                    forFamily = true;
            }

            var qualityRating = __instance.ReadDataAs<int>(forFamily ? "FamilyQualityRating" : "QualityRating");
            var (numBestQuality, numHighQuality, numMedQuality) = __instance.GetFishQualities(qualityRating);
            if (numBestQuality == 0 && numHighQuality == 0 && numMedQuality == 0)
            {
                __instance.WriteData(forFamily ? "FamilyQualityRating" : "QualityRating", (++qualityRating).ToString());
                return;
            }

            var roll = r.Next(__instance.FishCount - 1); // fish pond count has already been incremented at this point, so we consider -1;
            var fishlingQuality = roll < numBestQuality
                ? SObject.bestQuality
                : roll < numBestQuality + numHighQuality
                    ? SObject.highQuality
                    : roll < numBestQuality + numHighQuality + numMedQuality
                        ? SObject.medQuality
                        : SObject.lowQuality;

            qualityRating += (int) Math.Pow(16,
                fishlingQuality == SObject.bestQuality ? fishlingQuality - 1 : fishlingQuality);
            __instance.WriteData(forFamily ? "FamilyQualityRating" : "QualityRating", qualityRating.ToString());
        }
    }

    [HarmonyPatch(typeof(PondQueryMenu), nameof(PondQueryMenu.draw))]
    internal class PondQueryMenuDrawPatch
    {
        /// <summary>Adjust fish pond query menu for algae.</summary>
        [HarmonyPrefix]
        private static bool Prefix(PondQueryMenu __instance, float ____age,
            Rectangle ____confirmationBoxRectangle, string ____confirmationText, bool ___confirmingEmpty,
            string ___hoverText, SObject ____fishItem, FishPond ____pond, SpriteBatch b)
        {
            try
            {
                var isAlgaePond = ____fishItem.IsAlgae();
                var familyCount = ____pond.ReadDataAs<int>("FamilyLivingHere");
                var hasExtendedFamily =  familyCount > 0;
                if (!isAlgaePond && !hasExtendedFamily) return true; // run original logic

                var owner = Game1.getFarmerMaybeOffline(____pond.owner.Value) ?? Game1.MasterPlayer;
                var isAquarist = ModEntry.ModHelper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions") &&
                                 owner.professions.Contains(Farmer.pirate + 100);

                if (!Game1.globalFade)
                {
                    b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                    var hasUnresolvedNeeds = ____pond.neededItem.Value is not null && ____pond.HasUnresolvedNeeds() &&
                                             !____pond.hasCompletedRequest.Value;
                    var pondNameText = Game1.content.LoadString(
                        PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_Name"),
                        ____fishItem.DisplayName);
                    var textSize = Game1.smallFont.MeasureString(pondNameText);
                    Game1.DrawBox((int) (Game1.uiViewport.Width / 2 - (textSize.X + 64f) * 0.5f),
                        __instance.yPositionOnScreen - 4 + 128, (int) (textSize.X + 64f), 64);
                    SUtility.drawTextWithShadow(b, pondNameText, Game1.smallFont,
                        new(Game1.uiViewport.Width / 2 - textSize.X * 0.5f,
                            __instance.yPositionOnScreen - 4 + 160f - textSize.Y * 0.5f), Color.Black);
                    var displayedText = (string) _GetDisplayedText.Invoke(__instance, null)!;
                    var extraHeight = 0;
                    if (hasUnresolvedNeeds)
                        extraHeight += 116;

                    var extraTextHeight = (int) _MeasureExtraTextHeight.Invoke(__instance, new object?[] { displayedText })!;
                    Game1.drawDialogueBox(__instance.xPositionOnScreen, __instance.yPositionOnScreen + 128,
                        PondQueryMenu.width, PondQueryMenu.height - 128 + extraHeight + extraTextHeight, false, true);
                    var populationText = Game1.content.LoadString(
                        PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_Population"),
                        string.Concat(____pond.FishCount), ____pond.maxOccupants.Value);
                    textSize = Game1.smallFont.MeasureString(populationText);
                    SUtility.drawTextWithShadow(b, populationText, Game1.smallFont,
                        new(__instance.xPositionOnScreen + PondQueryMenu.width / 2 - textSize.X * 0.5f,
                            __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128),
                        Game1.textColor);

                    var slotsToDraw = ____pond.maxOccupants.Value;
                    var x = 0;
                    var y = 0;
                    var slotSpacing = Constants.REGULAR_SLOT_SPACING_F;
                    var unlockedMaxPopulation = false;
                    if (isAquarist)
                    {
                        if (hasExtendedFamily)
                        {
                            slotSpacing += 1f;
                        }
                        else
                        {
                            var fishPondData = (FishPondData?) _FishPondData.GetValue(____pond);
                            var populationGates = fishPondData?.PopulationGates;
                            if (populationGates is not null &&
                                ____pond.lastUnlockedPopulationGate.Value > populationGates.Keys.Max())
                            {
                                unlockedMaxPopulation = true;
                                slotSpacing -= 1f;
                            }
                            else if (populationGates is null)
                            {
                                unlockedMaxPopulation = true;
                            }
                        }
                    }

                    var seaweedCount = 0;
                    var greenAlgaeCount = 0;
                    var whiteAlgaeCount = 0;
                    SObject? itemToDraw = null;
                    if (hasExtendedFamily)
                    {
                        itemToDraw = new(Utility.ExtendedFamilyPairs[____fishItem.ParentSheetIndex], 1);
                    }
                    else if (isAlgaePond)
                    {
                        seaweedCount = ____pond.ReadDataAs<int>("SeaweedLivingHere");
                        greenAlgaeCount = ____pond.ReadDataAs<int>("GreenAlgaeLivingHere");
                        whiteAlgaeCount = ____pond.ReadDataAs<int>("WhiteAlgaeLivingHere");
                    }

                    for (var i = 0; i < slotsToDraw; ++i)
                    {
                        var yOffset = (float) Math.Sin(____age * 1f + x * 0.75f + y * 0.25f) * 2f;
                        var xPos = __instance.xPositionOnScreen - 20 + PondQueryMenu.width / 2 -
                            slotSpacing * Math.Min(slotsToDraw, 5) * 4f * 0.5f + slotSpacing * 4f * x + 12f;
                        var yPos = __instance.yPositionOnScreen + (int) (yOffset * 4f) + y * 4 * slotSpacing + 275.2f;
                        if (unlockedMaxPopulation) xPos -= 24f;
                        else if (hasExtendedFamily) xPos += 60f;

                        if (hasExtendedFamily)
                        {
                            if (i < ____pond.FishCount - familyCount)
                                ____fishItem.drawInMenu(b, new(xPos, yPos), 0.75f, 1f, 0f, StackDrawType.Hide, Color.White, false);
                            else if (i < ____pond.FishCount)
                                itemToDraw!.drawInMenu(b, new(xPos, yPos), 0.75f, 1f, 0f, StackDrawType.Hide, Color.White, false);
                            else
                                ____fishItem.drawInMenu(b, new(xPos, yPos), 0.75f, 0.35f, 0f, StackDrawType.Hide, Color.Black, false);
                        }
                        else if (isAlgaePond)
                        {
                            itemToDraw = seaweedCount-- > 0
                                ? new(Constants.SEAWEED_INDEX_I, 1)
                                : greenAlgaeCount-- > 0
                                    ? new(Constants.GREEN_ALGAE_INDEX_I, 1)
                                    : whiteAlgaeCount-- > 0
                                        ? new(Constants.WHITE_ALGAE_INDEX_I, 1)
                                        : null;

                            if (itemToDraw is not null)
                                itemToDraw.drawInMenu(b, new(xPos, yPos), 0.75f, 1f, 0f, StackDrawType.Hide, Color.White,
                                    false);
                            else
                                ____fishItem.drawInMenu(b, new(xPos, yPos), 0.75f, 0.35f, 0f, StackDrawType.Hide, Color.Black,
                                    false);
                        }

                        ++x;
                        if (x != (hasExtendedFamily ? 3 : unlockedMaxPopulation ? 6 : 5)) continue;

                        x = 0;
                        ++y;
                    }

                    textSize = Game1.smallFont.MeasureString(displayedText);
                    SUtility.drawTextWithShadow(b, displayedText, Game1.smallFont,
                        new(__instance.xPositionOnScreen + PondQueryMenu.width / 2 - textSize.X * 0.5f,
                            __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight -
                            (hasUnresolvedNeeds ? 32 : 48) - textSize.Y), Game1.textColor);
                    if (hasUnresolvedNeeds)
                    {
                        _DrawHorizontalPartition.Invoke(__instance, new object?[]
                        {
                        b, (int) (__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - 48f), false,
                        -1, -1, -1
                        });
                        SUtility.drawWithShadow(b, Game1.mouseCursors,
                            new(__instance.xPositionOnScreen + 60 + 8f * Game1.dialogueButtonScale / 10f,
                                __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 28),
                            new(412, 495, 5, 4), Color.White, (float)Math.PI / 2f, Vector2.Zero);
                        var bringText =
                            Game1.content.LoadString(
                                PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_StatusRequest_Bring"));
                        textSize = Game1.smallFont.MeasureString(bringText);
                        var leftX = __instance.xPositionOnScreen + 88;
                        float textX = leftX;
                        var iconX = textX + textSize.X + 4f;
                        if (LocalizedContentManager.CurrentLanguageCode.IsAnyOf(LocalizedContentManager.LanguageCode.ja,
                                LocalizedContentManager.LanguageCode.ko, LocalizedContentManager.LanguageCode.tr))
                        {
                            iconX = leftX - 8;
                            textX = leftX + 76;
                        }

                        SUtility.drawTextWithShadow(b, bringText, Game1.smallFont,
                            new(textX,
                                __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 24),
                            Game1.textColor);
                        b.Draw(Game1.objectSpriteSheet,
                            new(iconX,
                                __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 4),
                            Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                                ____pond.neededItem.Value?.ParentSheetIndex ?? 0, 16, 16), Color.Black * 0.4f, 0f,
                            Vector2.Zero, 4f, SpriteEffects.None, 1f);
                        b.Draw(Game1.objectSpriteSheet,
                            new(iconX + 4f,
                                __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight),
                            Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                                ____pond.neededItem.Value?.ParentSheetIndex ?? 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f,
                            SpriteEffects.None, 1f);
                        if (____pond.neededItemCount.Value > 1)
                            SUtility.drawTinyDigits(____pond.neededItemCount.Value, b,
                                new(iconX + 48f,
                                    __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 48), 3f, 1f,
                                Color.White);
                    }

                    __instance.okButton.draw(b);
                    __instance.emptyButton.draw(b);
                    __instance.changeNettingButton.draw(b);
                    if (___confirmingEmpty)
                    {
                        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds,
                            Color.Black * 0.75f);
                        var padding = 16;
                        ____confirmationBoxRectangle.Width += padding;
                        ____confirmationBoxRectangle.Height += padding;
                        ____confirmationBoxRectangle.X -= padding / 2;
                        ____confirmationBoxRectangle.Y -= padding / 2;
                        Game1.DrawBox(____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y,
                            ____confirmationBoxRectangle.Width, ____confirmationBoxRectangle.Height);
                        ____confirmationBoxRectangle.Width -= padding;
                        ____confirmationBoxRectangle.Height -= padding;
                        ____confirmationBoxRectangle.X += padding / 2;
                        ____confirmationBoxRectangle.Y += padding / 2;
                        b.DrawString(Game1.smallFont, ____confirmationText,
                            new(____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y),
                            Game1.textColor);
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
        private static void Postfix(PondQueryMenu __instance, bool ___confirmingEmpty, float ____age,
            SObject ____fishItem, FishPond ____pond, SpriteBatch b)
        {
            if (___confirmingEmpty) return;

            var isLegendaryPond = ____pond.IsLegendaryPond();
            var familyCount = ____pond.ReadDataAs<int>("FamilyLivingHere");

            var (numBestQuality, numHighQuality, numMedQuality) = ____pond.GetFishQualities();
            var (numBestFamilyQuality, numHighFamilyQuality, numMedFamilyQuality) =
                ____pond.GetFishQualities(forFamily: true);
            if (numBestQuality == 0 && numHighQuality == 0 && numMedQuality == 0 && (familyCount == 0 ||
                    numBestFamilyQuality == 0 && numHighFamilyQuality == 0 && numMedFamilyQuality == 0))
            {
                __instance.drawMouse(b);
                return;
            }

            var owner = Game1.getFarmerMaybeOffline(____pond.owner.Value) ?? Game1.MasterPlayer;
            var isAquarist = owner.professions.Contains(Farmer.pirate + 100);
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
                b.Draw(Game1.mouseCursors, new(xPos + xOffset, yPos + yOffset + 50f), qualityRect, Color.White,
                    0f, new(4f, 4f), 3f * 0.75f * (1f + yOffset), SpriteEffects.None, 0.9f);

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
                    var yPos = __instance.yPositionOnScreen + (int) (yOffset * 4f) + y * 4 * SLOT_SPACING_F + 275.2f;

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
    }

    #endregion harmony patches
}