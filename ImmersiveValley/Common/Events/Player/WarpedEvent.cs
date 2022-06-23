namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IPlayerEvents.Warped"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class WarpedEvent : BaseEvent
{
    /// <inheritdoc cref="IPlayerEvents.Warped"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnWarped(object sender, WarpedEventArgs e)
    {
        if (hooked.Value) OnWarpedImpl(sender, e);
    }

    /// <inheritdoc cref="OnWarped" />
    protected abstract void OnWarpedImpl(object sender, WarpedEventArgs e);
}