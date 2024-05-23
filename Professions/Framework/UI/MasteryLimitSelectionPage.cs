namespace DaLion.Professions.Framework.UI;

#region using directives

using System.Reflection.Metadata.Ecma335;
using DaLion.Professions.Framework.Limits;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

#endregion using directives

/// <summary>The Limit Break selection menu displayed upon Mastering the Combat skill.</summary>
public sealed class MasteryLimitSelectionPage : IClickableMenu
{
    private readonly List<ClickableTextureComponent> _textureComponents = [];
    private readonly Color _backItemColor = new(132, 160, 255, 220);
    private readonly Color _backItemColorHover = new(132, 160, 255, 150);
    private readonly string _menuTitle;
    private string _hoverText = string.Empty;
    private float _destroyTimer;
    private float _pressedButtonTimer;

    /// <summary>Initializes a new instance of the <see cref="MasteryLimitSelectionPage"/> class.</summary>
    public MasteryLimitSelectionPage()
        : base(
            (int)Utility.getTopLeftPositionForCenteringOnScreen(720, 320).X,
            (int)Utility.getTopLeftPositionForCenteringOnScreen(720, 320).Y,
            720,
            320,
            showUpperRightCloseButton: true)
    {
        this.closeSound = "stone_button";
        this._menuTitle = I18n.Prestige_Mastery_Choose();
        foreach (var profession in ((ISkill)Skill.Combat).TierTwoProfessions.Cast<Profession>())
        {
            var limit = LimitBreak.FromId(profession.Id);
            this._textureComponents.Add(new ClickableTextureComponent(Rectangle.Empty, Game1.mouseCursors, profession.TargetSheetRect, 4f, drawShadow: true)
            {
                name = limit.DisplayName,
                hoverText = limit.Description,
                myID = limit.Id,
                region = 0,
                myAlternateID = Game1.player.HasProfession(limit.ParentProfession) ? 1 : 0,
            });
        }

        var yHeight = 80;
        for (var i = 0; i < this._textureComponents.Count; i++)
        {
            this._textureComponents[i].bounds = new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + 64 + (int)yHeight, this.width - 128, 64);
            this._textureComponents[i].label = Game1.parseText(this._textureComponents[i].label, Game1.smallFont, this.width - 200);
            yHeight += (int)Game1.smallFont.MeasureString(this._textureComponents[i].label).Y;
            if (i < this._textureComponents.Count - 1)
            {
                yHeight += this._textureComponents[i].sourceRect.Height > 16 ? 132 : 80;
            }
        }

        this.height += yHeight;
        this.height -= 48;

        var num = this.yPositionOnScreen;
        this.yPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(800, this.height).Y;
        var offset = num - this.yPositionOnScreen;
        foreach (var c in this._textureComponents)
        {
            c.bounds.Y -= offset;
        }

        this.upperRightCloseButton.bounds.Y -= offset;
        if (!Game1.options.SnappyMenus)
        {
            return;
        }

        this.populateClickableComponentList();
        this.allClickableComponents.Reverse();
        ClickableComponent.ChainNeighborsUpDown(this.allClickableComponents);
        this.currentlySnappedComponent = this.getComponentWithID(0);
        this.snapCursorToCurrentSnappedComponent();
    }

    /// <inheritdoc />
    public override void draw(SpriteBatch b)
    {
        this.BackgroundDraw(b);
        SpriteText.drawStringHorizontallyCenteredAt(
            b,
            this._menuTitle,
            this.xPositionOnScreen + (this.width / 2),
            this.yPositionOnScreen + 48,
            9999,
            -1,
            9999,
            1f,
            0.88f,
            junimoText: false,
            Color.Black);
        foreach (var c in this._textureComponents)
        {
            drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(403, 373, 9, 9),
                c.bounds.X,
                c.bounds.Y,
                c.bounds.Width,
                c.bounds.Height,
                c.region == 0 ? this._backItemColor : this._backItemColorHover,
                3f,
                drawShadow: false);
            if (c.myAlternateID == 0)
            {
                drawTextureBox(
                    b,
                    Game1.mouseCursors,
                    new Rectangle(403, 373, 9, 9),
                    c.bounds.X,
                    c.bounds.Y,
                    c.bounds.Width,
                    c.bounds.Height,
                    Color.Black * (c.region == 0 ? 0.75f : 0.6f),
                    3f,
                    drawShadow: false);
            }

            var iconScale = 16 / Math.Max(c.sourceRect.Height, c.sourceRect.Width);
            Utility.drawWithShadow(
                b,
                c.texture,
                c.getVector2() + new Vector2(12f, 6f),
                c.sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                3f * iconScale,
                flipped: false,
                -1f,
                -1,
                -1,
                0.25f);
            Utility.drawTextWithColoredShadow(
                b,
                c.name,
                Game1.dialogueFont,
                c.getVector2() + new Vector2(72f, c.bounds.Height / 2) -
                    new Vector2(0f, (int)Math.Ceiling(Game1.dialogueFont.MeasureString(c.name).Y / 2f) - 3),
                Color.Black,
                Color.Black * 0.15f);
        }

        base.draw(b);
        if (this._hoverText.Length > 0)
        {
            NewDrawHoverText(
                b,
                Game1.parseText(this._hoverText, Game1.smallFont, 500),
                Game1.smallFont,
                boxTexture: Game1.mouseCursors_1_6,
                boxSourceRect: new Rectangle(1, 85,21, 21),
                textColor: Color.Black,
                textShadowColor: Color.Black * 0.15f,
                boxScale: 2f);
        }

        this.drawMouse(b);
    }

    /// <inheritdoc />
    public override void performHoverAction(int x, int y)
    {
        if (this._destroyTimer > 0f)
        {
            return;
        }

        this._hoverText = string.Empty;
        foreach (var c in this._textureComponents)
        {
            c.region = 0;
            if (!c.bounds.Contains(x, y))
            {
                continue;
            }

            Game1.SetFreeCursorDrag();
            c.region = 1;
            if (c.myAlternateID == 0)
            {
                this._hoverText = I18n.Prestige_Mastery_Cant();
            }
            else if (!string.IsNullOrEmpty(c.hoverText))
            {
                this._hoverText = c.hoverText;
            }
        }

        base.performHoverAction(x, y);
    }

    /// <inheritdoc />
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (this._destroyTimer > 0f)
        {
            return;
        }

        foreach (var c in this._textureComponents)
        {
            if (c.myID == -1 || c.myAlternateID != 1 || !c.bounds.Contains(x, y))
            {
                continue;
            }

            Game1.playSound("cowboy_monsterhit");
            DelayedAction.playSoundAfterDelay("cowboy_monsterhit", 200);
            this._pressedButtonTimer = 200f;
            State.LimitBreak = LimitBreak.FromId(c.myID);
            break;
        }
    }

    /// <inheritdoc />
    public override void update(GameTime time)
    {
        if (this._destroyTimer > 0f)
        {
            this._destroyTimer -= (int)time.ElapsedGameTime.TotalMilliseconds;
            if (this._destroyTimer <= 0f)
            {
                Game1.activeClickableMenu = null;
                Game1.playSound("discoverMineral");
                //Game1.delayedActions.Add(new DelayedAction(
                //    350,
                //    () => Game1.drawObjectDialogue(I18n.Prestige_Mastery_Unlocked(skill.DisplayName))));
            }
        }

        if (this._pressedButtonTimer > 0f)
        {
            this._pressedButtonTimer -= (int)time.ElapsedGameTime.TotalMilliseconds;
            if (this._pressedButtonTimer <= 0f)
            {
                this._destroyTimer = 100f;
            }
        }

        base.update(time);
    }

    /// <summary>Draws the menu background.</summary>
    /// <param name="b">The <see cref="SpriteBatch"/>.</param>
    private void BackgroundDraw(SpriteBatch b)
    {
        if (!Game1.options.showClearBackgrounds)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
        }

        drawTextureBox(
            b,
            Game1.mouseCursors_1_6,
            new Rectangle(1, 85, 21, 21),
            this.xPositionOnScreen,
            this.yPositionOnScreen,
            this.width,
            this.height,
            Color.White,
            4f);
        b.Draw(
            Game1.mouseCursors_1_6,
            this.Position + (new Vector2(6f, 7f) * 4f),
            new Rectangle(0, 144, 23, 23),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            0.88f);
        b.Draw(
            Game1.mouseCursors_1_6,
            this.Position + new Vector2(24f, this.height - 24),
            new Rectangle(0, 144, 23, 23),
            Color.White,
            -(float)Math.PI / 2f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            0.88f);
        b.Draw(
            Game1.mouseCursors_1_6,
            this.Position + new Vector2(this.width - 24, 28f),
            new Rectangle(0, 144, 23, 23),
            Color.White,
            -4.712389f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            0.88f);
        b.Draw(
            Game1.mouseCursors_1_6,
            this.Position + new Vector2(this.width - 24, this.height - 24),
            new Rectangle(0, 144, 23, 23),
            Color.White,
            (float)Math.PI,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            0.88f);
    }

    private static void NewDrawHoverText(
        SpriteBatch b,
        string text,
        SpriteFont font,
        int xOffset = 0,
        int yOffset = 0,
        string? boldTitleText = null,
        int overrideX = -1,
        int overrideY = -1,
        float alpha = 1f,
        Texture2D? boxTexture = null,
        Rectangle? boxSourceRect = null,
        Color? boxColor = null,
        Color? boxShadowColor = null,
        Color? textColor = null,
        Color? textShadowColor = null,
        float boxScale = 1f,
        int boxWidthOverride = -1,
        int boxHeightOverride = -1,
        bool drawBoxShadow = true,
        bool drawTextShadow = true)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        boxTexture ??= Game1.menuTexture;
        boxSourceRect ??= new Rectangle(0, 256, 60, 60);
        boxColor ??= Color.White;
        textColor ??= Game1.textColor;
        textShadowColor ??= Game1.textShadowColor;
        boxShadowColor ??= Color.Black;

        var boldTextSize =
            boldTitleText != null ? Game1.dialogueFont.MeasureString(boldTitleText) : new Vector2(0f, 0f);
        var x = overrideX != -1 ? overrideX : Game1.getOldMouseX() + 32 + xOffset;
        var y = overrideY != -1 ? overrideY : Game1.getOldMouseY() + 32 + yOffset;
        var width = (int)Math.Max(boldTextSize.X, font.MeasureString(text).X + 32f) + 4;
        var height = (int)(boldTitleText != null ? boldTextSize.Y + 16f : 0f) + (int)font.MeasureString(text).Y + 32;
        var textWidth = boxWidthOverride != -1 ? boxWidthOverride : width;
        var textHeight = boxHeightOverride != -1 ? boxHeightOverride : height;
        if (x + textWidth > Utility.getSafeArea().Right)
        {
            x = Utility.getSafeArea().Right - textWidth;
            y += 16;
        }

        if (y + textHeight > Utility.getSafeArea().Bottom)
        {
            x += 16;
            if (x + textWidth > Utility.getSafeArea().Right)
            {
                x = Utility.getSafeArea().Right - textWidth;
            }

            y = Utility.getSafeArea().Bottom - textHeight;
        }

        if (drawBoxShadow)
        {
            drawTextureBox(
                b,
                boxTexture,
                boxSourceRect.Value, x - 8, y + 8,
                textWidth,
                textHeight,
                boxShadowColor.Value * 0.5f * alpha,
                boxScale,
                drawShadow: false);
        }

        drawTextureBox(
            b,
            boxTexture,
            boxSourceRect.Value,
            x,
            y,
            textWidth,
            textHeight,
            boxColor.Value * alpha,
            boxScale,
            drawShadow: false);
        if (boldTitleText != null)
        {
            drawTextureBox(
                b, boxTexture,
                boxSourceRect.Value,
                x,
                y + 4,
                width,
                (int)boldTextSize.Y + 32 - 4,
                boxShadowColor.Value * 0.25f * alpha,
                boxScale,
                drawShadow: false);
            drawTextureBox(
                b,
                boxTexture,
                boxSourceRect.Value,
                x,
                y,
                width,
                (int)boldTextSize.Y + 32 - 4,
                boxColor.Value * alpha,
                boxScale,
                drawShadow: false);
            if (drawTextShadow)
            {
                b.DrawString(
                    Game1.dialogueFont,
                    boldTitleText,
                    new Vector2(x + 16, y + 16 + 4) + new Vector2(2f, 2f),
                    textShadowColor.Value * alpha);
                b.DrawString(
                    Game1.dialogueFont,
                    boldTitleText,
                    new Vector2(x + 16, y + 16 + 4) + new Vector2(0f, 2f),
                    textShadowColor.Value * alpha);
            }

            b.DrawString(
                Game1.dialogueFont,
                boldTitleText, new Vector2(x + 16, y + 16 + 4),
                textColor.Value * 0.9f * alpha);
            y += (int)Game1.dialogueFont.MeasureString(boldTitleText).Y + 16;
        }

        if (drawTextShadow)
        {
            b.DrawString(
                font, 
                text,
                new Vector2(x + 16, y + 16 + 4) + new Vector2(2f, 2f),
                textShadowColor.Value * alpha);
            b.DrawString(
                font,
                text,
                new Vector2(x + 16, y + 16 + 4) + new Vector2(0f, 2f),
                textShadowColor.Value * alpha);
            b.DrawString(
                font,
                text,
                new Vector2(x + 16, y + 16 + 4) + new Vector2(2f, 0f),
                textShadowColor.Value * alpha);
        }

        b.DrawString(font, text, new Vector2(x + 16, y + 16 + 4), textColor.Value * 0.9f * alpha);
    }
}
