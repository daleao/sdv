namespace DaLion.Professions.Framework.Events.Display.RenderedHud;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Initializes a new instance of the <see cref="ScavengerRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ScavengerRenderedHudEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        if (Config.DisableAlwaysTrack && !Config.ModKey.IsDown())
        {
            return;
        }

        var shouldHighlightOnScreen = Config.ModKey.IsDown();

        // track objects
        foreach (var (tile, @object) in Game1.currentLocation.Objects.Pairs)
        {
            if (!@object.ShouldBeTrackedBy(Profession.Scavenger))
            {
                continue;
            }

            HudPointer.Instance.DrawAsTrackingPointer(tile, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                HudPointer.Instance.DrawOverTile(tile, Color.Yellow);
            }
        }

        //track berries
        foreach (var feature in Game1.currentLocation.largeTerrainFeatures)
        {
            if (feature is not Bush bush || bush.townBush.Value || bush.tileSheetOffset.Value != 1 ||
                !bush.inBloom())
            {
                continue;
            }

            HudPointer.Instance.DrawAsTrackingPointer(bush.Tile, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                HudPointer.Instance.DrawOverTile(bush.Tile + new Vector2(0.5f, -1f), Color.Yellow);
            }
        }

        // track ginger
        foreach (var feature in Game1.currentLocation.terrainFeatures.Values)
        {
            if (feature is not HoeDirt { crop: { } crop } dirt || !crop.forageCrop.Value)
            {
                continue;
            }

            HudPointer.Instance.DrawAsTrackingPointer(dirt.Tile, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                HudPointer.Instance.DrawOverTile(dirt.Tile, Color.Yellow);
            }
        }

        // track coconuts
        foreach (var feature in Game1.currentLocation.terrainFeatures.Values)
        {
            if (feature is not Tree tree || !tree.hasSeed.Value || tree.treeType.Value != Tree.palmTree)
            {
                continue;
            }

            HudPointer.Instance.DrawAsTrackingPointer(tree.Tile, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                HudPointer.Instance.DrawOverTile(tree.Tile, Color.Yellow);
            }
        }
    }
}
