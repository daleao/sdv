using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.DayStarted;

internal class ScavengerHuntDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object sender, DayStartedEventArgs e)
    {
        if (ModEntry.State.Value.ScavengerHunt is not null)
            ModEntry.State.Value.ScavengerHunt.ResetAccumulatedBonus();
    }
}