#pragma warning disable CS1591
namespace DaLion.Shared.Enums;

public enum ObjectCategory
{
    /// <summary>The category for vegetables.</summary>
    Vegetables = SObject.VegetableCategory,

    /// <summary>The category for fruits.</summary>
    Fruits = SObject.FruitsCategory,

    /// <summary>The category for flowers.</summary>
    Flowers = SObject.flowersCategory,

    /// <summary>The category for gemstones.</summary>
    Gems = SObject.GemCategory,

    /// <summary>The category for fish.</summary>
    Fish = SObject.FishCategory,

    /// <summary>The category for eggs.</summary>
    Eggs = SObject.EggCategory,

    /// <summary>The category for milk.</summary>
    Milk = SObject.MilkCategory,

    /// <summary>The category for cooking ingredients.</summary>
    Cooking = SObject.CookingCategory,

    /// <summary>The category for crafting materials.</summary>
    Crafting = SObject.CraftingCategory,

    /// <summary>The category for minerals.</summary>
    Minerals = SObject.mineralsCategory,

    /// <summary>The category for meats (unused in vanilla).</summary>
    Meats = SObject.meatCategory,

    /// <summary>The category for metal ores and bars.</summary>
    Metals = SObject.metalResources,

    /// <summary>The category for junk items.</summary>
    Junk = SObject.junkCategory,

    /// <summary>The category for saps and syrups.</summary>
    Syrups = SObject.syrupCategory,

    /// <summary>The category for monster loot.</summary>
    MonsterLoot = SObject.monsterLootCategory,

    /// <summary>The category for artisan goods.</summary>
    ArtisanGoods = SObject.artisanGoodsCategory,

    /// <summary>The category for seeds.</summary>
    Seeds = SObject.SeedsCategory,

    /// <summary>The category for rings.</summary>
    Rings = SObject.ringCategory,

    /// <summary>The category for animal goods.</summary>
    AnimalGoods = SObject.sellAtPierresAndMarnies,

    /// <summary>The category for foraged goods.</summary>
    Greens = SObject.GreensCategory,

    /// <summary>Special case category for artifacts.</summary>
    Artifacts = int.MinValue,
}
