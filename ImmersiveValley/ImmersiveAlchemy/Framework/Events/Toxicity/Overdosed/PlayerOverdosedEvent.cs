namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using System;

using Common.Events;

#endregion using directives

/// <summary>A dynamic event raised when a player's Toxicity value crosses the overdose threshold.</summary>
internal class PlayerOverdosedEvent : ManagedEvent
{
    protected readonly Action<object?, IPlayerOverdosedEventArgs> _OnChargeInitiatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal PlayerOverdosedEvent(Action<object?, IPlayerOverdosedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        _OnChargeInitiatedImpl = callback;
    }

    /// <summary>Raised when a player's Toxicity value crosses the overdose threshold.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnOverdosed(object? sender, IPlayerOverdosedEventArgs e)
    {
        if (IsHooked) _OnChargeInitiatedImpl(sender, e);
    }
}