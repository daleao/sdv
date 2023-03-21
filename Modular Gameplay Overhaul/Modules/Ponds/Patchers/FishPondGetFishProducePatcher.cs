namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Ponds.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondGetFishProducePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondGetFishProducePatcher"/> class.</summary>
    internal FishPondGetFishProducePatcher()
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.GetFishProduce));
    }

    #region harmony patches

    /// <summary>Replace single production with multi-yield production.</summary>
    // ReSharper disable once RedundantAssignment
    [HarmonyPrefix]
    private static bool FishPondGetFishProducePrefix(FishPond __instance, ref SObject? __result, Random? random)
    {
        random ??= new Random(Guid.NewGuid().GetHashCode());

        try
        {
            var fish = __instance.GetFishObject();
            var held = __instance.DeserializeHeldItems();
            if (__instance.output.Value is not null)
            {
                held.Add(__instance.output.Value);
            }

            __result = null;
            // handle algae, which have no fish pond data, first
            if (fish.IsAlgae())
            {
                ProduceForAlgae(__instance, ref __result, held, random);
                return false; // don't run original logic
            }

            ProduceFromPondData(__instance, held, random);

            // handle roe/ink
            if (fish.ParentSheetIndex == Constants.CoralIndex)
            {
                ProduceForCoral(__instance, held, __instance.GetRoeChance(fish.Price), random);
            }
            else
            {
                ProduceRoe(__instance, fish, held, random);

                // check for enriched metals
                if (__instance.IsRadioactive())
                {
                    ProduceRadioactive(__instance, held);
                }
            }

            if (held.Count == 0)
            {
                return false; // don't run original logic
            }

            // choose output
            Utility.consolidateStacks(held);
            __result = held.OrderByDescending(h => h.salePrice()).FirstOrDefault() as SObject;
            if (__result is null)
            {
                return false; // don't run original logic
            }

            held.Remove(__result);
            if (held.Count > 0)
            {
                var serialized = held
                    .Take(36)
                    .Select(p => $"{p.ParentSheetIndex},{p.Stack},{((SObject)p).Quality}");
                __instance.Write(DataFields.ItemsHeld, string.Join(';', serialized));
            }
            else
            {
                __instance.Write(DataFields.ItemsHeld, null);
            }

            if (__result.ParentSheetIndex != Constants.RoeIndex)
            {
                return false; // don't run original logic
            }

            var fishIndex = fish.ParentSheetIndex;
            if (fish.IsLegendaryFish() && random.NextDouble() <
                __instance.Read<double>(DataFields.FamilyLivingHere) / __instance.FishCount)
            {
                fishIndex = Collections.ExtendedFamilyPairs[fishIndex];
            }

            var split = Game1.objectInformation[fishIndex].SplitWithoutAllocation('/');
            var c = fishIndex == 698
                ? new Color(61, 55, 42)
                : TailoringMenu.GetDyeColor(new SObject(fishIndex, 1)) ?? Color.Orange;
            var o = new ColoredObject(Constants.RoeIndex, __result.Stack, c);
            o.name = split[0].ToString() + " Roe";
            o.preserve.Value = SObject.PreserveType.Roe;
            o.preservedParentSheetIndex.Value = fishIndex;
            o.Price += int.Parse(split[1]) / 2;
            o.Quality = __result.Quality;
            __result = o;

            return false; // don't run original logic
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            __instance.Write(DataFields.FishQualities, $"{__instance.FishCount},0,0,0");
            __instance.Write(DataFields.FamilyQualities, null);
            __instance.Write(DataFields.FamilyLivingHere, null);
            return true; // default to original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches

    #region handlers

    private static void ProduceFromPondData(FishPond pond, List<Item> held, Random r)
    {
        var fishPondData = pond.GetFishPondData();
        if (fishPondData is null)
        {
            return;
        }

        for (var i = 0; i < fishPondData.ProducedItems.Count; i++)
        {
            var reward = fishPondData.ProducedItems[i];
            if (reward.ItemID is not (Constants.RoeIndex or Constants.SquidInkIndex) &&
                pond.currentOccupants.Value >= reward.RequiredPopulation &&
                r.NextDouble() < Utility.Lerp(0.15f, 0.95f, pond.currentOccupants.Value / 10f) &&
                r.NextDouble() < reward.Chance)
            {
                held.Add(new SObject(reward.ItemID, r.Next(reward.MinQuantity, reward.MaxQuantity + 1)));
            }
        }
    }

    private static void ProduceRoe(FishPond pond, SObject fish, List<Item> held, Random r)
    {
        var fishQualities = pond.Read(
                DataFields.FishQualities,
                $"{pond.FishCount - pond.Read<int>(DataFields.FamilyLivingHere)},0,0,0")
            .ParseList<int>();
        if (fishQualities.Count != 4)
        {
            ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
        }

        var familyQualities =
            pond.Read(DataFields.FamilyQualities, "0,0,0,0").ParseList<int>();
        if (familyQualities.Count != 4)
        {
            ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");
        }

        var totalQualities = fishQualities.Zip(familyQualities, (first, second) => first + second).ToList();
        if (totalQualities.Sum() != pond.FishCount)
        {
            ThrowHelper.ThrowInvalidDataException("Quality data had incorrect number of values.");
        }

        var productionChancePerFish = pond.GetRoeChance(fish.Price);
        var producedRoes = new int[4];
        for (var i = 0; i < 4; i++)
        {
            while (totalQualities[i]-- > 0)
            {
                if (r.NextDouble() < productionChancePerFish)
                {
                    producedRoes[i]++;
                }
            }
        }

        if (fish.ParentSheetIndex == Constants.SturgeonIndex)
        {
            for (var i = 0; i < 4; i++)
            {
                producedRoes[i] += r.Next(producedRoes[i]);
            }
        }

        if (producedRoes.Sum() <= 0)
        {
            return;
        }

        var roeIndex = fish.Name.Contains("Squid") ? Constants.SquidInkIndex : Constants.RoeIndex;
        for (var i = 3; i >= 0; i--)
        {
            if (producedRoes[i] <= 0)
            {
                continue;
            }

            var producedWithThisQuality = PondsModule.Config.RoeAlwaysSameQualityAsFish
                ? producedRoes[i]
                : r.Next(producedRoes[i]);
            held.Add(new SObject(roeIndex, producedWithThisQuality, quality: i == 3 ? 4 : i));
            if (i > 0)
            {
                producedRoes[i - 1] += producedRoes[i] - producedWithThisQuality;
            }
        }
    }

    private static void ProduceForAlgae(FishPond pond, ref SObject? result, List<Item> held, Random r)
    {
        var algaeStacks = new[] { 0, 0, 0 }; // green, white, seaweed
        var population = pond.Read<int>(DataFields.GreenAlgaeLivingHere);
        var chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[0]++;
            }
        }

        population = pond.Read<int>(DataFields.WhiteAlgaeLivingHere);
        chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[1]++;
            }
        }

        population = pond.Read<int>(DataFields.SeaweedLivingHere);
        chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[2]++;
            }
        }

        if (algaeStacks.Sum() > 0)
        {
            if (algaeStacks[0] > 0)
            {
                held.Add(new SObject(Constants.GreenAlgaeIndex, algaeStacks[0]));
            }

            if (algaeStacks[1] > 0)
            {
                held.Add(new SObject(Constants.WhiteAlgaeIndex, algaeStacks[1]));
            }

            if (algaeStacks[2] > 0)
            {
                held.Add(new SObject(Constants.SeaweedIndex, algaeStacks[2]));
            }

            result = pond.fishType.Value switch
            {
                Constants.GreenAlgaeIndex when algaeStacks[0] > 0 => new SObject(
                    Constants.GreenAlgaeIndex,
                    algaeStacks[0]),
                Constants.WhiteAlgaeIndex when algaeStacks[1] > 0 => new SObject(
                    Constants.WhiteAlgaeIndex,
                    algaeStacks[1]),
                Constants.SeaweedIndex when algaeStacks[2] > 0 => new SObject(
                    Constants.SeaweedIndex,
                    algaeStacks[2]),
                _ => null,
            };

            if (result is null)
            {
                var max = algaeStacks.ToList().IndexOfMax();
                result = max switch
                {
                    0 => new SObject(Constants.GreenAlgaeIndex, algaeStacks[0]),
                    1 => new SObject(Constants.WhiteAlgaeIndex, algaeStacks[1]),
                    2 => new SObject(Constants.SeaweedIndex, algaeStacks[2]),
                    _ => null,
                };
            }
        }

        if (result is not null)
        {
            held.Remove(result);
        }

        Utility.consolidateStacks(held);
        var serialized = held
            .Take(36)
            .Select(p => $"{p.ParentSheetIndex},{p.Stack},0");
        pond.Write(DataFields.ItemsHeld, string.Join(';', serialized));
    }

    private static void ProduceForCoral(FishPond pond, List<Item> held, double chance, Random r)
    {
        var algaeStacks = new[] { 0, 0, 0 }; // green, white, seaweed
        for (var i = 0; i < pond.FishCount; i++)
        {
            if (r.NextDouble() < chance)
            {
                switch (r.NextAlgae())
                {
                    case Constants.GreenAlgaeIndex:
                        algaeStacks[0]++;
                        break;
                    case Constants.WhiteAlgaeIndex:
                        algaeStacks[1]++;
                        break;
                    case Constants.SeaweedIndex:
                        algaeStacks[2]++;
                        break;
                }
            }
        }

        if (algaeStacks[0] > 0)
        {
            held.Add(new SObject(Constants.GreenAlgaeIndex, algaeStacks[0]));
        }

        if (algaeStacks[1] > 0)
        {
            held.Add(new SObject(Constants.WhiteAlgaeIndex, algaeStacks[1]));
        }

        if (algaeStacks[2] > 0)
        {
            held.Add(new SObject(Constants.SeaweedIndex, algaeStacks[2]));
        }
    }

    private static void ProduceRadioactive(FishPond pond, List<Item> held)
    {
        var heldMetals =
            pond.Read(DataFields.MetalsHeld)
                .ParseList<string>(";")
                .Select(li => li?.ParseTuple<int, int>())
                .WhereNotNull()
                .ToList();
        for (var i = heldMetals.Count - 1; i >= 0; i--)
        {
            var (index, timeLeft) = heldMetals[i];
            if (timeLeft > 0)
            {
                continue;
            }

            held.Add(index.IsOre()
                ? new SObject(Constants.RadioactiveOreIndex, 1)
                : new SObject(Constants.RadioactiveBarIndex, 1));
            heldMetals.RemoveAt(i);
        }

        pond.Write(
            DataFields.MetalsHeld,
            string.Join(';', heldMetals
                .Select(m => string.Join(',', m.Item1, m.Item2))));
    }

    #endregion handlers
}
