namespace DaLion.Core.Framework.Events.Debug;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DebugRenderedHudEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[Debug]
internal sealed class DebugRenderedHudEvent(EventManager? manager = null)
    : RenderedHudEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.DebugMode;

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        State.FpsCounter?.Draw(Game1.currentGameTime);

        var yOffset = Game1.dialogueFont.MeasureString("fps").Y;
        e.SpriteBatch.DrawString(Game1.dialogueFont, $"{Game1.getMouseX()}, {Game1.getMouseY()}", new Vector2(33f, 33f + yOffset), Color.Black);
        e.SpriteBatch.DrawString(Game1.dialogueFont, $"{Game1.getMouseX()}, {Game1.getMouseY()}", new Vector2(32f, 32f + yOffset), Color.White);
    }
}
