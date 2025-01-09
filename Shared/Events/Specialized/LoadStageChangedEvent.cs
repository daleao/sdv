namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="ISpecializedEvents.LoadStageChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class LoadStageChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LoadStageChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LoadStageChangedEvent(EventManager manager)
        : base(manager)
    {
        //manager.ModEvents.Specialized.LoadStageChanged += this.OnLoadStageChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        //this.Manager.ModEvents.Specialized.LoadStageChanged -= this.OnLoadStageChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnLoadStageChanged"/>
    protected abstract void OnLoadStageChangedImpl(object? sender, LoadStageChangedEventArgs e);

    /// <inheritdoc cref="ISpecializedEvents.LoadStageChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnLoadStageChanged(object? sender, LoadStageChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnLoadStageChangedImpl(sender, e);
        }
    }
}
