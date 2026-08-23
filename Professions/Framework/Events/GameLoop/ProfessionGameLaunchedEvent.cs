namespace DaLion.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;
using static StardewValley.Menus.CharacterCustomization;
using CropCategory = DaLion.Core.Framework.CropCategory;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProfessionGameLaunchedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ProfessionGameLaunchedEvent(EventManager? manager = null)
    : GameLaunchedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        Lookups.ArtisanMachines.UnionWith(ModHelper.GameContent.Load<Dictionary<string, string[]>>($"{UniqueId}_ArtisanMachines")["ArtisanMachines"]);
        Lookups.AnimalDerivedGoods.UnionWith(ModHelper.GameContent.Load<Dictionary<string, string[]>>($"{UniqueId}_AnimalDerivedGoods")["AnimalDerivedGoods"]);
        if (Config.BeesAreAnimals)
        {
            Lookups.AnimalDerivedGoods.Add(QIDs.Honey);
            Lookups.AnimalDerivedGoods.Add(QIDs.Mead);
        }

        Lookups.MachineTreatments.AddRange(ModHelper.GameContent.Load<Dictionary<string, MachineTreatmentRules>>($"{UniqueId}_MachineTreatments"));

        var cropCategories = ModHelper.GameContent.Load<Dictionary<string, HashSet<string>>>($"{UniqueId}_CropCategories");
        Lookups.CropsByCategory[CropCategory.Grains] = cropCategories["GrainsCategory"];
        Lookups.CropsByCategory[CropCategory.LeafyGreens] = cropCategories["LeafyGreensCategory"];
        Lookups.CropsByCategory[CropCategory.Legumes] = cropCategories["LegumesCategory"];
        Lookups.CropsByCategory[CropCategory.Roots] = cropCategories["RootsCategory"];
        Lookups.CropsByCategory[CropCategory.Tubers] = cropCategories["TubersCategory"];
        Lookups.CropsByCategory[CropCategory.Gourds] = cropCategories["GourdsCategory"];

        var inverted = Lookups.CropsByCategory
            .SelectMany(pair => pair.Value.Select(value => new { value, key = pair.Key }))
            .ToDictionary(x => x.value, x => x.key);
        Lookups.CategoryByCrop.AddRange(inverted);

        var favoredFeedsData = ModHelper.GameContent.Load<Dictionary<string, string[]>>($"{UniqueId}_AnimalFavoredFeeds");
        Lookups.AnimalFavoredFeeds.AddRange(favoredFeedsData.ToDictionary(
            pair => pair.Key,
            pair => pair.Value
                .Select(value => Enum.Parse<CropCategory>(value, ignoreCase: true))
                .ToHashSet()));

        var reproductiveTypesData = ModHelper.GameContent.Load<Dictionary<string, string[]>>($"{UniqueId}_AnimalReproductiveTypes");
        Lookups.AnimalReproductiveTypes = new(reproductiveTypesData["Mammals"], reproductiveTypesData["EggLayers"]);
    }
}
