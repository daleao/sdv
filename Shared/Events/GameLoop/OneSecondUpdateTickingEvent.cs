namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.OneSecondUpdateTicking"/> allowing dynamic enabling / disabling.</summary>
public abstract class OneSecondUpdateTickingEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="OneSecondUpdateTickingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected OneSecondUpdateTickingEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.OneSecondUpdateTicking += this.OnOneSecondUpdateTicking;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.OneSecondUpdateTicking -= this.OnOneSecondUpdateTicking;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicking"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnOneSecondUpdateTicking(object? sender, OneSecondUpdateTickingEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnOneSecondUpdateTickingImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnOneSecondUpdateTicking"/>
    protected abstract void OnOneSecondUpdateTickingImpl(object? sender, OneSecondUpdateTickingEventArgs e);
}
