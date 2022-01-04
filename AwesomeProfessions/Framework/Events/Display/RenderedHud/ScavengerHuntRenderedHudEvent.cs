using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Events.Display.RenderedHud;

internal class ScavengerHuntRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
    {
        if (!ModEntry.State.Value.ScavengerHunt.IsActive) return;

        // track and reveal treasure hunt target
        HUD.DrawTrackingArrowPointer(ModEntry.State.Value.ScavengerHunt.TreasureTile.Value, Color.Violet);
        var distanceSquared = (Game1.player.getTileLocation() - ModEntry.State.Value.ScavengerHunt.TreasureTile.Value)
            .LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
            HUD.DrawArrowPointerOverTarget(ModEntry.State.Value.ScavengerHunt.TreasureTile.Value, Color.Violet);
    }
}