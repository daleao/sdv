namespace DaLion.Professions.Framework.Events.GameLoop.DayEnding;

#region using directives

using DaLion.Shared.Events;
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
            if (location is not AnimalHouse house)
            {
                return true;
            }

            foreach (var animal in house.animals.Values)
            {
                if (animal is null)
                {
                    Log.T($"Found null animal in {house.Name}?");
                    continue;
                }

                var shortTermNutrition = Data.ReadAs<int>(animal, DataKeys.ShortTermNutrition);
                var longTermNutrition = Data.ReadAs<int>(animal, DataKeys.LongTermNutrition);
                if (Data.ReadAs<bool>(animal, DataKeys.WasSupplementedToday) && animal.fullness.Value > 200)
                {
                    shortTermNutrition += 25;
                    longTermNutrition += 10;
                }
                else if (animal.fullness.Value > 200) {
                    shortTermNutrition -= 10;
                }
                else if (animal.fullness.Value < 200)
                {
                    shortTermNutrition -= 50;
                }

                const int longTermNutritionCap = 500;
                var shortTermNutritionCap = animal.DoesOwnerHaveProfessionOrLax(Profession.Producer) ? 200 : 100;
                shortTermNutrition = Math.Clamp(shortTermNutrition, 0, shortTermNutritionCap);
                longTermNutrition = Math.Min(longTermNutrition, longTermNutritionCap);
                Data.Write(animal, DataKeys.ShortTermNutrition, shortTermNutrition.ToString());
                Data.Write(animal, DataKeys.LongTermNutrition, longTermNutrition.ToString());
            }

            return true;
        });
    }
}
