namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.SaveCreating"/> allowing dynamic enabling / disabling.</summary>
internal abstract class SaveCreatingEvent : ManagedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMainPlayer;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected SaveCreatingEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IGameLoopEvents.SaveCreating"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaveCreating(object? sender, SaveCreatingEventArgs e)
    {
        if (IsEnabled) OnSaveCreatingImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaveCreating" />
    protected abstract void OnSaveCreatingImpl(object? sender, SaveCreatingEventArgs e);
}