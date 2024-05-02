namespace DaLion.Shared.Enums;

/// <summary>A <see cref="SObject"/> category.</summary>
public enum ObjectCategory
{
    /// <summary>The category for gemstones.</summary>
    Gems = SObject.GemCategory, // -2

    /// <summary>The category for fish.</summary>
    Fish = SObject.FishCategory, // -4

    /// <summary>The category for eggs.</summary>
    Eggs = SObject.EggCategory, // -5

    /// <summary>The category for milk.</summary>
    Milk = SObject.MilkCategory, // -6

    /// <summary>The category for cooking ingredients.</summary>
    Cooking = SObject.CookingCategory, // -7

    /// <summary>The category for crafting materials.</summary>
    Crafting = SObject.CraftingCategory, // -8

    /// <summary>The category for minerals.</summary>
    Minerals = SObject.mineralsCategory, // -12

    /// <summary>The category for meats (unused in vanilla).</summary>
    Meats = SObject.meatCategory, // -14

    /// <summary>The category for metal ores and bars.</summary>
    Metals = SObject.metalResources, // -15

    /// <summary>The category for animal goods.</summary>
    AnimalGoods = SObject.sellAtPierresAndMarnies, // -18

    /// <summary>The category for junk items.</summary>
    Junk = SObject.junkCategory, // -20

    /// <summary>The category for artisan goods.</summary>
    ArtisanGoods = SObject.artisanGoodsCategory, // -26

    /// <summary>The category for saps and syrups.</summary>
    Syrups = SObject.syrupCategory, // -27

    /// <summary>The category for monster loot.</summary>
    MonsterLoot = SObject.monsterLootCategory, // -28

    /// <summary>The category for seeds.</summary>
    Seeds = SObject.SeedsCategory, // -74

    /// <summary>The category for vegetables.</summary>
    Vegetables = SObject.VegetableCategory, // -75

    /// <summary>The category for fruits.</summary>
    Fruits = SObject.FruitsCategory, // -79

    /// <summary>The category for flowers.</summary>
    Flowers = SObject.flowersCategory, // -80

    /// <summary>The category for foraged goods.</summary>
    Greens = SObject.GreensCategory, // -81

    /// <summary>The category for rings.</summary>
    Rings = SObject.ringCategory, // -96

    /// <summary>Special case category for artifacts.</summary>
    Artifacts = int.MinValue,
}
