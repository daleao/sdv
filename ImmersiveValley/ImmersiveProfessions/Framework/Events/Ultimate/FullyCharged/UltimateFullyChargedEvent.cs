namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

#endregion using directives

internal class UltimateFullyChargedEvent : BaseEvent
{
    private readonly Action<object, IUltimateFullyChargedEventArgs> _OnFullyChargedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateFullyChargedEvent(Action<object, IUltimateFullyChargedEventArgs> callback)
    {
        _OnFullyChargedImpl = callback;
    }

    /// <summary>Raised when the local player's ultimate charge value reaches max value.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnFullyCharged(object sender, IUltimateFullyChargedEventArgs e)
    {
        if (enabled.Value) _OnFullyChargedImpl(sender, e);
    }
}