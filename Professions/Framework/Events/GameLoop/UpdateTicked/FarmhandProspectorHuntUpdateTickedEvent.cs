namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="FarmhandProspectorHuntUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class FarmhandProspectorHuntUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    private IEnumerable<SObject> _stonesPrevious = null!; // set when enabled

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._stonesPrevious = Game1.player.currentLocation.Objects.Values.Where(o => o.IsStone()).ToList();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!e.IsMultipleOf(15))
        {
            return;
        }

        var hunt = State.ProspectorHunt!;
        if (!hunt.TreasureTile.HasValue)
        {
            this.Disable();
            return;
        }

        if (!Game1.player.currentLocation.Objects.ContainsKey(hunt.TreasureTile.Value))
        {
            hunt.Complete();
            this.Disable();
            return;
        }

        if (Config.UseLegacyProspectorHunt)
        {
            return;
        }

        var location = Game1.player.currentLocation;
        var stonesCurrent = location.Objects.Values.Where(o => o.IsStone()).ToList();
        var removed = this._stonesPrevious.Except(stonesCurrent).ToList();
        if (removed.Count != 1)
        {
            this._stonesPrevious = stonesCurrent;
            return;
        }

        var distanceToTreasure = (int)removed.Single().SquaredTileDistance(hunt.TreasureTile.Value);
        var detectionDistance = (int)(Config.ProspectorDetectionDistance * Config.ProspectorDetectionDistance);
        if (detectionDistance > 0 && !distanceToTreasure.IsIn(1..detectionDistance))
        {
            this._stonesPrevious = stonesCurrent;
            return;
        }

        var pitch = (int)(2400f * (1f - ((float)distanceToTreasure / detectionDistance)));
        Game1.playSound("detector", pitch);
        Log.A($"Beeped at frequency {pitch} Hz");

        hunt.Complete();
        this.Disable();
    }
}
