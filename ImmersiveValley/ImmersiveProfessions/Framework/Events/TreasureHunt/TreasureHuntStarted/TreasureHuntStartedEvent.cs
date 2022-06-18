namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using System;

#endregion using directives

internal sealed class TreasureHuntStartedEvent : BaseEvent
{
    private readonly Action<object, ITreasureHuntStartedEventArgs> _OnStartedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal TreasureHuntStartedEvent(Action<object, ITreasureHuntStartedEventArgs> callback)
    {
        _OnStartedImpl = callback;
    }

    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnStarted(object sender, ITreasureHuntStartedEventArgs e)
    {
        if (enabled.Value) _OnStartedImpl(sender, e);
    }
}