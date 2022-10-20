namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using DaLion.Common.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="Ultimates.IUltimate"/> gains any charge.</summary>
internal sealed class UltimateChargeIncreasedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateChargeIncreasedEventArgs> _onChargeIncreasedImpl;

    /// <summary>Initializes a new instance of the <see cref="UltimateChargeIncreasedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    internal UltimateChargeIncreasedEvent(
        Action<object?, IUltimateChargeIncreasedEventArgs> callback, bool alwaysEnabled = false)
        : base(ModEntry.Events)
    {
        this._onChargeIncreasedImpl = callback;
        this.AlwaysEnabled = alwaysEnabled;
    }

    /// <summary>Raised when a player's combat <see cref="Ultimates.IUltimate"/> gains any charge.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChargeIncreased(object? sender, IUltimateChargeIncreasedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onChargeIncreasedImpl(sender, e);
        }
    }
}
