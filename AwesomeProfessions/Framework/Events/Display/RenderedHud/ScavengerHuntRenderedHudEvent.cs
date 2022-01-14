namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class ScavengerHuntRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object sender, RenderedHudEventArgs e)
    {
        if (!ModEntry.State.Value.ScavengerHunt.TreasureTile.HasValue) return;

        var treasureTile = ModEntry.State.Value.ScavengerHunt.TreasureTile.Value;

        // track target
        ModEntry.State.Value.Pointer.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        var distanceSquared = (Game1.player.getTileLocation() - treasureTile).LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
            ModEntry.State.Value.Pointer.DrawOverTile(treasureTile, Color.Violet);
    }
}