namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.CursorMoved"/> allowing dynamic enabling / disabling.</summary>
internal abstract class CursorMovedEvent : BaseEvent
{
    /// <inheritdoc cref="IInputEvents.CursorMoved"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnCursorMoved(object sender, CursorMovedEventArgs e)
    {
        if (enabled.Value) OnCursorMovedImpl(sender, e);
    }

    /// <inheritdoc cref="OnCursorMoved" />
    protected abstract void OnCursorMovedImpl(object sender, CursorMovedEventArgs e);
}