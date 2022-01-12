using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop;

internal abstract class GameLaunchedEvent : BaseEvent
{
    /// <summary>Raised after the game is launched, right before the first update tick.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnGameLaunchedImpl(sender, e);
    }

    /// <inheritdoc cref="OnGameLaunched" />
    protected abstract void OnGameLaunchedImpl(object sender, GameLaunchedEventArgs e);
}