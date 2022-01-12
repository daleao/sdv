using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop;

internal abstract class DayStartedEvent : BaseEvent
{
    /// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        if (enabled.Value) OnDayStartedImpl(sender, e);
    }

    /// <inheritdoc cref="OnDayStarted" />
    protected abstract void OnDayStartedImpl(object sender, DayStartedEventArgs e);
}