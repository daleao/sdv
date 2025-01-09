namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for a <see cref="IGameLoopEvents.OneSecondUpdateTicked"/> which executes exactly once, after one second of game time has elapsed.</summary>
/// <remarks>Useful for set-up code which requires third-party mod integrations to be registered.</remarks>
public abstract class FirstSecondUpdateTickedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="FirstSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected FirstSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.OneSecondUpdateTicked += this.OnFirstSecondUpdateTicked;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.OneSecondUpdateTicked -= this.OnFirstSecondUpdateTicked;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnFirstSecondUpdateTicked"/>
    protected abstract void OnFirstSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e);

    /// <inheritdoc />
    protected sealed override void OnEnabled()
    {
    }

    /// <inheritdoc />
    protected sealed override void OnDisabled()
    {
    }

    /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnFirstSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        this.OnFirstSecondUpdateTickedImpl(sender, e);
        this.Manager.Unmanage(this);
    }
}
