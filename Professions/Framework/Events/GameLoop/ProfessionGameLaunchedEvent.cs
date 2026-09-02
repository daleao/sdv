namespace DaLion.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using Newtonsoft.Json.Linq;
using StardewModdingAPI.Events;
using static DaLion.Shared.Pathfinding.MovingTargetDStarLite;

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
        foreach (var (itemId, data) in Game1.bigCraftableData)
        {
            if (data.ContextTags is not null && data.ContextTags.Contains($"{UniqueId}_artisan_machine") && !Lookups.ArtisanMachines.Contains(itemId))
            {
                Lookups.ArtisanMachines.Add($"(BC){itemId}");
            }
        }

        Lookups.AnimalDerivedGoods.UnionWith(ModHelper.GameContent.Load<Dictionary<string, string[]>>($"{UniqueId}_AnimalDerivedGoods")["AnimalDerivedGoods"]);
        if (Config.BeesAreAnimals)
        {
            Lookups.AnimalDerivedGoods.Add(QIDs.Honey);
            Lookups.AnimalDerivedGoods.Add(QIDs.Mead);
        }

        foreach (var (itemId, data) in Game1.objectData)
        {
            if (data.ContextTags is not null && data.ContextTags.Contains($"{UniqueId}_animal_derived_good") && !Lookups.AnimalDerivedGoods.Contains(itemId))
            {
                Lookups.AnimalDerivedGoods.Add($"(O){itemId}");
            }
        }

        var reproductiveTypesData = ModHelper.GameContent.Load<Dictionary<string, HashSet<string>>>($"{UniqueId}_AnimalReproductiveTypes");
        var mammals = reproductiveTypesData["Mammals"].ToHashSet(StringComparer.OrdinalIgnoreCase);
        var eggLayers = reproductiveTypesData["EggLayers"].ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var (animalId, data) in Game1.farmAnimalData)
        {
            if (animalId.Contains("Chicken", StringComparison.OrdinalIgnoreCase) ||
                animalId.Contains("Cow", StringComparison.OrdinalIgnoreCase) ||
                mammals.Contains(animalId) || eggLayers.Contains(animalId) ||
                data.CustomFields?.TryGetValue($"{UniqueId}_ReproductiveType", out var type) != true)
            {
                continue;
            }

            switch (type)
            {
                case "Mammal":
                case "Mammals":
                    mammals.Add(animalId);
                    break;
                case "EggLayer":
                case "EggLayers":
                    eggLayers.Add(animalId);
                    break;
                default:
                    Log.W($"Failed to classify {animalId}'s reproductive type. Unknown value '{type}'.");
                    break;
            }
        }

        Lookups.AnimalReproductiveTypes = new([.. mammals], [.. eggLayers]);

        var feedsByCategory = ModHelper.GameContent.Load<Dictionary<string, HashSet<string>>>($"{UniqueId}_FeedsByCategory");
        foreach (var (categoryId, feeds) in feedsByCategory)
        {
            var category = FeedCategoryRegistry.GetOrRegister(categoryId);
            if (!Lookups.FeedsByCategory.TryAdd(category, feeds))
            {
                Lookups.FeedsByCategory[category].UnionWith(feeds);
            }
        }

        foreach (var (itemId, data) in Game1.objectData)
        {
            if (data.CustomFields is null || !data.CustomFields.TryGetValue($"{UniqueId}_FeedCategory", out var categoryId))
            {
                continue;
            }

            var category = FeedCategoryRegistry.GetOrRegister(categoryId);
            if (!Lookups.FeedsByCategory.TryAdd(category, [itemId]))
            {
                Lookups.FeedsByCategory[category].Add(itemId);
            }
        }

        var categoryByFeed = Lookups.FeedsByCategory
            .SelectMany(pair => pair.Value.Select(value => new { value, key = pair.Key }))
            .ToDictionary(x => x.value, x => x.key);
        Lookups.CategoryByFeed.AddRange(categoryByFeed);

        var favoredFeedsData = ModHelper.GameContent.Load<Dictionary<string, HashSet<string>>>($"{UniqueId}_AnimalFavoredFeeds");
        Lookups.FavoredFeedsByAnimalType.AddRange(favoredFeedsData.ToDictionary(
            pair => pair.Key,
            pair => pair.Value
                .Select(value => FeedCategoryRegistry.GetOrRegister(value))
                .ToHashSet()));

        foreach (var (animalId, data) in Game1.farmAnimalData)
        {
            if (data.CustomFields is null || !data.CustomFields.TryGetValue($"{UniqueId}_FavoredFeeds", out var favoredString))
            {
                continue;
            }

            var categories = favoredString.Split(',');
            foreach (var categoryId in categories)
            {
                var category = FeedCategoryRegistry.GetOrRegister(categoryId);
                if (!Lookups.FavoredFeedsByAnimalType.TryAdd(animalId, [category]))
                {
                    Lookups.FavoredFeedsByAnimalType[animalId].Add(category);
                }
            }
        }

        var catalystsByTreatment = ModHelper.GameContent.Load<Dictionary<string, HashSet<string>>>($"{UniqueId}_MachineTreatmentCatalysts");
        foreach (var (treatmentId, catalysts) in catalystsByTreatment)
        {
            var treatment = MachineTreatmentRegistry.GetOrRegister(treatmentId);
            Lookups.CatalystsByTreatment.TryAdd(treatment, catalysts);
        }

        foreach (var (itemId, data) in Game1.objectData)
        {
            if (data.CustomFields is null || !data.CustomFields.TryGetValue($"{UniqueId}_IsCatalystFor", out var treatmentId))
            {
                continue;
            }

            var treatment = MachineTreatmentRegistry.GetOrRegister(treatmentId);
            if (!Lookups.CatalystsByTreatment.TryAdd(treatment, [itemId]))
            {
                Lookups.CatalystsByTreatment[treatment].Add(itemId);
            }
        }

        var treatmentByCatalyst = Lookups.CatalystsByTreatment
            .SelectMany(pair => pair.Value.Select(value => new { value, key = pair.Key }))
            .ToDictionary(x => x.value, x => x.key);
        Lookups.TreatmentByCatalyst.AddRange(treatmentByCatalyst);

        Lookups.MachineTreatments.AddRange(ModHelper.GameContent.Load<Dictionary<string, MachineTreatmentRules>>($"{UniqueId}_MachineTreatments"));
        foreach (var (itemId, data) in Game1.bigCraftableData)
        {
            if (data.CustomFields is null || !data.CustomFields.TryGetValue($"{UniqueId}_MachineTreatments", out var treatmentsData))
            {
                //treatmentsData = "default,fermentation;juice_item,glazing";
                continue;
            }

            try
            {
                var parsed = treatmentsData.ParseDictionary<string, string>();
                var @default = MachineTreatmentRegistry.None;
                Dictionary<string, MachineTreatment> overrides = [];
                foreach (var (key, value) in parsed)
                {
                    var treatment = MachineTreatmentRegistry.GetOrRegister(value);
                    if (key == "default")
                    {
                        @default = treatment;
                    }
                    else
                    {
                        overrides[key] = treatment;
                    }
                }

                var rules = new MachineTreatmentRules(@default, overrides);
                if (Lookups.MachineTreatments.TryAdd($"{itemId}", rules))
                {
                    continue;
                }

                var existing = Lookups.MachineTreatments[itemId];
                if (existing.Default != rules.Default)
                {
                    Log.D($"The new default value '{rules.Default}' will be ignored." +
                        $" Already set to '{existing.Default}'.");
                }

                foreach (var @override in rules.Overrides)
                {
                    if (!existing.Overrides.TryAdd(@override.Key, @override.Value))
                    {
                        continue;
                    }

                    Log.D($"The override key '{@override.Key}' will be ignored." +
                    $" Already set to '{existing.Overrides[@override.Key]}'.");
                }
            }
            catch (InvalidOperationException)
            {
                Log.W($"Failed to parse machine rules from custom field data of {itemId}.");
                continue;
            }
        }
    }
}
