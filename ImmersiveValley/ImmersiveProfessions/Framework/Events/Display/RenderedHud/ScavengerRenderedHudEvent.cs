namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using Extensions;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
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
        if (Game1.currentLocation is not { IsOutdoors: true } outdoors ||
            ModEntry.Config.DisableAlwaysTrack && !ModEntry.Config.ModKey.IsDown()) return;

        var shouldHighlightOnScreen = ModEntry.Config.ModKey.IsDown();

        // track objects
        foreach (var (key, _) in outdoors.Objects.Pairs.Where(p =>
                     p.Value.ShouldBeTrackedBy(Profession.Scavenger)))
        {
            ModEntry.PlayerState.Pointer.DrawAsTrackingPointer(key, Color.Yellow);
            if (shouldHighlightOnScreen) ModEntry.PlayerState.Pointer.DrawOverTile(key, Color.Yellow);
        }

        //track berries
        foreach (var bush in outdoors.largeTerrainFeatures.OfType<Bush>().Where(b =>
                     !b.townBush.Value && b.tileSheetOffset.Value == 1 &&
                     b.inBloom(Game1.GetSeasonForLocation(outdoors), Game1.dayOfMonth)))
        {
            ModEntry.PlayerState.Pointer.DrawAsTrackingPointer(bush.tilePosition.Value, Color.Yellow);
            if (shouldHighlightOnScreen) ModEntry.PlayerState.Pointer.DrawOverTile(bush.tilePosition.Value, Color.Yellow);
        }

        // track ginger
        foreach (var crop in outdoors.terrainFeatures.Values.OfType<HoeDirt>()
                     .Where(d => d.crop is not null && d.crop.forageCrop.Value))
        {
            ModEntry.PlayerState.Pointer.DrawAsTrackingPointer(crop.currentTileLocation, Color.Yellow);
            if (shouldHighlightOnScreen) ModEntry.PlayerState.Pointer.DrawOverTile(crop.currentTileLocation, Color.Yellow);
        }

        // track coconuts
        foreach (var tree in outdoors.terrainFeatures.Values.OfType<Tree>()
                     .Where(t => t.hasSeed.Value && t.treeType.Value == Tree.palmTree))
        {
            ModEntry.PlayerState.Pointer.DrawAsTrackingPointer(tree.currentTileLocation, Color.Yellow);
            if (shouldHighlightOnScreen) ModEntry.PlayerState.Pointer.DrawOverTile(tree.currentTileLocation, Color.Yellow);
        }
    }
}