namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Ponds.Extensions;
using DaLion.Shared.Classes;
using DaLion.Shared.Constants;
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
            if (fish.ParentSheetIndex == ObjectIds.Coral)
            {
                ProduceForCoral(__instance, held, __instance.GetRoeChance(fish.Price), random);
            }
            else if (fish.ParentSheetIndex is 1127 or 1128)
            {
                ProduceForTuiLa(__instance, held, fish.ParentSheetIndex, __instance.GetRoeChance(fish.Price), random);
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
                __instance.Write(DataKeys.ItemsHeld, string.Join(';', serialized));
            }
            else
            {
                __instance.Write(DataKeys.ItemsHeld, null);
            }

            if (__result.ParentSheetIndex != ObjectIds.Roe)
            {
                return false; // don't run original logic
            }

            var fishIndex = fish.ParentSheetIndex;
            if (fish.IsLegendaryFish() && random.NextDouble() <
                __instance.Read<double>(DataKeys.FamilyLivingHere) / __instance.FishCount)
            {
                fishIndex = Lookups.FamilyPairs.TryGet(fishIndex, out var pairIndex) ? pairIndex : fishIndex;
            }

            var split = Game1.objectInformation[fishIndex].SplitWithoutAllocation('/');
            var c = fishIndex == 698
                ? new Color(61, 55, 42)
                : TailoringMenu.GetDyeColor(new SObject(fishIndex, 1)) ?? Color.Orange;
            var roe = new ColoredObject(ObjectIds.Roe, __result.Stack, c);
            roe.name = split[0].ToString() + " Roe";
            roe.preserve.Value = SObject.PreserveType.Roe;
            roe.preservedParentSheetIndex.Value = fishIndex;
            roe.Price += int.Parse(split[1]) / 2;
            roe.Quality = __result.Quality;
            __result = roe;

            return false; // don't run original logic
        }
        catch (InvalidDataException ex)
        {
            Log.W($"[PNDS]: {ex}\nThe data will be reset.");
            __instance.Write(DataKeys.FishQualities, $"{__instance.FishCount},0,0,0");
            __instance.Write(DataKeys.FamilyQualities, null);
            __instance.Write(DataKeys.FamilyLivingHere, null);
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
            if (reward.ItemID is not (ObjectIds.Roe or ObjectIds.SquidInk) &&
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
                DataKeys.FishQualities,
                $"{pond.FishCount - pond.Read<int>(DataKeys.FamilyLivingHere)},0,0,0")
            .ParseList<int>();
        if (fishQualities.Count != 4)
        {
            ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
        }

        var familyQualities =
            pond.Read(DataKeys.FamilyQualities, "0,0,0,0").ParseList<int>();
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

        if (fish.ParentSheetIndex == ObjectIds.Sturgeon)
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

        var roeIndex = fish.Name.Contains("Squid") ? ObjectIds.SquidInk : ObjectIds.Roe;
        for (var i = 3; i >= 0; i--)
        {
            if (producedRoes[i] <= 0)
            {
                continue;
            }

            var producedWithThisQuality = PondsModule.Config.RoeAlwaysFishQuality
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
        var population = pond.Read<int>(DataKeys.GreenAlgaeLivingHere);
        var chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[0]++;
            }
        }

        population = pond.Read<int>(DataKeys.WhiteAlgaeLivingHere);
        chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[1]++;
            }
        }

        population = pond.Read<int>(DataKeys.SeaweedLivingHere);
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
                held.Add(new SObject(ObjectIds.GreenAlgae, algaeStacks[0]));
            }

            if (algaeStacks[1] > 0)
            {
                held.Add(new SObject(ObjectIds.WhiteAlgae, algaeStacks[1]));
            }

            if (algaeStacks[2] > 0)
            {
                held.Add(new SObject(ObjectIds.Seaweed, algaeStacks[2]));
            }

            result = pond.fishType.Value switch
            {
                ObjectIds.GreenAlgae when algaeStacks[0] > 0 => new SObject(
                    ObjectIds.GreenAlgae,
                    algaeStacks[0]),
                ObjectIds.WhiteAlgae when algaeStacks[1] > 0 => new SObject(
                    ObjectIds.WhiteAlgae,
                    algaeStacks[1]),
                ObjectIds.Seaweed when algaeStacks[2] > 0 => new SObject(
                    ObjectIds.Seaweed,
                    algaeStacks[2]),
                _ => null,
            };

            if (result is null)
            {
                var max = algaeStacks.ToList().IndexOfMax();
                result = max switch
                {
                    0 => new SObject(ObjectIds.GreenAlgae, algaeStacks[0]),
                    1 => new SObject(ObjectIds.WhiteAlgae, algaeStacks[1]),
                    2 => new SObject(ObjectIds.Seaweed, algaeStacks[2]),
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
        pond.Write(DataKeys.ItemsHeld, string.Join(';', serialized));
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
                    case ObjectIds.GreenAlgae:
                        algaeStacks[0]++;
                        break;
                    case ObjectIds.WhiteAlgae:
                        algaeStacks[1]++;
                        break;
                    case ObjectIds.Seaweed:
                        algaeStacks[2]++;
                        break;
                }
            }
        }

        if (algaeStacks[0] > 0)
        {
            held.Add(new SObject(ObjectIds.GreenAlgae, algaeStacks[0]));
        }

        if (algaeStacks[1] > 0)
        {
            held.Add(new SObject(ObjectIds.WhiteAlgae, algaeStacks[1]));
        }

        if (algaeStacks[2] > 0)
        {
            held.Add(new SObject(ObjectIds.Seaweed, algaeStacks[2]));
        }
    }

    private static void ProduceRadioactive(FishPond pond, List<Item> held)
    {
        var heldMetals =
            pond.Read(DataKeys.MetalsHeld)
                .ParseList<string>(';')
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
                ? new SObject(ObjectIds.RadioactiveOre, 1)
                : new SObject(ObjectIds.RadioactiveBar, 1));
            heldMetals.RemoveAt(i);
        }

        pond.Write(
            DataKeys.MetalsHeld,
            string.Join(';', heldMetals
                .Select(m => string.Join(',', m.Item1, m.Item2))));
    }

    private static void ProduceForTuiLa(FishPond pond, List<Item> held, int which, double chance, Random r)
    {
        if (pond.FishCount > 1)
        {
            if (r.NextDouble() < chance && r.NextDouble() < chance)
            {
                held.Add(new SObject(ObjectIds.GalaxySoul, 1));
                return;
            }

            held.Add(new SObject(which == 1127 ? ObjectIds.VoidEssence : ObjectIds.SolarEssence, 1));
            if (r.NextDouble() < 0.8)
            {
                held.Last().Stack++;
            }
        }

        held.Add(new SObject(which == 1127 ? ObjectIds.SolarEssence : ObjectIds.VoidEssence, 1));
        if (r.NextDouble() < 0.8)
        {
            held.Last().Stack++;
        }
    }

    #endregion handlers
}
