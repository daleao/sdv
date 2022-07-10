namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Data;
using Common.Extensions;
using Common.Extensions.Collections;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondGetFishProducePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondGetFishProducePatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.GetFishProduce));
    }

    #region harmony patches

    /// <summary>Replace single production with multi-yield production.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool FishPondGetFishProducePrefix(FishPond __instance, ref SObject? __result, Random? random)
    {
        random ??= new(Guid.NewGuid().GetHashCode());

        try
        {
            var held = __instance.DeserializeObjectListData("ItemsHeld");
            if (__instance.output.Value is not null) held.Add(__instance.output.Value);

            var fish = __instance.GetFishObject();
            __result = null;
            if (__instance.HasAlgae()) // handle algae, which have no fish pond data
            {
                var algaeStacks = new[] { 0, 0, 0 }; // green, white, seaweed

                var population = ModDataIO.ReadFrom<int>(__instance, "GreenAlgaeLivingHere");
                var chance = Utility.Lerp(0.15f, 0.95f, population / (float)__instance.currentOccupants.Value);
                for (var i = 0; i < population; ++i)
                    if (random.NextDouble() < chance)
                        ++algaeStacks[0];

                population = ModDataIO.ReadFrom<int>(__instance, "WhiteAlgaeLivingHere");
                chance = Utility.Lerp(0.15f, 0.95f, population / (float)__instance.currentOccupants.Value);
                for (var i = 0; i < population; ++i)
                    if (random.NextDouble() < chance)
                        ++algaeStacks[1];

                population = ModDataIO.ReadFrom<int>(__instance, "SeaweedLivingHere");
                chance = Utility.Lerp(0.15f, 0.95f, population / (float)__instance.currentOccupants.Value);
                for (var i = 0; i < population; ++i)
                    if (random.NextDouble() < chance)
                        ++algaeStacks[2];

                if (algaeStacks.Sum() > 0)
                {
                    if (algaeStacks[0] > 0)
                        held.Add(new SObject(Constants.GREEN_ALGAE_INDEX_I, algaeStacks[0]));

                    if (algaeStacks[1] > 0)
                        held.Add(new SObject(Constants.WHITE_ALGAE_INDEX_I, algaeStacks[1]));

                    if (algaeStacks[2] > 0)
                        held.Add(new SObject(Constants.SEAWEED_INDEX_I, algaeStacks[2]));

                    __result = __instance.fishType.Value switch
                    {
                        Constants.GREEN_ALGAE_INDEX_I when algaeStacks[0] > 0 => new(Constants.GREEN_ALGAE_INDEX_I,
                            algaeStacks[0]),
                        Constants.WHITE_ALGAE_INDEX_I when algaeStacks[1] > 0 => new(Constants.WHITE_ALGAE_INDEX_I,
                            algaeStacks[1]),
                        Constants.SEAWEED_INDEX_I when algaeStacks[2] > 0 => new(Constants.SEAWEED_INDEX_I,
                            algaeStacks[2]),
                        _ => null
                    };

                    if (__result is null)
                    {
                        var max = algaeStacks.ToList().IndexOfMax();
#pragma warning disable CS8509
                        __result = max switch
#pragma warning restore CS8509
                        {
                            0 => new(Constants.GREEN_ALGAE_INDEX_I, algaeStacks[0]),
                            1 => new(Constants.WHITE_ALGAE_INDEX_I, algaeStacks[1]),
                            2 => new(Constants.SEAWEED_INDEX_I, algaeStacks[2])
                        };
                    }
                }

                if (__result is not null) held.Remove(__result);
                var serialized = held.Take(36).Select(p => $"{p.ParentSheetIndex},{p.Stack},0");
                ModDataIO.WriteTo(__instance, "ItemsHeld", string.Join(';', serialized));
                return false; // don't run original logic
            }

            // handle fish pond data
            var fishPondData = __instance.GetFishPondData();
            if (fishPondData is not null)
            {
                held.AddRange(from item in fishPondData.ProducedItems.Where(item =>
                        item.ItemID is not Constants.ROE_INDEX_I or Constants.SQUID_INK_INDEX_I &&
                        __instance.currentOccupants.Value >= item.RequiredPopulation &&
                        random.NextDouble() < Utility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f) &&
                        random.NextDouble() < item.Chance)
                              let stack = random.Next(item.MinQuantity, item.MaxQuantity + 1)
                              select new SObject(item.ItemID, stack));
            }

            // handle roe or ink
            if (fish.Name == "Coral")
            {
                var algaeStacks = new[] { 0, 0, 0 }; // green, white, seaweed
                var chance = Utils.GetRoeChance(fish.Price, __instance.FishCount - 1);
                for (var i = 0; i < __instance.FishCount; ++i)
                    if (random.NextDouble() < chance)
                        switch (Utils.ChooseAlgae())
                        {
                            case Constants.GREEN_ALGAE_INDEX_I:
                                ++algaeStacks[0];
                                break;
                            case Constants.WHITE_ALGAE_INDEX_I:
                                ++algaeStacks[1];
                                break;
                            case Constants.SEAWEED_INDEX_I:
                                ++algaeStacks[2];
                                break;
                        }

                if (algaeStacks[0] > 0)
                    held.Add(new SObject(Constants.GREEN_ALGAE_INDEX_I, algaeStacks[0]));

                if (algaeStacks[1] > 0)
                    held.Add(new SObject(Constants.WHITE_ALGAE_INDEX_I, algaeStacks[1]));

                if (algaeStacks[2] > 0)
                    held.Add(new SObject(Constants.SEAWEED_INDEX_I, algaeStacks[2]));
            }
            else
            {
                var fishQualities = ModDataIO.ReadFrom(__instance, "FishQualities",
                        $"{__instance.FishCount - ModDataIO.ReadFrom<int>(__instance, "FamilyLivingHere")},0,0,0")
                    .ParseList<int>()!;
                if (fishQualities.Count != 4)
                    throw new InvalidDataException("FishQualities data had incorrect number of values.");

                var familyQualities =
                    ModDataIO.ReadFrom(__instance, "FamilyQualities", "0,0,0,0").ParseList<int>()!;
                if (familyQualities.Count != 4)
                    throw new InvalidDataException("FamilyQualities data had incorrect number of values.");

                var totalQualities = fishQualities.Zip(familyQualities, (first, second) => first + second).ToList();
                if (totalQualities.Sum() != __instance.FishCount)
                    throw new InvalidDataException("Quality data had incorrect number of values.");

                var productionChancePerFish = Utils.GetRoeChance(fish.Price, __instance.FishCount - 1);
                var producedRoes = new int[4];
                for (var i = 0; i < 4; ++i)
                    while (totalQualities[i]-- > 0)
                        if (random.NextDouble() < productionChancePerFish)
                            ++producedRoes[i];

                if (fish.ParentSheetIndex == Constants.STURGEON_INDEX_I)
                    for (var i = 0; i < 4; ++i)
                        producedRoes[i] += random.Next(producedRoes[i]);

                if (producedRoes.Sum() > 0)
                {
                    var roeIndex = fish.Name.Contains("Squid") ? Constants.SQUID_INK_INDEX_I : Constants.ROE_INDEX_I;
                    for (var i = 0; i < 4; ++i)
                        if (producedRoes[i] > 0)
                            held.Add(new SObject(roeIndex, producedRoes[i], quality: i == 3 ? 4 : i));
                }

                // check for enriched metals
                if (__instance.HasRadioactiveFish())
                {
                    var heldMetals =
                        ModDataIO.ReadFrom(__instance, "MetalsHeld")
                            .ParseList<string>(";")?
                            .Select(li => li.ParseTuple<int, int>())
                            .WhereNotNull()
                            .ToList() ?? new List<(int, int)>();
                    var readyToHarvest = heldMetals.Where(m => m.Item2 <= 0).ToList();
                    if (readyToHarvest.Count > 0)
                    {
                        held.AddRange(readyToHarvest.Select(m =>
                            m.Item1.IsNonRadioactiveOre()
                                ? new SObject(Constants.RADIOACTIVE_ORE_INDEX_I, 1)
                                : new(Constants.RADIOACTIVE_BAR_INDEX_I, 1)));
                        heldMetals = heldMetals.Except(readyToHarvest).ToList();
                    }

                    ModDataIO.WriteTo(__instance, "MetalsHeld",
                        string.Join(';', heldMetals.Select(m => string.Join(',', m.Item1, m.Item2))));
                }
            }

            if (held.Count <= 0) return false; // don't run original logic

            // choose output
            Utility.consolidateStacks(held);
            __result = held.OrderByDescending(h => h.salePrice()).First() as SObject;
            held.Remove(__result!);
            if (held.Count > 0)
            {
                var serialized = held.Take(36).Select(p => $"{p.ParentSheetIndex},{p.Stack},{((SObject)p).Quality}");
                ModDataIO.WriteTo(__instance, "ItemsHeld", string.Join(';', serialized));
            }
            else
            {
                ModDataIO.WriteTo(__instance, "ItemsHeld", null);
            }

            if (__result!.ParentSheetIndex != Constants.ROE_INDEX_I) return false; // don't run original logic

            var fishIndex = fish.ParentSheetIndex;
            if (fish.IsLegendary() && random.NextDouble() <
                ModDataIO.ReadFrom<double>(__instance, "FamilyLivingHere") / __instance.FishCount)
                fishIndex = Utils.ExtendedFamilyPairs[fishIndex];

            var split = Game1.objectInformation[fishIndex].Split('/');
            var c = fishIndex == 698
                ? new(61, 55, 42)
                : TailoringMenu.GetDyeColor(new SObject(fishIndex, 1)) ?? Color.Orange;
            var o = new ColoredObject(Constants.ROE_INDEX_I, __result.Stack, c);
            o.name = split[0] + " Roe";
            o.preserve.Value = SObject.PreserveType.Roe;
            o.preservedParentSheetIndex.Value = fishIndex;
            o.Price += Convert.ToInt32(split[1]) / 2;
            o.Quality = __result.Quality;
            __result = o;

            return false; // don't run original logic
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            ModDataIO.WriteTo(__instance, "FishQualities", $"{__instance.FishCount},0,0,0");
            ModDataIO.WriteTo(__instance, "FamilyQualities", null);
            ModDataIO.WriteTo(__instance, "FamilyLivingHere", null);
            return true; // default to original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}