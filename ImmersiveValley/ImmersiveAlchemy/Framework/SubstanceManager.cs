namespace DaLion.Stardew.Alchemy.Framework;

#region using directives

using System.Linq;
using System.Collections.Generic;
using StardewModdingAPI;

using Enums;

using SObject = StardewValley.Object;

#endregion using directives

internal static class SubstanceManager
{
    /// <summary>Cache of loaded ingredients and contained substances.</summary>
    internal static readonly Dictionary<string, Composition> Ingredients = new()
    {
        // flowers
        {"Sweet Pea", new(PrimarySubstance.Rebis, SubstanceDensity.Weak, null)},
        {"Crocus", new(PrimarySubstance.Aether, SubstanceDensity.Weak, null)},
        {"Sunflower", new(PrimarySubstance.Hydragenum, SubstanceDensity.Weak, null)},
        {"Tulip", new(PrimarySubstance.Rebis, SubstanceDensity.Weak, null)},
        {"Summer Spangle", new(PrimarySubstance.Quebrith, SubstanceDensity.Weak, null)},
        {"Fairy Rose", new(PrimarySubstance.Aether, SubstanceDensity.Weak, null)},
        {"Blue Jazz", new(PrimarySubstance.Vitriol, SubstanceDensity.Weak, null)},
        {"Poppy", new(PrimarySubstance.Vermilion, SubstanceDensity.Weak, null)},

        // forage
        {"Daffodil", new(PrimarySubstance.Hydragenum, SubstanceDensity.Weak, null)},
        {"Dandelion", new(PrimarySubstance.Quebrith, SubstanceDensity.Weak, null)},
        {"Leek", new(PrimarySubstance.Vitriol, SubstanceDensity.Weak, null)},
        {"Spring Onion", new(PrimarySubstance.Rebis, SubstanceDensity.Weak, null)},
        {"Wild Horseradish", new(PrimarySubstance.Vermilion, SubstanceDensity.Weak, null)},
        {"Fiddlehead Fern", new(PrimarySubstance.Rebis, SubstanceDensity.Weak, null)},
        {"Spice Berry", new(PrimarySubstance.Vermilion, SubstanceDensity.Weak, null)},
        {"Wild Plum", new(PrimarySubstance.Quebrith, SubstanceDensity.Weak, null)},
        {"Crystal Fruit", new(PrimarySubstance.Hydragenum, SubstanceDensity.Weak, null)},
        {"Snow Yam", new(PrimarySubstance.Vitriol, SubstanceDensity.Weak, null)},
        {"Winter Root", new(PrimarySubstance.Rebis, SubstanceDensity.Weak, null)},
        {"Holly", new(PrimarySubstance.Vermilion, SubstanceDensity.Weak, null)},
        {"Cave Carrot", new(PrimarySubstance.Aether, SubstanceDensity.Weak, null)},
        {"Ginger", new(PrimarySubstance.Quebrith, SubstanceDensity.Moderate, null)},

        // mushrooms
        {"Common Mushroom", new(PrimarySubstance.Rebis, SubstanceDensity.Weak, null)},
        {"Morel", new(PrimarySubstance.Vitriol, SubstanceDensity.Weak, null)},
        {"Chanterelle", new(PrimarySubstance.Hydragenum, SubstanceDensity.Weak, null)},
        {"Red Mushroom", new(PrimarySubstance.Quebrith, SubstanceDensity.Weak, null)},
        {"Purple Mushroom", new(PrimarySubstance.Aether, SubstanceDensity.Weak, null)},
        {"Magma Cap", new(PrimarySubstance.Vermilion, SubstanceDensity.Weak, null)},

        // gems
        {"Emerald", new(PrimarySubstance.Rebis, SubstanceDensity.Moderate, null)},
        {"Aquamarine", new(PrimarySubstance.Vitriol, SubstanceDensity.Moderate, null)},
        {"Ruby", new(PrimarySubstance.Vermilion, SubstanceDensity.Moderate, null)},
        {"Amethyst", new(PrimarySubstance.Aether, SubstanceDensity.Moderate, null)},
        {"Topaz", new(PrimarySubstance.Quebrith, SubstanceDensity.Moderate, null)},
        {"Jade", new(PrimarySubstance.Rebis, SubstanceDensity.Moderate, null)},
        {"Diamond", new(PrimarySubstance.Hydragenum, SubstanceDensity.Rich, null)},

        // minerals
        {"Tigerseye", new(PrimarySubstance.Vermilion, SubstanceDensity.Rich, null)},
        {"Opal", new(PrimarySubstance.Vitriol, SubstanceDensity.Rich, null)},
        {"Fire Opal", new(PrimarySubstance.Aether, SubstanceDensity.Rich, null)},
        {"Alamite", new(PrimarySubstance.Hydragenum, SubstanceDensity.Rich, null)},
        {"Bixite", new(PrimarySubstance.Hydragenum, SubstanceDensity.Rich, null)},
        {"Baryte", new(PrimarySubstance.Vermilion, SubstanceDensity.Rich, null)},
        {"Aerinite", new(PrimarySubstance.Hydragenum, SubstanceDensity.Rich, null)},
        {"Calcite", new(PrimarySubstance.Quebrith, SubstanceDensity.Rich, null)},
        {"Dolomite", new(PrimarySubstance.Vitriol, SubstanceDensity.Rich, null)},
        {"Esperite", new(PrimarySubstance.Aether, SubstanceDensity.Rich, null)},
        {"Fluorapatite", new(PrimarySubstance.Aether, SubstanceDensity.Rich, null)},
        {"Germinite", new(PrimarySubstance.Rebis, SubstanceDensity.Rich, null)},
        {"Helvite", new(PrimarySubstance.Vermilion, SubstanceDensity.Rich, null)},
        {"Jamborite", new(PrimarySubstance.Rebis, SubstanceDensity.Rich, null)},
        {"Jagoite", new(PrimarySubstance.Quebrith, SubstanceDensity.Rich, null)},
        {"Kyanite", new(PrimarySubstance.Vitriol, SubstanceDensity.Rich, null)},
        {"Lunarite", new(PrimarySubstance.Hydragenum, SubstanceDensity.Rich, null)},
        {"Malachite", new(PrimarySubstance.Rebis, SubstanceDensity.Rich, null)},
        {"Neptunite", new(PrimarySubstance.Aether, SubstanceDensity.Rich, null)},
        {"Lemon Stone", new(PrimarySubstance.Vermilion, SubstanceDensity.Rich, null)},
        {"Nekoite", new(PrimarySubstance.Aether, SubstanceDensity.Rich, null)},
        {"Orpiment", new(PrimarySubstance.Vermilion, SubstanceDensity.Rich, null)},
        {"Petrified Slime", new(PrimarySubstance.Rebis, SubstanceDensity.Rich, null)},
        {"Thunder Egg", new(PrimarySubstance.Aether, SubstanceDensity.Rich, null)},
        {"Pyrite", new(PrimarySubstance.Quebrith, SubstanceDensity.Rich, null)},
        {"Ocean Stone", new(PrimarySubstance.Vitriol, SubstanceDensity.Rich, null)},
        {"Ghost Crystal", new(PrimarySubstance.Hydragenum, SubstanceDensity.Rich, null)},
        {"Jasper", new(PrimarySubstance.Vermilion, SubstanceDensity.Rich, null)},
        {"Celestine", new(PrimarySubstance.Hydragenum, SubstanceDensity.Rich, null)},
        {"Marble", new(PrimarySubstance.Quebrith, SubstanceDensity.Rich, null)},
        {"Sandstone", new(PrimarySubstance.Quebrith, SubstanceDensity.Rich, null)},
        {"Granite", new(PrimarySubstance.Quebrith, SubstanceDensity.Rich, null)},
        {"Basalt", new(PrimarySubstance.Quebrith, SubstanceDensity.Rich, null)},
        {"Limestone", new(PrimarySubstance.Rebis, SubstanceDensity.Rich, null)},
        {"Soapstone", new(PrimarySubstance.Vitriol, SubstanceDensity.Rich, null)},
        {"Hematine", new(PrimarySubstance.Quebrith, SubstanceDensity.Rich, null)},
        {"Mudstone", new(PrimarySubstance.Vitriol, SubstanceDensity.Rich, null)},
        {"Obsidian", new(PrimarySubstance.Aether, SubstanceDensity.Rich, null)},
        {"Slate", new(PrimarySubstance.Vitriol, SubstanceDensity.Rich, null)},
        {"Fairy Stone", new(PrimarySubstance.Aether, SubstanceDensity.Rich, null)},
        {"Star Shard", new(PrimarySubstance.Hydragenum, SubstanceDensity.Rich, null)},

        // forage minerals
        {"Quartz", new(PrimarySubstance.Hydragenum, SubstanceDensity.Moderate, null)},
        {"Earth Crystal", new(PrimarySubstance.Quebrith, SubstanceDensity.Moderate, null)},
        {"Frozen Tea", new(PrimarySubstance.Vitriol, SubstanceDensity.Moderate, null)},
        {"Fire Quartz", new(PrimarySubstance.Vermilion, SubstanceDensity.Moderate, null)},

        // other
        {"Dinosaur Egg", new(PrimarySubstance.Rebis, SubstanceDensity.Weak, null)}
    };

    /// <summary>Cache of loaded alchemical bases.</summary>
    internal static readonly Dictionary<string, (BaseType Type, BasePurity Purity)> Bases = new()
    {
        // alcohols
        {"Methanol", (BaseType.Alcohol, BasePurity.High)},
        {"Ethanol", (BaseType.Alcohol, BasePurity.High)},
        {"Amaretto", (BaseType.Alcohol, BasePurity.Low)},
        {"Bitter", (BaseType.Alcohol, BasePurity.Standard)},
        {"Bourbon", (BaseType.Alcohol, BasePurity.Standard)},
        {"Brandy", (BaseType.Alcohol, BasePurity.Standard)},
        {"Campari", (BaseType.Alcohol, BasePurity.Low)},
        {"Chambord", (BaseType.Alcohol, BasePurity.Low)},
        {"Cointreau", (BaseType.Grease, BasePurity.Standard)},
        {"Creme de Cacao", (BaseType.Alcohol, BasePurity.Low)},
        {"Frangelico", (BaseType.Alcohol, BasePurity.Low)},
        {"Gin", (BaseType.Alcohol, BasePurity.Standard)},
        {"Mezcal", (BaseType.Alcohol, BasePurity.Standard)},
        {"Moonshine", (BaseType.Alcohol, BasePurity.Standard)},
        {"Rum", (BaseType.Alcohol, BasePurity.Standard)},
        {"Scotch", (BaseType.Alcohol, BasePurity.Standard)},
        {"Shochu", (BaseType.Alcohol, BasePurity.Low)},
        {"Tequila", (BaseType.Alcohol, BasePurity.Standard)},
        {"Vermouth", (BaseType.Alcohol, BasePurity.Low)},
        {"Vodka", (BaseType.Alcohol, BasePurity.Low)},
        {"Whiskey", (BaseType.Alcohol, BasePurity.Standard)},

        // oils
        {"Oil", (BaseType.Grease, BasePurity.Low)},
        {"Truffle Oil", (BaseType.Grease, BasePurity.Standard)},
        {"Coconut Oil", (BaseType.Grease, BasePurity.Standard)},
        {"Cottonseed Oil", (BaseType.Grease, BasePurity.Standard)},
        {"Olive Oil", (BaseType.Grease, BasePurity.Standard)},
        {"Peanut Oil", (BaseType.Grease, BasePurity.Standard)},
        {"Sesame Oil", (BaseType.Grease, BasePurity.Standard)},
        {"Ancient Olive Oil", (BaseType.Grease, BasePurity.High)},

        // butters
        {"Butter", (BaseType.Grease, BasePurity.Low)},
        {"Goat Butter", (BaseType.Grease, BasePurity.Low)},
        {"Slime Butter", (BaseType.Grease, BasePurity.Low)},
        {"Margarine", (BaseType.Grease, BasePurity.Low)},
        {"Almond Butter", (BaseType.Grease, BasePurity.Low)},
        {"Cashew Butter", (BaseType.Grease, BasePurity.Low)},
        {"Coconut Butter", (BaseType.Grease, BasePurity.Low)},
        {"Hazelnut Butter", (BaseType.Grease, BasePurity.Low)},
        {"Pecan Butter", (BaseType.Grease, BasePurity.Low)},
        {"Smooth Peanut Butter", (BaseType.Grease, BasePurity.Low)},
        {"Soynut Butter", (BaseType.Grease, BasePurity.Low)},
        {"Sunflower Butter", (BaseType.Grease, BasePurity.Low)},
        {"Walnut Butter", (BaseType.Grease, BasePurity.Low)},
        {"Ancient Nut Butter", (BaseType.Grease, BasePurity.Standard)},

        // powders
        {"Coal", (BaseType.Powder, BasePurity.Low)}
    };
    
    /// <summary>Load content packs and cache ingredients and bases.</summary>
    /// <param name="helper">API for managing content packs.</param>
    internal static void Init(IContentPackHelper helper)
    {
        foreach (var pack in helper.GetOwned())
        {
            foreach (var (name, composition) in TryLoadIngredients(pack))
                Ingredients[name] = composition;

            foreach (var (name, type, purity) in TryLoadAlchemyBases(pack))
                Bases[name] = (type, purity);
        }

        if (!Ingredients.Any())
            Log.W(
                "There were no ingredients loaded! You won't be able to create mixtures until you download some content packs.");
    }

    /// <summary>Look for valid ingredients in the content pack.</summary>
    /// <param name="pack">A content pack.</param>
    internal static IEnumerable<(string, Composition)> TryLoadIngredients(IContentPack pack)
    {
        var ingredients = pack.ReadJsonFile<List<(string name, Composition composition)>>("ingredients.json");
        if (ingredients is not null) return ingredients;
        
        Log.T($"No valid ingredients are defined in the content pack {pack.Manifest.UniqueID}.");
        return new List<(string, Composition)>();
    }

    /// <summary>Look for valid alchemical bases in the content pack.</summary>
    /// <param name="pack">A content pack.</param>
    internal static IEnumerable<(string, BaseType, BasePurity)> TryLoadAlchemyBases(IContentPack pack)
    {
        var bases = pack.ReadJsonFile<List<(string name, BaseType type, BasePurity purity)>>("bases.json");
        if (bases is not null) return bases;

        Log.T($"No valid bases are defined in the content pack {pack.Manifest.UniqueID}.");
        return new List<(string, BaseType, BasePurity)>();
    }
}