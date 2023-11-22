namespace DaLion.Overhaul.Modules.Professions.Events.World.TerrainFeatureListChanged;

#region using directives

using DaLion.Overhaul.Modules.Professions.TreasureHunts;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerHuntTerrainFeatureListChangedEvent : TerrainFeatureListChangedEvent
{
    private ScavengerHunt? _hunt;

    /// <summary>Initializes a new instance of the <see cref="ScavengerHuntTerrainFeatureListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ScavengerHuntTerrainFeatureListChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._hunt ??= Game1.player.Get_ScavengerHunt();
    }

    /// <inheritdoc />
    protected override void OnTerrainFeatureListChangedImpl(object? sender, TerrainFeatureListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation)
        {
            return;
        }

        if (!this._hunt!.TreasureTile.HasValue)
        {
            this.Disable();
            return;
        }

        if (!e.Location.terrainFeatures.TryGetValue(this._hunt.TreasureTile.Value, out var feature) ||
            feature is not HoeDirt)
        {
            return;
        }

        this._hunt.Complete();
        this.Disable();
    }
}
