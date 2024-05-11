namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.SaveCreating"/> allowing dynamic enabling / disabling.</summary>
public abstract class SaveCreatingEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SaveCreatingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected SaveCreatingEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.SaveCreating += this.OnSaveCreating;
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMainPlayer;

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.SaveCreating -= this.OnSaveCreating;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IGameLoopEvents.SaveCreating"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnSaveCreating(object? sender, SaveCreatingEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnSaveCreatingImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnSaveCreating"/>
    protected abstract void OnSaveCreatingImpl(object? sender, SaveCreatingEventArgs e);
}
