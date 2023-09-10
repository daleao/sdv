namespace DaLion.Overhaul.Modules.Professions.TreasureHunts;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

/// <summary>HUD message for treasure hunts.</summary>
internal class HuntNotification : HUDMessage
{
    private readonly Rectangle _sourceRect;

    /// <summary>Initializes a new instance of the <see cref="HuntNotification"/> class.</summary>
    /// <param name="message">The message to display.</param>
    public HuntNotification(string message)
        : base(message)
    {
        this.whatType = 0;
        this.noIcon = true;
        this.timeLeft = 5250f;
        this.fadeIn = false;
    }

    /// <summary>Initializes a new instance of the <see cref="HuntNotification"/> class.</summary>
    /// <param name="message">The message to display.</param>
    /// <param name="iconSourceRect">Source rectangle of the icon to display.</param>
    public HuntNotification(string message, Rectangle iconSourceRect)
        : base(message)
    {
        this.whatType = 0;
        this.noIcon = false;
        this.timeLeft = 5250f;
        this.fadeIn = true;
        this._sourceRect = iconSourceRect;
    }

    /// <summary>Draws the notification to the game sprite batch.</summary>
    /// <param name="b">The <see cref="SpriteBatch"/>.</param>
    /// <param name="i">Unclear.</param>
    public override void draw(SpriteBatch b, int i)
    {
        var titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
        if (this.noIcon)
        {
            var overrideX = titleSafeArea.Left + 16;
            var overrideY = (Game1.uiViewport.Width < 1400 ? -64 : 0) + titleSafeArea.Bottom -
                            ((i + 1) * 64 * 7 / 4) - 21 - (int)Game1.dialogueFont.MeasureString(this.message).Y;
            IClickableMenu.drawHoverText(
                b,
                this.message,
                Game1.dialogueFont,
                0,
                0,
                -1,
                null,
                -1,
                null,
                null,
                0,
                -1,
                -1,
                overrideX,
                overrideY,
                this.transparency);
            return;
        }

        var itemBoxPosition =
            new Vector2(titleSafeArea.Left + 16, titleSafeArea.Bottom - ((i + 1) * 64 * 7 / 4) - 64);
        if (Game1.isOutdoorMapSmallerThanViewport())
        {
            itemBoxPosition.X = Math.Max(titleSafeArea.Left + 16, -Game1.uiViewport.X + 16);
        }

        if (Game1.uiViewport.Width < 1400)
        {
            itemBoxPosition.Y -= 48f;
        }

        b.Draw(
            Game1.mouseCursors,
            itemBoxPosition,
            new Rectangle(293, 360, 26, 24),
            Color.White * this.transparency,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            1f);

        var messageWidth = Game1.smallFont.MeasureString(this.message).X;
        b.Draw(
            Game1.mouseCursors,
            new Vector2(itemBoxPosition.X + 104f, itemBoxPosition.Y),
            new Rectangle(319, 360, 1, 24),
            Color.White * this.transparency,
            0f,
            Vector2.Zero,
            new Vector2(messageWidth, 4f),
            SpriteEffects.None,
            1f);
        b.Draw(
            Game1.mouseCursors,
            new Vector2(itemBoxPosition.X + 104f + messageWidth, itemBoxPosition.Y),
            new Rectangle(323, 360, 6, 24),
            Color.White * this.transparency,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            1f);

        itemBoxPosition.X += 16f;
        itemBoxPosition.Y += 16f;
        b.Draw(
            Game1.mouseCursors,
            itemBoxPosition + (new Vector2(8f, 8f) * 4f),
            this._sourceRect,
            Color.White * this.transparency,
            0f,
            new Vector2(8f, 8f),
            4f + Math.Max(0f, (this.timeLeft - 3000f) / 900f),
            SpriteEffects.None,
            1f);

        itemBoxPosition.X += 51f;
        itemBoxPosition.Y += 51f;
        if (this.number > 1)
        {
            Utility.drawTinyDigits(
                this.number,
                b,
                itemBoxPosition,
                3f,
                1f,
                Color.White * this.transparency);
        }

        itemBoxPosition.X += 32f;
        itemBoxPosition.Y -= 33f;
        Utility.drawTextWithShadow(
            b,
            this.message,
            Game1.smallFont,
            itemBoxPosition,
            Game1.textColor * this.transparency,
            1f,
            1f,
            -1,
            -1,
            this.transparency);
    }
}
