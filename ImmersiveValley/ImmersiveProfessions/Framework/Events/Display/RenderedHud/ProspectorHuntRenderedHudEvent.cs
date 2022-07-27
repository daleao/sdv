namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using System;
using TreasureHunts;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntRenderedHudEvent : RenderedHudEvent
{
    private ProspectorHunt? Hunt;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ProspectorHuntRenderedHudEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        Hunt ??= (ProspectorHunt)ModEntry.State.ProspectorHunt.Value;
        if (!Hunt.TreasureTile.HasValue) return;

        var treasureTile = Hunt.TreasureTile.Value;

        // track target
        ModEntry.Pointer.Value.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        var distanceSquared = (Game1.player.getTileLocation() - treasureTile).LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
            ModEntry.Pointer.Value.DrawOverTile(treasureTile, Color.Violet);
    }
}