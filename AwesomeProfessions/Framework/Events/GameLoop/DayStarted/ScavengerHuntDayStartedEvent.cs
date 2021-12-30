using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class ScavengerHuntDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    public override void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        if (ModState.ScavengerHunt is not null) ModState.ScavengerHunt.ResetAccumulatedBonus();
    }
}