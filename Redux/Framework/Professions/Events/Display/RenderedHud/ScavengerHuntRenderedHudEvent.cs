namespace DaLion.Redux.Framework.Professions.Events.Display;

#region using directives

using DaLion.Redux.Framework.Professions.TreasureHunts;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerHuntRenderedHudEvent : RenderedHudEvent
{
    private ScavengerHunt? _hunt;

    /// <summary>Initializes a new instance of the <see cref="ScavengerHuntRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ScavengerHuntRenderedHudEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        this._hunt ??= (ScavengerHunt)ModEntry.State.Professions.ScavengerHunt.Value;
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
