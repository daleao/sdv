using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Events.Display.RenderedHud;

internal class ProspectorHuntRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
    {
        if (!ModEntry.State.Value.ProspectorHunt.IsActive) return;

        // reveal treasure hunt target
        var distanceSquared = (Game1.player.getTileLocation() - ModEntry.State.Value.ProspectorHunt.TreasureTile.Value)
            .LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
            HUD.DrawArrowPointerOverTarget(ModEntry.State.Value.ProspectorHunt.TreasureTile.Value, Color.Violet);
    }
}