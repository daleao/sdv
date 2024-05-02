namespace DaLion.Professions.Framework.Limits;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Fullscreen tinted overlay activated during Limit Break.</summary>
internal sealed class LimitOverlay
{
    private const float MaxOpacity = 0.3f;
    private readonly Color _color;

    private float _opacity;

    /// <summary>Initializes a new instance of the <see cref="LimitOverlay"/> class.</summary>
    /// <param name="color">The overlay <see cref="Color"/>.</param>
    internal LimitOverlay(Color color)
    {
        this._color = color;
        this._opacity = 0f;
    }

    /// <summary>Draws the overlay over the world.</summary>
    /// <param name="b">A <see cref="SpriteBatch"/> to draw to.</param>
    /// <remarks>This should be called from a <see cref="Shared.Events.RenderedWorldEvent"/>.</remarks>
    internal void Draw(SpriteBatch b)
    {
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, this._color * this._opacity);
    }

    /// <summary>Gradually increases the overlay's opacity.</summary>
    internal void FadeIn()
    {
        if (this._opacity < MaxOpacity)
        {
            this._opacity += 0.01f;
        }

        if (this._opacity >= MaxOpacity)
        {
            EventManager.Disable<LimitOverlayFadeInUpdateTickedEvent>();
        }
    }

    /// <summary>Gradually decrease the overlay's opacity.</summary>
    internal void FadeOut()
    {
        if (this._opacity > 0)
        {
            this._opacity -= 0.01f;
        }

        if (!(this._opacity <= 0))
        {
            return;
        }

        EventManager.Disable<LimitOverlayFadeOutUpdateTickedEvent>();
    }
}
