namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondGetFishProducePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondGetFishProducePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondGetFishProducePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.GetFishProduce));
    }

    #region harmony patches

    /// <summary>Replace single production with multi-yield production.</summary>
    [HarmonyPrefix]
    private static bool FishPondGetFishProducePrefix(FishPond __instance, ref Item? __result, Random? random)
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
            if (fish.QualifiedItemId == QualifiedObjectIds.Coral)
            {
                ProduceForCoral(__instance, held, __instance.GetRoeChance(fish.Price), random);
            }
            else if (fish.ItemId is "MNF.MoreNewFish_tui" or "MNF.MoreNewFish_la")
            {
                ProduceForTuiLa(__instance, held, fish.ItemId, __instance.GetRoeChance(fish.Price), random);
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
            __result = held.MaxBy(h => h.salePrice()) as SObject;
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
                Data.Write(__instance, DataKeys.ItemsHeld, string.Join(';', serialized));
            }
            else
            {
                Data.Write(__instance, DataKeys.ItemsHeld, null);
            }

            if (__result.QualifiedItemId != QualifiedObjectIds.Roe)
            {
                return false; // don't run original logic
            }

            if (fish.IsBossFish())
            {
                var bosses = __instance.ParsePondFishes();
                if (bosses.Choose()?.Id is { } id)
                {
                   fish = ItemRegistry.Create<SObject>($"(O){id}");
                }
            }

            var roe = ItemRegistry.GetObjectTypeDefinition().CreateFlavoredRoe(fish);
            roe.Stack = __result.Stack;
            roe.Quality = __result.Quality;
            __result = roe;

            return false; // don't run original logic
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

        foreach (var reward in fishPondData.ProducedItems)
        {
            if (reward.ItemId is not (QualifiedObjectIds.Roe or QualifiedObjectIds.SquidInk) &&
                pond.currentOccupants.Value >= reward.RequiredPopulation &&
                r.NextDouble() < Utility.Lerp(0.15f, 0.95f, pond.currentOccupants.Value / 10f) &&
                r.NextDouble() < reward.Chance)
            {
                held.Add(ItemRegistry.Create<SObject>(reward.ItemId, r.Next(reward.MinQuantity, reward.MaxQuantity + 1)));
            }
        }
    }

    private static void ProduceRoe(FishPond pond, SObject fish, List<Item> held, Random r)
    {
        var fishes = pond.ParsePondFishes();
        var fishQualities = new int[4];
        foreach (var f in fishes)
        {
            if (f?.Quality == SObject.bestQuality)
            {
                fishQualities[3]++;
            }
            else
            {
                fishQualities[f?.Quality ?? 0]++;
            }
        }

        var productionChancePerFish = pond.GetRoeChance(fish.Price);
        var roeQualities = new int[4];
        for (var i = 0; i < 4; i++)
        {
            while (fishQualities[i]-- > 0)
            {
                if (r.NextBool(productionChancePerFish))
                {
                    roeQualities[i]++;
                }
            }
        }

        if (fish.QualifiedItemId == QualifiedObjectIds.Sturgeon)
        {
            for (var i = 0; i < 4; i++)
            {
                roeQualities[i] += r.Next(roeQualities[i]);
            }
        }

        if (roeQualities.Sum() <= 0)
        {
            return;
        }

        var roeIndex = fish.Name.Contains("Squid") ? QualifiedObjectIds.SquidInk : QualifiedObjectIds.Roe;
        for (var i = 3; i >= 0; i--)
        {
            if (roeQualities[i] <= 0)
            {
                continue;
            }

            var producedWithThisQuality = r.Next(roeQualities[i]);
            held.Add(ItemRegistry.Create<SObject>(roeIndex, producedWithThisQuality, quality: i == 3 ? 4 : i));
            if (i > 0)
            {
                roeQualities[i - 1] += roeQualities[i] - producedWithThisQuality;
            }
        }
    }

    private static void ProduceForAlgae(FishPond pond, ref Item? result, List<Item> held, Random r)
    {
        var algae = pond.ParsePondFishes();
        var algaeStacks = new[] { 0, 0, 0 }; // green, white, seaweed
        var population = algae.Count(f => f?.Id == "153");
        var chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[0]++;
            }
        }

        population = algae.Count(f => f?.Id == "157");
        chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[1]++;
            }
        }

        population = algae.Count(f => f?.Id == "152");
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
                held.Add(ItemRegistry.Create<SObject>(QualifiedObjectIds.GreenAlgae, algaeStacks[0]));
            }

            if (algaeStacks[1] > 0)
            {
                held.Add(ItemRegistry.Create<SObject>(QualifiedObjectIds.WhiteAlgae, algaeStacks[1]));
            }

            if (algaeStacks[2] > 0)
            {
                held.Add(ItemRegistry.Create<SObject>(QualifiedObjectIds.Seaweed, algaeStacks[2]));
            }

            result = pond.fishType.Value switch
            {
                QualifiedObjectIds.GreenAlgae when algaeStacks[0] > 0 => ItemRegistry.Create<SObject>(
                    QualifiedObjectIds.GreenAlgae,
                    algaeStacks[0]),
                QualifiedObjectIds.WhiteAlgae when algaeStacks[1] > 0 => ItemRegistry.Create<SObject>(
                    QualifiedObjectIds.WhiteAlgae,
                    algaeStacks[1]),
                QualifiedObjectIds.Seaweed when algaeStacks[2] > 0 => ItemRegistry.Create<SObject>(
                    QualifiedObjectIds.Seaweed,
                    algaeStacks[2]),
                _ => null,
            };

            if (result is null)
            {
                var max = algaeStacks.ToList().IndexOfMax();
                result = max switch
                {
                    0 => ItemRegistry.Create<SObject>(QualifiedObjectIds.GreenAlgae, algaeStacks[0]),
                    1 => ItemRegistry.Create<SObject>(QualifiedObjectIds.WhiteAlgae, algaeStacks[1]),
                    2 => ItemRegistry.Create<SObject>(QualifiedObjectIds.Seaweed, algaeStacks[2]),
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
        Data.Write(pond, DataKeys.ItemsHeld, string.Join(';', serialized));
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
                    case QualifiedObjectIds.GreenAlgae:
                        algaeStacks[0]++;
                        break;
                    case QualifiedObjectIds.WhiteAlgae:
                        algaeStacks[1]++;
                        break;
                    case QualifiedObjectIds.Seaweed:
                        algaeStacks[2]++;
                        break;
                }
            }
        }

        if (algaeStacks[0] > 0)
        {
            held.Add(ItemRegistry.Create<SObject>(QualifiedObjectIds.GreenAlgae, algaeStacks[0]));
        }

        if (algaeStacks[1] > 0)
        {
            held.Add(ItemRegistry.Create<SObject>(QualifiedObjectIds.WhiteAlgae, algaeStacks[1]));
        }

        if (algaeStacks[2] > 0)
        {
            held.Add(ItemRegistry.Create<SObject>(QualifiedObjectIds.Seaweed, algaeStacks[2]));
        }
    }

    private static void ProduceRadioactive(FishPond pond, List<Item> held)
    {
        var heldMetals =
            Data.Read(pond, DataKeys.MetalsHeld)
                .ParseList<string>(';')
                .Select(li => li?.ParseTuple<string, int>())
                .WhereNotNull()
                .Where(li => !string.IsNullOrEmpty(li.Item1))
                .ToList();
        for (var i = heldMetals.Count - 1; i >= 0; i--)
        {
            var (id, timeLeft) = heldMetals[i];
            if (timeLeft > 0)
            {
                continue;
            }

            held.Add(id!.IsOreId()
                ? ItemRegistry.Create<SObject>(QualifiedObjectIds.RadioactiveOre, 1)
                : ItemRegistry.Create<SObject>(QualifiedObjectIds.RadioactiveBar, 1));
            heldMetals.RemoveAt(i);
        }

        Data.Write(
            pond,
            DataKeys.MetalsHeld,
            string.Join(';', heldMetals
                .Select(m => string.Join(',', m.Item1, m.Item2))));
    }

    private static void ProduceForTuiLa(FishPond pond, List<Item> held, string which, double chance, Random r)
    {
        if (pond.FishCount > 1)
        {
            if (r.NextDouble() < chance && r.NextDouble() < chance)
            {
                held.Add(ItemRegistry.Create<SObject>(QualifiedObjectIds.GalaxySoul, 1));
                return;
            }

            held.Add(ItemRegistry.Create<SObject>(which == "MNF.MoreNewFish_tui" ? QualifiedObjectIds.VoidEssence : QualifiedObjectIds.SolarEssence, 1));
            if (r.NextDouble() < 0.8)
            {
                held.Last().Stack++;
            }
        }

        held.Add(ItemRegistry.Create<SObject>(which == "MNF.MoreNewFish_tui" ? QualifiedObjectIds.SolarEssence : QualifiedObjectIds.VoidEssence, 1));
        if (r.NextDouble() < 0.8)
        {
            held.Last().Stack++;
        }
    }

    #endregion handlers
}
