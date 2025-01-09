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
    /// <returns>The total values of agriculture activities, livestock and buildings on the <paramref name="farm"/>, as well as the total number of tiles used by all of those activities.</returns>
    internal static (int AgricultureValue, int LivestockValue, int ArtisanValue, int BuildingValue, int UsedTiles, int TreeCount) Appraise(this Farm farm)
    {
        var agricultureValue = 0;
        var livestockValue = 0;
        var artisanValue = 0;
        var buildingValue = 0;
        var treeCount = 0;
        var usedTiles = 45; // discount farmhouse tiles
        foreach (var dirt in farm.terrainFeatures.Values.OfType<HoeDirt>())
        {
            if (dirt.crop is not { } crop)
            {
                continue;
            }

            usedTiles++;
            var averageYield = (int)((crop.GetData().HarvestMinStack + crop.GetData().HarvestMaxStack) / 2f);
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
                    (int)((float)(28 - crop.phaseDays.TakeWhile(t => t != 99999).Sum()) / regrowth);
            }

            var harvestValue = harvest.salePrice() * averageYield * expectedHarvests;
            agricultureValue += harvestValue;
            switch (harvest.Category)
            {
                case SObject.FruitsCategory:
                    artisanValue += (int)(harvestValue * 3f);
                    break;
                case SObject.GreensCategory or SObject.VegetableCategory:
                    artisanValue += (int)(harvestValue * 2.25f);
                    break;
                default:
                    artisanValue += (int)(harvestValue * Config.DefaultArtisanValueCropMultiplier);
                    Log.T($"Unknown crop category '{harvest.Category}'. Using default artisan value multiplier.");
                    break;
            }
        }

        foreach (var fruitTree in farm.terrainFeatures.Values.OfType<FruitTree>())
        {
            usedTiles++;
            treeCount++;
            if (fruitTree.daysUntilMature.Value > 0)
            {
                continue;
            }

            var averageFruitValue = 0;
            var fruitData = fruitTree.GetData().Fruit;
            foreach (var fruit in fruitData)
            {
                var fruitObject = ItemRegistry.Create<SObject>(fruit.ItemId);
                var minStack = Math.Max(fruit.MinStack, 1);
                var maxStack = Math.Max(fruit.MaxStack, minStack);
                averageFruitValue += (int)((minStack + maxStack) / 2f * fruit.Chance * fruitObject.salePrice());
            }

            averageFruitValue /= fruitData.Count;
            var monthlyFruitValue = averageFruitValue * 28;
            agricultureValue += monthlyFruitValue;
            artisanValue += (int)(monthlyFruitValue * 3f);
        }

        foreach (var _ in farm.terrainFeatures.Values.OfType<Tree>())
        {
            usedTiles++;
            treeCount++;
        }

        foreach (var building in farm.buildings)
        {
            var buildingData = Game1.buildingData[building.buildingType.Value];
            usedTiles += buildingData.Size.X * buildingData.Size.Y;
            if (building.magical.Value && Config.ExemptMagicalBuildings)
            {
                continue;
            }

            blueprintAppraisal:
            buildingValue += buildingData.BuildCost;
            if (buildingData.BuildMaterials is not null)
            {
                foreach (var materialData in buildingData.BuildMaterials)
                {
                    var material = ItemRegistry.Create<SObject>(materialData.ItemId, materialData.Amount);
                    buildingValue += material.salePrice() * material.Stack;
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
                var animalData = animal.GetAnimalData();
                var animalProduce = animalData.ProduceItemIds.FirstOrDefault();
                if (animalProduce is null)
                {
                    continue;
                }

                var produceObject = ItemRegistry.Create<SObject>(animalProduce.ItemId);
                var expectedYield = (animal.isBaby() ? 28 - animalData.DaysToMature : 28) / animalData.DaysToProduce;
                if (expectedYield < 1)
                {
                    continue;
                }

                var produceValue = (produceObject.salePrice() * expectedYield);
                livestockValue += produceValue + animalData.SellPrice;

                float artisanMultiplier;
                switch (animal.type.Value)
                {
                    case "Duck":
                        artisanMultiplier = (float)ItemRegistry.Create<SObject>(QualifiedObjectIds.DuckMayonnaise).salePrice() /
                                            ItemRegistry.Create<SObject>(QualifiedObjectIds.DuckEgg).salePrice();
                        break;
                    case "Rabbit":
                    case "Sheep":
                        artisanMultiplier = (float)ItemRegistry.Create<SObject>(QualifiedObjectIds.Cloth).salePrice() /
                                            ItemRegistry.Create<SObject>(QualifiedObjectIds.Wool).salePrice();
                        break;
                    case "Goat":
                        artisanMultiplier = (float)ItemRegistry.Create<SObject>(QualifiedObjectIds.GoatCheese).salePrice() /
                                            ItemRegistry.Create<SObject>(QualifiedObjectIds.GoatMilk).salePrice();
                        break;
                    case "Pig":
                        artisanMultiplier = (float)ItemRegistry.Create<SObject>(QualifiedObjectIds.TruffleOil).salePrice() /
                                            ItemRegistry.Create<SObject>(QualifiedObjectIds.Truffle).salePrice();
                        break;
                    case "Ostrich":
                        artisanMultiplier = (float)ItemRegistry.Create<SObject>(QualifiedObjectIds.Mayonnaise).salePrice() * 10 /
                                            ItemRegistry.Create<SObject>(QualifiedObjectIds.OstrichEgg).salePrice();
                        break;
                    default:
                        if (animal.type.Value.Contains("Cow"))
                        {
                            artisanMultiplier = (float)ItemRegistry.Create<SObject>(QualifiedObjectIds.Cheese).salePrice() /
                                                ItemRegistry.Create<SObject>(QualifiedObjectIds.Milk).salePrice();
                        }
                        else if (animal.type.Value.Contains("Chicken"))
                        {
                            artisanMultiplier = (float)ItemRegistry.Create<SObject>(QualifiedObjectIds.Mayonnaise).salePrice() /
                                                ItemRegistry.Create<SObject>(QualifiedObjectIds.Egg_White).salePrice();
                        }
                        else
                        {
                            artisanMultiplier = Config.DefaultArtisanValueProduceMultiplier;
                            Log.T($"Unknown animal type '{animal.type.Value}'. Using default artisan value multiplier.");
                        }

                        break;
                }

                artisanValue += (int)(produceValue * artisanMultiplier);
                usedTiles++;
            }
        }

        foreach (var farmer in Game1.getAllFarmers())
        {
            if (farmer.HouseUpgradeLevel <= 0)
            {
                continue;
            }

            buildingValue += 10000;
            buildingValue += ItemRegistry.Create<SObject>(QualifiedObjectIds.Wood).Price * 450;
            if (farmer.HouseUpgradeLevel <= 1)
            {
                continue;
            }

            buildingValue += 50000;
            buildingValue += ItemRegistry.Create<SObject>(QualifiedObjectIds.Hardwood).Price * 150;
            if (farmer.HouseUpgradeLevel > 2)
            {
                buildingValue += 100000;
            }
        }

        return (agricultureValue, livestockValue, artisanValue, buildingValue, usedTiles, treeCount);
    }
}
