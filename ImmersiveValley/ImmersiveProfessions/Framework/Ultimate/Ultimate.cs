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

    internal static event EventHandler<UltimateActivatedEventArgs> Activated;
    internal static event EventHandler<UltimateDeactivatedEventArgs> Deactivated;
    internal static event EventHandler<UltimateChargeInitiatedEventArgs> ChargeInitiated;
    internal static event EventHandler<UltimateChargeGainedEventArgs> ChargeGained;
    internal static event EventHandler<UltimateFullyChargedEventArgs> FullyCharged;
    internal static event EventHandler<UltimateEmptiedEventArgs> Emptied;

    /// <summary>Construct an instance.</summary>
    protected Ultimate()
    {
        Log.D($"Initializing Ultimate as {GetType().Name}.");
        _activationTimer = _ActivationTimerMax;
        
        EnableEvents();
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
                Emptied?.Invoke(this, new());
            }
            else
            {
                if (_chargeValue == 0f) ChargeInitiated?.Invoke(this, new(value));

                if (value > _chargeValue)
                {
                    ChargeGained?.Invoke(this, new(_chargeValue, value));
                    if (value >= MaxValue) FullyCharged?.Invoke(this, new());
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

        Activated?.Invoke(this, new());
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

        Deactivated?.Invoke(this, new());
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