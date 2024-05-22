namespace DaLion.Shared.Pathfinding;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PathfinderRenderedWorldEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[Debug]
internal sealed class PathfinderRenderedWorldEvent(EventManager manager)
    : RenderedWorldEvent(manager)
{
    /// <inheritdoc />
    public override bool IsEnabled => false; //Pathfinder is not null;

    internal static MTDStarLite? Pathfinder { get; set; }

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
#if DEBUG
        if (Pathfinder!.Start is null || Pathfinder.Goal is null)
        {
            return;
        }

        foreach (var state in Pathfinder.OpenSet.Concat(Pathfinder.ClosedSet))
        {
            var r = new Rectangle(
                (state.Position.X * Game1.tileSize) + 8,
                (state.Position.Y * Game1.tileSize) + 8,
                Game1.tileSize - 8,
                Game1.tileSize - 8);
            r.X -= Game1.viewport.X;
            r.Y -= Game1.viewport.Y;
            var color = state.IsGoal
                ? Color.Blue
                : Pathfinder.PathSet.Contains(state.Position)
                    ? Color.Green
                    : !state.IsWalkable
                        ? Color.Red
                        : Color.Yellow;
            r.Highlight(color * 0.2f, e.SpriteBatch);

            Utility.drawTinyDigits(
                state.G == int.MaxValue ? 999 : state.G,
                e.SpriteBatch,
                new Vector2(r.X + 16, r.Y + 16),
                2f,
                1f,
                Color.White);
            Utility.drawTinyDigits(
                state.RHS == int.MaxValue ? 999 : state.RHS,
                e.SpriteBatch,
                new Vector2(r.X + 16, r.Y + 32),
                2f,
                1f,
                Color.White);
        }
#endif
    }
}
