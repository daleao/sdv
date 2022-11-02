namespace DaLion.Redux.Framework.Professions.Events.Ultimate;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="Ultimates.IUltimate"/> reaches the maximum charge value.</summary>
internal sealed class UltimateFullyChargedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateFullyChargedEventArgs> _onFullyChargedImpl;

    /// <summary>Initializes a new instance of the <see cref="UltimateFullyChargedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateFullyChargedEvent(
        Action<object?, IUltimateFullyChargedEventArgs> callback)
        : base(ModEntry.Events)
    {
        this._onFullyChargedImpl = callback;
    }

    /// <summary>Raised when the local player's <see cref="Ultimates.IUltimate"/> charge value reaches max value.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnFullyCharged(object? sender, IUltimateFullyChargedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onFullyChargedImpl(sender, e);
        }
    }
}
