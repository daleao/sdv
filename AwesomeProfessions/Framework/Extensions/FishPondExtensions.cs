namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using StardewValley.Objects;

using Common.Extensions;

using SObject = StardewValley.Object;

#endregion using directives

public static class FishPondExtensions
{
    private const int ROE_INDEX_I = 812;
    private const int SQUID_INK_INDEX_I = 814;
    private const int SEAWEED_INDEX_I = 152;
    private const int ALGAE_INDEX_I = 153;

    private static readonly Func<int, double> _productionChanceByValue = x => (double) 14765 / (x + 120) + 1.5;

    public static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var fishPondData = ModEntry.ModHelper.Reflection.GetField<FishPondData>(pond, "_fishPondData").GetValue();
        return fishPondData?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= fishPondData.PopulationGates.Keys.Max();
    }

    /// <summary>Increase Roe/Ink stack and quality based on population size and average quality.</summary>
    public static void AddBonusRoeAmountAndQuality(this FishPond pond)
    {
        var produce = pond.output.Value as SObject;
        if (produce is not null && !produce.ParentSheetIndex.IsAnyOf(ROE_INDEX_I, SQUID_INK_INDEX_I)) return;

        var fish = pond.GetFishObject();
        
        var r = new Random(Guid.NewGuid().GetHashCode());
        var quality = pond.GetRoeQuality(r);
        if (produce is not null) produce.Quality = quality;

        var bonusStack = 0;
        var productionChancePerFish = _productionChanceByValue(fish.Price) / 100;
        for (var i = 0; i < pond.FishCount; ++i)
            if (r.NextDouble() < productionChancePerFish)
                ++bonusStack;

        if (bonusStack <= 0) return;

        if (produce is null)
        {
            int produceId;
            if (fish.Name.Contains("Squid"))
                produceId = SQUID_INK_INDEX_I;
            else if (fish.Name != "Coral")
                produceId = ROE_INDEX_I;
            else
                produceId = r.Next(SEAWEED_INDEX_I, ALGAE_INDEX_I + 1);

            if (produceId != ROE_INDEX_I)
            {
                produce = new(produceId, bonusStack);
            }
            else
            {
                var split = Game1.objectInformation[pond.fishType.Value].Split('/');
                var c = TailoringMenu.GetDyeColor(pond.GetFishObject()) ??
                        (pond.fishType.Value == 698 ? new(61, 55, 42) : Color.Orange);
                produce = new ColoredObject(ROE_INDEX_I, 1, c);
                produce.name = split[0] + " Roe";
                produce.preserve.Value = SObject.PreserveType.Roe;
                produce.preservedParentSheetIndex.Value = pond.fishType.Value;
                produce.Price += Convert.ToInt32(split[1]) / 2;
                produce.Stack = bonusStack;
            }

            produce.Quality = quality;
        }
        else
        {
            produce.Stack += bonusStack;
        }

        pond.output.Value = produce;
    }

    /// <summary>Determine which quality should be deducted from the total quality rating after fishing in this pond.</summary>
    public static int GetLowestFishQuality(this FishPond pond)
    {
        var who = Game1.getFarmerMaybeOffline(pond.owner.Value) ?? Game1.MasterPlayer;
        var qualityRatingByFishPond =
            ModData.Read(DataField.QualityRatingByFishPond, who).ToDictionary<int, int>(",", ";");
        var thisFishPond = pond.GetCenterTile().ToString().GetDeterministicHashCode();
        qualityRatingByFishPond.TryGetValue(thisFishPond, out var qualityRatingForThisFishPond);

        var currentRating = qualityRatingForThisFishPond;
        var numBestQuality = currentRating / 4096; // 16^3
        currentRating -= numBestQuality * 4096;

        var numHighQuality = currentRating / 256; // 16^2
        currentRating -= numHighQuality * 256;

        var numMedQuality = currentRating / 16;

        return numBestQuality + numHighQuality + numMedQuality < pond.FishCount
            ? SObject.lowQuality
            : numMedQuality > 0
                ? SObject.medQuality
                : numHighQuality > 0
                    ? SObject.highQuality
                    : SObject.bestQuality;
    }

    /// <summary>Determine the amount of fish of each quality currently in this pond.</summary>
    public static (int, int, int) GetAllFishQualities(this FishPond pond)
    {
        var who = Game1.getFarmerMaybeOffline(pond.owner.Value) ?? Game1.MasterPlayer;
        var qualityRatingByFishPond =
            ModData.Read(DataField.QualityRatingByFishPond, who).ToDictionary<int, int>(",", ";");
        var thisFishPond = pond.GetCenterTile().ToString().GetDeterministicHashCode();
        qualityRatingByFishPond.TryGetValue(thisFishPond, out var qualityRatingForThisFishPond);

        var currentRating = qualityRatingForThisFishPond;
        var numBestQuality = currentRating / 4096; // 16^3
        currentRating -= numBestQuality * 4096;

        var numHighQuality = currentRating / 256; // 16^2
        currentRating -= numHighQuality * 256;

        var numMedQuality = currentRating / 16;

        return (numBestQuality, numHighQuality, numMedQuality);
    }

    /// <summary>Choose the quality value for today's produce by parsing stored quality rating data.</summary>
    /// <param name="r">A random number generator.</param>
    private static int GetRoeQuality(this FishPond pond, Random r)
    {
        var who = Game1.getFarmerMaybeOffline(pond.owner.Value) ?? Game1.MasterPlayer;
        var qualityRatingByFishPond =
            ModData.Read(DataField.QualityRatingByFishPond, who).ToDictionary<int, int>(",", ";");
        var thisFishPond = pond.GetCenterTile().ToString().GetDeterministicHashCode();
        qualityRatingByFishPond.TryGetValue(thisFishPond, out var qualityRatingForThisFishPond);

        var currentRating = qualityRatingForThisFishPond;
        var numBestQuality = currentRating / 4096; // 16^3
        currentRating -= numBestQuality * 4096;

        var numHighQuality = currentRating / 256; // 16^2
        currentRating -= numHighQuality * 256;

        var numMedQuality = currentRating / 16;

        var roll = r.Next(pond.FishCount + 1);
        return roll < numBestQuality
            ? SObject.bestQuality
            : roll < numBestQuality + numHighQuality
                ? SObject.highQuality
                : roll < numBestQuality + numHighQuality + numMedQuality
                    ? SObject.medQuality
                    : SObject.lowQuality;
    }
}