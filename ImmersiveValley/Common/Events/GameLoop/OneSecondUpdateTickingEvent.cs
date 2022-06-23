namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.OneSecondUpdateTicking"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class OneSecondUpdateTickingEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicking"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnOneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnOneSecondUpdateTickingImpl(sender, e);
    }

    /// <inheritdoc cref="OnOneSecondUpdateTicking" />
    protected abstract void OnOneSecondUpdateTickingImpl(object sender, OneSecondUpdateTickingEventArgs e);
}