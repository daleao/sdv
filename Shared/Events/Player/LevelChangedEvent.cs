namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IPlayerEvents.LevelChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class LevelChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LevelChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LevelChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Player.LevelChanged += this.OnLevelChanged;
    }

    /// <inheritdoc cref="IPlayerEvents.LevelChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnLevelChanged(object? sender, LevelChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnLevelChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnLevelChanged"/>
    protected abstract void OnLevelChangedImpl(object? sender, LevelChangedEventArgs e);
}
