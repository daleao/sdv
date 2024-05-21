namespace DaLion.Shared.Pathfinding;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PathfinderObjectListChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PathfinderObjectListChangedEvent(EventManager manager)
    : ObjectListChangedEvent(manager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Pathfinders.Any();

    internal static ConditionalWeakTable<MTDStarLite, object?> Pathfinders { get; } = [];

    /// <inheritdoc />
    protected override void OnObjectListChangedImpl(object? sender, ObjectListChangedEventArgs e)
    {
        foreach (var pathfinder in Pathfinders)
        {
            e.Added.Concat(e.Removed).ForEach(pair =>
            {
                var p = pair.Key.ToPoint();
                pathfinder.Key.UpdateEdges(e.Location, p);
            });
        }
    }
}
