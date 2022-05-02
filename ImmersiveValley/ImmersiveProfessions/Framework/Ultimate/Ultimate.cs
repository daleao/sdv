namespace DaLion.Stardew.Professions.Framework.Ultimate;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

using Events.Display;
using Events.GameLoop;
using Events.Input;
using Events.Player;
using Extensions;
using Sounds;

#endregion using directives

/// <summary>Base class for handling Ultimate activation.</summary>
internal abstract class Ultimate : IUltimate
{
    public const int BASE_MAX_VALUE_I = 100;

    private int _activationTimer;
    private double _chargeValue;

    private static int _ActivationTimerMax => (int) (ModEntry.Config.UltimateActivationDelay * 60);

    /// <summary>Construct an instance.</summary>
    protected Ultimate()
    {
        Log.D($"Initializing Ultimate as {GetType().Name}.");
        _activationTimer = _ActivationTimerMax;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        DisableEvents();
    }

    #region public properties

    public bool IsActive { get; protected set; }
    public UltimateMeter Meter { get; protected set; }
    public UltimateOverlay Overlay { get; protected set; }

    public abstract SFX ActivationSfx { get; }
    public abstract Color GlowColor { get; }
    public abstract UltimateIndex Index { get; }

    public double ChargeValue
    {
        get => _chargeValue;
        set
        {
            if (Math.Abs(_chargeValue - value) < 0.01) return;

            if (value <= 0)
            {
                _chargeValue = 0;
                OnEmptied();
            }
            else
            {
                if (_chargeValue == 0f) OnGainedFromZero();

                if (value > _chargeValue)
                {
                    OnChargeGained();
                    if (value >= MaxValue) OnFullyCharged();
                }

                var delta = value - _chargeValue;
                var scaledDelta = delta * ((double) MaxValue / BASE_MAX_VALUE_I) * (delta >= 0
                    ? ModEntry.Config.UltimateGainFactor
                    : ModEntry.Config.UltimateDrainFactor);
                _chargeValue = Math.Min(scaledDelta + _chargeValue, MaxValue);
            }
        }
    }

    public static int MaxValue => BASE_MAX_VALUE_I + (Game1.player.CombatLevel > 10 ? Game1.player.CombatLevel * 5 : 0);

    public float PercentCharge => (float) (ChargeValue / MaxValue);

    public bool IsFullyCharged => ChargeValue >= MaxValue;

    public bool IsEmpty => ChargeValue == 0;

    #endregion public properties

    #region public methods

    /// <inheritdoc />
    public virtual void Activate()
    {
        IsActive = true;

        // fade in overlay and begin countdown
        EventManager.Enable(typeof(UltimateCountdownUpdateTickedEvent), typeof(UltimateOverlayFadeInUpdateTickedEvent),
            typeof(UltimateOverlayRenderedWorldEvent));

        // stop updating, awaiting activation and shaking the hud meter
        EventManager.Disable(typeof(UltimateButtonsChangedEvent), typeof(UltimateGaugeShakeUpdateTickedEvent),
            typeof(UltimateUpdateTickedEvent));

        // play sound effect
        SoundBank.Play(ActivationSfx);

        // notify peers
        ModEntry.ModHelper.Multiplayer.SendMessage("Active", "ToggledUltimate",
            new[] { ModEntry.Manifest.UniqueID });
    }

    /// <inheritdoc />
    public virtual void Deactivate()
    {
        IsActive = false;
        ChargeValue = 0;

        // fade out overlay
        EventManager.Enable(typeof(UltimateOverlayFadeOutUpdateTickedEvent));

        // stop countdown
        EventManager.Disable(typeof(UltimateCountdownUpdateTickedEvent));

        // stop glowing if necessary
        Game1.player.stopGlowing();

        // notify peers
        ModEntry.ModHelper.Multiplayer.SendMessage("Inactive", "ToggledUltimate",
            new[] { ModEntry.Manifest.UniqueID });
    }

    /// <inheritdoc />
    public void CheckForActivation()
    {
        if (ModEntry.Config.UltimateKey.JustPressed() && CanActivate())
        {
            if (ModEntry.Config.HoldKeyToActivateUltimate)
            {
                _activationTimer = _ActivationTimerMax;
                EventManager.Enable(typeof(UltimateUpdateTickedEvent));
            }
            else
            {
                Activate();
            }
        }
        else if (ModEntry.Config.UltimateKey.GetState() == SButtonState.Released && _activationTimer > 0)
        {
            _activationTimer = -1;
            EventManager.Disable(typeof(UltimateUpdateTickedEvent));
        }
    }

    /// <inheritdoc />
    public void UpdateInput()
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass() || Game1.eventUp || Game1.player.UsingTool ||
            _activationTimer <= 0) return;

        --_activationTimer;
        if (_activationTimer > 0) return;

        Activate();
    }

    /// <inheritdoc />
    public abstract void Countdown(double elapsed);

    #endregion public methods

    #region protected methods

    /// <summary>Check whether all activation conditions are met.</summary>
    protected virtual bool CanActivate()
    {
        return ModEntry.Config.EnableUltimates && !IsActive && IsFullyCharged;
    }

    /// <summary>Enable all events required for Ultimate functionality.</summary>
    protected void EnableEvents()
    {
        EventManager.Enable(typeof(UltimateWarpedEvent));
        if (Game1.currentLocation.IsDungeon())
            EventManager.Enable(typeof(UltimateMeterRenderingHudEvent));
    }

    /// <summary>Disable all events related to Ultimate functionality.</summary>
    protected virtual void DisableEvents()
    {
        EventManager.DisableAllStartingWith("Ultimate");
    }

    /// <summary>Raised when charge value increases.</summary>
    protected virtual void OnChargeGained()
    {
    }

    /// <summary>Raised when charge value is raised from zero to any value greater than zero.</summary>
    protected virtual void OnGainedFromZero()
    {
        EventManager.Enable(typeof(UltimateMeterRenderingHudEvent));
    }

    /// <summary>Raised when charge value is set to the max value.</summary>
    protected virtual void OnFullyCharged()
    {
        EventManager.Enable(typeof(UltimateButtonsChangedEvent), typeof(UltimateGaugeShakeUpdateTickedEvent));
    }

    /// <summary>Raised when charge value is set to zero.</summary>
    protected virtual void OnEmptied()
    {
        EventManager.Disable(typeof(UltimateGaugeShakeUpdateTickedEvent));
        Meter.ForceStopShake();

        if (IsActive) Deactivate();

        if (!Game1.currentLocation.IsDungeon())
            EventManager.Enable(typeof(UltimateGaugeFadeOutUpdateTickedEvent));
    }

    #endregion protected methods
}