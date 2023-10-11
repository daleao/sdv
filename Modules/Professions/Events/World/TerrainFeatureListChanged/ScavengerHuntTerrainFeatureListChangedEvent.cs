namespace DaLion.Overhaul.Modules.Professions.Events.World.TerrainFeatureListChanged;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.TreasureHunts;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

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
    protected override void OnTerrainFeatureListChangedImpl(object? sender, TerrainFeatureListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation)
        {
            return;
        }

        this._hunt ??= Game1.player.Get_ScavengerHunt();
        if (!this._hunt.TreasureTile.HasValue)
        {
            this.Disable();
            return;
        }

        if (e.Added
                .Where(a => a.Value is HoeDirt)
                .Select(a => a.Key)
                .All(key => key != this._hunt.TreasureTile.Value))
        {
            return;
        }

        this._hunt.Complete();
        this.Disable();
    }
}
