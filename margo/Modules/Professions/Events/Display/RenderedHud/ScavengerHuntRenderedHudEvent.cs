namespace DaLion.Overhaul.Modules.Professions.Events.Display.RenderedHud;

#region using directives

using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Overhaul.Modules.Professions.TreasureHunts;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerHuntRenderedHudEvent : RenderedHudEvent
{
    private ScavengerHunt? _hunt;

    /// <summary>Initializes a new instance of the <see cref="ScavengerHuntRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ScavengerHuntRenderedHudEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._hunt ??= Game1.player.Get_ScavengerHunt();
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        if (!this._hunt!.TreasureTile.HasValue)
        {
            return;
        }

        var treasureTile = this._hunt.TreasureTile.Value;

        // track target
        HudPointer.Instance.Value.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        var distanceSquared = (Game1.player.getTileLocation() - treasureTile).LengthSquared();
        if (distanceSquared <= Math.Pow(ProfessionsModule.Config.ScavengerDetectionDistance, 2))
        {
            HudPointer.Instance.Value.DrawOverTile(treasureTile, Color.Violet);
        }
    }
}
