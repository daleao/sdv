namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal class ScavengerHuntDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object sender, DayStartedEventArgs e)
    {
        if (ModEntry.PlayerState.Value.ScavengerHunt is not null)
            ModEntry.PlayerState.Value.ScavengerHunt.ResetAccumulatedBonus();
    }
}