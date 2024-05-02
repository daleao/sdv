namespace DaLion.Professions.Framework.Events.Display.RenderedHud;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProspectorHuntRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorHuntRenderedHudEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        State.ProspectorHunt ??= new ProspectorHunt();
        if (!State.ProspectorHunt.TreasureTile.HasValue)
        {
            return;
        }

        var treasureTile = State.ProspectorHunt.TreasureTile.Value;

        // track target
        HudPointer.Instance.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        if (Game1.player.DistanceTo(treasureTile) <= Config.ProspectorDetectionDistance)
        {
            HudPointer.Instance.DrawOverTile(treasureTile, Color.Violet);
        }
    }
}
