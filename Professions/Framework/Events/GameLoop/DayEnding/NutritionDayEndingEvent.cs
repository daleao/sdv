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
            foreach (var animal in location.animals.Values)
            {
                if (animal is null)
                {
                    continue;
                }

                var nutrition = Data.ReadAs<int>(animal, DataKeys.ShortTermNutrition);
                if (!State.WasFedCropToday.Contains(animal))
                {
                    if (animal.fullness.Value < 200)
                    {
                        nutrition -= 50;
                    }
                    else
                    {
                        nutrition -= 10;
                    }
                }
                else if (animal.fullness.Value > 200)
                {
                    nutrition += 25;
                }

                var nutritionCeiling = animal.DoesOwnerHaveProfessionOrLax(Profession.Producer) ? 200 : 100;
                nutrition = Math.Clamp(nutrition, 0, nutritionCeiling);
                Data.Write(animal, DataKeys.ShortTermNutrition, nutrition.ToString());
            }

            return true;
        });

        State.WasFedCropToday.Clear();
    }
}
