namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Professions.Framework.TreasureHunts;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerHuntRenderedHudEvent : RenderedHudEvent
{
    private ScavengerHunt? _hunt;

    /// <summary>Initializes a new instance of the <see cref="ScavengerHuntRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ScavengerHuntRenderedHudEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        this._hunt ??= (ScavengerHunt)ModEntry.State.ScavengerHunt.Value;
        if (!this._hunt.TreasureTile.HasValue)
        {
            return;
        }

        var treasureTile = this._hunt.TreasureTile.Value;

        // track target
        ModEntry.Pointer.Value.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        var distanceSquared = (Game1.player.getTileLocation() - treasureTile).LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
        {
            ModEntry.Pointer.Value.DrawOverTile(treasureTile, Color.Violet);
        }
    }
}
