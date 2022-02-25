namespace DaLion.Stardew.Professions.Framework.TreasureHunt;

#region using directives

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

#endregion using directives

/// <summary>Pointer for highlighting on-screen and off-screen objects of interest for tracker professions.</summary>
internal class HudPointer
{
    private const float MAX_STEP_F = 3f, MIN_STEP_F = -3f;

    private float _height = -42f, _jerk = 1f, _step;

    /// <summary>The texture that will be used to draw the indicator.</summary>
    public Texture2D Texture { get; } =
        Game1.content.Load<Texture2D>(Path.Combine(ModEntry.Manifest.UniqueID, "HudPointer"));

    /// <summary>Draw the indicator at the edge of the screen, pointing to a target off-screen.</summary>
    /// <param name="target">The target tile to point to.</param>
    /// <param name="color">The color of the indicator.</param>
    public void DrawAsTrackingPointer(Vector2 target, Color color)
    {
        if (Utility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

        var vpBounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
        Vector2 onScreenPosition = default;
        var rotation = 0f;
        if (target.X * 64f > Game1.viewport.MaxCorner.X - 64)
        {
            onScreenPosition.X = vpBounds.Right - 8;
            rotation = (float) Math.PI / 2f;
        }
        else if (target.X * 64f < Game1.viewport.X)
        {
            onScreenPosition.X = 8f;
            rotation = -(float) Math.PI / 2f;
        }
        else
        {
            onScreenPosition.X = target.X * 64f - Game1.viewport.X;
        }

        if (target.Y * 64f > Game1.viewport.MaxCorner.Y - 64)
        {
            onScreenPosition.Y = vpBounds.Bottom - 8;
            rotation = (float) Math.PI;
        }
        else if (target.Y * 64f < Game1.viewport.Y)
        {
            onScreenPosition.Y = 8f;
        }
        else
        {
            onScreenPosition.Y = target.Y * 64f - Game1.viewport.Y;
        }

        if ((int) onScreenPosition.X == 8 && (int) onScreenPosition.Y == 8) rotation += (float) Math.PI / 4f;

        if ((int) onScreenPosition.X == 8 && (int) onScreenPosition.Y == vpBounds.Bottom - 8)
            rotation += (float) Math.PI / 4f;

        if ((int) onScreenPosition.X == vpBounds.Right - 8 && (int) onScreenPosition.Y == 8)
            rotation -= (float) Math.PI / 4f;

        if ((int) onScreenPosition.X == vpBounds.Right - 8 && (int) onScreenPosition.Y == vpBounds.Bottom - 8)
            rotation -= (float) Math.PI / 4f;

        var srcRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
        var safePos = Utility.makeSafe(
            renderSize: new(srcRect.Width * Game1.pixelZoom, srcRect.Height * Game1.pixelZoom),
            renderPos: onScreenPosition
        );

        Game1.spriteBatch.Draw(
            Texture,
            safePos,
            srcRect,
            color,
            rotation,
            new(2f, 2f),
            Game1.pixelZoom,
            SpriteEffects.None,
            1f
        );
    }

    /// <summary>Draw the indicator over a target tile on-screen.</summary>
    /// <param name="target">A target tile.</param>
    /// <param name="color">The color of the indicator.</param>
    /// <remarks>Credit to <c>Bpendragon</c>.</remarks>
    public void DrawOverTile(Vector2 target, Color color)
    {
        if (!Utility.isOnScreen(target * 64f + new Vector2(32f, 32f), 64)) return;

        var srcRect = new Rectangle(0, 0, 5, 4);
        var targetPixel = new Vector2(target.X * 64f + 32f, target.Y * 64f + 32f + _height);
        var adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
        adjustedPixel = Utility.ModifyCoordinatesForUIScale(adjustedPixel);

        Game1.spriteBatch.Draw(
            Texture,
            adjustedPixel,
            srcRect,
            color,
            (float) Math.PI,
            new(2f, 2f),
            Game1.pixelZoom,
            SpriteEffects.None,
            1f
        );
    }

    /// <summary>Advance the indicator's vertical offset motion by one step, in a bobbing fashion.</summary>
    public void Update(uint ticks)
    {
        if (ticks % 4 != 0) return;

        if (_step is MAX_STEP_F or MIN_STEP_F) _jerk = -_jerk;
        _step += _jerk;
        _height += _step;
    }
}