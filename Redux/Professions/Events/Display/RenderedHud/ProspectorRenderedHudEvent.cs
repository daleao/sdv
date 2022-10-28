namespace DaLion.Redux.Professions.Events.Display;

#region using directives

using System.Linq;
using DaLion.Redux.Professions.Extensions;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProspectorRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorRenderedHudEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        if (ModEntry.Config.Professions.DisableAlwaysTrack && !ModEntry.Config.Professions.ModKey.IsDown())
        {
            return;
        }

        var shouldHighlightOnScreen = ModEntry.Config.Professions.ModKey.IsDown();

        // reveal on-screen trackable objects
        foreach (var (tile, _) in Game1.currentLocation.Objects.Pairs.Where(p =>
                     p.Value.ShouldBeTrackedBy(Profession.Prospector)))
        {
            tile.TrackWhenOffScreen(Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                tile.TrackWhenOnScreen(Color.Yellow);
            }
        }

        // reveal on-screen panning point
        if (!Game1.currentLocation.orePanPoint.Value.Equals(Point.Zero))
        {
            var tile = Game1.currentLocation.orePanPoint.Value.ToVector2() * 64f;
            tile.TrackWhenOffScreen(Color.Lime);
            if (shouldHighlightOnScreen)
            {
                tile.TrackWhenOnScreen(Color.Lime);
            }
        }

        if (Game1.currentLocation is not MineShaft shaft)
        {
            return;
        }

        foreach (var tile in shaft.GetLadderTiles())
        {
            tile.TrackWhenOffScreen(Color.Lime);
            if (shouldHighlightOnScreen)
            {
                tile.TrackWhenOnScreen(Color.Lime);
            }
        }
    }
}
