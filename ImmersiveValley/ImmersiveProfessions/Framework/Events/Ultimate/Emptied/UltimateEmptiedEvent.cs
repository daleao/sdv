namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using Common.Events;
using System;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="Ultimates.IUltimate"> charge value returns to zero.</summary>
internal sealed class UltimateEmptiedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateEmptiedEventArgs> _OnEmptiedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    internal UltimateEmptiedEvent(Action<object?, IUltimateEmptiedEventArgs> callback, bool alwaysEnabled = false)
        : base(ModEntry.EventManager)
    {
        _OnEmptiedImpl = callback;
        AlwaysEnabled = alwaysEnabled;
    }

    /// <summary>Raised when the local player's <see cref="Ultimates.IUltimate"/> charge value returns to zero.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnEmptied(object? sender, IUltimateEmptiedEventArgs e)
    {
        if (IsEnabled) _OnEmptiedImpl(sender, e);
    }
}