namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IPlayerEvents.LevelChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class LevelChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IPlayerEvents.LevelChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnLevelChanged(object sender, LevelChangedEventArgs e)
    {
        if (hooked.Value) OnLevelChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLevelChanged" />
    protected abstract void OnLevelChangedImpl(object sender, LevelChangedEventArgs e);
}