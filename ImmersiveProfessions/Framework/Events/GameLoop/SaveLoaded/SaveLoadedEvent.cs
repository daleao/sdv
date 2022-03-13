namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal abstract class SaveLoadedEvent : BaseEvent
{
    /// <summary>
    ///     Raised after loading a save (including the first day after creating a new save), or connecting to a
    ///     multiplayer world.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        if (enabled.Value) OnSaveLoadedImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaveLoaded" />
    protected abstract void OnSaveLoadedImpl(object sender, SaveLoadedEventArgs e);
}