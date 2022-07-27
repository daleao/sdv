namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ProspectorRenderedHudEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        if (ModEntry.Config.DisableAlwaysTrack && !ModEntry.Config.ModKey.IsDown()) return;

        var shouldHighlightOnScreen = ModEntry.Config.ModKey.IsDown();

        // reveal on-screen trackable objects
        foreach (var (tile, _) in Game1.currentLocation.Objects.Pairs.Where(p =>
                     p.Value.ShouldBeTrackedBy(Profession.Prospector)))
        {
            tile.TrackWhenOffScreen(Color.Yellow);
            if (shouldHighlightOnScreen) tile.TrackWhenOnScreen(Color.Yellow);
        }

        // reveal on-screen panning point
        if (!Game1.currentLocation.orePanPoint.Value.Equals(Point.Zero))
        {
            var tile = Game1.currentLocation.orePanPoint.Value.ToVector2() * 64f;
            tile.TrackWhenOffScreen(Color.Lime);
            if (shouldHighlightOnScreen) tile.TrackWhenOnScreen(Color.Lime);
        }

        if (Game1.currentLocation is not MineShaft shaft) return;

        foreach (var tile in shaft.GetLadderTiles())
        {
            tile.TrackWhenOffScreen(Color.Lime);
            if (shouldHighlightOnScreen) tile.TrackWhenOnScreen(Color.Lime);
        }
    }
}