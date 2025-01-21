namespace DaLion.Shared.Pathfinding;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="AsyncPathfinderObjectListChangedEvent"/> class.</summary>
/// <param name="eventManager">The <see cref="EventManager"/> instance that manages this event.</param>
/// <param name="pathfindingManager">The <see cref="PathfindingManagerAsync"/> instance that should be informed of updates.</param>
[ImplicitIgnore]
internal sealed class AsyncPathfinderObjectListChangedEvent(EventManager eventManager, PathfindingManagerAsync pathfindingManager)
    : ObjectListChangedEvent(eventManager)
{
    /// <inheritdoc />
    protected override void OnObjectListChangedImpl(object? sender, ObjectListChangedEventArgs e)
    {
        e.Added.Concat(e.Removed).ForEach(pair =>
        {
            var p = pair.Key.ToPoint();
            pathfindingManager.QueueUpdate(e.Location, p);
        });
    }
}
