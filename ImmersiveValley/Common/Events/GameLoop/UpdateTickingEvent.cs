namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.UpdateTicking"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class UpdateTickingEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.UpdateTicking"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnUpdateTickingImpl(sender, e);
    }

    /// <inheritdoc cref="OnUpdateTicking" />
    protected abstract void OnUpdateTickingImpl(object sender, UpdateTickingEventArgs e);
}