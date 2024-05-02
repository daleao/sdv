namespace DaLion.Professions.Framework.Events.World.TerrainFeatureListChanged;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerHuntTerrainFeatureListChangedEvent : TerrainFeatureListChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ScavengerHuntTerrainFeatureListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ScavengerHuntTerrainFeatureListChangedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnTerrainFeatureListChangedImpl(object? sender, TerrainFeatureListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation)
        {
            return;
        }

        var hunt = State.ScavengerHunt!;
        if (!hunt.TreasureTile.HasValue)
        {
            this.Disable();
            return;
        }

        if (!e.Location.terrainFeatures.TryGetValue(hunt.TreasureTile.Value, out var feature) ||
            feature is not HoeDirt)
        {
            return;
        }

        hunt.Complete();
        this.Disable();
    }
}
