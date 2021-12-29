using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class SuperModeRenderedWorldEvent : RenderedWorldEvent
{
    /// <inheritdoc />
    public override void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
        // draw color tint overlay
        e.SpriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds,
            ModState.SuperModeOverlayColor * ModState.SuperModeOverlayAlpha);
    }
}