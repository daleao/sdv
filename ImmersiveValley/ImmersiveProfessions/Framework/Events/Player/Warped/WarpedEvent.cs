namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IPlayerEvents.Warped"/> allowing dynamic enabling / disabling.</summary>
internal abstract class WarpedEvent : BaseEvent
{
    /// <summary>Raised after the current player moves to a new location.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnWarped(object sender, WarpedEventArgs e)
    {
        if (enabled.Value) OnWarpedImpl(sender, e);
    }

    /// <inheritdoc cref="OnWarped" />
    protected abstract void OnWarpedImpl(object sender, WarpedEventArgs e);
}