namespace DaLion.Professions.Framework.UI;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Monsters;
using HealthBarPosition = DaLion.Professions.ProfessionsConfig.HealthBarPosition;
using ScreenAnchorPosition = DaLion.Professions.ProfessionsConfig.ScreenAnchorPosition;

#endregion using directives

internal sealed class PipedMinionHudComponent : ClickableTextureComponent
{
    private const int CLICK_COOLDOWN = 120;

    private PipedSlime _piped;
    private PipedMinionHud _parent;
    private int _clickCooldown;
    private int _relativeX, _relativeY;
    private Vector2 _position;

    internal PipedMinionHudComponent(PipedSlime piped, PipedMinionHud parent, int indexInMenu = -1)
        : base(
            new Rectangle(parent.xPositionOnScreen, parent.yPositionOnScreen, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom),
            Textures.Minion,
            new Rectangle(0, 0, 9, 9),
            Game1.pixelZoom)
    {
        if (indexInMenu == -1)
        {
            indexInMenu = parent.allClickableComponents.Count;
        }

        this._piped = piped;
        this._parent = parent;
        this.myID = indexInMenu;

        const int sideLength = 9;
        const int padding = 2;
        switch (Config.MinionHudAnchorPosition)
        {
            case ScreenAnchorPosition.Top:
                this._relativeX = indexInMenu * (sideLength + padding) * Game1.pixelZoom;
                break;
            default:
                this._relativeY = indexInMenu * (sideLength + padding) * Game1.pixelZoom;
                break;
        }

        this.bounds.X += this._relativeX;
        this.bounds.Y += this._relativeY;
        this._position = new Vector2(this.bounds.X, this.bounds.Y);

        if (this._Slime.IsTigerSlime())
        {
            this.sourceRect.X += 9;
        }
    }

    private GreenSlime _Slime => this._piped.Slime;

    public override void draw(SpriteBatch b)
    {
        // draw portrait
        var color = this._Slime.Health <= 0 || this._piped.IsDismissed
            ? Color.DimGray
            : this._Slime.IsTigerSlime()
                ? Color.White
                : this._piped.Color;
        b.Draw(
            texture: this.texture,
            position: this._position,
            sourceRectangle: this.sourceRect,
            color: color,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: Game1.pixelZoom,
            effects: Config.MinionHudAnchorPosition == ScreenAnchorPosition.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
            layerDepth: 1.1f);

        if (!this._Slime.IsTigerSlime())
        {
            // redraw eyes
            b.Draw(
                texture: this.texture,
                position: this._position + (new Vector2(3, 4) * Game1.pixelZoom),
                sourceRectangle: new Rectangle(2, 4, 1, 3),
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Game1.pixelZoom,
                effects: Config.MinionHudAnchorPosition == ScreenAnchorPosition.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: 1.1f);

            b.Draw(
                texture: this.texture,
                position: this._position + (new Vector2(6, 4) * Game1.pixelZoom),
                sourceRectangle: new Rectangle(5, 4, 1, 3),
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Game1.pixelZoom,
                effects: Config.MinionHudAnchorPosition == ScreenAnchorPosition.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: 1.1f);
        }

        if (this._clickCooldown-- > 0)
        {
            var fillPct = (float)this._clickCooldown / CLICK_COOLDOWN;
            var cooldownRect = this.bounds;
            var fullHeight = this.bounds.Height;
            cooldownRect.Height = (int)(fillPct * cooldownRect.Height);
            cooldownRect.Y += fullHeight - cooldownRect.Height;
            b.Draw(
                texture: Game1.staminaRect,
                destinationRectangle: cooldownRect,
                color: Color.Black * 0.6f);

            var text = ((this._clickCooldown / 60) + 1).ToString();
            var textSize = Game1.smallFont.MeasureString(text);
            var textOffset = new Vector2(
                (int)(((4.5f - 1f) * Game1.pixelZoom) - (textSize.X / 2f)),
                (int)((4.5f * Game1.pixelZoom) - (textSize.Y / 2f)));
            Utility.drawBoldText(b, text, Game1.smallFont, this._position + textOffset, Color.White);

            if (this._clickCooldown == 0)
            {
                Game1.playSound("objectiveComplete");
            }

            return;
        }

        if (this._Slime.Health <= 0)
        {
            var text = this._piped.RespawnTimer.ToString();
            var textSize = Game1.smallFont.MeasureString(text);
            var textOffset = new Vector2(
                (int)(((4.5f - 1f) * Game1.pixelZoom) - (textSize.X / 2f)),
                (int)((4.5f * Game1.pixelZoom) - (textSize.Y / 2f)));
            Utility.drawBoldText(b, text, Game1.smallFont, this._position + textOffset, Color.Red);
            return;
        }

        if (Config.MinionHealthBarPosition is not (HealthBarPosition.Hud or HealthBarPosition.Both) || this._piped.IsDismissed)
        {
            return;
        }

        // draw health bar
        var fullBarLength = 9 * Game1.pixelZoom;
        var barWidth = 1 * Game1.pixelZoom;
        var fillPercent = (float)this._Slime.Health / this._Slime.MaxHealth;
        var barLength = fullBarLength * fillPercent;
        color = Utility.getRedToGreenLerpColor(fillPercent);

        const int sideLength = 9;
        var destinationRect = Rectangle.Empty;
        switch (Config.MinionHudAnchorPosition)
        {
            case ScreenAnchorPosition.Top:
                destinationRect = new Rectangle(
                    this.bounds.X,
                    this.bounds.Y + (sideLength * Game1.pixelZoom),
                    (int)barLength,
                    barWidth);
                break;
            case ScreenAnchorPosition.Left:
                destinationRect = new Rectangle(
                    this.bounds.X + (sideLength * Game1.pixelZoom),
                    this.bounds.Y,
                    barWidth,
                    (int)barLength);
                break;
            case ScreenAnchorPosition.Right:
                destinationRect = new Rectangle(
                    this.bounds.X - (sideLength * Game1.pixelZoom),
                    this.bounds.Y,
                    barWidth,
                    (int)barLength);
                break;
        }

        b.Draw(
            texture: Game1.staminaRect,
            destinationRectangle: destinationRect,
            color: color);
    }

    public void DrawTooltip(SpriteBatch b)
    {
        var boxTexture = Game1.menuTexture;
        var boxSourceRect = new Rectangle(0, 256, 60, 60);
        var font = Game1.smallFont;

        var healthStr = this._Slime.MaxHealth.ToString();
        var attackStr = this._Slime.DamageToFarmer.ToString();
        var defenseStr = this._Slime.resilience.Value.ToString();
        var dismissStr = I18n.Piper_Slime_Dismiss();
        var healthStrLength = (int)font.MeasureString(healthStr).X;
        var attackStrLength = (int)font.MeasureString(attackStr).X;
        var defenseStrLength = (int)font.MeasureString(defenseStr).X;
        var width = 32 + (healthStrLength + 40 + 32) + (attackStrLength + 40 + 32) + (defenseStrLength + 40 + 32);
        var height = ((int)Math.Max(font.MeasureString("TT").Y, 48f) * 2) + 32;
        var (x, y) = Game1.getMousePosition();
        switch (Config.MinionHudAnchorPosition)
        {
            case ScreenAnchorPosition.Left:
                y -= height;
                break;
            case ScreenAnchorPosition.Right:
                y -= height;
                x -= width;
                break;
            case ScreenAnchorPosition.Top:
                x -= width;
                break;
        }

        IClickableMenu.drawTextureBox(b, boxTexture, boxSourceRect, x, y, width, height, Color.White, 1);
        var startingX = x;

        b.DrawHealthIcon(new Vector2(x + 20f, y + 20f));
        Utility.drawTextWithShadow(b, healthStr, font, new Vector2(x + 68f, y + 28f), Game1.textColor * 0.9f);
        x += healthStrLength + 40 + 32;

        b.DrawAttackIcon(new Vector2(x + 20f, y + 20f));
        Utility.drawTextWithShadow(b, attackStr, font, new Vector2(x + 68f, y + 28f), Game1.textColor * 0.9f);
        x += attackStrLength + 40 + 32;

        b.DrawDefenseIcon(new Vector2(x + 20f, y + 20f));
        Utility.drawTextWithShadow(b, defenseStr, font, new Vector2(x + 68f, y + 28f), Game1.textColor * 0.9f);
        x = startingX;

        y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        Utility.drawTextWithShadow(b, dismissStr, font, new Vector2(x + 20f, y + 28f), Game1.textColor * 0.9f);
    }

    public void OnClick()
    {
        if (this._clickCooldown > 0)
        {
            return;
        }

        this._piped.IsDismissed = !this._piped.IsDismissed;
        this._clickCooldown = CLICK_COOLDOWN;
        Game1.playSound("bob");
    }

    public void OnClick(int x, int y)
    {
        if (this.containsPoint(x, y))
        {
            this.OnClick();
        }
    }
}
