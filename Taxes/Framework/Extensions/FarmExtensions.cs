namespace DaLion.Taxes.Framework.Extensions;

#region using directives

using System.Linq;
using StardewValley;
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
            var averageYield = (crop.GetData().HarvestMinStack + crop.GetData().HarvestMaxStack) / 2f;
            var harvest = !crop.forageCrop.Value
                ? ItemRegistry.Create<SObject>(crop.indexOfHarvest.Value)
                : crop.whichForageCrop.Value == Crop.forageCrop_springOnionID
                    ? ItemRegistry.Create<SObject>(QualifiedObjectIds.SpringOnion)
                    : crop.whichForageCrop.Value == Crop.forageCrop_gingerID
                        ? ItemRegistry.Create<SObject>(QualifiedObjectIds.Ginger) : null;
            if (harvest is null)
            {
                continue;
            }

            var expectedHarvests = 1;
            if (crop.GetData().RegrowDays is { } regrowth and > 0)
            {
                expectedHarvests +=
                    (int)((float)(28 - Game1.dayOfMonth - crop.phaseDays.TakeWhile(t => t != 99999).Sum()) / regrowth);
            }

            totalAgricultureValue += (int)(harvest.salePrice() * averageYield * expectedHarvests);
        }

        foreach (var fruitTree in farm.terrainFeatures.Values.OfType<FruitTree>())
        {
            usedTiles++;
            var averageFruitValue = 0f;
            var fruitData = fruitTree.GetData().Fruit;
            foreach (var fruit in fruitData)
            {
                var fruitObject = ItemRegistry.Create<SObject>(fruit.ItemId);
                averageFruitValue += (fruit.MinStack + fruit.MaxStack) / 2f * fruit.Chance * fruitObject.salePrice();
            }

            averageFruitValue /= fruitData.Count;
            totalAgricultureValue += (int)(averageFruitValue * 28);
        }

        usedTiles += farm.terrainFeatures.Values.OfType<Tree>().Count();

        foreach (var building in farm.buildings)
        {
            var buildingData = Game1.buildingData[building.buildingType.Value];
            usedTiles += buildingData.Size.X * buildingData.Size.Y;
            if (building.magical.Value && Config.ExemptMagicalBuildings)
            {
                continue;
            }

            blueprintAppraisal:
            totalBuildingValue += buildingData.BuildCost;
            if (buildingData.BuildMaterials is not null)
            {
                foreach (var materialData in buildingData.BuildMaterials)
                {
                    var material = ItemRegistry.Create<SObject>(materialData.ItemId, materialData.Amount);
                    totalBuildingValue += material.salePrice() * material.Stack;
                }
            }

            if (buildingData.BuildingToUpgrade is not null)
            {
                buildingData = Game1.buildingData[buildingData.BuildingToUpgrade];
                goto blueprintAppraisal;
            }

            if (building.indoors.Value is not AnimalHouse house)
            {
                continue;
            }

            foreach (var animal in house.Animals.Values)
            {
                var averageProduceValue = 0f;
                var animalData = animal.GetAnimalData();
                var produceData = animalData.ProduceItemIds;
                foreach (var produce in produceData)
                {
                    var produceObject = ItemRegistry.Create<SObject>(produce.ItemId);
                    averageProduceValue += (float)produceObject.salePrice() / animalData.DaysToProduce;
                }

                averageProduceValue /= produceData.Count;
                totalLivestockValue += (int)(averageProduceValue * 28) + animalData.SellPrice;
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
            totalBuildingValue += ItemRegistry.Create<SObject>(QualifiedObjectIds.Wood).Price * 450;
            if (farmer.HouseUpgradeLevel <= 1)
            {
                continue;
            }

            totalBuildingValue += 50000;
            totalBuildingValue += ItemRegistry.Create<SObject>(QualifiedObjectIds.Hardwood).Price * 150;
            if (farmer.HouseUpgradeLevel > 2)
            {
                totalBuildingValue += 100000;
            }
        }

        var currentSeason = Game1.season;
        var weight = ((int)currentSeason * 2) + (Game1.dayOfMonth > 8 ? 2 : 1);
        var previousAgricultureValue = Data.ReadAs<int>(farm, DataKeys.AgricultureValue);
        var previousLiveStockValue = Data.ReadAs<int>(farm, DataKeys.LivestockValue);
        var previousBuildingValue = Data.ReadAs<int>(farm, DataKeys.BuildingValue);
        if (previousAgricultureValue + previousLiveStockValue + previousBuildingValue > 0)
        {
            if (currentSeason != Season.Winter)
            {
                totalAgricultureValue = (int)((float)(totalAgricultureValue + previousAgricultureValue) / weight);
            }

            totalLivestockValue = (int)((float)(totalLivestockValue + previousLiveStockValue) / weight);
            totalBuildingValue = (int)((float)(totalBuildingValue + previousBuildingValue) / weight);
        }

        var previousUsedTiles = Data.ReadAs<int>(farm, DataKeys.UsedTiles);
        if (currentSeason != Season.Winter)
        {
            usedTiles = (int)((float)(usedTiles + previousUsedTiles) / weight);
        }

        if (!forReal)
        {
            return (totalAgricultureValue, totalLivestockValue, totalBuildingValue, usedTiles);
        }

        Data.Write(farm, DataKeys.AgricultureValue, totalAgricultureValue.ToString());
        Data.Write(farm, DataKeys.LivestockValue, totalLivestockValue.ToString());
        Data.Write(farm, DataKeys.BuildingValue, totalBuildingValue.ToString());
        Data.Write(farm, DataKeys.UsedTiles, usedTiles.ToString());
        return (totalAgricultureValue, totalLivestockValue, totalBuildingValue, usedTiles);
    }
}
