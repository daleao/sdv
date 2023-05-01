namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="Ultimates.IUltimate"/> ends.</summary>
internal sealed class UltimateDeactivatedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateDeactivatedEventArgs> _onDeactivatedImpl;

    /// <summary>Initializes a new instance of the <see cref="UltimateDeactivatedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateDeactivatedEvent(
        Action<object?, IUltimateDeactivatedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        this._onDeactivatedImpl = callback;
        Ultimates.Ultimate.Deactivated += this.OnDeactivated;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        Ultimates.Ultimate.Deactivated -= this.OnDeactivated;
    }

    /// <summary>Raised when a player's combat <see cref="Ultimates.IUltimate"/> ends.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnDeactivated(object? sender, IUltimateDeactivatedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onDeactivatedImpl(sender, e);
        }
    }
}
