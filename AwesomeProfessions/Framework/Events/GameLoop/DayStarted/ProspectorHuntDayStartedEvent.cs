using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.DayStarted;

internal class ProspectorHuntDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object sender, DayStartedEventArgs e)
    {
        if (ModEntry.State.Value.ProspectorHunt is not null)
            ModEntry.State.Value.ProspectorHunt.ResetAccumulatedBonus();
    }
}