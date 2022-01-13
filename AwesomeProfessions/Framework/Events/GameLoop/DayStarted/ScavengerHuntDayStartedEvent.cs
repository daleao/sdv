using StardewModdingAPI.Events;

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

internal class ScavengerHuntDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object sender, DayStartedEventArgs e)
    {
        if (ModEntry.State.Value.ScavengerHunt is not null)
            ModEntry.State.Value.ScavengerHunt.ResetAccumulatedBonus();
    }
}