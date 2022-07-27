namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IPlayerEvents.LevelChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class LevelChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LevelChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IPlayerEvents.LevelChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnLevelChanged(object? sender, LevelChangedEventArgs e)
    {
        if (IsEnabled) OnLevelChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLevelChanged" />
    protected abstract void OnLevelChangedImpl(object? sender, LevelChangedEventArgs e);
}