namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.NpcListChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class NpcListChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected NpcListChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IWorldEvents.NpcListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnNpcListChanged(object? sender, NpcListChangedEventArgs e)
    {
        if (IsEnabled) OnNpcListChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnNpcListChanged" />
    protected abstract void OnNpcListChangedImpl(object? sender, NpcListChangedEventArgs e);
}