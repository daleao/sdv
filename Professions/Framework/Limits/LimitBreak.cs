namespace DaLion.Professions.Framework.Limits;

#region using directives

using DaLion.Professions.Framework.Events.Display.RenderingHud;
using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.Events.Limit.Activated;
using DaLion.Professions.Framework.Events.Limit.ChargeIncreased;
using DaLion.Professions.Framework.Events.Limit.ChargeInitiated;
using DaLion.Professions.Framework.Events.Limit.Deactivated;
using DaLion.Professions.Framework.Events.Limit.Emptied;
using DaLion.Professions.Framework.Events.Limit.FullyCharged;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Base class for handling LimitBreak activation.</summary>
public abstract class LimitBreak : ILimitBreak
{
    /// <summary>The maximum charge value at the base level 10.</summary>
    public const double BASE_MAX_CHARGE = 100d;

    private int _activationTimer = ActivationTimerMax;
    private double _chargeValue;

    /// <summary>Initializes a new instance of the <see cref="LimitBreak"/> class.</summary>
    /// <param name="id">The <see cref="LimitBreak"/> ID, which equals the corresponding combat profession index.</param>
    /// <param name="name">The technical name of the <see cref="LimitBreak"/>.</param>
    /// <param name="color">The <see cref="LimitBreak"/>'s principal color. Used for the <see cref="LimitGauge"/> and player glow.</param>
    /// <param name="overlayColor">The color of the <see cref="LimitOverlay"/>.</param>
    protected LimitBreak(int id, string name, Color color, Color overlayColor)
    {
        this.Id = id;
        this.Name = name;
        this.ParentProfession = Profession.FromValue(id);
        this.DisplayName = _I18n.Get(this.ParentProfession.Name.ToLower() + ".limit.title" +
                                     (Game1.player.IsMale ? ".male" : ".female"));
        this.Description = _I18n.Get(this.ParentProfession.Name.ToLower() + ".limit.explain");
        this.BuffId = UniqueId + ".Buffs.Limit." + this.Name;
        this.Color = color;
        this.Gauge = new LimitGauge(this, color);
        this.Overlay = new LimitOverlay(overlayColor);
    }

    /// <inheritdoc cref="OnActivated"/>
    internal static event EventHandler<ILimitActivatedEventArgs>? Activated;

    /// <inheritdoc cref="OnDeactivated"/>
    internal static event EventHandler<ILimitDeactivatedEventArgs>? Deactivated;

    /// <inheritdoc cref="OnChargeInitiated"/>
    internal static event EventHandler<ILimitChargeInitiatedEventArgs>? ChargeInitiated;

    /// <inheritdoc cref="OnChargeIncreased"/>
    internal static event EventHandler<ILimitChargeIncreasedEventArgs>? ChargeIncreased;

    /// <inheritdoc cref="OnFullyCharged"/>
    internal static event EventHandler<ILimitFullyChargedEventArgs>? FullyCharged;

    /// <inheritdoc cref="OnEmptied"/>
    internal static event EventHandler<ILimitEmptiedEventArgs>? Emptied;

    /// <inheritdoc />
    public Profession ParentProfession { get; }

    /// <inheritdoc />
    public int Id { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <summary>Gets the ID of the buff that displays while the instance is active.</summary>
    public string BuffId { get; }

    /// <inheritdoc />
    public Color Color { get; }

    /// <inheritdoc />
    public double ChargeValue
    {
        get => this._chargeValue;
        set
        {
            if (!Config.Masteries.UnlockLimitBreaks)
            {
                return;
            }

            var delta = value - this._chargeValue;
            if (Math.Abs(delta) < 0.01)
            {
                return;
            }

            if (value <= 0)
            {
                this.Gauge.ForceStopShake();

                if (this.IsActive)
                {
                    this.Deactivate();
                }

                if (!Game1.currentLocation.IsDungeon())
                {
                    EventManager.Enable<LimitGaugeFadeOutUpdateTickedEvent>();
                }

                this.OnEmptied();
                this._chargeValue = 0;
            }
            else
            {
                var scaledDelta = delta * (MaxCharge / BASE_MAX_CHARGE) * (delta >= 0
                    ? Config.Masteries.LimitGainFactor
                    : Config.Masteries.LimitDrainFactor);
                value = Math.Min(scaledDelta + this._chargeValue, MaxCharge);
                if (this._chargeValue == 0f)
                {
                    EventManager.Enable<LimitGaugeRenderingHudEvent>();
                    this.OnChargeInitiated(value);
                }

                if (value > this._chargeValue)
                {
                    this.OnChargeIncreased(this._chargeValue, value);
                    if (value >= MaxCharge)
                    {
                        this.OnFullyCharged();
                    }
                }

                this._chargeValue = value;
            }
        }
    }

    /// <inheritdoc />
    public bool IsActive { get; protected set; }

    /// <inheritdoc />
    public virtual bool CanActivate => !this.IsActive && this.ChargeValue >= MaxCharge;

    /// <inheritdoc />
    public bool IsGaugeVisible => LimitGauge.IsVisible;

    /// <summary>Gets the maximum charge value.</summary>
    internal static double MaxCharge => BASE_MAX_CHARGE + (Game1.player.CombatLevel > 10 ? Game1.player.CombatLevel * 5 : 0);

    /// <summary>Gets a multiplier which extends the buff duration when above level 10.</summary>
    internal static double GetDurationMultiplier => MaxCharge / BASE_MAX_CHARGE / Config.Masteries.LimitDrainFactor;

    /// <inheritdoc cref="LimitGauge"/>
    internal LimitGauge Gauge { get; }

    /// <inheritdoc cref="LimitOverlay"/>
    internal LimitOverlay Overlay { get; }

    private static int ActivationTimerMax => (int)Math.Round(Config.Masteries.HoldDelayMilliseconds * 6d / 100d);

    /// <inheritdoc />
    public bool Equals(ILimitBreak? other)
    {
        return this.Id == other?.Id;
    }

    /// <summary>Instantiates the <see cref="LimitBreak"/> with the specified <paramref name="id"/>.</summary>
    /// <param name="id">The <see cref="LimitBreak"/> ID, which equals the corresponding combat profession index.</param>
    /// <returns>A new <see cref="LimitBreak"/> instance of the requested type, if valid.</returns>
    /// <exception cref="ArgumentException">If the <paramref name="id"/> is invalid.</exception>
    internal static LimitBreak FromId(int id)
    {
        return id switch
        {
            Farmer.brute => new BruteFrenzy(),
            Farmer.defender => new PoacherAmbush(),
            Farmer.acrobat => new PiperConcerto(),
            Farmer.desperado => new DesperadoBlossom(),
            _ => ThrowHelper.ThrowArgumentException<LimitBreak>(),
        };
    }

    /// <summary>Instantiates the <see cref="LimitBreak"/> with the specified <paramref name="name"/>.</summary>
    /// <param name="name">The technical name of the <see cref="LimitBreak"/>.</param>
    /// <returns>A new <see cref="LimitBreak"/> instance of the requested type, if valid.</returns>
    /// <exception cref="ArgumentException">If the <paramref name="name"/> is invalid.</exception>
    internal static LimitBreak FromName(string name)
    {
        return name switch
        {
            "Frenzy" => new BruteFrenzy(),
            "Ambush" => new PoacherAmbush(),
            "Concerto" => new PiperConcerto(),
            "Blossom" => new DesperadoBlossom(),
            _ => ThrowHelper.ThrowArgumentException<LimitBreak>(),
        };
    }

    /// <summary>Enumerates all available <see cref="LimitBreak"/> types.</summary>
    /// <returns>A <see cref="IEnumerable{T}"/> of all <see cref="LimitBreak"/> types.</returns>
    internal static IEnumerable<LimitBreak> All()
    {
        yield return new BruteFrenzy();
        yield return new PoacherAmbush();
        yield return new DesperadoBlossom();
        yield return new PiperConcerto();
    }

    /// <summary>Activates the <see cref="LimitBreak"/> for the local player.</summary>
    internal virtual void Activate()
    {
        this.IsActive = true;

        // fade in overlay and begin countdown
        EventManager.Enable<LimitOverlayFadeInUpdateTickedEvent>();

        // notify peers
        Broadcaster.Broadcast("Active", "ToggledLimitBreak");

        // invoke callbacks
        this.OnActivated();
    }

    /// <summary>Deactivates the <see cref="LimitBreak"/> for the local player.</summary>
    internal virtual void Deactivate()
    {
        this.IsActive = false;
        this.ChargeValue = 0;

        // fade out overlay
        EventManager.Enable<LimitOverlayFadeOutUpdateTickedEvent>();

        // stop glowing if necessary
        Game1.player.stopGlowing();

        // notify peers
        Broadcaster.Broadcast("Inactive", "ToggledLimitBreak");

        // invoke callbacks
        this.OnDeactivated();
    }

    /// <summary>Detects and handles activation input.</summary>
    internal void CheckForActivation()
    {
        if (!Config.Masteries.UnlockLimitBreaks)
        {
            return;
        }

        if (Config.Masteries.LimitBreakKey.JustPressed())
        {
            if (Config.Masteries.HoldKeyToLimitBreak)
            {
                this._activationTimer = ActivationTimerMax;
                EventManager.Enable<LimitInputUpdateTickedEvent>();
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
        else if (Config.Masteries.LimitBreakKey.GetState() == SButtonState.Released && this._activationTimer > 0)
        {
            this._activationTimer = -1;
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

    /// <summary>Sets the <see cref="IsActive"/> flag without triggering activation/deactivation behavior.</summary>
    /// <param name="value">The value to set.</param>
    internal void SetActive(bool value)
    {
        this.IsActive = value;
    }

    /// <summary>Counts down the charge value.</summary>
    internal abstract void Countdown();

    /// <summary>Raised when a player activates their combat LimitBreak.</summary>
    protected void OnActivated()
    {
        Activated?.Invoke(this, new LimitActivatedEventArgs(Game1.player));
    }

    /// <summary>Raised when a player's combat LimitBreak ends.</summary>
    protected void OnDeactivated()
    {
        Deactivated?.Invoke(this, new LimitDeactivatedEventArgs(Game1.player));
    }

    /// <summary>Raised when a player's combat LimitBreak gains any charge while it was previously empty.</summary>
    /// <param name="newValue">The new charge value.</param>
    protected void OnChargeInitiated(double newValue)
    {
        ChargeInitiated?.Invoke(this, new LimitChargeInitiatedEventArgs(Game1.player, newValue));
    }

    /// <summary>Raised when a player's combat LimitBreak gains any charge.</summary>
    /// <param name="oldValue">The old charge value.</param>
    /// <param name="newValue">The new charge value.</param>
    protected void OnChargeIncreased(double oldValue, double newValue)
    {
        ChargeIncreased?.Invoke(this, new LimitChargeIncreasedEventArgs(Game1.player, oldValue, newValue));
    }

    /// <summary>Raised when the local player's Limit Break charge value reaches max value.</summary>
    protected void OnFullyCharged()
    {
        FullyCharged?.Invoke(this, new LimitFullyChargedEventArgs(Game1.player));
    }

    /// <summary>Raised when the local player's Limit Break charge value returns to zero.</summary>
    protected void OnEmptied()
    {
        Emptied?.Invoke(this, new LimitEmptiedEventArgs(Game1.player));
    }
}
