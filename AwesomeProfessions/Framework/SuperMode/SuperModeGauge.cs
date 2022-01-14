namespace DaLion.Stardew.Professions.Framework.SuperMode;

#region using directives

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

using AssetLoaders;
using Events.Display;
using Events.GameLoop;
using Events.Input;
using Extensions;
using Patches.Foraging;

#endregion using directives

/// <summary>HUD component to show the player their current Super Stat value.</summary>
internal class SuperModeGauge
{
    private const int MAX_BAR_HEIGHT_I = 168,
        TEXTURE_HEIGHT_I = 46,
        TICKS_BETWEEN_SHAKES_I = 120,
        SHAKE_DURATION_I = 15,
        FADE_OUT_DELAY_I = 180,
        FADE_OUT_DURATION_I = 30;

    private double _value,
        _shakeTimer = SHAKE_DURATION_I,
        _nextShake = TICKS_BETWEEN_SHAKES_I,
        _fadeOutTimer = FADE_OUT_DELAY_I + FADE_OUT_DURATION_I;
    private float _opacity = 1f;
    private bool _shake;

    #region properties

    /// <summary>The texture that will be used to draw the gauge.</summary>
    public static Texture2D Texture { get; } = Textures.SuperModeGaugeTx;

    /// <summary>The current value of the player's Super Mode gauge.</summary>
    public double CurrentValue
    {
        get => _value;
        set
        {
            if (Math.Abs(_value - value) < 0.01) return;

            if (value <= 0)
            {
                _value = 0;
                OnGaugeReturnedToZero();
            }
            else
            {
                if (_value == 0f) OnGaugeRaisedAboveZero();

                if (value >= MaxValue) OnGaugeFilled();

                _value = Math.Min(value, MaxValue);
            }
        }
    }

    public bool IsFull => CurrentValue >= MaxValue;

    /// <summary>The maximum value of the player's Super Mode gauge.</summary>
    public static int MaxValue =>
        Game1.player.CombatLevel >= 10
            ? Game1.player.CombatLevel * 50
            : 500;

    #endregion properties

    #region public methods

    /// <summary>Draw the gauge and all it's components to the HUD.</summary>
    /// <param name="b">A <see cref="SpriteBatch" /> to draw to.</param>
    /// <remarks>This should be called from a <see cref="RenderingHudEvent" />.</remarks>
    public void Draw(SpriteBatch b)
    {
        if (_opacity <= 0f) return;

        var isSuperModeActive = ModEntry.State.Value.SuperMode.IsActive;

        // get bar position
        var topOfBar = new Vector2(
            Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 56,
            Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 200
        );

        if (Game1.isOutdoorMapSmallerThanViewport())
            topOfBar.X = Math.Min(topOfBar.X,
                -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 48);

        if (Game1.showingHealth) topOfBar.X -= 100;
        else topOfBar.X -= 44;

        // shake horizontally if full and on stand-by, if active also shake vertically
        if (_shake || isSuperModeActive)
        {
            topOfBar.X += Game1.random.Next(-3, 4);
            if (isSuperModeActive) topOfBar.Y += Game1.random.Next(-3, 4);
        }

        // draw bar in thirds for flexibility
        Rectangle srcRect, destRect;

        // top
        srcRect = new(0, 0, 9, 16);
        b.Draw(
            Texture,
            topOfBar,
            srcRect,
            Color.White * _opacity,
            0f,
            Vector2.Zero,
            Game1.pixelZoom,
            SpriteEffects.None,
            1f
        );

        // middle
        srcRect = new(0, 16, 9, 16);
        destRect = new((int) topOfBar.X, (int) (topOfBar.Y + 64f), 36, 56);
        b.Draw(
            Texture,
            destRect,
            srcRect,
            Color.White * _opacity
        );

        // bottom
        srcRect = new(0, 30, 9, 16);
        b.Draw(
            Texture,
            new(topOfBar.X, topOfBar.Y + 120f),
            srcRect,
            Color.White * _opacity,
            0f,
            Vector2.Zero,
            Game1.pixelZoom,
            SpriteEffects.None,
            1f
        );

        var ratio = CurrentValue / MaxValue;
        var srcHeight = (int) (TEXTURE_HEIGHT_I * ratio) - 2;
        var destHeight = (int) (MAX_BAR_HEIGHT_I * ratio);

        srcRect = new(10, TEXTURE_HEIGHT_I - srcHeight, 3, srcHeight);
        destRect = new((int) topOfBar.X + 12, (int) topOfBar.Y + 8 + (MAX_BAR_HEIGHT_I - destHeight), 12,
            destHeight);

        b.Draw(
            Texture,
            destRect,
            srcRect,
            Color.White,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1f
        );

        // draw hover text
        if (Game1.getOldMouseX() >= topOfBar.X && Game1.getOldMouseY() >= topOfBar.Y &&
            Game1.getOldMouseX() < topOfBar.X + 24f)
            Game1.drawWithBorder( Math.Max(0, (int) CurrentValue) + "/" + 500, Color.Black * 0f,
                Color.White,
                topOfBar + new Vector2(0f - Game1.dialogueFont.MeasureString("999/999").X - 32f, 64f));

        if (Math.Abs(ratio - 1f) >= 0.002f && !isSuperModeActive) return;

        // draw top shadow
        destRect.Height = 2;
        b.Draw(
            Game1.staminaRect,
            destRect,
            srcRect,
            Color.Black * 0.3f,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1f
        );
    }

    /// <summary>Countdown the gauge value.</summary>
    /// <param name="amount">Milliseconds to deduct.</param>
    public void Countdown(double amount)
    {
        if (!Game1.game1.IsActive && Game1ShouldTimePassPatch.Game1ShouldTimePassOriginal(Game1.game1, true)) return;
        CurrentValue -= amount;
    }

    /// <summary>Countdown the gauge shake timer .</summary>
    public void UpdateShake()
    {
        if (!Game1.game1.IsActive && Game1ShouldTimePassPatch.Game1ShouldTimePassOriginal(Game1.game1, true)) return;

        if (_shakeTimer > 0)
        {
            --_shakeTimer;
            if (_shakeTimer <= 0) _shake = false;
        }
        else if (_nextShake > 0)
        {
            --_nextShake;
            if (_nextShake > 0) return;

            _shake = true;
            _shakeTimer = SHAKE_DURATION_I;
            _nextShake = TICKS_BETWEEN_SHAKES_I;
        }
    }

    /// <summary>Gradually reduce the gauge's opacity value.</summary>
    public void FadeOut()
    {
        --_fadeOutTimer;
        if (_fadeOutTimer >= FADE_OUT_DURATION_I) return;

        var ratio = (float) _fadeOutTimer / FADE_OUT_DURATION_I;
        _opacity = (float) (-1.0 / (1.0 + Math.Exp(12.0 * ratio - 6.0)) + 1.0);
        if (_fadeOutTimer > 0) return;

        ModEntry.EventManager.Disable(typeof(SuperModeGaugeFadeOutUpdateTickedEvent),
            typeof(SuperModeGaugeRenderingHudEvent));
        _fadeOutTimer = FADE_OUT_DELAY_I + FADE_OUT_DURATION_I;
        _opacity = 1f;
    }

    #endregion public methods

    #region private methods

    /// <summary>Raised when SuperModeGauge is set to the max value.</summary>
    private void OnGaugeFilled()
    {
        if (!ModEntry.Config.EnableSuperMode) return;
        ModEntry.EventManager.Enable(typeof(SuperModeButtonsChangedEvent),
            typeof(SuperModeGaugeShakeUpdateTickedEvent));
    }

    /// <summary>Raised when SuperModeGauge is raised from zero to any value greater than zero.</summary>
    private void OnGaugeRaisedAboveZero()
    {
        ModEntry.EventManager.Enable(typeof(SuperModeBuffDisplayUpdateTickedEvent));
        if (ModEntry.Config.EnableSuperMode) ModEntry.EventManager.Enable(typeof(SuperModeGaugeRenderingHudEvent));
    }

    /// <summary>Raised when SuperModeGauge is set to zero.</summary>
    private void OnGaugeReturnedToZero()
    {
        ModEntry.EventManager.Disable(typeof(SuperModeBuffDisplayUpdateTickedEvent));

        if (ModEntry.State.Value.SuperMode.IsActive) ModEntry.State.Value.SuperMode.Deactivate();
        
        if (Game1.currentLocation.IsCombatZone() || !ModEntry.Config.EnableSuperMode) return;

        ModEntry.EventManager.Enable(typeof(SuperModeGaugeFadeOutUpdateTickedEvent));
    }

    #endregion private methods
}