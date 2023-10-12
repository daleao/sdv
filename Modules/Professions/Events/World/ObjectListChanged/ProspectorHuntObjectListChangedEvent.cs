namespace DaLion.Overhaul.Modules.Professions.Events.World.ObjectListChanged;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.TreasureHunts;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntObjectListChangedEvent : ObjectListChangedEvent
{
    private ProspectorHunt? _hunt;

    /// <summary>Initializes a new instance of the <see cref="ProspectorHuntObjectListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorHuntObjectListChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnObjectListChangedImpl(object? sender, ObjectListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation)
        {
            return;
        }

        this._hunt ??= Game1.player.Get_ProspectorHunt();
        if (!this._hunt.TreasureTile.HasValue)
        {
            this.Disable();
            return;
        }

        var removed = e.Removed.ToList();
        if (!removed.Any(r => r.Value.IsStone() && r.Key == this._hunt.TreasureTile.Value))
        {
            return;
        }

        this._hunt.Complete();
        this.Disable();
    }
}
