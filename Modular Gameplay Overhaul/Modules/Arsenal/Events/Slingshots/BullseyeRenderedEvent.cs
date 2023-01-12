namespace DaLion.Overhaul.Modules.Arsenal.Events.Slingshots;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BullseyeRenderedEvent : RenderedEvent
{
    /// <summary>Initializes a new instance of the <see cref="BullseyeRenderedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BullseyeRenderedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedImpl(object? sender, RenderedEventArgs e)
    {
        if (Game1.player.CurrentTool is not Slingshot)
        {
            this.Disable();
            return;
        }

        var cursorPosition = new Vector2(Game1.getMouseX(), Game1.getMouseY());
        e.SpriteBatch.Draw(
            Game1.mouseCursors,
            cursorPosition,
            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 43),
            Color.White,
            0f,
            new Vector2(32f, 32f),
            1f,
            SpriteEffects.None,
            0.999999f);
    }
}
