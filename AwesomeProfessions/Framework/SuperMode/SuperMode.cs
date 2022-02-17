namespace DaLion.Stardew.Professions.Framework.SuperMode;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

using AssetLoaders;
using Events.Display;
using Events.GameLoop;
using Events.Input;
using Events.Player;
using Extensions;

#endregion using directives

/// <summary>Base class for handling Super Mode activation.</summary>
internal abstract class SuperMode : ISuperMode
{
    public const int INITIAL_MAX_VALUE_I = 500;

    private int _activationTimer;
    private double _chargeValue;

    private static int _ActivationTimerMax => (int)(ModEntry.Config.SuperModeActivationDelay * 60);

    /// <summary>Construct an instance.</summary>
    protected SuperMode()
    {
        Log.D($"Initializing Super Mode as {GetType().Name}.");
        _activationTimer = _ActivationTimerMax;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        DisableEvents();
    }

    #region public properties

    public bool IsActive { get; protected set; }
    public SuperModeGauge Gauge { get; protected set; }
    public SuperModeOverlay Overlay { get; protected set; }

    public abstract SFX ActivationSfx { get; }
    public abstract Color GlowColor { get; }
    public abstract SuperModeIndex Index { get; }


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
                if (_chargeValue == 0f) OnRaisedFromZero();

                if (value > _chargeValue)
                {
                    OnRaised();
                    if (value >= MaxValue) OnFullyCharged();
                }

                _chargeValue = Math.Min(value, MaxValue);
            }
        }
    }

    public static int MaxValue =>
        Game1.player.CombatLevel >= 10
            ? Game1.player.CombatLevel * 50
            : INITIAL_MAX_VALUE_I;

    public float PercentCharge => (float)(ChargeValue / MaxValue);

    public bool IsFullyCharged => ChargeValue >= MaxValue;

    public bool IsEmpty => ChargeValue == 0;

    #endregion public properties

    #region public methods

    /// <inheritdoc />
    public virtual void Activate()
    {
        IsActive = true;

        // fade in overlay and begin countdown
        EventManager.Enable(typeof(SuperModeActiveOverlayRenderedWorldEvent),
            typeof(SuperModeActiveUpdateTickedEvent), typeof(SuperModeOverlayFadeInUpdateTickedEvent));

        // stop updating, awaiting activation and shaking gauge
        EventManager.Disable(typeof(SuperModeUpdateTickedEvent),
            typeof(SuperModeButtonsChangedEvent), typeof(SuperModeGaugeShakeUpdateTickedEvent));

        // play sound effect
        SoundBank.Play(ActivationSfx);

        // notify peers
        ModEntry.ModHelper.Multiplayer.SendMessage(Index, "ToggledSuperMode/On",
            new[] { ModEntry.Manifest.UniqueID });
    }

    /// <inheritdoc />
    public virtual void Deactivate()
    {
        IsActive = false;

        // fade out overlay
        EventManager.Enable(typeof(SuperModeOverlayFadeOutUpdateTickedEvent));

        // stop countdown
        EventManager.Disable(typeof(SuperModeActiveUpdateTickedEvent));

        // stop glowing if necessary
        Game1.player.stopGlowing();

        // notify peers
        ModEntry.ModHelper.Multiplayer.SendMessage(GetType().ToString(), "ToggledSuperMode/Off",
            new[] { ModEntry.Manifest.UniqueID });
    }

    /// <inheritdoc />
    public void CheckForActivation()
    {
        if (ModEntry.Config.SuperModeKey.JustPressed() && CanActivate())
        {
            if (ModEntry.Config.HoldKeyToActivateSuperMode) _activationTimer = _ActivationTimerMax;
            else Activate();
        }
        else if (ModEntry.Config.SuperModeKey.GetState() == SButtonState.Released && _activationTimer > 0)
        {
            _activationTimer = -1;
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
    public void Countdown(double amount)
    {
        if (Game1.game1.IsActive && Game1.shouldTimePass())
            ChargeValue -= amount;
    }

    /// <inheritdoc />
    public abstract void AddBuff();

    #endregion public methods

    #region protected methods

    /// <summary>Check whether all activation conditions are met.</summary>
    protected virtual bool CanActivate()
    {
        return !IsActive && IsFullyCharged;
    }

    /// <summary>Enable all events required for Super Mode functionality.</summary>
    protected void EnableEvents()
    {
        EventManager.Enable(typeof(SuperModeWarpedEvent));
        if (Game1.currentLocation.IsCombatZone() && ModEntry.Config.EnableSuperMode)
            EventManager.Enable(typeof(SuperModeGaugeRenderingHudEvent));
    }

    /// <summary>Disable all events related to Super Mode functionality.</summary>
    protected virtual void DisableEvents()
    {
        EventManager.DisableAllStartingWith("SuperMode");
    }

    /// <summary>Raised when charge value value increases.</summary>
    protected virtual void OnRaised()
    {
    }

    /// <summary>Raised when charge value is raised from zero to any value greater than zero.</summary>
    protected virtual void OnRaisedFromZero()
    {
        if (ModEntry.Config.EnableSuperMode)
            EventManager.Enable(typeof(SuperModeButtonsChangedEvent), typeof(SuperModeGaugeRenderingHudEvent),
                typeof(SuperModeUpdateTickedEvent));
    }

    /// <summary>Raised when charge value is set to the max value.</summary>
    protected virtual void OnFullyCharged()
    {
        if (ModEntry.Config.EnableSuperMode) EventManager.Enable(typeof(SuperModeGaugeShakeUpdateTickedEvent));
    }

    /// <summary>Raised when charge value is set to zero.</summary>
    protected virtual void OnEmptied()
    {
        EventManager.Disable(typeof(SuperModeButtonsChangedEvent), typeof(SuperModeGaugeShakeUpdateTickedEvent),
            typeof(SuperModeUpdateTickedEvent));
        Gauge.ForceStopShake();

        if (ModEntry.State.Value.SuperMode.IsActive) ModEntry.State.Value.SuperMode.Deactivate();

        if (!Game1.currentLocation.IsCombatZone())
            EventManager.Enable(typeof(SuperModeGaugeFadeOutUpdateTickedEvent));
    }

    #endregion protected methods
}