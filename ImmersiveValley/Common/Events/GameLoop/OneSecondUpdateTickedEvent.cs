namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.OneSecondUpdateTicked"/> allowing dynamic enabling / disabling.</summary>
internal abstract class OneSecondUpdateTickedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected OneSecondUpdateTickedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (IsEnabled) OnOneSecondUpdateTickedImpl(sender, e);
    }

    /// <inheritdoc cref="OnOneSecondUpdateTicked" />
    protected abstract void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e);
}