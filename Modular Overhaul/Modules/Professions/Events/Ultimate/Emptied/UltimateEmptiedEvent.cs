namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="Ultimates.IUltimate"/> charge value returns to zero.</summary>
internal sealed class UltimateEmptiedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateEmptiedEventArgs> _onEmptiedImpl;

    /// <summary>Initializes a new instance of the <see cref="UltimateEmptiedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateEmptiedEvent(Action<object?, IUltimateEmptiedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        this._onEmptiedImpl = callback;
        Ultimates.Ultimate.Emptied += this.OnEmptied;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        Ultimates.Ultimate.Emptied -= this.OnEmptied;
    }

    /// <summary>Raised when the local player's <see cref="Ultimates.IUltimate"/> charge value returns to zero.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnEmptied(object? sender, IUltimateEmptiedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onEmptiedImpl(sender, e);
        }
    }
}
