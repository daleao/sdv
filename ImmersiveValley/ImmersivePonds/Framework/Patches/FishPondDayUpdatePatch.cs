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
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using StardewValley.Objects;

using Common;
using Common.Harmony;
using Common.Extensions;
using Common.Extensions.Reflection;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondDayUpdatePatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.dayUpdate));
    }

    #region harmony patches

    /// <summary>Rest held items each morning.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool FishPondDayUpdatePrefix(FishPond __instance, int dayOfMonth)
    {
        __instance.WriteData("ItemsHeld", null);
#if RELEASE
        return true; // run original logic
#elif DEBUG
        // Replacement to help debugging.

        if (__instance.isUnderConstruction()) return true;

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
            if (r.NextDouble() < StardewValley.Utility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f))
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

        BuildingDayUpdatePatch.BuildingDayUpdateReverse(__instance, dayOfMonth);
        return false; // replaces original logic
#endif
    }

    /// <summary>Spontaneously grow algae + calculate roe production.</summary>
    [HarmonyPostfix]
    private static void FishPondDayUpdatePostfix(FishPond __instance, ref FishPondData ____fishPondData)
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

            var productionChancePerFish = Framework.Utils.GetRoeChance(fish.Price, __instance.FishCount - 1);
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
                ? Framework.Utils.ExtendedFamilyPairs[__instance.fishType.Value]
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
                o = new(roeIndex, producedRoes[highest]) { Quality = highest == 3 ? 4 : highest };
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
    private static IEnumerable<CodeInstruction> FishPondDayUpdateTranspiler(IEnumerable<CodeInstruction> instructions,
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
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}