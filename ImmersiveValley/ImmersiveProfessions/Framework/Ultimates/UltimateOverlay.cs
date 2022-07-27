using DaLion.Common.Events;

namespace DaLion.Stardew.Professions.Framework.Ultimates;

#region using directives

using Events.Display;
using Events.GameLoop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Fullscreen tinted overlay activated during Ultimate.</summary>
internal class UltimateOverlay
{
    private const float MAX_OPACITY_F = 0.3f;

    private float _opacity;
    private readonly Color _color;

    /// <summary>Construct an instance.</summary>
    /// <param name="index">The currently registered Super Mode profession's index.</param>
    internal UltimateOverlay(Color color)
    {
        _color = color;
        _opacity = 0f;
    }

    /// <summary>Draw the overlay over the world.</summary>
    /// <param name="b">A <see cref="SpriteBatch"/> to draw to.</param>
    /// <remarks>This should be called from a <see cref="RenderedWorldEvent"/>.</remarks>
    internal void Draw(SpriteBatch b)
    {
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, _color * _opacity);
    }

    /// <summary>Gradually increase the overlay's opacity.</summary>
    internal void FadeIn()
    {
        if (_opacity < MAX_OPACITY_F) _opacity += 0.01f;
        if (_opacity >= MAX_OPACITY_F)
            ModEntry.EventManager.Disable<UltimateOverlayFadeInUpdateTickedEvent>();
    }

    /// <summary>Gradually decrease the overlay's opacity.</summary>
    internal void FadeOut()
    {
        if (_opacity > 0) _opacity -= 0.01f;
        if (!(_opacity <= 0)) return;
        ModEntry.EventManager.Disable<UltimateOverlayFadeOutUpdateTickedEvent>();
        ModEntry.EventManager.Disable<UltimateOverlayRenderedWorldEvent>();
    }
}