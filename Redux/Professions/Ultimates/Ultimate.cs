namespace DaLion.Redux.Professions.Ultimates;

#region using directives

using Ardalis.SmartEnum;
using DaLion.Redux.Professions.Events.Display;
using DaLion.Redux.Professions.Events.GameLoop;
using DaLion.Redux.Professions.Events.Input;
using DaLion.Redux.Professions.Events.Ultimate;
using DaLion.Redux.Professions.Extensions;
using DaLion.Redux.Professions.Sounds;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Base class for handling Ultimate activation.</summary>
public abstract class Ultimate : SmartEnum<Ultimate>, IUltimate
{
    /// <summary>The maximum charge value at base level 10.</summary>
    public const int BaseMaxValue = 100;

    #region enum entries

    /// <summary>The <see cref="Ultimate"/> of <see cref="Professions.Profession.Brute"/>.</summary>
    public static readonly Ultimate BruteFrenzy = new Frenzy();

    /// <summary>The <see cref="Ultimate"/> of <see cref="Professions.Profession.Poacher"/>.</summary>
    public static readonly Ultimate PoacherAmbush = new Ambush();

    /// <summary>The <see cref="Ultimate"/> of <see cref="Professions.Profession.Piper"/>.</summary>
    public static readonly Ultimate PiperConcerto = new Concerto();

    /// <summary>The <see cref="Ultimate"/> of <see cref="Professions.Profession.Desperado"/>.</summary>
    public static readonly Ultimate DesperadoBlossom = new DeathBlossom();

    #endregion enum entires

    private int _activationTimer = ActivationTimerMax;
    private double _chargeValue;

    /// <summary>Initializes a new instance of the <see cref="Ultimate"/> class.</summary>
    /// <param name="name">The name of the enum entry.</param>
    /// <param name="value">The value of the enum entry.</param>
    /// <param name="meterColor">The color applied to the <see cref="UltimateHud"/>.</param>
    /// <param name="overlayColor">The color of the <see cref="UltimateOverlay"/>.</param>
    protected Ultimate(string name, int value, Color meterColor, Color overlayColor)
        : base(name, value)
    {
        this.BuffId = ModEntry.Manifest.UniqueID.GetHashCode() + this.Value + 4;
        this.Hud = new UltimateHud(this, meterColor);
        this.Overlay = new UltimateOverlay(overlayColor);
        this.Profession = Professions.Profession.FromValue(value);
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
    public virtual string DisplayName => ModEntry.i18n.Get(this.Name.ToLower() + ".title");

    /// <inheritdoc />
    public virtual string Description => ModEntry.i18n.Get(this.Name.ToLower() + ".desc");

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
            if (Math.Abs(this._chargeValue - value) < 0.01)
            {
                return;
            }

            if (value <= 0)
            {
                ModEntry.Events.Disable<UltimateGaugeShakeUpdateTickedEvent>();
                this.Hud.ForceStopShake();

                if (this.IsActive)
                {
                    this.Deactivate();
                }

                if (!Game1.currentLocation.IsDungeon())
                {
                    ModEntry.Events.Enable<UltimateGaugeFadeOutUpdateTickedEvent>();
                }

                this.OnEmptied();
                this._chargeValue = 0;
            }
            else
            {
                var delta = value - this._chargeValue;
                var scaledDelta = delta * ((double)this.MaxValue / BaseMaxValue) * (delta >= 0
                    ? ModEntry.Config.Professions.SpecialGainFactor
                    : ModEntry.Config.Professions.SpecialDrainFactor);
                value = Math.Min(scaledDelta + this._chargeValue, this.MaxValue);

                if (this._chargeValue == 0f)
                {
                    ModEntry.Events.Enable<UltimateMeterRenderingHudEvent>();
                    this.OnChargeInitiated(value);
                }

                if (value > this._chargeValue)
                {
                    this.OnChargeIncreased(this._chargeValue, value);
                    if (value >= this.MaxValue)
                    {
                        ModEntry.Events.Enable<UltimateButtonsChangedEvent>();
                        ModEntry.Events.Enable<UltimateGaugeShakeUpdateTickedEvent>();
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

    /// <summary>Gets the default duration of the buff.</summary>
    internal abstract int MillisecondsDuration { get; }

    /// <inheritdoc cref="UltimateHud"/>
    internal UltimateHud Hud { get; }

    /// <inheritdoc cref="UltimateOverlay"/>
    internal UltimateOverlay Overlay { get; }

    /// <summary>Gets the sound effect that plays when this Ultimate is activated.</summary>
    internal abstract Sfx ActivationSfx { get; }

    /// <summary>Gets the glow color applied to the player while this Ultimate is active.</summary>
    internal abstract Color GlowColor { get; }

    private static int ActivationTimerMax => (int)(ModEntry.Config.Professions.SpecialActivationDelay * 60);

    /// <summary>Activates the <see cref="Ultimate"/> for the local player.</summary>
    internal virtual void Activate()
    {
        this.IsActive = true;

        // interrupt fade out if necessary
        ModEntry.Events.Disable<UltimateOverlayFadeOutUpdateTickedEvent>();

        // stop updating, awaiting activation and shaking the hud meter
        ModEntry.Events.Disable<UltimateButtonsChangedEvent>();
        ModEntry.Events.Disable<UltimateGaugeShakeUpdateTickedEvent>();
        ModEntry.Events.Disable<UltimateInputUpdateTickedEvent>();

        // fade in overlay and begin countdown
        ModEntry.Events.Enable<UltimateActiveUpdateTickedEvent>();
        ModEntry.Events.Enable<UltimateOverlayFadeInUpdateTickedEvent>();
        ModEntry.Events.Enable<UltimateOverlayRenderedWorldEvent>();

        // play sound effect
        this.ActivationSfx.Play();

        // notify peers
        ModEntry.Broadcaster.Broadcast("Active", "ToggledUltimate");

        // invoke callbacks
        this.OnActivated();
    }

    /// <summary>Deactivates the <see cref="Ultimate"/> for the local player.</summary>
    internal virtual void Deactivate()
    {
        this.IsActive = false;
        this.ChargeValue = 0;

        // fade out overlay
        ModEntry.Events.Enable<UltimateOverlayFadeOutUpdateTickedEvent>();

        // stop countdown
        ModEntry.Events.Disable<UltimateActiveUpdateTickedEvent>();

        // stop glowing if necessary
        Game1.player.stopGlowing();

        // notify peers
        ModEntry.Broadcaster.Broadcast("Inactive", "ToggledUltimate");

        // invoke callbacks
        this.OnDeactivated();
    }

    /// <summary>Detects and handles activation input.</summary>
    internal void CheckForActivation()
    {
        if (!ModEntry.Config.Professions.EnableSpecials)
        {
            return;
        }

        if (ModEntry.Config.Professions.SpecialActivationKey.JustPressed())
        {
            if (ModEntry.Config.Professions.HoldKeyToActivateSpecial)
            {
                this._activationTimer = ActivationTimerMax;
                ModEntry.Events.Enable<UltimateInputUpdateTickedEvent>();
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
        else if (ModEntry.Config.Professions.SpecialActivationKey.GetState() == SButtonState.Released && this._activationTimer > 0)
        {
            this._activationTimer = -1;
            ModEntry.Events.Disable<UltimateInputUpdateTickedEvent>();
        }
    }

    /// <summary>Updates internal activation state.</summary>
    internal void UpdateInput()
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass() || this._activationTimer <= 0)
        {
            return;
        }

        --this._activationTimer;
        if (this._activationTimer > 0)
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

    /// <summary>Get the localized pronoun for the <see cref="Ultimate"/>'s buff.</summary>
    /// <returns>A localized and gendered pronoun to qualify <see cref="Ultimate"/>'s buff.</returns>
    internal virtual string GetBuffPronoun()
    {
        return LocalizedContentManager.CurrentLanguageCode is
            LocalizedContentManager.LanguageCode.fr or
            LocalizedContentManager.LanguageCode.es or
            LocalizedContentManager.LanguageCode.pt
            ? ModEntry.i18n.Get("article.definite.female")
            : string.Empty;
    }

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
