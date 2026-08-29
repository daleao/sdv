namespace DaLion.Professions.Framework.Events.GameLoop.DayEnding;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="NutritionDayEndingEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class NutritionDayEndingEvent(EventManager? manager = null)
    : DayEndingEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        Utility.ForEachLocation(location =>
        {
            var feedsPerCategory = Data.Read(location, DataKeys.PiecesOfFeed).ParseDictionary<CropCategory, int>();
            foreach (var animal in location.animals.Values)
            {
                if (animal is null)
                {
                    continue;
                }

                var shortTermNutrition = Data.ReadAs<int>(animal, DataKeys.ShortTermNutrition);
                var longTermNutrition = Data.ReadAs<int>(animal, DataKeys.LongTermNutrition);
                if (!State.WasFedCropToday.Contains(animal))
                {
                    if (Lookups.AnimalFavoredFeeds.TryGetValue(animal.GetAnimalType(), out var favoredFeeds))
                    {
                        if (favoredFeeds.FirstOrDefault(feedsPerCategory.ContainsKey) is { } first)
                        {
                            feedsPerCategory[first]--;
                            if (animal.fullness.Value > 200)
                            {
                                shortTermNutrition += 25;
                                longTermNutrition += 10;
                            }
                        }
                    }
                    else
                    {
                        if (animal.fullness.Value < 200)
                        {
                            shortTermNutrition -= 50;
                        }
                        else
                        {
                            shortTermNutrition -= 10;
                        }
                    }
                }
                else if (animal.fullness.Value > 200)
                {
                    shortTermNutrition += 25;
                    longTermNutrition += 10;
                }

                var nutritionCeiling = animal.DoesOwnerHaveProfessionOrLax(Profession.Producer) ? 200 : 100;
                shortTermNutrition = Math.Clamp(shortTermNutrition, 0, nutritionCeiling);
                longTermNutrition = Math.Min(longTermNutrition, 500);
                Data.Write(animal, DataKeys.ShortTermNutrition, shortTermNutrition.ToString());
                Data.Write(animal, DataKeys.LongTermNutrition, longTermNutrition.ToString());
            }

            Data.Write(location, DataKeys.PiecesOfFeed, feedsPerCategory.Stringify());
            return true;
        });

        State.WasFedCropToday.Clear();
    }
}
