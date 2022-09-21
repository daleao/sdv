namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;
using DaLion.Common.Events;

#endregion using directives

/// <summary>
///     A dynamic event raised when a
///     <see cref="Ultimates.IUltimate"/> is gains any charge while it was previously empty.
/// </summary>
internal sealed class UltimateChargeInitiatedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateChargeInitiatedEventArgs> _onChargeInitiatedImpl;

    /// <summary>Initializes a new instance of the <see cref="UltimateChargeInitiatedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    internal UltimateChargeInitiatedEvent(
        Action<object?, IUltimateChargeInitiatedEventArgs> callback, bool alwaysEnabled = false)
        : base(ModEntry.Events)
    {
        this._onChargeInitiatedImpl = callback;
        this.AlwaysEnabled = alwaysEnabled;
    }

    /// <summary>
    ///     Raised when a player's combat <see cref="Ultimates.IUltimate"/> gains any charge while it was previously
    ///     empty.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChargeInitiated(object? sender, IUltimateChargeInitiatedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onChargeInitiatedImpl(sender, e);
        }
    }
}
