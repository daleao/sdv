using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class ProspectorHuntDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    public override void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        if (ModState.ProspectorHunt is not null) ModState.ProspectorHunt.ResetAccumulatedBonus();
    }
}