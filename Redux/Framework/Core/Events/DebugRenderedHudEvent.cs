namespace DaLion.Redux.Framework.Core.Events;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Initializes a new instance of the <see cref="DebugRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DebugRenderedHudEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        // show FPS counter
        Globals.FpsCounter?.Draw(Game1.currentGameTime);

        if (Globals.DebugCursorPosition is null)
        {
            return;
        }

        var coords =
            $"X: {Globals.DebugCursorPosition.Tile.X} Tile / {Globals.DebugCursorPosition.GetScaledAbsolutePixels().X} Absolute";
        coords +=
            $"\nY: {Globals.DebugCursorPosition.Tile.Y} Tile / {Globals.DebugCursorPosition.GetScaledAbsolutePixels().Y} Absolute";

        // draw cursor coordinates
        e.SpriteBatch.DrawString(
            Game1.dialogueFont,
            coords,
            new Vector2(33f, 82f),
            Color.Black);
        e.SpriteBatch.DrawString(
            Game1.dialogueFont,
            coords,
            new Vector2(32f, 81f),
            Color.White);

        // draw current location
        e.SpriteBatch.DrawString(
            Game1.dialogueFont,
            $"Location: {Game1.player.currentLocation.NameOrUniqueName}",
            new Vector2(33f, 167f),
            Color.Black);
        e.SpriteBatch.DrawString(
            Game1.dialogueFont,
            $"Location: {Game1.player.currentLocation.NameOrUniqueName}",
            new Vector2(32f, 166f),
            Color.White);
    }
}
