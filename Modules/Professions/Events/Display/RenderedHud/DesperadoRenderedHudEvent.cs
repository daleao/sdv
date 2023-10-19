namespace DaLion.Overhaul.Modules.Professions.Events.Display.RenderedHud;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class DesperadoRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Initializes a new instance of the <see cref="DesperadoRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DesperadoRenderedHudEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        var lastUser = Game1.player;
        if (lastUser.CurrentTool is not Slingshot slingshot || !lastUser.usingSlingshot ||
            slingshot.CanAutoFire())
        {
            return;
        }

        var percent = slingshot.GetOvercharge() - 1f;
        if (percent <= 0f)
        {
            return;
        }

        e.SpriteBatch.Draw(
            Game1.mouseCursors,
            Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-48f, -160f)),
            new Rectangle(193, 1868, 47, 12),
            Color.White,
            0f,
            Vector2.Zero,
            Game1.pixelZoom,
            SpriteEffects.None,
            0.885f);

        e.SpriteBatch.Draw(
            Game1.staminaRect,
            new Rectangle(
                (int)Game1.GlobalToLocal(Game1.viewport, lastUser.Position).X - 36,
                (int)Game1.GlobalToLocal(Game1.viewport, lastUser.Position).Y - 148,
                (int)(164f * percent),
                25),
            Game1.staminaRect.Bounds,
            Utility.getRedToGreenLerpColor(percent),
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            0.887f);
    }
}
