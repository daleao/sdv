namespace DaLion.Stardew.Professions.Framework.Ultimate;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

using Events.Display;
using Events.GameLoop;

#endregion using directives

/// <summary>Fullscreen tinted overlay activated during Ultimate.</summary>
internal class UltimateOverlay
{
    private const float MAX_OPACITY_F = 0.3f;

    private float _opacity;
    private readonly Color _color;

    /// <summary>Construct an instance.</summary>
    /// <param name="index">The currently registered Super Mode profession's index.</param>
    public UltimateOverlay(Color color)
    {
        _color = color;
        _opacity = 0f;
    }

    /// <summary>Draw the overlay over the world.</summary>
    /// <param name="b">A <see cref="SpriteBatch" /> to draw to.</param>
    /// <remarks>This should be called from a <see cref="RenderedWorldEvent" />.</remarks>
    public void Draw(SpriteBatch b)
    {
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, _color * _opacity);
    }

    /// <summary>Gradually increase the overlay's opacity.</summary>
    public void FadeIn()
    {
        if (_opacity < MAX_OPACITY_F) _opacity += 0.01f;
        if (_opacity >= MAX_OPACITY_F)
            EventManager.Disable(typeof(UltimateOverlayFadeInUpdateTickedEvent));
    }

    /// <summary>Gradually decrease the overlay's opacity.</summary>
    public void FadeOut()
    {
        if (_opacity > 0) _opacity -= 0.01f;
        if (_opacity <= 0)
            EventManager.Disable(typeof(UltimateOverlayFadeOutUpdateTickedEvent),
                typeof(UltimateOverlayRenderedWorldEvent));
    }
}