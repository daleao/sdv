namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.SaveCreating"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class SaveCreatingEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.SaveCreating"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaveCreating(object sender, SaveCreatingEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnSaveCreatingImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaveCreating" />
    protected abstract void OnSaveCreatingImpl(object sender, SaveCreatingEventArgs e);
}