namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal class ProspectorHuntDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object sender, DayStartedEventArgs e)
    {
        if (ModEntry.PlayerState.ProspectorHunt is not null)
            ModEntry.PlayerState.ProspectorHunt.ResetAccumulatedBonus();
    }
}