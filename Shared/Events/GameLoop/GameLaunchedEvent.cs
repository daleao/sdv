namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.GameLaunched"/> allowing dynamic enabling / disabling.</summary>
public abstract class GameLaunchedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="GameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected GameLaunchedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.GameLaunched -= this.OnGameLaunched;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.OnGameLaunchedImpl(sender, e);
        this.Manager.Unmanage(this);
    }

    /// <inheritdoc cref="OnGameLaunched"/>
    protected abstract void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e);

    /// <inheritdoc />
    protected sealed override void OnEnabled()
    {
    }

    /// <inheritdoc />
    protected sealed override void OnDisabled()
    {
    }
}
