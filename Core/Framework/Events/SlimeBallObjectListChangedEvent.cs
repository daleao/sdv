namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SlimeBallObjectListChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
public sealed class SlimeBallObjectListChangedEvent(EventManager? manager = null)
    : ObjectListChangedEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnObjectListChangedImpl(object? sender, ObjectListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation || e.Location is not SlimeHutch)
        {
            return;
        }

        foreach (var (key, value) in e.Removed)
        {
            if (value.QualifiedItemId != QualifiedBigCraftableIds.SlimeBall)
            {
                continue;
            }

            var drops = new SlimeBall(value, key).GetDrops();
            foreach (var (id, stack) in drops)
            {
                Game1.createMultipleObjectDebris(
                    id,
                    (int)key.X,
                    (int)key.Y,
                    stack,
                    1f + (Game1.player.FacingDirection == 2 ? 0f : (float)Game1.random.NextDouble()));
            }
        }
    }
}
