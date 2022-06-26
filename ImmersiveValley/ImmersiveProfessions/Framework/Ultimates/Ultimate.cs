namespace DaLion.Stardew.Professions.Framework.Ultimates;

#region using directives

using Common;
using Events.Display;
using Events.GameLoop;
using Events.Input;
using Events.Player;
using Extensions;
using Framework.Events.Ultimate;
using Microsoft.Xna.Framework;
using Sounds;
using StardewModdingAPI;
using StardewValley;
using System;

#endregion using directives

/// <summary>Base class for handling Ultimate activation.</summary>
internal abstract class Ultimate : IUltimate
{
    public const int BASE_MAX_VALUE_I = 100;

    private int _activationTimer;
    private double _chargeValue;

    private static int _ActivationTimerMax => (int)(ModEntry.Config.SpecialActivationDelay * 60);

    #region event handlers

    /// <inheritdoc cref="OnActivated"/>
    internal static event EventHandler<IUltimateActivatedEventArgs>? Activated;

    /// <inheritdoc cref="OnDeactivated"/>
    internal static event EventHandler<IUltimateDeactivatedEventArgs>? Deactivated;

    /// <inheritdoc cref="OnChargeInitiated"/>
    internal static event EventHandler<IUltimateChargeInitiatedEventArgs>? ChargeInitiated;

    /// <inheritdoc cref="OnChargeIncreased"/>
    internal static event EventHandler<IUltimateChargeIncreasedEventArgs>? ChargeIncreased;

    /// <inheritdoc cref="OnFullyCharged"/>
    internal static event EventHandler<IUltimateFullyChargedEventArgs>? FullyCharged;

    /// <inheritdoc cref="OnEmptied"/>
    internal static event EventHandler<IUltimateEmptiedEventArgs>? Emptied;

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
        UnhookEvents();
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
                ModEntry.EventManager.Unhook<UltimateGaugeShakeUpdateTickedEvent>();
                Hud.ForceStopShake();

                if (IsActive) Deactivate();

                if (!Game1.currentLocation.IsDungeon())
                    ModEntry.EventManager.Hook<UltimateGaugeFadeOutUpdateTickedEvent>();

                OnEmptied();
                _chargeValue = 0;
            }
            else
            {
                var delta = value - _chargeValue;
                var scaledDelta = delta * ((double)MaxValue / BASE_MAX_VALUE_I) * (delta >= 0
                    ? ModEntry.Config.SpecialGainFactor
                    : ModEntry.Config.SpecialDrainFactor);
                value = Math.Min(scaledDelta + _chargeValue, MaxValue);

                if (_chargeValue == 0f)
                {
                    ModEntry.EventManager.Hook<UltimateMeterRenderingHudEvent>();
                    OnChargeInitiated(value);
                }

                if (value > _chargeValue)
                {
                    OnChargeIncreased(_chargeValue, value);
                    if (value >= MaxValue)
                    {
                        ModEntry.EventManager.Hook<UltimateButtonsChangedEvent>();
                        ModEntry.EventManager.Hook<UltimateGaugeShakeUpdateTickedEvent>();
                        OnFullyCharged();
                    }
                }

                _chargeValue = value;
            }
        }
    }

    /// <inheritdoc />
    public int MaxValue => BASE_MAX_VALUE_I + (Game1.player.CombatLevel > 10 ? Game1.player.CombatLevel * 5 : 0);

    /// <inheritdoc />
    public float PercentCharge => (float)(ChargeValue / MaxValue);

    /// <inheritdoc />
    public bool IsFullyCharged => ChargeValue >= MaxValue;

    /// <inheritdoc />
    public bool IsEmpty => ChargeValue == 0;

    /// <inheritdoc />
    public bool IsHudVisible => Hud.IsVisible;

    /// <inheritdoc />
    public bool IsActive { get; protected set; }

    /// <inheritdoc />
    public virtual bool CanActivate => ModEntry.Config.EnableSpecials && !IsActive && IsFullyCharged;

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

    /// <inheritdoc />
    public override string ToString() => Index.ToString();

    #endregion public methods

    #region internal methods

    /// <summary>Activate Ultimate for the local player.</summary>
    internal virtual void Activate()
    {
        IsActive = true;

        // fade in overlay and begin countdown
        ModEntry.EventManager.Hook<UltimateActiveUpdateTickedEvent>();
        ModEntry.EventManager.Hook<UltimateOverlayFadeInUpdateTickedEvent>();
        ModEntry.EventManager.Hook<UltimateOverlayRenderedWorldEvent>();

        // stop updating, awaiting activation and shaking the hud meter
        ModEntry.EventManager.Unhook<UltimateButtonsChangedEvent>();
        ModEntry.EventManager.Unhook<UltimateGaugeShakeUpdateTickedEvent>();
        ModEntry.EventManager.Unhook<UltimateInputUpdateTickedEvent>();

        // play sound effect
        ActivationSfx.Play();

        // notify peers
        ModEntry.Broadcaster.Broadcast("Active", "ToggledUltimate");

        // invoke callbacks
        OnActivated();
    }

    /// <summary>Deactivate Ultimate for the local player.</summary>
    internal virtual void Deactivate()
    {
        IsActive = false;
        ChargeValue = 0;

        // fade out overlay
        ModEntry.EventManager.Hook<UltimateOverlayFadeOutUpdateTickedEvent>();

        // stop countdown
        ModEntry.EventManager.Unhook<UltimateActiveUpdateTickedEvent>();

        // stop glowing if necessary
        Game1.player.stopGlowing();

        // notify peers
        ModEntry.Broadcaster.Broadcast("Inactive", "ToggledUltimate");

        // invoke callbacks
        OnDeactivated();
    }

    /// <summary>Detect and handle activation input.</summary>
    internal void CheckForActivation()
    {
        if (ModEntry.Config.SpecialActivationKey.JustPressed() && CanActivate)
        {
            if (ModEntry.Config.HoldKeyToActivateSpecial)
            {
                _activationTimer = _ActivationTimerMax;
                ModEntry.EventManager.Hook<UltimateInputUpdateTickedEvent>();
            }
            else
            {
                Activate();
            }
        }
        else if (ModEntry.Config.SpecialActivationKey.GetState() == SButtonState.Released && _activationTimer > 0)
        {
            _activationTimer = -1;
            ModEntry.EventManager.Unhook<UltimateInputUpdateTickedEvent>();
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

    #region event callbacks

    /// <summary>Raised when a player activates their combat Ultimate.</summary>
    protected void OnActivated()
    {
        Activated?.Invoke(this, new UltimateActivatedEventArgs(Game1.player));
    }

    /// <summary>Raised when a player's combat Ultimate ends.</summary>
    protected void OnDeactivated()
    {
        Deactivated?.Invoke(this, new UltimateDeactivatedEventArgs(Game1.player));
    }

    /// <summary>Raised when a player's combat Ultimate gains any charge while it was previously empty.</summary>
    /// <param name="newValue">The new charge value.</param>
    protected void OnChargeInitiated(double newValue)
    {
        ChargeInitiated?.Invoke(this, new UltimateChargeInitiatedEventArgs(Game1.player, newValue));
    }

    /// <summary>Raised when a player's combat Ultimate gains any charge.</summary>
    /// <param name="newValue">The old charge value.</param>
    /// <param name="newValue">The new charge value.</param>
    protected void OnChargeIncreased(double oldValue, double newValue)
    {
        ChargeIncreased?.Invoke(this, new UltimateChargeIncreasedEventArgs(Game1.player, oldValue, newValue));
    }

    /// <summary>Raised when the local player's ultimate charge value reaches max value.</summary>
    protected void OnFullyCharged()
    {
        FullyCharged?.Invoke(this, new UltimateFullyChargedEventArgs(Game1.player));
    }

    /// <summary>Raised when the local player's ultimate charge value returns to zero.</summary>
    protected void OnEmptied()
    {
        Emptied?.Invoke(this, new UltimateEmptiedEventArgs(Game1.player));
    }

    #endregion event callbacks

    #region private methods

    /// <summary>Enable all events required for Ultimate functionality.</summary>
    private static void EnableEvents()
    {
        ModEntry.EventManager.Hook<UltimateWarpedEvent>();
        if (Game1.currentLocation.IsDungeon())
            ModEntry.EventManager.Hook<UltimateMeterRenderingHudEvent>();
    }

    /// <summary>Disable all events related to Ultimate functionality.</summary>
    private static void UnhookEvents()
    {
        ModEntry.EventManager.UnhookStartingWith("Ultimate");
    }

    #endregion private methods
}