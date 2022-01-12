using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events.Display;

internal class ScavengerHuntRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object sender, RenderedHudEventArgs e)
    {
        // track target
        ModEntry.State.Value.Indicator.DrawAsTrackingPointer(ModEntry.State.Value.ScavengerHunt.TreasureTile.Value,
            Color.Violet);

        // reveal if close enough
        var distanceSquared = (Game1.player.getTileLocation() - ModEntry.State.Value.ScavengerHunt.TreasureTile.Value)
            .LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
            ModEntry.State.Value.Indicator.DrawOverTile(ModEntry.State.Value.ScavengerHunt.TreasureTile.Value,
                Color.Violet);
    }
}