namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Professions.TreasureHunts;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmhandProspectorHuntUpdateTickedEvent : UpdateTickedEvent
{
    private ProspectorHunt? _hunt;
    private IEnumerable<SObject> _stonesPrevious = null!; // set when enabled

    /// <summary>Initializes a new instance of the <see cref="FarmhandProspectorHuntUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal FarmhandProspectorHuntUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._hunt ??= Game1.player.Get_ProspectorHunt();
        this._stonesPrevious = Game1.player.currentLocation.Objects.Values.Where(o => o.IsStone()).ToList();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!e.IsMultipleOf(15))
        {
            return;
        }

        if (!this._hunt!.TreasureTile.HasValue)
        {
            this.Disable();
            return;
        }

        if (!Game1.player.currentLocation.Objects.ContainsKey(this._hunt.TreasureTile.Value))
        {
            this._hunt.Complete();
            this.Disable();
            return;
        }

        if (ProfessionsModule.Config.UseLegacyProspectorHunt)
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

        var distanceToTreasure = (int)removed.Single().DistanceTo(this._hunt!.TreasureTile.Value);
        var detectionDistance = (int)ProfessionsModule.Config.ProspectorDetectionDistance;
        if (detectionDistance > 0 && !distanceToTreasure.IsIn(1..detectionDistance))
        {
            this._stonesPrevious = stonesCurrent;
            return;
        }

        var pitch = (int)(2400f * (1f - ((float)distanceToTreasure / detectionDistance)));
        Game1.playSoundPitched("detector", pitch);
        Log.A($"Beeped at frequency {pitch} Hz");

        this._hunt.Complete();
        this.Disable();
    }
}
