namespace DaLion.Overhaul.Modules.Professions.Events.Display.RenderedHud;

#region using directives

using DaLion.Overhaul.Modules.Core.UI;
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
        foreach (var (tile, @object) in Game1.currentLocation.Objects.Pairs)
        {
            if (!@object.ShouldBeTrackedBy(Profession.Scavenger))
            {
                continue;
            }

            HudPointer.Instance.Value.DrawAsTrackingPointer(tile, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                HudPointer.Instance.Value.DrawOverTile(tile, Color.Yellow);
            }
        }

        //track berries
        for (var i = 0; i < Game1.currentLocation.largeTerrainFeatures.Count; i++)
        {
            var feature = Game1.currentLocation.largeTerrainFeatures[i];
            if (feature is not Bush bush || bush.townBush.Value || bush.tileSheetOffset.Value != 1 ||
                !bush.inBloom(Game1.GetSeasonForLocation(Game1.currentLocation), Game1.dayOfMonth))
            {
                continue;
            }

            HudPointer.Instance.Value.DrawAsTrackingPointer(bush.tilePosition.Value, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                HudPointer.Instance.Value.DrawOverTile(bush.tilePosition.Value + new Vector2(0.5f, -1f), Color.Yellow);
            }
        }

        // track ginger
        foreach (var feature in Game1.currentLocation.terrainFeatures.Values)
        {
            if (feature is not HoeDirt { crop: { } crop } dirt || !crop.forageCrop.Value)
            {
                continue;
            }

            HudPointer.Instance.Value.DrawAsTrackingPointer(dirt.currentTileLocation, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                HudPointer.Instance.Value.DrawOverTile(dirt.currentTileLocation, Color.Yellow);
            }
        }

        // track coconuts
        foreach (var feature in Game1.currentLocation.terrainFeatures.Values)
        {
            if (feature is not Tree tree || !tree.hasSeed.Value || tree.treeType.Value != Tree.palmTree)
            {
                continue;
            }

            HudPointer.Instance.Value.DrawAsTrackingPointer(tree.currentTileLocation, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                HudPointer.Instance.Value.DrawOverTile(tree.currentTileLocation, Color.Yellow);
            }
        }
    }
}
