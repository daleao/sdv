namespace DaLion.Professions.Framework.Events.Display.RenderedHud;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;

/// <summary>Initializes a new instance of the <see cref="ProspectorRenderedHudEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorRenderedHudEvent(EventManager? manager = null)
    : RenderedHudEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        if (Config.DisableAlwaysTrack && !Config.ModKey.IsDown())
        {
            return;
        }

        var shouldHighlightOnScreen = Config.ModKey.IsDown();

        // track objects, such as ore nodes
        foreach (var (tile, @object) in Game1.currentLocation.Objects.Pairs)
        {
            if (@object.ShouldBeTrackedBy(Profession.Prospector))
            {
                tile.TrackWhenOffScreen(Color.OrangeRed);
                if (shouldHighlightOnScreen)
                {
                    tile.TrackWhenOnScreen(Color.OrangeRed);
                }
            }
            else if (@object.QualifiedItemId == QIDs.ArtifactSpot)
            {
                tile.TrackWhenOffScreen(Color.Lime);
                if (shouldHighlightOnScreen)
                {
                    tile.TrackWhenOnScreen(Color.Lime);
                }
            }
        }

        // track resource clumps
        foreach (var clump in Game1.currentLocation.resourceClumps)
        {
            if (!Lookups.ResourceClumpIds.Contains(clump.parentSheetIndex.Value))
            {
                continue;
            }

            var tile = clump.Tile + new Vector2(0.5f, 0f);
            tile.TrackWhenOffScreen(Color.Orange);
            if (shouldHighlightOnScreen)
            {
                tile.TrackWhenOnScreen(Color.Orange);
            }
        }

        // track panning spots
        if (!Game1.currentLocation.orePanPoint.Value.Equals(Point.Zero))
        {
            var tile = Game1.currentLocation.orePanPoint.Value.ToVector2();
            tile.TrackWhenOffScreen(Color.Cyan);
            if (shouldHighlightOnScreen)
            {
                tile.TrackWhenOnScreen(Color.Cyan);
            }
        }

        if (Game1.currentLocation is not MineShaft shaft)
        {
            return;
        }

        // track mine ladders and shafts
        foreach (var tile in shaft.GetLadderTiles())
        {
            tile.TrackWhenOffScreen(Color.Cyan);
            if (shouldHighlightOnScreen)
            {
                tile.TrackWhenOnScreen(Color.Cyan);
            }
        }
    }
}
