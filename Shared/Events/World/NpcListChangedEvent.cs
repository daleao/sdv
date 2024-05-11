    namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.NpcListChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class NpcListChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="NpcListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected NpcListChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.NpcListChanged += this.OnNpcListChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.NpcListChanged -= this.OnNpcListChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IWorldEvents.NpcListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnNpcListChanged(object? sender, NpcListChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnNpcListChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnNpcListChanged"/>
    protected abstract void OnNpcListChangedImpl(object? sender, NpcListChangedEventArgs e);
}
