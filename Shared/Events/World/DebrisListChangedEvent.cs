namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.DebrisListChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class DebrisListChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DebrisListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected DebrisListChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.DebrisListChanged += this.OnDebrisListChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.DebrisListChanged -= this.OnDebrisListChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnDebrisListChanged"/>
    protected abstract void OnDebrisListChangedImpl(object? sender, DebrisListChangedEventArgs e);

    /// <inheritdoc cref="IWorldEvents.DebrisListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnDebrisListChanged(object? sender, DebrisListChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnDebrisListChangedImpl(sender, e);
        }
    }
}
