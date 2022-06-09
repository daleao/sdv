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
using Framework.Events.Ultimate;
using Sounds;

#endregion using directives

/// <summary>Base class for handling Ultimate activation.</summary>
internal abstract class Ultimate : IUltimate
{
    public const int BASE_MAX_VALUE_I = 100;

    private int _activationTimer;
    private double _chargeValue;

    private static int _ActivationTimerMax => (int) (ModEntry.Config.UltimateActivationDelay * 60);

    #region event handlers

    /// <inheritdoc cref="OnActivated"/>
    internal static event EventHandler<IUltimateActivatedEventArgs> Activated;

    /// <inheritdoc cref="OnDeactivated"/>
    internal static event EventHandler<IUltimateDeactivatedEventArgs> Deactivated;

    /// <inheritdoc cref="OnChargeInitiated"/>
    internal static event EventHandler<IUltimateChargeInitiatedEventArgs> ChargeInitiated;

    /// <inheritdoc cref="OnChargeIncreased"/>
    internal static event EventHandler<IUltimateChargeIncreasedEventArgs> ChargeIncreased;

    /// <inheritdoc cref="OnFullyCharged"/>
    internal static event EventHandler<IUltimateFullyChargedEventArgs> FullyCharged;

    /// <inheritdoc cref="OnEmptied"/>
    internal static event EventHandler<IUltimateEmptiedEventArgs> Emptied;

    #endregion event handlers

    /// <summary>Construct an instance.</summary>
    protected Ultimate(Color meterColor, Color overlayColor)
    {
        Log.D($"Initializing Ultimate as {GetType().Name}.");
        _activationTimer = _ActivationTimerMax;
        Hud = new(this, meterColor);
        Overlay = new(overlayColor);
        EnableEvents();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        DisableEvents();
    }

    #region public properties

    /// <inheritdoc />
    public abstract UltimateIndex Index { get; }

    /// <inheritdoc />
    public double ChargeValue
    {
        get => _chargeValue;
        set
        {
            if (Math.Abs(_chargeValue - value) < 0.01) return;

            if (value <= 0)
            {
                EventManager.Disable(typeof(UltimateGaugeShakeUpdateTickedEvent));
                ModEntry.PlayerState.RegisteredUltimate.Hud.ForceStopShake();

                if (ModEntry.PlayerState.RegisteredUltimate.IsActive) ModEntry.PlayerState.RegisteredUltimate.Deactivate();

                if (!Game1.currentLocation.IsDungeon())
                    EventManager.Enable(typeof(UltimateGaugeFadeOutUpdateTickedEvent));

                OnEmptied(Game1.player);
                _chargeValue = 0;
            }
            else
            {
                var delta = value - _chargeValue;
                var scaledDelta = delta * ((double) MaxValue / BASE_MAX_VALUE_I) * (delta >= 0
                    ? ModEntry.Config.UltimateGainFactor
                    : ModEntry.Config.UltimateDrainFactor);
                value = Math.Min(scaledDelta + _chargeValue, MaxValue);

                if (_chargeValue == 0f)
                {
                    EventManager.Enable(typeof(UltimateMeterRenderingHudEvent));
                    OnChargeInitiated(Game1.player, value);
                }

                if (value > _chargeValue)
                {
                    OnChargeIncreased(Game1.player, _chargeValue, value);
                    if (value >= MaxValue)
                    {
                        EventManager.Enable(typeof(UltimateButtonsChangedEvent), typeof(UltimateGaugeShakeUpdateTickedEvent));
                        OnFullyCharged(Game1.player);
                    }
                }

                _chargeValue = value;
            }
        }
    }

    /// <inheritdoc />
    public int MaxValue => BASE_MAX_VALUE_I + (Game1.player.CombatLevel > 10 ? Game1.player.CombatLevel * 5 : 0);

    /// <inheritdoc />
    public float PercentCharge => (float) (ChargeValue / MaxValue);

    /// <inheritdoc />
    public bool IsFullyCharged => ChargeValue >= MaxValue;

    /// <inheritdoc />
    public bool IsEmpty => ChargeValue == 0;

    /// <inheritdoc />
    public bool IsHudVisible => Hud.IsVisible;

    /// <inheritdoc />
    public bool IsActive { get; protected set; }

    /// <inheritdoc />
    public virtual bool CanActivate => ModEntry.Config.EnableUltimates && !IsActive && IsFullyCharged;

    #endregion public properties

    #region internal properties

    /// <inheritdoc cref="UltimateHUD"/>
    internal UltimateHUD Hud { get; }

    /// <inheritdoc cref="UltimateOverlay"/>
    internal UltimateOverlay Overlay { get; }

    /// <summary>The sound effect that plays when this Ultimate is activated.</summary>
    internal abstract SFX ActivationSfx { get; }

    /// <summary>The glow color applied to the player while this Ultimate is active.</summary>
    internal abstract Color GlowColor { get; }

    #endregion internal properties

    #region public methods

    /// <summary>Returns the string representation of this instance's <see cref="UltimateIndex"/>.</summary>
    public override string ToString()
    {
        return Index.ToString();
    }

    #endregion public methods

    #region internal methods

    /// <summary>Activate Ultimate for the local player.</summary>
    internal virtual void Activate()
    {
        IsActive = true;

        // fade in overlay and begin countdown
        EventManager.Enable(typeof(UltimateActiveUpdateTickedEvent), typeof(UltimateOverlayFadeInUpdateTickedEvent),
            typeof(UltimateOverlayRenderedWorldEvent));

        // stop updating, awaiting activation and shaking the hud meter
        EventManager.Disable(typeof(UltimateButtonsChangedEvent), typeof(UltimateGaugeShakeUpdateTickedEvent),
            typeof(UltimateInputUpdateTickedEvent));

        // play sound effect
        SoundBank.Play(ActivationSfx);

        // notify peers
        ModEntry.Broadcaster.Broadcast("Active", "ToggledUltimate");

        // invoke callbacks
        OnActivated(Game1.player);
    }

    /// <summary>Deactivate Ultimate for the local player.</summary>
    internal virtual void Deactivate()
    {
        IsActive = false;
        ChargeValue = 0;

        // fade out overlay
        EventManager.Enable(typeof(UltimateOverlayFadeOutUpdateTickedEvent));

        // stop countdown
        EventManager.Disable(typeof(UltimateActiveUpdateTickedEvent));

        // stop glowing if necessary
        Game1.player.stopGlowing();

        // notify peers
        ModEntry.Broadcaster.Broadcast("Inactive", "ToggledUltimate");

        // invoke callbacks
        OnDeactivated(Game1.player);
    }

    /// <summary>Detect and handle activation input.</summary>
    internal void CheckForActivation()
    {
        if (ModEntry.Config.UltimateKey.JustPressed() && CanActivate)
        {
            if (ModEntry.Config.HoldKeyToActivateUltimate)
            {
                _activationTimer = _ActivationTimerMax;
                EventManager.Enable(typeof(UltimateInputUpdateTickedEvent));
            }
            else
            {
                Activate();
            }
        }
        else if (ModEntry.Config.UltimateKey.GetState() == SButtonState.Released && _activationTimer > 0)
        {
            _activationTimer = -1;
            EventManager.Disable(typeof(UltimateInputUpdateTickedEvent));
        }
    }

    /// <summary>UpdateInput internal activation state.</summary>
    internal void UpdateInput()
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass() || Game1.eventUp || Game1.player.UsingTool ||
            _activationTimer <= 0) return;

        --_activationTimer;
        if (_activationTimer > 0) return;

        Activate();
    }

    /// <summary>Countdown the charge value.</summary>
    internal abstract void Countdown(double elapsed);

    #endregion internal methods

    #region protected methods

    /// <summary>Raised when a player activates their combat Ultimate.</summary>
    protected void OnActivated(Farmer player)
    {
        Activated?.Invoke(this, new UltimateActivatedEventArgs(player));
    }

    /// <summary>Raised when a player's combat Ultimate ends.</summary>
    protected void OnDeactivated(Farmer player)
    {
        Deactivated?.Invoke(this, new UltimateDeactivatedEventArgs(player));
    }

    /// <summary>Raised when a player's combat Ultimate gains any charge while it was previously empty.</summary>
    protected void OnChargeInitiated(Farmer player, double newValue)
    {
        ChargeInitiated?.Invoke(this, new UltimateChargeInitiatedEventArgs(player, newValue));
    }

    /// <summary>Raised when a player's combat Ultimate gains any charge.</summary>
    protected void OnChargeIncreased(Farmer player, double oldValue, double newValue)
    {
        ChargeIncreased?.Invoke(this, new UltimateChargeIncreasedEventArgs(player, oldValue, newValue));
    }

    /// <summary>Raised when the local player's ultimate charge value reaches max value.</summary>
    protected void OnFullyCharged(Farmer player)
    {
        FullyCharged?.Invoke(this, new UltimateFullyChargedEventArgs(player));
    }

    /// <summary>Raised when the local player's ultimate charge value returns to zero.</summary>
    protected void OnEmptied(Farmer player)
    {
        Emptied?.Invoke(this, new UltimateEmptiedEventArgs(player));
    }

    #endregion protected methods

    #region private methods

    /// <summary>Enable all events required for Ultimate functionality.</summary>
    private static void EnableEvents()
    {
        EventManager.Enable(typeof(UltimateWarpedEvent));
        if (Game1.currentLocation.IsDungeon())
            EventManager.Enable(typeof(UltimateMeterRenderingHudEvent));
    }

    /// <summary>Disable all events related to Ultimate functionality.</summary>
    private static void DisableEvents()
    {
        EventManager.DisableAllStartingWith("Ultimate");
    }

    #endregion private methods
}