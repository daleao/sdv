namespace DaLion.Alchemy.Events.Toxicity.Cleared;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a player's Toxicity value drops back to zero.</summary>
internal class ToxicityClearedEvent : ManagedEvent
{
    protected readonly Action<object?, IToxicityClearedEventArgs> OnChargeInitiatedImpl;

    /// <summary>Initializes a new instance of the <see cref="ToxicityClearedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal ToxicityClearedEvent(Action<object?, IToxicityClearedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        this.OnChargeInitiatedImpl = callback;
    }

    /// <summary>Raised when a player's Toxicity value drops back to zero.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnCleared(object? sender, IToxicityClearedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnChargeInitiatedImpl(sender, e);
        }
    }
}
