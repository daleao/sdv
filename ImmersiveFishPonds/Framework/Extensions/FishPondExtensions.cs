namespace DaLion.Stardew.FishPonds.Framework.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using StardewValley.Objects;

using Common.Extensions;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
internal static class FishPondExtensions
{
    private static readonly Func<int, double> _productionChanceByValue = x => (double) 14765 / (x + 120) + 1.5;
    private static readonly FieldInfo _FishPondData = typeof(FishPond).Field("_fishPondData");

    /// <summary>Whether the instance's population has been fully unlocked.</summary>
    internal static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var fishPondData = (FishPondData) _FishPondData.GetValue(pond);
        return fishPondData?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= fishPondData.PopulationGates.Keys.Max();
    }

    /// <summary>Whether a legendary fish lives in this pond.</summary>
    internal static bool IsLegendaryPond(this FishPond pond)
    {
        return pond.GetFishObject().HasContextTag("fish_legendary");
    }

    /// <summary>Whether this pond is infested with algae.</summary>
    internal static bool IsAlgaePond(this FishPond pond)
    {
        return pond.fishType.Value.IsAlgae();
    }

    /// <summary>Increase Roe/Ink stack and quality based on population size and average quality.</summary>
    internal static void GetIndividualFishProduce(this FishPond pond)
    {
        var r = new Random(Guid.NewGuid().GetHashCode());
        var produceDict = new Dictionary<int, int>();
        if (pond.IsAlgaePond())
        {
            var seaweedCount = 0;
            for (var i = 0; i < pond.ReadDataAs<int>("SeaweedLivingHere"); ++i)
            {
                if (r.NextDouble() < Utility.Lerp(0.15f, 0.95f, pond.currentOccupants.Value / 10f))
                    ++seaweedCount;
            }

            var greenAlgaeCount = 0;
            for (var i = 0; i < pond.ReadDataAs<int>("GreenAlgaeLivingHere"); ++i)
            {
                if (r.NextDouble() < Utility.Lerp(0.15f, 0.95f, pond.currentOccupants.Value / 10f))
                    ++greenAlgaeCount;
            }

            var whiteAlgaeCount = 0;
            for (var i = 0; i < pond.ReadDataAs<int>("WhiteAlgaeLivingHere"); ++i)
            {
                if (r.NextDouble() < Utility.Lerp(0.15f, 0.95f, pond.currentOccupants.Value / 10f))
                    ++whiteAlgaeCount;
            }

            if (seaweedCount > 0) produceDict[Constants.SEAWEED_INDEX_I] = seaweedCount;
            if (greenAlgaeCount > 0) produceDict[Constants.GREEN_ALGAE_INDEX_I] = greenAlgaeCount;
            if (whiteAlgaeCount > 0) produceDict[Constants.WHITE_ALGAE_INDEX_I] = whiteAlgaeCount;

            pond.output.Value = pond.fishType.Value switch
            {
                Constants.SEAWEED_INDEX_I when seaweedCount > 0 => new SObject(Constants.SEAWEED_INDEX_I, 1),
                Constants.GREEN_ALGAE_INDEX_I when greenAlgaeCount > 0 => new SObject(Constants.GREEN_ALGAE_INDEX_I, 1),
                Constants.WHITE_ALGAE_INDEX_I when whiteAlgaeCount > 0 => new SObject(Constants.WHITE_ALGAE_INDEX_I, 1),
                _ => seaweedCount > 0 ? new SObject(Constants.SEAWEED_INDEX_I, 1) :
                    greenAlgaeCount > 0 ? new SObject(Constants.GREEN_ALGAE_INDEX_I, 1) :
                    whiteAlgaeCount > 0 ? new SObject(Constants.WHITE_ALGAE_INDEX_I, 1) : null
            };
            pond.WriteData("ItemsHeld", produceDict.Stringify());
            return;
        }

        var fishPondData = pond.GetFishPondData();
        if (fishPondData is null) return;

        for (var i = 0; i < pond.currentOccupants.Value; ++i)
        {
            foreach (var item in fishPondData.ProducedItems.Where(item =>
                         item.ItemID is not Constants.ROE_INDEX_I or Constants.SQUID_INK_INDEX_I &&
                         pond.currentOccupants.Value >= item.RequiredPopulation &&
                         r.NextDouble() < Utility.Lerp(0.15f, 0.95f, pond.currentOccupants.Value / 10f) &&
                         r.NextDouble() < item.Chance))
            {
                var stack = r.Next(item.MinQuantity, item.MaxQuantity + 1);
                if (produceDict.ContainsKey(item.ItemID)) produceDict[item.ItemID] += stack;
                else produceDict[item.ItemID] = stack;
            }
        }

        var fish = pond.GetFishObject();
        var productionChancePerFish = _productionChanceByValue(fish.Price) / 100;
        var roeStack = 0;
        for (var i = 0; i < pond.FishCount; ++i)
            if (r.NextDouble() < productionChancePerFish) ++roeStack;

        if (fish.ParentSheetIndex == Constants.STURGEON_INDEX_I)
        {
            for (var i = 0; i < roeStack; ++i)
                if (r.NextDouble() < Utility.Lerp(0.15f, 0.95f, pond.currentOccupants.Value / 10f))
                    ++roeStack;
        }

        if (roeStack > 0)
        {
            int roeIndex;
            if (fish.Name == "Coral")
            {
                for (var i = 0; i < roeStack; ++i)
                {
                    roeIndex = r.NextDouble() < 0.25
                        ? Constants.WHITE_ALGAE_INDEX_I
                        : r.Next(Constants.SEAWEED_INDEX_I, Constants.GREEN_ALGAE_INDEX_I + 1);
                    if (produceDict.ContainsKey(roeIndex)) ++produceDict[roeIndex];
                    else produceDict[roeIndex] = 1;
                }
            }
            else if (fish.Name.Contains("Squid"))
            {
                roeIndex = Constants.SQUID_INK_INDEX_I;
                produceDict[roeIndex] = roeStack;
            }
            else
            {
                roeIndex = Constants.ROE_INDEX_I;
                produceDict[roeIndex] = roeStack;
            }
        }

        if (!produceDict.Any())
        {
            pond.output.Value = null;
            return;
        }

        var toDisplay = produceDict
            .Select(p => new SObject(p.Key, 1))
            .OrderByDescending(o => o.Price)
            .First();
        if (toDisplay.ParentSheetIndex == Constants.ROE_INDEX_I)
        {
            var split = Game1.objectInformation[pond.fishType.Value].Split('/');
            var c = TailoringMenu.GetDyeColor(pond.GetFishObject()) ??
                    (pond.fishType.Value == 698 ? new(61, 55, 42) : Color.Orange);
            var o = new ColoredObject(Constants.ROE_INDEX_I, 1, c)
            {
                name = split[0] + " Roe",
                preserve =
                {
                    Value = SObject.PreserveType.Roe
                },
                preservedParentSheetIndex =
                {
                    Value = pond.fishType.Value
                }
            };

            toDisplay = o;
        }

        pond.output.Value = toDisplay;
        pond.WriteData("ItemsHeld", produceDict.Stringify());
    }

    /// <summary>Determine the amount of fish of each quality currently in this pond.</summary>
    internal static (int, int, int) GetFishQualities(this FishPond pond, int qualityRating = -1, bool forFamily = false)
    {
        if (qualityRating < 0) qualityRating = pond.ReadDataAs<int>(forFamily ? "FamilyQualityRating" : "QualityRating");

        var numBestQuality = qualityRating / 4096; // 16^3
        qualityRating -= numBestQuality * 4096;

        var numHighQuality = qualityRating / 256; // 16^2
        qualityRating -= numHighQuality * 256;

        var numMedQuality = qualityRating / 16;

        return (numBestQuality, numHighQuality, numMedQuality);
    }

    /// <summary>Determine which quality should be deducted from the total quality rating after fishing in this pond.</summary>
    internal static int GetLowestFishQuality(this FishPond pond, int qualityRating = -1, bool forFamily = false)
    {
        var (numBestQuality, numHighQuality, numMedQuality) = GetFishQualities(pond, qualityRating, forFamily);
        var familyCount = pond.ReadDataAs<int>("FamilyLivingHere");
        return numBestQuality + numHighQuality + numMedQuality < (forFamily ? familyCount : pond.FishCount - familyCount + 1) // fish pond count has already been deducted at this point, so we consider +1
            ? SObject.lowQuality
            : numMedQuality > 0
                ? SObject.medQuality
                : numHighQuality > 0
                    ? SObject.highQuality
                    : SObject.bestQuality;
    }

    /// <summary>Choose the quality value for today's produce by parsing stored quality rating data.</summary>
    /// <param name="r">A random number generator.</param>
    private static int GetRoeQuality(this FishPond pond, Random r)
    {
        var (numBestQuality, numHighQuality, numMedQuality) = pond.GetFishQualities();
        if (pond.IsLegendaryPond())
        {
            var (numBestFamilyQuality, numHighFamilyQuality, numMedFamilyQuality) = pond.GetFishQualities(forFamily: true);
            numBestQuality += numBestFamilyQuality;
            numHighQuality += numHighFamilyQuality;
            numMedQuality += numMedFamilyQuality;
        }

        var roll = r.Next(pond.FishCount);
        return roll < numBestQuality
            ? SObject.bestQuality
            : roll < numBestQuality + numHighQuality
                ? SObject.highQuality
                : roll < numBestQuality + numHighQuality + numMedQuality
                    ? SObject.medQuality
                    : SObject.lowQuality;
    }

    /// <summary>Opens a <see cref="ItemGrabMenu"/> instance to allow retrieve multiple items from the pond's chum bucket.</summary>
    /// <returns>Always returns <c>True</c> (required by the game).</returns>
    internal static bool OpenChumBucketMenu(this FishPond pond)
    {
        var produceDict = pond.ReadData("ItemsHeld").ToDictionary<int, int>();
        var produceList = new List<Item>();
        var r = new Random(Guid.NewGuid().GetHashCode());
        foreach (var (index, stack) in produceDict)
        {
            SObject o;
            if (index == Constants.ROE_INDEX_I)
            {
                var split = Game1.objectInformation[pond.fishType.Value].Split('/');
                var c = TailoringMenu.GetDyeColor(pond.GetFishObject()) ??
                        (pond.fishType.Value == 698 ? new(61, 55, 42) : Color.Orange);
                o = new ColoredObject(Constants.ROE_INDEX_I, 1, c);
                o.name = split[0] + " Roe";
                o.preserve.Value = SObject.PreserveType.Roe;
                o.preservedParentSheetIndex.Value = pond.fishType.Value;
                o.Price += Convert.ToInt32(split[1]) / 2;
                o.Quality = pond.GetRoeQuality(r);
                o.Stack = stack;
            }
            else
            {
                o = new(index, stack);
            }

            produceList.Add(o);
        }

        Game1.activeClickableMenu = new ItemGrabMenu(produceList, pond).setEssential(false);
        ((ItemGrabMenu)Game1.activeClickableMenu).source = ItemGrabMenu.source_fishingChest;
        
        pond.WriteData("ItemsHeld", null);
        pond.output.Value = null;
        return true;
    }
}