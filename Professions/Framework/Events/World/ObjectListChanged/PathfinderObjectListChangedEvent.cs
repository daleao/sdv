namespace DaLion.Professions.Framework.Events.World.ObjectListChanged;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class PathfinderObjectListChangedEvent : ObjectListChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PathfinderObjectListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PathfinderObjectListChangedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnObjectListChangedImpl(object? sender, ObjectListChangedEventArgs e)
    {
        Point p;
        if (GameLocation_Pathfinder.Values.TryGetValue(e.Location, out var aStar))
        {
            e.Added.Concat(e.Removed).ForEach(pair =>
            {
                p = pair.Key.ToPoint();
                aStar.UpdateGrid(p);
            });
        }

        foreach (var piped in State.SummonedSlimes)
        {
            if (piped is not null && GreenSlime_Pathfinder.Values.TryGetValue(piped.Instance, out var dStarLite))
            {
                e.Added.Concat(e.Removed).ForEach(pair =>
                {
                    p = pair.Key.ToPoint();
                    dStarLite.UpdateEdges(p);
                });
            }
        }
    }
}
