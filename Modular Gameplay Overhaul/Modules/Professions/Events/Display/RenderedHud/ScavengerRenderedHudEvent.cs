namespace DaLion.Overhaul.Modules.Professions.Events.Display;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
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
    internal ScavengerRenderedHudEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        if (ProfessionsModule.Config.DisableAlwaysTrack && !ProfessionsModule.Config.ModKey.IsDown())
        {
            return;
        }

        var shouldHighlightOnScreen = ProfessionsModule.Config.ModKey.IsDown();

        // track objects
        foreach (var (key, _) in Game1.currentLocation.Objects.Pairs.Where(p =>
                     p.Value.ShouldBeTrackedBy(Profession.Scavenger)))
        {
            Globals.Pointer.Value.DrawAsTrackingPointer(key, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                Globals.Pointer.Value.DrawOverTile(key, Color.Yellow);
            }
        }

        //track berries
        foreach (var bush in Game1.currentLocation.largeTerrainFeatures.OfType<Bush>().Where(b =>
                     !b.townBush.Value && b.tileSheetOffset.Value == 1 &&
                     b.inBloom(Game1.GetSeasonForLocation(Game1.currentLocation), Game1.dayOfMonth)))
        {
            Globals.Pointer.Value.DrawAsTrackingPointer(bush.tilePosition.Value, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                Globals.Pointer.Value.DrawOverTile(bush.tilePosition.Value, Color.Yellow);
            }
        }

        // track ginger
        foreach (var crop in Game1.currentLocation.terrainFeatures.Values.OfType<HoeDirt>()
                     .Where(d => d.crop is not null && d.crop.forageCrop.Value))
        {
            Globals.Pointer.Value.DrawAsTrackingPointer(crop.currentTileLocation, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                Globals.Pointer.Value.DrawOverTile(crop.currentTileLocation, Color.Yellow);
            }
        }

        // track coconuts
        foreach (var tree in Game1.currentLocation.terrainFeatures.Values.OfType<Tree>()
                     .Where(t => t.hasSeed.Value && t.treeType.Value == Tree.palmTree))
        {
            Globals.Pointer.Value.DrawAsTrackingPointer(tree.currentTileLocation, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                Globals.Pointer.Value.DrawOverTile(tree.currentTileLocation, Color.Yellow);
            }
        }
    }
}
