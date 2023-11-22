namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Professions.TreasureHunts;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmhandProspectorHuntUpdateTickedEvent : UpdateTickedEvent
{
    private ProspectorHunt? _hunt;

    /// <summary>Initializes a new instance of the <see cref="FarmhandProspectorHuntUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal FarmhandProspectorHuntUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._hunt ??= Game1.player.Get_ProspectorHunt();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!e.IsMultipleOf(15))
        {
            return;
        }

        if (!this._hunt!.TreasureTile.HasValue)
        {
            this.Disable();
            return;
        }

        if (Game1.player.currentLocation.Objects.ContainsKey(this._hunt.TreasureTile.Value))
        {
            return;
        }

        this._hunt.Complete();
        this.Disable();
    }
}
