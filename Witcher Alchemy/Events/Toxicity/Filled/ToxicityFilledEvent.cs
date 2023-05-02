namespace DaLion.Alchemy.Events.Toxicity.Filled;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a player's Toxicity reaches the maximum value.</summary>
internal class ToxicityFilledEvent : ManagedEvent
{
    protected readonly Action<object?, IToxicityFilledEventArgs> OnChargeInitiatedImpl;

    /// <summary>Initializes a new instance of the <see cref="ToxicityFilledEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal ToxicityFilledEvent(Action<object?, IToxicityFilledEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        this.OnChargeInitiatedImpl = callback;
    }

    /// <summary>Raised when a player's Toxicity reaches the maximum value.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnFilled(object? sender, IToxicityFilledEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnChargeInitiatedImpl(sender, e);
        }
    }
}
