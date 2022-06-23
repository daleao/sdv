namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object sender, RenderedHudEventArgs e)
    {
        if (!ModEntry.PlayerState.ProspectorHunt.TreasureTile.HasValue) return;

        var treasureTile = ModEntry.PlayerState.ProspectorHunt.TreasureTile.Value;

        // track target
        ModEntry.PlayerState.Pointer.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        var distanceSquared = (Game1.player.getTileLocation() - treasureTile).LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
            ModEntry.PlayerState.Pointer.DrawOverTile(treasureTile, Color.Violet);
    }
}