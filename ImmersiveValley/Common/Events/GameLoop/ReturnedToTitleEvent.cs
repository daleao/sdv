namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.ReturnedToTitle"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class ReturnedToTitleEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.ReturnedToTitle"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnReturnedToTitleImpl(sender, e);
    }

    /// <inheritdoc cref="OnReturnedToTitle" />
    protected abstract void OnReturnedToTitleImpl(object sender, ReturnedToTitleEventArgs e);
}