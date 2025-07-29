namespace DaLion.Professions.Framework.Events.Display.RenderedHud;

#region using directives

using DaLion.Professions.Framework.UI;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ScavengerHuntRenderedHudEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ScavengerHuntRenderedHudEvent(EventManager? manager = null)
    : RenderedHudEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.ScavengerHunt?.IsActive ?? false;

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        var treasureTile = State.ScavengerHunt!.TargetTile!.Value;
        if (Game1.player.SquaredTileDistance(treasureTile) >= (Config.ScavengerHuntRange * Config.ScavengerHuntRange))
        {
            HudPointer.Instance.DrawAsTrackingPointer(e.SpriteBatch, treasureTile, Color.Violet);
        }
    }
}
