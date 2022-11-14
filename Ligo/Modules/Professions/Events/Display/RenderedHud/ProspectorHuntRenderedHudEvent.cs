namespace DaLion.Ligo.Modules.Professions.Events.Display;

#region using directives

using DaLion.Ligo.Modules.Professions.TreasureHunts;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntRenderedHudEvent : RenderedHudEvent
{
    private ProspectorHunt? _hunt;

    /// <summary>Initializes a new instance of the <see cref="ProspectorHuntRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorHuntRenderedHudEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        this._hunt ??= (ProspectorHunt)ModEntry.State.Professions.ProspectorHunt.Value;
        if (!this._hunt.TreasureTile.HasValue)
        {
            return;
        }

        var treasureTile = this._hunt.TreasureTile.Value;

        // track target
        Globals.Pointer.Value.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        var distanceSquared = (Game1.player.getTileLocation() - treasureTile).LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.Professions.TreasureDetectionDistance, 2))
        {
            Globals.Pointer.Value.DrawOverTile(treasureTile, Color.Violet);
        }
    }
}
