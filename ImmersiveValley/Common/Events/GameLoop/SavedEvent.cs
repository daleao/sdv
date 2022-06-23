namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.Saved"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class SavedEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.Saved"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaved(object sender, SavedEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnSavedImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaved" />
    protected abstract void OnSavedImpl(object sender, SavedEventArgs e);
}