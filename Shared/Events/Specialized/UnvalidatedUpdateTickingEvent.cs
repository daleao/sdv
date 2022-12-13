namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="ISpecializedEvents.UnvalidatedUpdateTicking"/> allowing dynamic enabling / disabling.</summary>
internal abstract class UnvalidatedUpdateTickingEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UnvalidatedUpdateTickingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected UnvalidatedUpdateTickingEvent(EventManager manager)
        : base(manager)
    {
        //manager.ModEvents.Specialized.UnvalidatedUpdateTicking += this.OnUnvalidatedUpdateTicking;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        //this.Manager.ModEvents.Specialized.UnvalidatedUpdateTicking -= this.OnUnvalidatedUpdateTicking;
    }

    /// <inheritdoc cref="ISpecializedEvents.UnvalidatedUpdateTicking"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnUnvalidatedUpdateTicking(object? sender, UnvalidatedUpdateTickingEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnUnvalidatedUpdateTickingImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnUnvalidatedUpdateTicking"/>
    protected abstract void OnUnvalidatedUpdateTickingImpl(object? sender, UnvalidatedUpdateTickingEventArgs e);
}
