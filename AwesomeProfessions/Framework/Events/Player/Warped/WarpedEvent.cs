using StardewModdingAPI.Events;

namespace DaLion.Stardew.Professions.Framework.Events.Player;

internal abstract class WarpedEvent : BaseEvent
{
    /// <summary>Raised after the current player moves to a new location.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnWarped(object sender, WarpedEventArgs e)
    {
        if (enabled.Value) OnWarpedImpl(sender, e);
    }

    /// <inheritdoc cref="OnWarped" />
    protected abstract void OnWarpedImpl(object sender, WarpedEventArgs e);
}