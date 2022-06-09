// ReSharper disable PossibleLossOfFraction
#nullable enable
namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.IO;
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
using StardewValley.Objects;
using StardewValley.Tools;

using Common.Extensions;
using Common.Extensions.Reflection;
using Common.Extensions.Xna;
using Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;
using SUtility = StardewValley.Utility;

#endregion using directives

/// <summary>Patches the game code to implement modded pond behavior.</summary>
[UsedImplicitly]
internal static class Patches
{
    private static readonly FieldInfo _FishPondData = typeof(FishPond).RequireField("_fishPondData")!;
    private static readonly MethodInfo _CalculateBobberTile = typeof(FishingRod).RequireMethod("calculateBobberTile");

    private static readonly Func<PondQueryMenu, string> _GetDisplayedText =
        (Func<PondQueryMenu, string>) Delegate.CreateDelegate(typeof(Func<PondQueryMenu, string>),
            typeof(PondQueryMenu).RequireMethod("getDisplayedText"));

    private static readonly Func<PondQueryMenu, string, int> _MeasureExtraTextHeight =
        (Func<PondQueryMenu, string, int>) Delegate.CreateDelegate(typeof(Func<PondQueryMenu, string, int>),
            typeof(PondQueryMenu).RequireMethod("measureExtraTextHeight"));

    private static readonly Action<PondQueryMenu, SpriteBatch, int, bool, int, int, int> _DrawHorizontalPartition =
        (Action<PondQueryMenu, SpriteBatch, int, bool, int, int, int>) Delegate.CreateDelegate(
            typeof(Action<PondQueryMenu, SpriteBatch, int, bool, int, int, int>),
            typeof(PondQueryMenu).RequireMethod("drawHorizontalPartition"));

    #region harmony patches

    [HarmonyPatch(typeof(FishingRod), nameof(FishingRod.pullFishFromWater))]
    internal class FishingRodPullFishFromWaterPatch
    {
        /// <summary>Decrement total Fish Pond quality ratings.</summary>
        [HarmonyPrefix]
        protected static void Prefix(FishingRod __instance, ref int whichFish, ref int fishQuality, bool fromFishPond)
        {
            if (!fromFishPond || whichFish.IsTrash()) return;

            var (x, y) = (Vector2) _CalculateBobberTile.Invoke(__instance, null)!;
            var pond = Game1.getFarm().buildings.OfType<FishPond>().FirstOrDefault(p =>
                x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
                y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
            if (pond is null) return;

            if (pond.IsAlgaePond())
            {
                fishQuality = SObject.lowQuality;

                var seaweedCount = pond.ReadDataAs<int>("SeaweedLivingHere");
                var greenAlgaeCount = pond.ReadDataAs<int>("GreenAlgaeLivingHere");
                var whiteAlgaeCount = pond.ReadDataAs<int>("WhiteAlgaeLivingHere");

                var roll = Game1.random.Next(seaweedCount + greenAlgaeCount + whiteAlgaeCount);
                if (roll < seaweedCount)
                {
                    whichFish = Constants.SEAWEED_INDEX_I;
                    pond.WriteData("SeaweedLivingHere", (--seaweedCount).ToString());
                }
                else if (roll < seaweedCount + greenAlgaeCount)
                {
                    whichFish = Constants.GREEN_ALGAE_INDEX_I;
                    pond.WriteData("GreenAlgaeLivingHere", (--greenAlgaeCount).ToString());
                }
                else if (roll < seaweedCount + greenAlgaeCount + whiteAlgaeCount)
                {
                    whichFish = Constants.WHITE_ALGAE_INDEX_I;
                    pond.WriteData("WhiteAlgaeLivingHere", (--whiteAlgaeCount).ToString());
                }

                return;
            }

            try
            {
                var fishQualities = pond.ReadData("FishQualities",
                    $"{pond.FishCount - pond.ReadDataAs<int>("FamilyLivingHere")},0,0,0").ParseList<int>()!;
                if (fishQualities.Count != 4 || fishQualities.Any(q => 0 > q || q > pond.FishCount + 1)) // FishCount has already been decremented at this point, so we increment 1 to compensate
                    throw new InvalidDataException("FishQualities data had incorrect number of values.");

                var lowestFish = fishQualities.FindIndex(i => i > 0);
                if (pond.IsLegendaryPond())
                {
                    var familyCount = pond.ReadDataAs<int>("FamilyLivingHere");
                    if (fishQualities.Sum() + familyCount != pond.FishCount + 1) // FishCount has already been decremented at this point, so we increment 1 to compensate
                        throw new InvalidDataException("FamilyLivingHere data is invalid.");

                    if (familyCount > 0)
                    {
                        var familyQualities =
                            pond.ReadData("FamilyQualities", $"{pond.ReadDataAs<int>("FamilyLivingHere")},0,0,0")
                                .ParseList<int>()!;
                        if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
                            throw new InvalidDataException("FamilyQualities data had incorrect number of values.");

                        var lowestFamily = familyQualities.FindIndex(i => i > 0);
                        if (lowestFamily < lowestFish || lowestFamily == lowestFish && Game1.random.NextDouble() < 0.5)
                        {
                            whichFish = Framework.Utility.ExtendedFamilyPairs[whichFish];
                            fishQuality = lowestFamily == 3 ? 4 : lowestFamily;
                            --familyQualities[lowestFamily];
                            pond.WriteData("FamilyQualities", string.Join(",", familyQualities));
                            pond.IncrementData("FamilyLivingHere", -1);
                        }
                        else
                        {
                            fishQuality = lowestFish == 3 ? 4 : lowestFish;
                            --fishQualities[lowestFish];
                            pond.WriteData("FishQualities", string.Join(",", fishQualities));
                        }
                    }
                    else
                    {
                        fishQuality = lowestFish == 3 ? 4 : lowestFish;
                        --fishQualities[lowestFish];
                        pond.WriteData("FishQualities", string.Join(",", fishQualities));
                    }
                }
                else
                {
                    fishQuality = lowestFish == 3 ? 4 : lowestFish;
                    --fishQualities[lowestFish];
                    pond.WriteData("FishQualities", string.Join(",", fishQualities));
                }
            }
            catch (InvalidDataException ex)
            {
                Log.W($"{ex}\nThe data will be reset.");
                pond.WriteData("FishQualities", $"{pond.FishCount},0,0,0");
                pond.WriteData("FamilyQualities", null);
                pond.WriteData("FamilyLivingHere", null);
            }
            catch (Exception ex)
            {
                Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            }
        }
    }

    [HarmonyPatch(typeof(FishPond), MethodType.Constructor, typeof(BluePrint), typeof(Vector2))]
    internal class FishPondCtorPatch
    {
        /// <summary>Compensates for the game calling dayUpdate *twice* immediately upon construction.</summary>
        [HarmonyPostfix]
        protected static void Postfix(FishPond __instance)
        {
            __instance.WriteData("DaysEmpty", (-3).ToString()); // it's -3 for good measure (and also immersion; a fresh pond takes longer to get dirty)
        }
    }

    [HarmonyPatch(typeof(FishPond), "addFishToPond")]
    internal class FishPondAddFishToPond
    {
        /// <summary>Distinguish extended family pairs + increment total Fish Pond quality ratings.</summary>
        [HarmonyPostfix]
        protected static void Postfix(FishPond __instance, SObject fish)
        {
            try
            {
                if (fish.HasContextTag("fish_legendary") && fish.ParentSheetIndex != __instance.fishType.Value)
                {
                    var familyQualities = __instance
                        .ReadData("FamilyQualities", $"{__instance.ReadDataAs<int>("FamilyLivingHere")},0,0,0")
                        .ParseList<int>()!;
                    if (familyQualities.Count != 4 || familyQualities.Sum() != __instance.ReadDataAs<int>("FamilyLivingHere"))
                        throw new InvalidDataException("FamilyQualities data had incorrect number of values.");

                    ++familyQualities[fish.Quality == 4 ? 3 : fish.Quality];
                    __instance.IncrementData<int>("FamilyLivingHere");
                    __instance.WriteData("FamilyQualities", string.Join(',', familyQualities));
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
                    var fishQualities = __instance.ReadData("FishQualities",
                            $"{__instance.FishCount - __instance.ReadDataAs<int>("FamilyLivingHere") - 1},0,0,0") // already added at this point, so consider - 1
                        .ParseList<int>()!;
                    if (fishQualities.Count != 4 || fishQualities.Any(q => 0 > q || q > __instance.FishCount - 1))
                        throw new InvalidDataException("FishQualities data had incorrect number of values.");

                    ++fishQualities[fish.Quality == 4 ? 3 : fish.Quality];
                    __instance.WriteData("FishQualities", string.Join(',', fishQualities));
                }
            }
            catch (InvalidDataException ex)
            {
                Log.W($"{ex}\nThe data will be reset.");
                __instance.WriteData("FishQualities", $"{__instance.FishCount},0,0,0");
                __instance.WriteData("FamilyQualities", null);
                __instance.WriteData("FamilyLivingHere", null);
            }
        }
    }

    [HarmonyPatch(typeof(FishPond), nameof(FishPond.dayUpdate))]
    internal class FishPondDayUpdate
    {
        /// <summary>Rest held items each morning.</summary>
        [HarmonyPrefix]
        protected static void Prefix(FishPond __instance)
        {
            __instance.WriteData("ItemsHeld", null);
        }

#if DEBUG
        /// <summary>Replacement to help debugging.</summary>
        [HarmonyPrefix]
        protected static bool Debug(FishPond __instance, int dayOfMonth)
        {
            __instance.hasSpawnedFish.Value = false;
            ModEntry.ModHelper.Reflection.GetField<bool>(__instance, "_hasAnimatedSpawnedFish").SetValue(false);
            if (__instance.hasCompletedRequest.Value)
            {
                __instance.neededItem.Value = null;
                __instance.neededItemCount.Set(-1);
                __instance.hasCompletedRequest.Value = false;
            }

            var fishPondData = __instance.GetFishPondData();
            if (fishPondData is null)
            {
                Log.W(
                    $"Invalid Fish Pond at {__instance.GetCenterTile()}.\nThe object {__instance.GetFishObject().Name} does not have an associated entry in the FishPondData dictionary. Please clear this pond and replace the object with a valid fish.");
                return false;
            }

            if (__instance.currentOccupants.Value > 0)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                if (r.NextDouble() < SUtility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f))
                    __instance.output.Value = __instance.GetFishProduce(r);
                
                __instance.daysSinceSpawn.Value += 1;
                if (__instance.daysSinceSpawn.Value > fishPondData.SpawnTime)
                    __instance.daysSinceSpawn.Value = fishPondData.SpawnTime;
                
                if (__instance.daysSinceSpawn.Value >= fishPondData.SpawnTime)
                {
                    var (key, value) = ModEntry.ModHelper.Reflection.GetMethod(__instance, "_GetNeededItemData")
                        .Invoke<KeyValuePair<int, int>>();
                    if (key != -1)
                    {
                        if (__instance.currentOccupants.Value >= __instance.maxOccupants.Value && __instance.neededItem.Value == null)
                        {
                            __instance.neededItem.Value = new(key, 1);
                            __instance.neededItemCount.Set(value);
                        }
                    }
                    else
                    {
                        __instance.SpawnFish();
                    }
                }

                if (__instance.currentOccupants.Value == 10 && __instance.fishType.Value == 717)
                    foreach (var farmer in Game1.getAllFarmers())
                        if (!farmer.mailReceived.Contains("FullCrabPond"))
                        {
                            farmer.mailReceived.Add("FullCrabPond");
                            farmer.activeDialogueEvents.Add("FullCrabPond", 14);
                        }

                ModEntry.ModHelper.Reflection.GetMethod(__instance, "doFishSpecificWaterColoring").Invoke();
            }

            BuildingDayUpdatePatch.Reverse(__instance, dayOfMonth);
            return false; // replaces original logic
        }
#endif

        /// <summary>Spontaneously grow algae + calculate roe production.</summary>
        [HarmonyPostfix]
        protected static void Postfix(FishPond __instance, ref FishPondData? ____fishPondData)
        {
            if (__instance.IsAlgaePond()) return;

            Random r = new(Guid.NewGuid().GetHashCode());

            // spontaneously grow algae/seaweed
            if (__instance.currentOccupants.Value == 0)
            {
                __instance.IncrementData<int>("DaysEmpty");
                if (__instance.ReadDataAs<int>("DaysEmpty") < 3) return;

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
                return;
            }

            try
            {
                var fish = __instance.GetFishObject();
                var produce = __instance.ReadData("ItemsHeld", null)?.ParseList<string>(";") ?? new();

                // handle coral
                if (fish.Name == "Coral")
                {
                    int greenAlgaeStack = 0, whiteAlgaeStack = 0, seaweedStack = 0;
                    for (var i = 0; i < __instance.FishCount; ++i)
                    {
                        if (r.NextDouble() < 0.25) ++whiteAlgaeStack;
                        else if (r.NextDouble() < 0.5) ++greenAlgaeStack;
                        else ++seaweedStack;
                    }

                    if (greenAlgaeStack + whiteAlgaeStack + seaweedStack == 0) return;

                    var displayedIndex = Constants.SEAWEED_INDEX_I;
                    if (greenAlgaeStack > seaweedStack) displayedIndex = Constants.GREEN_ALGAE_INDEX_I;
                    if (whiteAlgaeStack > greenAlgaeStack) displayedIndex = Constants.WHITE_ALGAE_INDEX_I;

                    switch (displayedIndex)
                    {
                        case Constants.SEAWEED_INDEX_I:
                            if (greenAlgaeStack > 0) produce.Add($"{Constants.GREEN_ALGAE_INDEX_I},{greenAlgaeStack},0");
                            if (whiteAlgaeStack > 0) produce.Add($"{Constants.WHITE_ALGAE_INDEX_I},{whiteAlgaeStack},0");
                        
                            if (__instance.output.Value is null)
                                __instance.output.Value = new SObject(displayedIndex, seaweedStack);
                            else
                                produce.Add($"{Constants.SEAWEED_INDEX_I},{seaweedStack},0");
                            break;
                        case Constants.GREEN_ALGAE_INDEX_I:
                            if (seaweedStack > 0) produce.Add($"{Constants.SEAWEED_INDEX_I},{seaweedStack},0");
                            if (whiteAlgaeStack > 0) produce.Add($"{Constants.WHITE_ALGAE_INDEX_I},{whiteAlgaeStack},0");

                            if (__instance.output.Value is null)
                                __instance.output.Value = new SObject(displayedIndex, greenAlgaeStack);
                            else
                                produce.Add($"{Constants.GREEN_ALGAE_INDEX_I},{greenAlgaeStack},0");
                            break;
                        case Constants.WHITE_ALGAE_INDEX_I:
                            if (seaweedStack > 0) produce.Add($"{Constants.SEAWEED_INDEX_I},{seaweedStack},0");
                            if (greenAlgaeStack > 0) produce.Add($"{Constants.GREEN_ALGAE_INDEX_I},{greenAlgaeStack},0");

                            if (__instance.output.Value is null)
                                __instance.output.Value = new SObject(displayedIndex, whiteAlgaeStack);
                            else
                                produce.Add($"{Constants.WHITE_ALGAE_INDEX_I},{whiteAlgaeStack},0");
                            break;
                    }

                    if (produce.Any()) __instance.WriteData("ItemsHeld", string.Join(";", produce));

                    return;
                }
            
                // handle fish + squid
                var fishQualities = __instance.ReadData("FishQualities",
                        $"{__instance.FishCount - __instance.ReadDataAs<int>("FamilyLivingHere")},0,0,0")
                    .ParseList<int>()!;
                if (fishQualities.Count != 4)
                    throw new InvalidDataException("FishQualities data had incorrect number of values.");
                var familyQualities = __instance.ReadData("FamilyQualities", "0,0,0,0").ParseList<int>()!;
                if (familyQualities.Count != 4)
                    throw new InvalidDataException("FamilyQualities data had incorrect number of values.");

                var totalQualities = fishQualities.Zip(familyQualities, (first, second) => first + second).ToList();
                if (totalQualities.Sum() != __instance.FishCount)
                    throw new InvalidDataException("Quality data had incorrect number of values.");

                var productionChancePerFish = Framework.Utility.GetRoeChance(fish.Price, __instance.FishCount - 1) / 100;
                var producedRoes = new int[4];
                for (var i = 0; i < 4; ++i)
                    while (totalQualities[i]-- > 0)
                        if (r.NextDouble() < productionChancePerFish)
                            ++producedRoes[i];

                if (fish.ParentSheetIndex == Constants.STURGEON_INDEX_I)
                    for (var i = 0; i < 4; ++i)
                        producedRoes[i] += r.Next(producedRoes[i]);

                if (producedRoes.Sum() <= 0) return;

                var roeIndex = fish.Name.Contains("Squid") ? Constants.SQUID_INK_INDEX_I : Constants.ROE_INDEX_I;
                for (var i = 0; i < 4; ++i)
                    if (producedRoes[i] > 0)
                        produce.Add($"{roeIndex},{producedRoes[i]},{(i == 3 ? 4 : i)}");

                if (__instance.output.Value is not null)
                {
                    __instance.WriteData("ItemsHeld", string.Join(';', produce));
                    return;
                }


                var highest = Array.FindLastIndex(producedRoes, i => i > 0);
                var forFamily = r.NextDouble() < __instance.ReadDataAs<double>("FamilyLivingHere") / __instance.FishCount;
                var fishIndex = forFamily
                    ? Framework.Utility.ExtendedFamilyPairs[__instance.fishType.Value]
                    : __instance.fishType.Value;
                SObject o;
                if (roeIndex == Constants.ROE_INDEX_I)
                {
                    var split = Game1.objectInformation[fishIndex].Split('/');
                    var c = __instance.fishType.Value == 698
                        ? new(61, 55, 42)
                        : TailoringMenu.GetDyeColor(new SObject(fishIndex, 1)) ?? Color.Orange;
                    o = new ColoredObject(Constants.ROE_INDEX_I, producedRoes[highest], c);
                    o.name = split[0] + " Roe";
                    o.preserve.Value = SObject.PreserveType.Roe;
                    o.preservedParentSheetIndex.Value = __instance.fishType.Value;
                    o.Price += Convert.ToInt32(split[1]) / 2;
                    o.Quality = highest == 3 ? 4 : highest;
                }
                else
                {
                    o = new(roeIndex, producedRoes[highest]) {Quality = highest == 3 ? 4 : highest};
                }

                produce.Remove($"{roeIndex},{producedRoes[highest]},{(highest == 3 ? 4 : highest)}");
                producedRoes[highest] = 0;
                if (produce.Any()) __instance.WriteData("ItemsHeld", string.Join(';', produce));
                __instance.output.Value = o;
            }
            catch (InvalidDataException ex)
            {
                Log.W($"{ex}\nThe data will be reset.");
                __instance.WriteData("FishQualities", $"{__instance.FishCount},0,0,0");
                __instance.WriteData("FamilyQualities", null);
                __instance.WriteData("FamilyLivingHere", null);
            }
        }

        /// <summary>Removes population-based role from <see cref="FishPond.dayUpdate"/> (moved to <see cref="FishPond.GetFishProduce"/>).</summary>
        protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Callvirt, typeof(Random).RequireMethod(nameof(Random.NextDouble)))
                    )
                    .RemoveUntil(
                        new CodeInstruction(OpCodes.Bge_Un_S)
                    )
                    .AdvanceUntil(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.daysSinceSpawn)))
                    )
                    .RemoveLabels();
            }
            catch (Exception ex)
            {
                Log.E($"Failed removing day update production roll.\nHelper returned {ex}");
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
        protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            /// From: if (output.Value != null) {...} return true;
            /// To: if (output.Value != null)
            /// {
            ///     this.RewardExp(who);
            ///     return this.OpenChumBucketMenu();
            /// }

            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.output))),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(NetRef<Item>).RequirePropertyGetter(nameof(NetRef<Item>.Value))),
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
                        new CodeInstruction(OpCodes.Ldarg_2),
                        new CodeInstruction(OpCodes.Call,
                            typeof(FishPondExtensions).RequireMethod(nameof(FishPondExtensions.RewardExp))),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_2),
                        new CodeInstruction(OpCodes.Call,
                            typeof(FishPondExtensions).RequireMethod(nameof(FishPondExtensions.OpenChumBucketMenu)))
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
                        new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.fishType))),
                        new CodeInstruction(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).RequireMethod("op_Implicit")),
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
                            typeof(Framework.Utility).RequireMethod(nameof(Framework.Utility.IsExtendedFamilyMember)))
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
        /// <summary>Recolor for algae/seaweed.</summary>
        [HarmonyPostfix]
        protected static void Postfix(FishPond __instance)
        {
            if (__instance.fishType.Value.IsAlgae())
            {
                var shift = -5 - 3 * __instance.FishCount;
                __instance.overrideWaterColor.Value = new Color(60, 126, 150).ShiftHue(shift);
            }
            else if (__instance.GetFishObject().Name.ContainsAnyOf("Mutant", "Radioactive"))
            {
                __instance.overrideWaterColor.Value = new(40, 255, 40);
            }
        }
    }

    [HarmonyPatch(typeof(FishPond), nameof(FishPond.GetFishProduce))]
    internal class FishPondGetFishProducePatch
    {
        /// <summary>Replace single production with multi-yield production.</summary>
        [HarmonyPrefix]
        // ReSharper disable once RedundantAssignment
        protected static bool Prefix(FishPond __instance, ref SObject? __result, Random? random)
        {
            random ??= new(Guid.NewGuid().GetHashCode());

            try
            {
                var produce = new List<(int, int)>();
                SObject? output;
                if (__instance.IsAlgaePond())
                {
                    var seaweedProduce = 0;
                    for (var i = 0; i < __instance.ReadDataAs<int>("SeaweedLivingHere"); ++i)
                    {
                        if (random.NextDouble() < SUtility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f))
                            ++seaweedProduce;
                    }

                    var greenAlgaeProduced = 0;
                    for (var i = 0; i < __instance.ReadDataAs<int>("GreenAlgaeLivingHere"); ++i)
                    {
                        if (random.NextDouble() < SUtility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f))
                            ++greenAlgaeProduced;
                    }

                    var whiteAlgaeProduced = 0;
                    for (var i = 0; i < __instance.ReadDataAs<int>("WhiteAlgaeLivingHere"); ++i)
                    {
                        if (random.NextDouble() < SUtility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f))
                            ++whiteAlgaeProduced;
                    }

                    if (seaweedProduce + greenAlgaeProduced + whiteAlgaeProduced == 0)
                        return false; // don't run original logic

                    if (seaweedProduce > 0) produce.Add((Constants.SEAWEED_INDEX_I, seaweedProduce));
                    if (greenAlgaeProduced > 0) produce.Add((Constants.GREEN_ALGAE_INDEX_I, greenAlgaeProduced));
                    if (whiteAlgaeProduced > 0) produce.Add((Constants.WHITE_ALGAE_INDEX_I, whiteAlgaeProduced));

                    switch (__instance.fishType.Value)
                    {
                        case Constants.SEAWEED_INDEX_I when seaweedProduce > 0:
                            output = new(Constants.SEAWEED_INDEX_I, seaweedProduce);
                            break;
                        case Constants.GREEN_ALGAE_INDEX_I when greenAlgaeProduced > 0:
                            output = new(Constants.GREEN_ALGAE_INDEX_I, greenAlgaeProduced);
                            break;
                        case Constants.WHITE_ALGAE_INDEX_I when whiteAlgaeProduced > 0:
                            output = new(Constants.WHITE_ALGAE_INDEX_I, whiteAlgaeProduced);
                            break;
                        default:
                            if (seaweedProduce > 0 && seaweedProduce > greenAlgaeProduced &&
                                seaweedProduce > whiteAlgaeProduced)
                                output = new(Constants.SEAWEED_INDEX_I, seaweedProduce);
                            else if (greenAlgaeProduced > 0 && greenAlgaeProduced > seaweedProduce &&
                                     greenAlgaeProduced > whiteAlgaeProduced)
                                output = new(Constants.GREEN_ALGAE_INDEX_I, greenAlgaeProduced);
                            else if (whiteAlgaeProduced > 0 && whiteAlgaeProduced > seaweedProduce &&
                                     whiteAlgaeProduced > greenAlgaeProduced)
                                output = new(Constants.WHITE_ALGAE_INDEX_I, whiteAlgaeProduced);
                            else output = null;
                            break;
                    }

                    if (output is not null) produce.Remove((output.ParentSheetIndex, output.Stack));

                    if (produce.Any())
                    {
                        var data = produce.Select(p => $"{p.Item1},{p.Item2},0");
                        __instance.WriteData("ItemsHeld", string.Join(';', data));
                    }
                }
                else
                {
                    var fishPondData = __instance.GetFishPondData();
                    if (fishPondData is null)
                    {
                        __result = null;
                        return false; // don't run original logic
                    }

                    foreach (var item in fishPondData.ProducedItems.Where(item =>
                                 item.ItemID is not Constants.ROE_INDEX_I or Constants.SQUID_INK_INDEX_I &&
                                 __instance.currentOccupants.Value >= item.RequiredPopulation &&
                                 random.NextDouble() <
                                 SUtility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f) &&
                                 random.NextDouble() < item.Chance))
                    {
                        var stack = random.Next(item.MinQuantity, item.MaxQuantity + 1);
                        var existing = produce.FindIndex(p => p.Item1 == item.ItemID);
                        if (existing >= 0) produce[existing] = (item.ItemID, stack + produce[existing].Item2);
                        else produce.Add((item.ItemID, stack));
                    }

                    if (!produce.Any())
                    {
                        __result = null;
                        return false; // don't run original logic
                    }

                    output = produce
                        .Select(p => new SObject(p.Item1, p.Item2))
                        .OrderByDescending(o => o.Price)
                        .First();
                    produce.Remove((output.ParentSheetIndex, output.Stack));
                    if (produce.Any())
                    {
                        var data = produce.Select(p => $"{p.Item1},{p.Item2},0");
                        __instance.WriteData("ItemsHeld", string.Join(';', data));
                    }
                }

                __result = output;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
                return true; // default to original logic
            }
        }
    }

    [HarmonyPatch(typeof(FishPond), nameof(FishPond.JumpFish))]
    internal class FishPondJumpFishPatch
    {
        /// <summary>Prevent un-immersive jumping algae.</summary>
        [HarmonyPrefix]
        protected static bool Prefix(FishPond __instance, ref bool __result)
        {
            if (!__instance.fishType.Value.IsAlgae()) return true; // run original logic

            __result = false;
            return false; // don't run original logic
        }
    }

    [HarmonyPatch(typeof(FishPond), nameof(FishPond.OnFishTypeChanged))]
    internal class FishPondOnFishTypeChangedPatch
    {
        /// <summary>Reset Fish Pond data.</summary>
        [HarmonyPostfix]
        protected static void Postfix(FishPond __instance)
        {
            if (__instance.fishType.Value > 0) return;

            __instance.WriteData("FishQualities", null);
            __instance.WriteData("FamilyQualities", null);
            __instance.WriteData("FamilyLivingHere", null);
            __instance.WriteData("DaysEmpty", 0.ToString());
            __instance.WriteData("SeaweedLivingHere", null);
            __instance.WriteData("GreenAlgaeLivingHere", null);
            __instance.WriteData("WhiteAlgaeLivingHere", null);
            __instance.WriteData("CheckedToday", null);
            __instance.WriteData("ItemsHeld", null);
        }
    }

    [HarmonyPatch(typeof(FishPond), nameof(FishPond.SpawnFish))]
    internal class FishPondSpawnFishPatch
    {
        /// <summary>Set the quality of newborn fishes.</summary>
        [HarmonyPostfix]
        protected static void Postfix(FishPond __instance)
        {
            if (__instance.currentOccupants.Value >= __instance.maxOccupants.Value &&
                !__instance.hasSpawnedFish.Value) return;

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

            try
            {
                var forFamily = false;
                var familyCount = 0;
                if (__instance.IsLegendaryPond())
                {
                    familyCount = __instance.ReadDataAs<int>("FamilyLivingHere");
                    if (0 > familyCount || familyCount > __instance.FishCount)
                        throw new InvalidDataException("FamilyLivingHere data is invalid.");

                    if (familyCount > 0 && Game1.random.NextDouble() < (double) familyCount / (__instance.FishCount - 1)) // fish pond count has already been incremented at this point, so we consider -1;
                        forFamily = true;
                }

                var @default = forFamily
                    ? $"{familyCount},0,0,0"
                    : $"{__instance.FishCount - familyCount - 1},0,0,0";
                var qualities = __instance.ReadData(forFamily ? "FamilyQualities" : "FishQualities", @default)
                    .ParseList<int>()!;
                if (qualities.Count != 4 || qualities.Sum() != (forFamily ? familyCount : __instance.FishCount - familyCount - 1))
                    throw new InvalidDataException("FishQualities data had incorrect number of values.");

                if (qualities.Sum() == 0)
                {
                    ++qualities[0];
                    __instance.WriteData(forFamily ? "FamilyQualities" : "FishQualities",
                        string.Join(',', qualities));
                    return;
                }

                var roll = r.Next(forFamily ? familyCount : __instance.FishCount - familyCount - 1);
                var fishlingQuality = roll < qualities[3]
                    ? SObject.bestQuality
                    : roll < qualities[3] + qualities[2]
                        ? SObject.highQuality
                        : roll < qualities[3] + qualities[2] + qualities[1]
                            ? SObject.medQuality
                            : SObject.lowQuality;

                ++qualities[fishlingQuality == 4 ? 3 : fishlingQuality];
                __instance.WriteData(forFamily ? "FamilyQualities" : "FishQualities", string.Join(',', qualities));
            }
            catch (InvalidDataException ex)
            {
                Log.W($"{ex}\nThe data will be reset.");
                __instance.WriteData("FishQualities", $"{__instance.FishCount},0,0,0");
                __instance.WriteData("FamilyQualities", null);
                __instance.WriteData("FamilyLivingHere", null);
            }
        }
    }

    [HarmonyPatch(typeof(PondQueryMenu), MethodType.Constructor, typeof(FishPond))]
    internal class PondQueryMenuCtorPatch
    {
        /// <summary>Handle invalid data on menu open.</summary>
        [HarmonyPrefix]
        protected static bool Prefix(FishPond fish_pond)
        {
            try
            {
                fish_pond.ReadData("FishQualities", null)?.ParseTuple<int, int, int, int>();
            }
            catch (InvalidOperationException ex)
            {
                Log.W($"FishQualities data is invalid. {ex}\nThe data will be reset");
                fish_pond.WriteData("FishQualities", $"{fish_pond.FishCount},0,0,0");
                fish_pond.WriteData("FamilyQualities", null);
                fish_pond.WriteData("FamilyLivingHere", null);
            }

            try
            {
                fish_pond.ReadData("FamilyQualities", null)?.ParseTuple<int, int, int, int>();
            }
            catch (InvalidOperationException ex)
            {
                Log.W($"FamilyQuality data is invalid. {ex}\nThe data will be reset");
                fish_pond.WriteData("FishQualities", $"{fish_pond.FishCount},0,0,0");
                fish_pond.WriteData("FamilyQualities", null);
                fish_pond.WriteData("FamilyLivingHere", null);
            }

            return true; // run original logic
        }
    }

    [HarmonyPatch(typeof(PondQueryMenu), nameof(PondQueryMenu.draw))]
    internal class PondQueryMenuDrawPatch
    {
        /// <summary>Adjust fish pond query menu for algae.</summary>
        [HarmonyPrefix]
        protected static bool Prefix(PondQueryMenu __instance, float ____age,
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

                    familyCount = ____pond.ReadDataAs<int>("FamilyLivingHere");
                    hasExtendedFamily = familyCount > 0;
                }

                if (!isAlgaePond && !isLegendaryPond) return true; // run original logic

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
                        __instance.yPositionOnScreen - 4 + 128, (int) (textSize.X + 64f), 64);
                    SUtility.drawTextWithShadow(b, pondNameText, Game1.smallFont,
                        new(Game1.uiViewport.Width / 2 - textSize.X * 0.5f,
                            __instance.yPositionOnScreen - 4 + 160f - textSize.Y * 0.5f), Color.Black);
                    //var displayedText = (string) _GetDisplayedText.Invoke(__instance, null)!;
                    var displayedText = _GetDisplayedText(__instance);
                    var extraHeight = 0;
                    if (hasUnresolvedNeeds)
                        extraHeight += 116;

                    //var extraTextHeight = (int) _MeasureExtraTextHeight.Invoke(__instance, new object?[] {displayedText})!;
                    var extraTextHeight = _MeasureExtraTextHeight(__instance, displayedText);
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
                            var fishPondData = (FishPondData?) _FishPondData.GetValue(____pond);
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
                        itemToDraw = new(Framework.Utility.ExtendedFamilyPairs[____fishItem.ParentSheetIndex], 1);
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
                    SUtility.drawTextWithShadow(b, displayedText, Game1.smallFont,
                        new(__instance.xPositionOnScreen + PondQueryMenu.width / 2 - textSize.X * 0.5f,
                            __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight -
                            (hasUnresolvedNeeds ? 32 : 48) - textSize.Y), Game1.textColor);
                    if (hasUnresolvedNeeds)
                    {
                        //_DrawHorizontalPartition.Invoke(__instance, new object?[]
                        //{
                        //    b, (int) (__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - 48f), false,
                        //    -1, -1, -1
                        //});
                        _DrawHorizontalPartition(__instance, b,
                            (int) (__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - 48f), false,
                            -1, -1, -1);
                        SUtility.drawWithShadow(b, Game1.mouseCursors,
                            new(__instance.xPositionOnScreen + 60 + 8f * Game1.dialogueButtonScale / 10f,
                                __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 28),
                            new(412, 495, 5, 4), Color.White, (float) Math.PI / 2f, Vector2.Zero);
                        var bringText =
                            Game1.content.LoadString(
                                PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_StatusRequest_Bring"));
                        textSize = Game1.smallFont.MeasureString(bringText);
                        var leftX = __instance.xPositionOnScreen + 88;
                        float textX = leftX;
                        var iconX = textX + textSize.X + 4f;
                        if (LocalizedContentManager.CurrentLanguageCode.IsIn(LocalizedContentManager.LanguageCode.ja,
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
                                ____pond.neededItem.Value?.ParentSheetIndex ?? 0, 16, 16), Color.White, 0f,
                            Vector2.Zero, 4f,
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
                        const int padding = 16;
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
        protected static void Postfix(PondQueryMenu __instance, bool ___confirmingEmpty, float ____age,
            FishPond ____pond, SpriteBatch b)
        {
            if (___confirmingEmpty) return;

            var isLegendaryPond = ____pond.IsLegendaryPond();
            var familyCount = ____pond.ReadDataAs<int>("FamilyLivingHere");

            var (_, numMedQuality, numHighQuality, numBestQuality) =
                ____pond.ReadData("FishQualities", $"{____pond.FishCount - familyCount},0,0,0")
                    .ParseTuple<int, int, int, int>();
            var (_, numMedFamilyQuality, numHighFamilyQuality, numBestFamilyQuality) =
                ____pond.ReadData("FamilyQualities", $"{familyCount},0,0,0").ParseTuple<int, int, int, int>();

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
    }

    [HarmonyPatch(typeof(ItemGrabMenu), nameof(ItemGrabMenu.readyToClose))]
    internal class ItemGrabMenuReadyToClosePatch
    {
        /// <summary>Update ItemsHeld on grab menu close.</summary>
        [HarmonyPostfix]
        protected static void Postfix(ItemGrabMenu __instance)
        {
            if (__instance.context is not FishPond pond) return;

            var items = __instance.ItemsToGrabMenu?.actualInventory;
            if (items is null || !items.Any() || items.All(i => i is null))
            {
                pond.WriteData("ItemsHeld", null);
                pond.output.Value = null;
                return;
            }

            var objects = items.Cast<SObject>().ToList();
            var output = objects.OrderByDescending(o => o?.Price).First();
            objects.Remove(output);
            if (objects.Any() && !objects.All(o => o is null))
            {
                var data = objects.Select(o => $"{o.ParentSheetIndex},{o.Stack},{o.Quality}");
                pond.WriteData("ItemsHeld", string.Join(';', data));
            }
            else
            {
                pond.WriteData("ItemsHeld", null);
            }

            pond.output.Value = output;
        }
    }

#if DEBUG
    /// <summary>Required by DayUpdate prefix.</summary>
    [HarmonyPatch(typeof(Building), nameof(Building.dayUpdate))]
    internal class BuildingDayUpdatePatch
    {
        /// <summary>Stub for base FishPond.dayUpdate</summary>
        [HarmonyReversePatch]
        internal static void Reverse(object instance, int dayOfMonth)
        {
            // its a stub so it has no initial content
            throw new NotImplementedException("It's a stub.");
        }
    }
#endif

#endregion harmony patches
}