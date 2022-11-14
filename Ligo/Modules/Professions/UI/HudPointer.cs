namespace DaLion.Ligo.Modules.Professions.UI;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Pointer for highlighting on-screen and off-screen objects of interest for tracker professions.</summary>
internal sealed class HudPointer
{
    private const float MaxStep = 3f;
    private const float MinStep = -3f;

    private readonly Rectangle _srcRect;

    private float _height = -42f;
    private float _jerk = 1f;
    private float _step;

    /// <summary>Initializes a new instance of the <see cref="HudPointer"/> class.</summary>
    public HudPointer()
    {
        this._srcRect = new Rectangle(0, 0, this.Texture.Width, this.Texture.Height);
    }

    /// <summary>Gets the texture that will be used to draw the pointer.</summary>
    public Texture2D Texture => Textures.PointerTx;

    /// <summary>Gets the user's chosen scale multiplier for the pointer.</summary>
    private static float Scale => ModEntry.Config.Professions.TrackPointerScale;

    /// <summary>Draw the pointer at the edge of the screen, pointing to a target tile off-screen.</summary>
    /// <param name="target">The target tile to point to.</param>
    /// <param name="color">The color of the pointer.</param>
    public void DrawAsTrackingPointer(Vector2 target, Color color)
    {
        if (Utility.isOnScreen((target * 64f) + new Vector2(32f, 32f), 64))
        {
            return;
        }

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
            onScreenPosition.X = (target.X * 64f) - Game1.viewport.X;
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
            onScreenPosition.Y = (target.Y * 64f) - Game1.viewport.Y;
        }

        if ((int)onScreenPosition.X == 8 && (int)onScreenPosition.Y == 8)
        {
            rotation += (float)Math.PI / 4f;
        }

        if ((int)onScreenPosition.X == 8 && (int)onScreenPosition.Y == vpBounds.Bottom - 8)
        {
            rotation += (float)Math.PI / 4f;
        }

        if ((int)onScreenPosition.X == vpBounds.Right - 8 && (int)onScreenPosition.Y == 8)
        {
            rotation -= (float)Math.PI / 4f;
        }

        if ((int)onScreenPosition.X == vpBounds.Right - 8 && (int)onScreenPosition.Y == vpBounds.Bottom - 8)
        {
            rotation -= (float)Math.PI / 4f;
        }

        var safePos = Utility.makeSafe(
            renderSize: new Vector2(
                this._srcRect.Width * Game1.pixelZoom * Scale,
                this._srcRect.Height * Game1.pixelZoom * Scale),
            renderPos: onScreenPosition);

        Game1.spriteBatch.Draw(
            this.Texture,
            safePos,
            this._srcRect,
            color,
            rotation,
            new Vector2(2f, 2f),
            Game1.pixelZoom * Scale,
            SpriteEffects.None,
            1f);
    }

    /// <summary>Draw the pointer over a target tile on-screen.</summary>
    /// <param name="target">A target tile.</param>
    /// <param name="color">The color of the pointer.</param>
    /// <remarks>Credit to <c>Bpendragon</c>.</remarks>
    public void DrawOverTile(Vector2 target, Color color)
    {
        if (!Utility.isOnScreen((target * 64f) + new Vector2(32f, 32f), 64))
        {
            return;
        }

        var targetPixel = new Vector2(
            (target.X * Game1.tileSize) + 32f,
            (target.Y * Game1.tileSize) + 32f + this._height);
        var adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
        adjustedPixel = Utility.ModifyCoordinatesForUIScale(adjustedPixel);
        Game1.spriteBatch.Draw(
            this.Texture,
            adjustedPixel,
            this._srcRect,
            color,
            (float)Math.PI,
            new Vector2(2f, 2f),
            Game1.pixelZoom * Scale,
            SpriteEffects.None,
            1f);
    }

    /// <summary>Advance the pointer's bobbing motion one step.</summary>
    /// <param name="ticks">The number of ticks elapsed since the game started.</param>
    public void Update(uint ticks)
    {
        if (ticks % (4f / ModEntry.Config.Professions.TrackPointerBobbingRate) != 0)
        {
            return;
        }

        if (this._step is MaxStep or MinStep)
        {
            this._jerk = -this._jerk;
        }

        this._step += this._jerk;
        this._height += this._step;
    }
}
