namespace DaLion.Professions.Framework.Events.GameLoop.DayStarted;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="NutritionDayStartedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class NutritionDayStartedEvent(EventManager? manager = null)
    : DayStartedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        Utility.ForEachLocation(location =>
        {
            if (location is not AnimalHouse house)
            {
                return true;
            }

            var parent = house.GetParentLocation();
            var storedFeedsPerCategory = Data.Read(parent, DataKeys.PiecesOfFeed).ParseDictionary<string, int>();
            foreach (var animal in house.animals.Values)
            {
                if (animal is null)
                {
                    Log.T($"Found null animal in {house.Name}?");
                    continue;
                }

                Data.Write(animal, DataKeys.WasSupplementedToday, "false".ToString());
                if (!house.HasMapPropertyWithValue("AutoFeed") || !storedFeedsPerCategory.Any(feed => feed.Value > 0))
                {
                    continue;
                }

                if (!Lookups.FavoredFeedsByAnimalType.TryGetValue(animal.type.Value, out var favoredFeeds))
                {
                    if (animal.type.Value.Contains("Chicken", StringComparison.OrdinalIgnoreCase))
                    {
                        favoredFeeds = Lookups.FavoredFeedsByAnimalType["Chicken"];
                    }
                    else if (animal.type.Value.Contains("Cow", StringComparison.OrdinalIgnoreCase))
                    {
                        favoredFeeds = Lookups.FavoredFeedsByAnimalType["Cow"];
                    }
                    else
                    {
                        favoredFeeds = [];
                    }
                }

                if (favoredFeeds.FirstOrDefault(category => storedFeedsPerCategory.TryGetValue(category.Id, out var feeds) &&
                    feeds > 0) is { } favored && !string.IsNullOrEmpty(favored.Id))
                {
                    storedFeedsPerCategory[favored.Id]--;
                    if (storedFeedsPerCategory[favored.Id] <= 0)
                    {
                        storedFeedsPerCategory.Remove(favored.Id);
                    }

                    Data.Write(animal, DataKeys.WasSupplementedToday, "true".ToString());
                }
            }

            Data.Write(parent, DataKeys.PiecesOfFeed, storedFeedsPerCategory.Stringify());
            return true;
        });
    }
}
