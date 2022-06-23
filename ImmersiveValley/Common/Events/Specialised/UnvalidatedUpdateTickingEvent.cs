namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="ISpecializedEvents.UnvalidatedUpdateTicking"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class UnvalidatedUpdateTickingEvent : BaseEvent
{
    /// <inheritdoc cref="ISpecializedEvents.UnvalidatedUpdateTicking"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnUnvalidatedUpdateTicking(object sender, UnvalidatedUpdateTickingEventArgs e)
    {
        if (hooked.Value) OnUnvalidatedUpdateTickingImpl(sender, e);
    }

    /// <inheritdoc cref="OnUnvalidatedUpdateTicking" />
    protected abstract void OnUnvalidatedUpdateTickingImpl(object sender, UnvalidatedUpdateTickingEventArgs e);
}