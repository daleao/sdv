namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using Common.Events;
using System;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="Ultimates.IUltimate"> reaches the maximum charge value.</summary>
internal sealed class UltimateFullyChargedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateFullyChargedEventArgs> _OnFullyChargedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateFullyChargedEvent(Action<object?, IUltimateFullyChargedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        _OnFullyChargedImpl = callback;
    }

    /// <summary>Raised when the local player's <see cref="Ultimates.IUltimate"/> charge value reaches max value.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnFullyCharged(object? sender, IUltimateFullyChargedEventArgs e)
    {
        if (IsHooked) _OnFullyChargedImpl(sender, e);
    }
}