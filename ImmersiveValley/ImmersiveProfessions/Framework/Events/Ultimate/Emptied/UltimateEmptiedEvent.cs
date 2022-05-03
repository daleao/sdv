namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

#endregion using directives

internal class UltimateEmptiedEvent : BaseEvent
{
    private readonly Action<object, IUltimateEmptiedEventArgs> _OnEmptiedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateEmptiedEvent(Action<object, IUltimateEmptiedEventArgs> callback)
    {
        _OnEmptiedImpl = callback;
    }

    /// <summary>Raised when the local player's ultimate charge value returns to zero.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnEmptied(object sender, IUltimateEmptiedEventArgs e)
    {
        if (enabled.Value) _OnEmptiedImpl(sender, e);
    }
}