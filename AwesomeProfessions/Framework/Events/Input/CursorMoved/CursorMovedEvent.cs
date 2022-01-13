using StardewModdingAPI.Events;

namespace DaLion.Stardew.Professions.Framework.Events.Input;

internal abstract class CursorMovedEvent : BaseEvent
{
    /// <summary>Raised after the player moves the in-game cursor.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnCursorMoved(object sender, CursorMovedEventArgs e)
    {
        if (enabled.Value) OnCursorMovedImpl(sender, e);
    }

    /// <inheritdoc cref="OnCursorMoved" />
    protected abstract void OnCursorMovedImpl(object sender, CursorMovedEventArgs e);
}