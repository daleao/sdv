namespace DaLion.Ponds.Framework.Extensions;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;
using StardewValley.GameData.FishPonds;
using StardewValley.Menus;
using StardewValley.Mods;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
internal static class FishPondExtensions
{
    /// <summary>Determines whether a legendary fish lives in this <paramref name="pond"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="pond"/> houses a legendary fish species, otherwise <see langword="false"/>.</returns>
    internal static bool HasLegendaryFish(this FishPond pond)
    {
        return !string.IsNullOrEmpty(pond.fishType.Value) && Lookups.LegendaryFishes.Contains(pond.GetFishObject().QualifiedItemId);
    }

    /// <summary>Determines whether this <paramref name="pond"/> is infested with algae.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="pond"/> houses any algae, otherwise <see langword="false"/>.</returns>
    internal static bool HasAlgae(this FishPond pond)
    {
        return !string.IsNullOrEmpty(pond.fishType.Value) && pond.GetFishObject().IsAlgae();
    }

    /// <summary>Determines whether the <paramref name="pond"/>'s radioactivity is sufficient to enrich metallic nuclei.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="pond"/> houses a mutant or radioactive fish species, otherwise <see langword="false"/>.</returns>
    internal static bool IsRadioactive(this FishPond pond)
    {
        return !string.IsNullOrEmpty(pond.fishType.Value) && pond.GetFishObject().IsRadioactiveFish() && pond.FishCount > 1;
    }

    /// <summary>Gets the number of days required to enrich a given <paramref name="metal"/> resource.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="metal">An ore or bar <see cref="SObject"/>.</param>
    /// <returns>The number of days required to enrich the nucleus of the metal.</returns>
    internal static int GetEnrichmentTime(this FishPond pond, SObject metal)
    {
        var populationFactor = pond.FishCount / 3;
        if (populationFactor == 0)
        {
            return 0;
        }

        var days = 0;
        if (metal.Name.Contains("Copper"))
        {
            days = 13;
        }
        else if (metal.Name.Contains("Iron"))
        {
            days = 8;
        }
        else if (metal.Name.Contains("Gold"))
        {
            days = 5;
        }
        else if (metal.Name.Contains("Iridium"))
        {
            days = 3;
        }

        return Math.Max(days - populationFactor, 1);
    }

    /// <summary>Gives the player fishing experience for harvesting the <paramref name="pond"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="who">The player.</param>
    internal static void RewardExp(this FishPond pond, Farmer who)
    {
        if (Data.ReadAs<bool>(pond, DataKeys.CheckedToday))
        {
            return;
        }

        var bonus = (int)(pond.output.Value is SObject obj
            ? obj.sellToStorePrice() * FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER
            : 0);
        who.gainExperience(Farmer.fishingSkill, FishPond.HARVEST_BASE_EXP + bonus);
    }

    /// <summary>
    ///     Opens an <see cref="ItemGrabMenu"/> instance to allow retrieve multiple items from the
    ///     <paramref name="pond"/>'s chum bucket.
    /// </summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="who">The <see cref="Farmer"/> interacting with the <paramref name="pond"/>.</param>
    /// <returns>Always <see langword="true"/> (required by vanilla code).</returns>
    internal static bool OpenChumBucketMenu(this FishPond pond, Farmer who)
    {
        var held = pond.DeserializeHeldItems();
        if (held.Count == 0)
        {
            if (who.addItemToInventoryBool(pond.output.Value))
            {
                Game1.playSound("coin");
                pond.output.Value = null;
            }
            else
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            }
        }
        else
        {
            var inventory = new List<Item> { pond.output.Value };
            try
            {
                foreach (var item in held)
                {
                    if (item.QualifiedItemId == QualifiedObjectIds.Roe)
                    {
                        var roe = ItemRegistry.GetObjectTypeDefinition().CreateFlavoredRoe(pond.GetFishObject());
                        roe.Quality = ((SObject)item).Quality;
                        inventory.Add(roe);
                    }
                    else
                    {
                        inventory.Add(item);
                    }
                }

                Utility.consolidateStacks(inventory);
                var menu = new ItemGrabMenu(inventory, pond).setEssential(false);
                menu.source = ItemGrabMenu.source_fishingChest;
                Game1.activeClickableMenu = menu;
            }
            catch (InvalidOperationException ex)
            {
                Log.W($"ItemsHeld data is invalid. {ex}\nThe data will be reset");
                Data.Write(pond, DataKeys.ItemsHeld, null);
            }
        }

        Data.Write(pond, DataKeys.CheckedToday, true.ToString());
        return true; // expected by vanilla code
    }

    /// <summary>
    ///     Reads the serialized held item list from the <paramref name="pond"/>'s <seealso cref="ModDataDictionary"/> and
    ///     returns a deserialized <see cref="List{T}"/> of <seealso cref="SObject"/>s.
    /// </summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="Item"/>s encoded in the <paramref name="pond"/>'s held items data.</returns>
    internal static List<Item> DeserializeHeldItems(this FishPond pond)
    {
        return Data.Read(pond, DataKeys.ItemsHeld)
            .ParseList<string>(';')
            .Select(s => s?.ParseTuple<string, int, int>())
            .WhereNotNull()
            .Select(t => ItemRegistry.Create(t.Item1, t.Item2, t.Item3))
            .ToList();
    }

    /// <summary>Gets a fish's chance to produce roe in this <paramref name="pond"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="value">The fish's sale value.</param>
    /// <returns>The percentage chance of a fish with the given <paramref name="value"/> to produce roe in this <paramref name="pond"/>.</returns>
    internal static double GetRoeChance(this FishPond pond, int? value)
    {
        const int maxValue = 700;
        var cappedValue = Math.Min(value ?? pond.GetFishObject().Price, maxValue);

        // Mean daily roe value (/w Aquarist profession) by fish value
        // assuming regular-quality roe and fully-populated pond:
        //     30g -> ~324g (~90% roe chance per fish)
        //     700g -> ~1512g (~18% roe chance per fish) <-- capped here
        //     5000g -> ~4050g (~13.5% roe chance per fish)
        const double a = 335d / 4d;
        const double b = 275d / 2d;
        var neighbors = pond.FishCount - 1;
        return a / (cappedValue + b) * (1d + ((neighbors - 1) / 11d)) * Config.RoeProductionChanceMultiplier;
    }

    /// <summary>Sets the spawn time to account for Angler couples.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="data">The <see cref="FishPondData"/>, if available.</param>
    internal static void SetAnglerSpawnTime(this FishPond pond, FishPondData? data = null)
    {
        // enable reproduction for Mr. and Ms. Angler
        var anglers = pond.ParsePondFishes();
        var males = anglers.Where(f => f?.Id == "160");
        var females = anglers.Where(f => f?.Id == "899");
        var mates = Math.Min(males.Count(), females.Count());
        if (mates == 0)
        {
            return;
        }

        data ??= pond.GetFishPondData();
        data.SpawnTime = 18 / mates;
    }

    /// <summary>Parses the stored <see cref="PondFish"/> data from this <paramref name="pond"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns>A <see cref="List"/> of parsed <see cref="PondFish"/>.</returns>
    internal static List<PondFish> ParsePondFishes(this FishPond pond)
    {
        return Data.Read(pond, DataKeys.PondFish).Split(';').Select(PondFish.FromString).WhereNotNull().ToList();
    }


    /// <summary>Resets the <see cref="PondFish"/> data back to default values, effectively erasing stores qualities.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    internal static void ResetPondFishData(this FishPond pond)
    {
        var fish = Enumerable.Repeat(new PondFish(pond.fishType.Value, SObject.lowQuality), pond.FishCount);
        if (pond.HasLegendaryFish() &&
            Data.ReadAs<int>(pond, "FamilyLivingHere", modId: "DaLion.Professions") is var familyLivingHere &&
            familyLivingHere > 0)
        {
            fish = fish
                .Take(pond.FishCount - familyLivingHere)
                .Concat(Enumerable.Repeat(
                    new PondFish(Lookups.FamilyPairs[$"(O){pond.fishType.Value}"], SObject.lowQuality),
                    familyLivingHere))
                .ToList();
        }

        Data.Write(pond, DataKeys.PondFish, string.Join(';', fish));
    }
}
