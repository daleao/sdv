namespace DaLion.Stardew.Professions.Framework.SuperMode;

#region using directives

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
    private int _activationTimer = -1;
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
    public void Update()
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass() || Game1.eventUp || Game1.player.UsingTool ||
            _activationTimer <= 0) return;

        --_activationTimer;
        if (_activationTimer > 0) return;

        Activate();
    }

    /// <inheritdoc />
    public abstract void AddBuff();

    #endregion public methods

    #region protected methods

    /// <summary>Check whether all activation conditions are met.</summary>
    protected virtual bool CanActivate()
    {
        return !IsActive && Gauge.IsFull;
    }

    /// <summary>Enable all events required for Super Mode functionality.</summary>
    protected void EnableEvents()
    {
        EventManager.Enable(typeof(SuperModeWarpedEvent));
        if (Game1.currentLocation.IsCombatZone() && ModEntry.Config.EnableSuperMode)
            EventManager.Enable(typeof(SuperModeGaugeRenderingHudEvent));
    }

    /// <summary>Disable all events related to Super Mode functionality.</summary>
    protected void DisableEvents()
    {
        EventManager.DisableAllStartingWith("SuperMode");
    }

    #endregion protected methods
}