namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IPlayerEvents.Warped"/> allowing dynamic enabling / disabling.</summary>
internal abstract class WarpedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected WarpedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Player.Warped += this.OnWarped;
    }

    /// <inheritdoc cref="IPlayerEvents.Warped"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (e.IsLocalPlayer && this.IsEnabled && !e.NewLocation.Equals(e.OldLocation))
        {
            this.OnWarpedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnWarped"/>
    protected abstract void OnWarpedImpl(object? sender, WarpedEventArgs e);
}
