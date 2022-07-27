namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerHuntRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ScavengerHuntRenderedHudEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        if (!ModEntry.Player.ScavengerHunt.TreasureTile.HasValue) return;

        var treasureTile = ModEntry.Player.ScavengerHunt.TreasureTile.Value;

        // track target
        ModEntry.Player.Pointer.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        var distanceSquared = (Game1.player.getTileLocation() - treasureTile).LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
            ModEntry.Player.Pointer.DrawOverTile(treasureTile, Color.Violet);
    }
}