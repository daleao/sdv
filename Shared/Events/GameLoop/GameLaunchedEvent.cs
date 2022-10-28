namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.GameLaunched"/> allowing dynamic enabling / disabling.</summary>
[AlwaysEnabled]
internal abstract class GameLaunchedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="GameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected GameLaunchedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.OnGameLaunchedImpl(sender, e);
        this.Manager.ModEvents.GameLoop.GameLaunched -= this.OnGameLaunched;
    }

    /// <inheritdoc cref="OnGameLaunched"/>
    protected abstract void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e);
}
