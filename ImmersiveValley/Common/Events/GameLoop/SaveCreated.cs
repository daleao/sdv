namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.SaveCreated"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class SaveCreatedEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.SaveCreated"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaveCreated(object sender, SaveCreatedEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnSaveCreatedImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaveCreated" />
    protected abstract void OnSaveCreatedImpl(object sender, SaveCreatedEventArgs e);
}