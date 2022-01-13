using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

namespace DaLion.Stardew.Professions.Framework.Events.Display;

internal class ProspectorHuntRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object sender, RenderedHudEventArgs e)
    {
        if (!ModEntry.State.Value.ProspectorHunt.TreasureTile.HasValue) return;

        var treasureTile = ModEntry.State.Value.ProspectorHunt.TreasureTile.Value;

        // track target
        ModEntry.State.Value.Indicator.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        var distanceSquared = (Game1.player.getTileLocation() - treasureTile).LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
            ModEntry.State.Value.Indicator.DrawOverTile(treasureTile, Color.Violet);
    }
}