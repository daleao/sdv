namespace DaLion.Stardew.Professions.Framework;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#endregion using directives

/// <summary>Pointer for highlighting on-screen and off-screen objects of interest for tracker professions.</summary>
internal class HudPointer
{
    private const float MAX_STEP_F = 3f, MIN_STEP_F = -3f;

    private readonly Rectangle _srcRect;

    private float _height = -42f, _jerk = 1f, _step;

    private float _Scale => ModEntry.Config.TrackPointerScale;

    /// <summary>Construct an instance.</summary>
    public HudPointer()
    {
        _srcRect = new(0, 0, Texture.Width, Texture.Height);
    }

    /// <summary>The texture that will be used to draw the pointer.</summary>
    public Texture2D Texture => Textures.Textures.PointerTx;

    /// <summary>Draw the pointer at the edge of the screen, pointing to a target tile off-screen.</summary>
    /// <param name="target">The target tile to point to.</param>
    /// <param name="color">The color of the pointer.</param>
    public void DrawAsTrackingPointer(Vector2 target, Color color)
    {
        if (StardewValley.Utility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

        var vpBounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
        Vector2 onScreenPosition = default;
        var rotation = 0f;
        if (target.X * 64f > Game1.viewport.MaxCorner.X - 64)
        {
            onScreenPosition.X = vpBounds.Right - 8;
            rotation = (float)Math.PI / 2f;
        }
        else if (target.X * 64f < Game1.viewport.X)
        {
            onScreenPosition.X = 8f;
            rotation = -(float)Math.PI / 2f;
        }
        else
        {
            onScreenPosition.X = target.X * 64f - Game1.viewport.X;
        }

        if (target.Y * 64f > Game1.viewport.MaxCorner.Y - 64)
        {
            onScreenPosition.Y = vpBounds.Bottom - 8;
            rotation = (float)Math.PI;
        }
        else if (target.Y * 64f < Game1.viewport.Y)
        {
            onScreenPosition.Y = 8f;
        }
        else
        {
            onScreenPosition.Y = target.Y * 64f - Game1.viewport.Y;
        }

        if ((int)onScreenPosition.X == 8 && (int)onScreenPosition.Y == 8) rotation += (float)Math.PI / 4f;

        if ((int)onScreenPosition.X == 8 && (int)onScreenPosition.Y == vpBounds.Bottom - 8)
            rotation += (float)Math.PI / 4f;

        if ((int)onScreenPosition.X == vpBounds.Right - 8 && (int)onScreenPosition.Y == 8)
            rotation -= (float)Math.PI / 4f;

        if ((int)onScreenPosition.X == vpBounds.Right - 8 && (int)onScreenPosition.Y == vpBounds.Bottom - 8)
            rotation -= (float)Math.PI / 4f;

        var safePos = StardewValley.Utility.makeSafe(
            renderSize: new(_srcRect.Width * Game1.pixelZoom * _Scale, _srcRect.Height * Game1.pixelZoom * _Scale),
            renderPos: onScreenPosition
        );

        Game1.spriteBatch.Draw(
            texture: Texture,
            position: safePos,
            sourceRectangle: _srcRect,
            color: color,
            rotation: rotation,
            origin: new(2f, 2f),
            scale: Game1.pixelZoom * _Scale,
            effects: SpriteEffects.None,
            layerDepth: 1f
        );
    }

    /// <summary>Draw the pointer over a target tile on-screen.</summary>
    /// <param name="target">A target tile.</param>
    /// <param name="color">The color of the pointer.</param>
    /// <remarks>Credit to <c>Bpendragon</c>.</remarks>
    public void DrawOverTile(Vector2 target, Color color)
    {
        if (!StardewValley.Utility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

        var targetPixel = new Vector2(target.X * Game1.tileSize + 32f, target.Y * Game1.tileSize + 32f + _height);
        var adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
        adjustedPixel = StardewValley.Utility.ModifyCoordinatesForUIScale(adjustedPixel);

        Game1.spriteBatch.Draw(
            texture: Texture,
            position: adjustedPixel,
            sourceRectangle: _srcRect,
            color: color,
            rotation: (float)Math.PI,
            origin: new(2f, 2f),
            scale: Game1.pixelZoom * _Scale,
            effects: SpriteEffects.None,
            layerDepth: 1f
        );
    }

    /// <summary>Advance the pointer's bobbing motion one step.</summary>
    public void Update(uint ticks)
    {
        if (ticks % (4f / ModEntry.Config.TrackPointerBobbingRate) != 0) return;

        if (_step is MAX_STEP_F or MIN_STEP_F) _jerk = -_jerk;
        _step += _jerk;
        _height += _step;
    }
}