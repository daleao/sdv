using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

internal abstract class UpdateTickedEvent : BaseEvent
{
    /// <summary>Raised after the game state is updated.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (enabled.Value) OnUpdateTickedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUpdateTicked" />
    protected abstract void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e);
}