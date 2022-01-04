using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.DayStarted;

internal class ProspectorHuntDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    public override void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        if (ModEntry.State.Value.ProspectorHunt is not null) ModEntry.State.Value.ProspectorHunt.ResetAccumulatedBonus();
    }
}