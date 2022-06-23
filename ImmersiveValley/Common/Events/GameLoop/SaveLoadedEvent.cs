namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.SaveLoaded"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class SaveLoadedEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnSaveLoadedImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaveLoaded" />
    protected abstract void OnSaveLoadedImpl(object sender, SaveLoadedEventArgs e);
}