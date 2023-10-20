namespace DaLion.Overhaul.Modules.Professions.Ultimates;

#region using directives

using Ardalis.SmartEnum;
using DaLion.Overhaul.Modules.Core;
using DaLion.Overhaul.Modules.Professions.Events.Display.RenderedWorld;
using DaLion.Overhaul.Modules.Professions.Events.Display.RenderingHud;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Events.Input.ButtonsChanged;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.Activated;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.ChargeIncreased;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.ChargeInitiated;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.Deactivated;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.Emptied;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.FullyCharged;
using DaLion.Overhaul.Modules.Professions.Extensions;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Base class for handling Ultimate activation.</summary>
public abstract class Ultimate : SmartEnum<Ultimate>, IUltimate
{
    /// <summary>The maximum charge value at base level 10.</summary>
    public const int BaseMaxValue = 100;

    #region enum entries

    /// <summary>The <see cref="Ultimate"/> of <see cref="Modules.Professions.Profession.Brute"/>.</summary>
    public static readonly Ultimate BruteFrenzy = new Frenzy();

    /// <summary>The <see cref="Ultimate"/> of <see cref="Modules.Professions.Profession.Poacher"/>.</summary>
    public static readonly Ultimate PoacherAmbush = new Ambush();

    /// <summary>The <see cref="Ultimate"/> of <see cref="Modules.Professions.Profession.Piper"/>.</summary>
    public static readonly Ultimate PiperConcerto = new Concerto();

    /// <summary>The <see cref="Ultimate"/> of <see cref="Modules.Professions.Profession.Desperado"/>.</summary>
    public static readonly Ultimate DesperadoBlossom = new DeathBlossom();

    #endregion enum entires

    private int _activationTimer = ActivationTimerMax;
    private double _chargeValue;

    /// <summary>Initializes a new instance of the <see cref="Ultimate"/> class.</summary>
    /// <param name="name">The name of the enum entry.</param>
    /// <param name="profession">The <see cref="IProfession"/> which owns this <see cref="Ultimate"/>.</param>
    /// <param name="meterColor">The color applied to the <see cref="UltimateHud"/>.</param>
    /// <param name="overlayColor">The color of the <see cref="UltimateOverlay"/>.</param>
    protected Ultimate(string name, IProfession profession, Color meterColor, Color overlayColor)
        : base(name, profession.Id)
    {
        this.Profession = profession;
        this.BuffId = Manifest.UniqueID.GetHashCode() + profession.Id + 4;
        this.BuffSheetIndex = profession.Id + 22;
        this.Hud = new UltimateHud(this, meterColor);
        this.Overlay = new UltimateOverlay(overlayColor);
    }

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

    /// <inheritdoc />
    public IProfession Profession { get; }

    /// <inheritdoc />
    public abstract string DisplayName { get; }

    /// <inheritdoc />
    public abstract string Description { get; }

    /// <inheritdoc />
    public int Index => this.Value;

    /// <inheritdoc />
    public bool IsActive { get; protected set; }

    /// <inheritdoc />
    public double ChargeValue
    {
        get => this._chargeValue;
        set
        {
            if (!ProfessionsModule.Config.EnableLimitBreaks)
            {
                return;
            }

            if (Math.Abs(this._chargeValue - value) < 0.01)
            {
                return;
            }

            if (value <= 0)
            {
                EventManager.Disable<UltimateGaugeShakeUpdateTickedEvent>();
                this.Hud.ForceStopShake();

                if (this.IsActive)
                {
                    this.Deactivate();
                }

                if (!Game1.currentLocation.IsDungeon())
                {
                    EventManager.Enable<UltimateGaugeFadeOutUpdateTickedEvent>();
                }

                this.OnEmptied();
                this._chargeValue = 0;
            }
            else
            {
                var delta = value - this._chargeValue;
                var scaledDelta = delta * ((double)this.MaxValue / BaseMaxValue) * (delta >= 0
                    ? ProfessionsModule.Config.LimitGainFactor
                    : ProfessionsModule.Config.LimitDrainFactor);
                value = Math.Min(scaledDelta + this._chargeValue, this.MaxValue);

                if (this._chargeValue == 0f)
                {
                    EventManager.Enable<UltimateMeterRenderingHudEvent>();
                    this.OnChargeInitiated(value);
                }

                if (value > this._chargeValue)
                {
                    this.OnChargeIncreased(this._chargeValue, value);
                    if (value >= this.MaxValue)
                    {
                        EventManager.Enable<UltimateButtonsChangedEvent>();
                        EventManager.Enable<UltimateGaugeShakeUpdateTickedEvent>();
                        this.OnFullyCharged();
                    }
                }

                this._chargeValue = value;
            }
        }
    }

    /// <inheritdoc />
    public int MaxValue => BaseMaxValue + (Game1.player.CombatLevel > 10 ? Game1.player.CombatLevel * 5 : 0);

    /// <inheritdoc />
    public virtual bool CanActivate => !this.IsActive && this.ChargeValue >= this.MaxValue;

    /// <inheritdoc />
    public bool IsHudVisible => this.Hud.IsVisible;

    /// <summary>Gets the ID of the buff that displays while the instance is active.</summary>
    internal int BuffId { get; }

    /// <summary>Gets tilesheet index of the buff that displays while the instance is active.</summary>
    internal int BuffSheetIndex { get; }

    /// <summary>Gets the default duration of the buff.</summary>
    internal abstract int MillisecondsDuration { get; }

    /// <inheritdoc cref="UltimateHud"/>
    internal UltimateHud Hud { get; }

    /// <inheritdoc cref="UltimateOverlay"/>
    internal UltimateOverlay Overlay { get; }

    /// <summary>Gets the sound effect that plays when this Ultimate is activated.</summary>
    internal abstract SoundEffectPlayer ActivationSoundEffectPlayer { get; }

    /// <summary>Gets the glow color applied to the player while this Ultimate is active.</summary>
    internal abstract Color GlowColor { get; }

    private static int ActivationTimerMax => (int)(ProfessionsModule.Config.LimitBreakHoldDelaySeconds * 60);

    /// <summary>Activates the <see cref="Ultimate"/> for the local player.</summary>
    internal virtual void Activate()
    {
        this.IsActive = true;

        // interrupt fade out if necessary
        EventManager.Disable<UltimateOverlayFadeOutUpdateTickedEvent>();

        // stop updating, awaiting activation and shaking the hud meter
        EventManager.Disable<UltimateButtonsChangedEvent>();
        EventManager.Disable<UltimateGaugeShakeUpdateTickedEvent>();
        EventManager.Disable<UltimateInputUpdateTickedEvent>();

        // fade in overlay and begin countdown
        EventManager.Enable<UltimateActiveUpdateTickedEvent>();
        EventManager.Enable<UltimateOverlayFadeInUpdateTickedEvent>();
        EventManager.Enable<UltimateOverlayRenderedWorldEvent>();

        // play sound effect
        this.ActivationSoundEffectPlayer.Play();

        // notify peers
        Broadcaster.Broadcast("Active", OverhaulModule.Professions.Namespace + "ToggledUltimate");

        // invoke callbacks
        this.OnActivated();
    }

    /// <summary>Deactivates the <see cref="Ultimate"/> for the local player.</summary>
    internal virtual void Deactivate()
    {
        this.IsActive = false;
        this.ChargeValue = 0;

        // fade out overlay
        EventManager.Enable<UltimateOverlayFadeOutUpdateTickedEvent>();

        // stop countdown
        EventManager.Disable<UltimateActiveUpdateTickedEvent>();

        // stop glowing if necessary
        Game1.player.stopGlowing();

        // notify peers
        Broadcaster.Broadcast("Inactive", OverhaulModule.Professions.Namespace + "ToggledUltimate");

        // invoke callbacks
        this.OnDeactivated();
    }

    /// <summary>Detects and handles activation input.</summary>
    internal void CheckForActivation()
    {
        if (!ProfessionsModule.Config.EnableLimitBreaks)
        {
            return;
        }

        if (ProfessionsModule.Config.LimitBreakKey.JustPressed())
        {
            if (ProfessionsModule.Config.HoldKeyToLimitBreak)
            {
                this._activationTimer = ActivationTimerMax;
                EventManager.Enable<UltimateInputUpdateTickedEvent>();
            }
            else if (this.CanActivate)
            {
                this.Activate();
            }
            else
            {
                Game1.playSound("cancel");
            }
        }
        else if (ProfessionsModule.Config.LimitBreakKey.GetState() == SButtonState.Released && this._activationTimer > 0)
        {
            this._activationTimer = -1;
            EventManager.Disable<UltimateInputUpdateTickedEvent>();
        }
    }

    /// <summary>Updates internal activation state.</summary>
    internal void UpdateInput()
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass() || this._activationTimer <= 0)
        {
            return;
        }

        if (--this._activationTimer > 0)
        {
            return;
        }

        if (this.CanActivate)
        {
            this.Activate();
        }
        else
        {
            Game1.playSound("cancel");
        }
    }

    /// <summary>Counts down the charge value.</summary>
    internal abstract void Countdown();

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
    /// <param name="oldValue">The old charge value.</param>
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
}
