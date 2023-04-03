namespace DaLion.Overhaul.Modules.Taxes.Extensions;

#region using directives

using System.Linq;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Farm"/> class.</summary>
internal static class FarmExtensions
{
    /// <summary>Determines the total property value of the <paramref name="farm"/>.</summary>
    /// <param name="farm">The <see cref="Farm"/>.</param>
    /// <param name="forReal">Optional flag to avoid recording the new appraisal.</param>
    /// <returns>The total values of agriculture activities, livestock and buildings on the <paramref name="farm"/>, as well as the total number of tiles used by all of those activities.</returns>
    internal static (int AgricultureValue, int LivestockValue, int BuildingValue, int UsedTiles) Appraise(this Farm farm, bool forReal = true)
    {
        var totalAgricultureValue = 0;
        var totalLivestockValue = 0;
        var totalBuildingValue = 0;
        var usedTiles = 54;
        foreach (var dirt in farm.terrainFeatures.Values.OfType<HoeDirt>())
        {
            if (dirt.crop is not { } crop)
            {
                continue;
            }

            usedTiles++;
            var averageYield = (crop.minHarvest.Value + crop.maxHarvest.Value) / 2f;
            var harvest = !crop.forageCrop.Value
                ? new SObject(crop.indexOfHarvest.Value, 1)
                : crop.whichForageCrop.Value == Crop.forageCrop_springOnion
                    ? new SObject(ItemIDs.SpringOnion, 1)
                    : crop.whichForageCrop.Value == Crop.forageCrop_ginger
                        ? new SObject(ItemIDs.Ginger, 1) : null;
            if (harvest is null)
            {
                continue;
            }

            var expectedHarvests = 1;
            if (crop.regrowAfterHarvest.Value > 0)
            {
                expectedHarvests +=
                    (int)((float)(28 - Game1.dayOfMonth - crop.phaseDays.Sum()) / crop.regrowAfterHarvest.Value);
            }

            totalAgricultureValue += (int)(harvest.salePrice() * averageYield * expectedHarvests);
        }

        foreach (var fruitTree in farm.terrainFeatures.Values.OfType<FruitTree>())
        {
            usedTiles++;
            var fruit = new SObject(fruitTree.indexOfFruit.Value, 1);
            totalAgricultureValue += fruit.salePrice() * 28;
        }

        usedTiles += farm.terrainFeatures.Values.OfType<Tree>().Count();

        foreach (var building in farm.buildings)
        {
            var blueprint = new BluePrint(building.buildingType.Value);
            usedTiles += blueprint.tilesHeight * blueprint.tilesWidth;
            if (building.magical.Value && TaxesModule.Config.ExemptMagicalBuilding)
            {
                continue;
            }

            blueprintAppraisal:
            totalBuildingValue += blueprint.moneyRequired;
            foreach (var pair in blueprint.itemsRequired)
            {
                var material = new SObject(pair.Key, pair.Value);
                totalBuildingValue += material.Price * material.Stack;
            }

            if (blueprint.blueprintType == "Upgrade")
            {
                blueprint = new BluePrint(blueprint.nameOfBuildingToUpgrade);
                goto blueprintAppraisal;
            }

            if (building.indoors.Value is not AnimalHouse house)
            {
                continue;
            }

            foreach (var animal in house.Animals.Values)
            {
                var produce = new SObject(animal.defaultProduceIndex.Value, 1);
                totalLivestockValue += (int)(produce.salePrice() * (28f / animal.daysToLay.Value));
                usedTiles++;
            }
        }

        foreach (var farmer in Game1.getAllFarmers())
        {
            if (farmer.HouseUpgradeLevel <= 0)
            {
                continue;
            }

            totalBuildingValue += 10000;
            totalBuildingValue += new SObject(SObject.wood, 1).Price * 450;
            if (farmer.HouseUpgradeLevel <= 1)
            {
                continue;
            }

            totalBuildingValue += 50000;
            totalBuildingValue += new SObject(ItemIDs.Hardwood, 1).Price * 150;
            if (farmer.HouseUpgradeLevel > 2)
            {
                totalBuildingValue += 100000;
            }
        }

        var previousAgricultureValue = 0;
        var previousLiveStockValue = 0;
        var previousBuildingValue = 0;
        if (Game1.currentSeason != "spring" || Game1.dayOfMonth <= 8)
        {
            previousAgricultureValue = farm.Read<int>(DataKeys.AgricultureValue);
            previousLiveStockValue = farm.Read<int>(DataKeys.LivestockValue);
            previousBuildingValue = farm.Read<int>(DataKeys.BuildingValue);
        }

        if (previousAgricultureValue + previousLiveStockValue + previousBuildingValue > 0)
        {
            if (!SeasonExtensions.TryParse(Game1.currentSeason, true, out var currentSeason))
            {
                Log.E($"Failed to parse the current season {Game1.currentSeason}");
                return (-1, -1, -1, -1);
            }

            var weight = ((int)currentSeason * 2) + Game1.dayOfMonth > 8 ? 2 : 1;
            totalAgricultureValue = (int)((float)(totalAgricultureValue + previousAgricultureValue) / weight);
            totalLivestockValue = (int)((float)(totalLivestockValue + previousLiveStockValue) / weight);
            totalBuildingValue = (int)((float)(totalBuildingValue + previousBuildingValue) / weight);
        }

        if (forReal)
        {
            farm.Write(DataKeys.AgricultureValue, totalAgricultureValue.ToString());
            farm.Write(DataKeys.LivestockValue, totalLivestockValue.ToString());
            farm.Write(DataKeys.BuildingValue, totalBuildingValue.ToString());
            farm.Write(DataKeys.UsedTiles, usedTiles.ToString());
        }

        return (totalAgricultureValue, totalLivestockValue, totalBuildingValue, usedTiles);
    }
}
