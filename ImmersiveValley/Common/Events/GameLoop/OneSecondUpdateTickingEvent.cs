namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.OneSecondUpdateTicking"/> allowing dynamic enabling / disabling.</summary>
internal abstract class OneSecondUpdateTickingEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected OneSecondUpdateTickingEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicking"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnOneSecondUpdateTicking(object? sender, OneSecondUpdateTickingEventArgs e)
    {
        if (IsEnabled) OnOneSecondUpdateTickingImpl(sender, e);
    }

    /// <inheritdoc cref="OnOneSecondUpdateTicking" />
    protected abstract void OnOneSecondUpdateTickingImpl(object? sender, OneSecondUpdateTickingEventArgs e);
}