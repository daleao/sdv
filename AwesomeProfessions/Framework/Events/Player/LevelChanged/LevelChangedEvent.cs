namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal abstract class LevelChangedEvent : BaseEvent
{
    /// <summary>Raised after a player's skill level changes.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnLevelChanged(object sender, LevelChangedEventArgs e)
    {
        if (enabled.Value) OnLevelChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLevelChanged" />
    protected abstract void OnLevelChangedImpl(object sender, LevelChangedEventArgs e);
}