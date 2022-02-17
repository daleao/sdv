namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal class DebugRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object sender, RenderedHudEventArgs e)
    {
        if (!ModEntry.Config.DebugKey.IsDown()) return;
        
        // show FPS counter
        ModEntry.FpsCounter?.Draw(Game1.currentGameTime);

        if (ModEntry.State.Value.DebugCursorPosition is null) return;

        var coords =
            $"X: {ModEntry.State.Value.DebugCursorPosition.Tile.X} Tile / {ModEntry.State.Value.DebugCursorPosition.GetScaledAbsolutePixels().X} Absolute";
        coords +=
            $"\nY: {ModEntry.State.Value.DebugCursorPosition.Tile.Y} Tile / {ModEntry.State.Value.DebugCursorPosition.GetScaledAbsolutePixels().Y} Absolute";

        // draw cursor coordinates
        e.SpriteBatch.DrawString(Game1.dialogueFont, coords, new(33f, 82f), Color.Black);
        e.SpriteBatch.DrawString(Game1.dialogueFont, coords, new(32f, 81f), Color.White);

        // draw current location
        e.SpriteBatch.DrawString(Game1.dialogueFont, $"Location: {Game1.player.currentLocation.NameOrUniqueName}", new(33f, 167f), Color.Black);
        e.SpriteBatch.DrawString(Game1.dialogueFont, $"Location: {Game1.player.currentLocation.NameOrUniqueName}", new(32f, 166f), Color.White);
    }
}