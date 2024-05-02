namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            if (fish.QualifiedItemId == QualifiedObjectIds.Coral)
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

            var fishId = fish.QualifiedItemId;
            if (fish.IsBossFish() && random.NextDouble() <
                Data.ReadAs<double>(__instance, DataKeys.FamilyLivingHere, modId: "DaLion.Professions") / __instance.FishCount)
            {
                fishId = Lookups.FamilyPairs.TryGet(fishId, out var pairIndex) ? pairIndex : fishId;
            }

            var split = Game1.objectData[fishId];
            var c = fishId == QualifiedObjectIds.Sturgeon
                ? new Color(61, 55, 42)
                : TailoringMenu.GetDyeColor(ItemRegistry.Create<SObject>(fishId)) ?? Color.Orange;
            var roe = ItemRegistry.GetObjectTypeDefinition().CreateFlavoredRoe(fish);
            roe.Stack = __result.Stack;
            roe.Quality = __result.Quality;
            __result = roe;

            return false; // don't run original logic
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            Data.Write(__instance, DataKeys.FishQualities, $"{__instance.FishCount},0,0,0");
            Data.Write(__instance, DataKeys.FamilyQualities, null);
            Data.Write(__instance, DataKeys.FamilyLivingHere, null);
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
            if (reward.ItemId is not (QualifiedObjectIds.Roe or QualifiedObjectIds.SquidInk) &&
                pond.currentOccupants.Value >= reward.RequiredPopulation &&
                r.NextDouble() < Utility.Lerp(0.15f, 0.95f, pond.currentOccupants.Value / 10f) &&
                r.NextDouble() < reward.Chance)
            {
                held.Add(new SObject(reward.ItemId, r.Next(reward.MinQuantity, reward.MaxQuantity + 1)));
            }
        }
    }

    private static void ProduceRoe(FishPond pond, SObject fish, List<Item> held, Random r)
    {
        var fishQualities = Data.Read(
                pond,
                DataKeys.FishQualities,
                $"{pond.FishCount - Data.ReadAs<int>(pond, DataKeys.FamilyLivingHere, modId: "DaLion.Professions")},0,0,0")
            .ParseList<int>();
        if (fishQualities.Count != 4)
        {
            ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
        }

        var familyQualities =
            Data.Read(pond, DataKeys.FamilyQualities, "0,0,0,0").ParseList<int>();
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

        if (fish.QualifiedItemId == QualifiedObjectIds.Sturgeon)
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

        var roeIndex = fish.Name.Contains("Squid") ? QualifiedObjectIds.SquidInk : QualifiedObjectIds.Roe;
        for (var i = 3; i >= 0; i--)
        {
            if (producedRoes[i] <= 0)
            {
                continue;
            }

            var producedWithThisQuality = Config.RoeAlwaysFishQuality
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
        var population = Data.ReadAs<int>(pond, DataKeys.GreenAlgaeLivingHere);
        var chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[0]++;
            }
        }

        population = Data.ReadAs<int>(pond, DataKeys.WhiteAlgaeLivingHere);
        chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[1]++;
            }
        }

        population = Data.ReadAs<int>(pond, DataKeys.SeaweedLivingHere);
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
                held.Add(new SObject(QualifiedObjectIds.GreenAlgae, algaeStacks[0]));
            }

            if (algaeStacks[1] > 0)
            {
                held.Add(new SObject(QualifiedObjectIds.WhiteAlgae, algaeStacks[1]));
            }

            if (algaeStacks[2] > 0)
            {
                held.Add(new SObject(QualifiedObjectIds.Seaweed, algaeStacks[2]));
            }

            result = pond.fishType.Value switch
            {
                QualifiedObjectIds.GreenAlgae when algaeStacks[0] > 0 => new SObject(
                    QualifiedObjectIds.GreenAlgae,
                    algaeStacks[0]),
                QualifiedObjectIds.WhiteAlgae when algaeStacks[1] > 0 => new SObject(
                    QualifiedObjectIds.WhiteAlgae,
                    algaeStacks[1]),
                QualifiedObjectIds.Seaweed when algaeStacks[2] > 0 => new SObject(
                    QualifiedObjectIds.Seaweed,
                    algaeStacks[2]),
                _ => null,
            };

            if (result is null)
            {
                var max = algaeStacks.ToList().IndexOfMax();
                result = max switch
                {
                    0 => new SObject(QualifiedObjectIds.GreenAlgae, algaeStacks[0]),
                    1 => new SObject(QualifiedObjectIds.WhiteAlgae, algaeStacks[1]),
                    2 => new SObject(QualifiedObjectIds.Seaweed, algaeStacks[2]),
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
            held.Add(new SObject(QualifiedObjectIds.GreenAlgae, algaeStacks[0]));
        }

        if (algaeStacks[1] > 0)
        {
            held.Add(new SObject(QualifiedObjectIds.WhiteAlgae, algaeStacks[1]));
        }

        if (algaeStacks[2] > 0)
        {
            held.Add(new SObject(QualifiedObjectIds.Seaweed, algaeStacks[2]));
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
                ? new SObject(QualifiedObjectIds.RadioactiveOre, 1)
                : new SObject(QualifiedObjectIds.RadioactiveBar, 1));
            heldMetals.RemoveAt(i);
        }

        Data.Write(
            pond,
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
                held.Add(new SObject(QualifiedObjectIds.GalaxySoul, 1));
                return;
            }

            held.Add(new SObject(which == 1127 ? QualifiedObjectIds.VoidEssence : QualifiedObjectIds.SolarEssence, 1));
            if (r.NextDouble() < 0.8)
            {
                held.Last().Stack++;
            }
        }

        held.Add(new SObject(which == 1127 ? QualifiedObjectIds.SolarEssence : QualifiedObjectIds.VoidEssence, 1));
        if (r.NextDouble() < 0.8)
        {
            held.Last().Stack++;
        }
    }

    #endregion handlers
}
