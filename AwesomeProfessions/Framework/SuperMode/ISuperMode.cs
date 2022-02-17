namespace DaLion.Stardew.Professions.Framework.SuperMode;

#region using directives

using System;
using Microsoft.Xna.Framework;

using AssetLoaders;

#endregion using directives

/// <summary>Interface for Super Modes.</summary>
internal interface ISuperMode : IDisposable
{
    #region public properties
    public double ChargeValue { get; set; }
    public float PercentCharge { get; }
    public bool IsFullyCharged { get; }
    public bool IsEmpty { get; }
    public bool IsActive { get; }
    public SuperModeIndex Index { get; }
    public SuperModeGauge Gauge { get; }
    public SuperModeOverlay Overlay { get; }
    public SFX ActivationSfx { get; }
    public Color GlowColor { get; }

    #endregion public properties

    #region public methods

    /// <summary>Activate Super Mode for the local player.</summary>
    public void Activate();

    /// <summary>Deactivate Super Mode for the local player.</summary>
    public void Deactivate();

    /// <summary>Detect and handle activation input.</summary>
    public void CheckForActivation();

    /// <summary>UpdateInput internal activation state.</summary>
    public void UpdateInput();

    /// <summary>Countdown the charge value.</summary>
    public void Countdown(double amount);

    /// <summary>Add the Super Stat buff to the player.</summary> />
    public void AddBuff();

    #endregion public methods
}