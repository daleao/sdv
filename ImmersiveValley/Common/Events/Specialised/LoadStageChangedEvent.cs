namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="ISpecializedEvents.LoadStageChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class LoadStageChangedEvent : BaseEvent
{
    /// <inheritdoc cref="ISpecializedEvents.LoadStageChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnLoadStageChanged(object sender, LoadStageChangedEventArgs e)
    {
        if (hooked.Value) OnLoadStageChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLoadStageChanged" />
    protected abstract void OnLoadStageChangedImpl(object sender, LoadStageChangedEventArgs e);
}