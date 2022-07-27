namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ScavengerRenderedHudEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        if (ModEntry.Config.DisableAlwaysTrack && !ModEntry.Config.ModKey.IsDown()) return;

        var shouldHighlightOnScreen = ModEntry.Config.ModKey.IsDown();

        // track objects
        foreach (var (key, _) in Game1.currentLocation.Objects.Pairs.Where(p =>
                     p.Value.ShouldBeTrackedBy(Profession.Scavenger)))
        {
            ModEntry.Pointer.Value.DrawAsTrackingPointer(key, Color.Yellow);
            if (shouldHighlightOnScreen) ModEntry.Pointer.Value.DrawOverTile(key, Color.Yellow);
        }

        //track berries
        foreach (var bush in Game1.currentLocation.largeTerrainFeatures.OfType<Bush>().Where(b =>
                     !b.townBush.Value && b.tileSheetOffset.Value == 1 &&
                     b.inBloom(Game1.GetSeasonForLocation(Game1.currentLocation), Game1.dayOfMonth)))
        {
            ModEntry.Pointer.Value.DrawAsTrackingPointer(bush.tilePosition.Value, Color.Yellow);
            if (shouldHighlightOnScreen) ModEntry.Pointer.Value.DrawOverTile(bush.tilePosition.Value, Color.Yellow);
        }

        // track ginger
        foreach (var crop in Game1.currentLocation.terrainFeatures.Values.OfType<HoeDirt>()
                     .Where(d => d.crop is not null && d.crop.forageCrop.Value))
        {
            ModEntry.Pointer.Value.DrawAsTrackingPointer(crop.currentTileLocation, Color.Yellow);
            if (shouldHighlightOnScreen) ModEntry.Pointer.Value.DrawOverTile(crop.currentTileLocation, Color.Yellow);
        }

        // track coconuts
        foreach (var tree in Game1.currentLocation.terrainFeatures.Values.OfType<Tree>()
                     .Where(t => t.hasSeed.Value && t.treeType.Value == Tree.palmTree))
        {
            ModEntry.Pointer.Value.DrawAsTrackingPointer(tree.currentTileLocation, Color.Yellow);
            if (shouldHighlightOnScreen) ModEntry.Pointer.Value.DrawOverTile(tree.currentTileLocation, Color.Yellow);
        }
    }
}