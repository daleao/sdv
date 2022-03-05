namespace DaLion.Stardew.Professions.Framework.Ultimate;

#region using directives

using System;
using Microsoft.Xna.Framework;

using AssetLoaders;

#endregion using directives

/// <summary>Interface for Ultimate abilities.</summary>
internal interface IUltimate : IDisposable
{
    #region public properties
    public double ChargeValue { get; set; }
    public float PercentCharge { get; }
    public bool IsFullyCharged { get; }
    public bool IsEmpty { get; }
    public bool IsActive { get; }
    public UltimateIndex Index { get; }
    public UltimateMeter Meter { get; }
    public UltimateOverlay Overlay { get; }
    public SFX ActivationSfx { get; }
    public Color GlowColor { get; }

    #endregion public properties

    #region public methods

    /// <summary>Activate Ultimate for the local player.</summary>
    public void Activate();

    /// <summary>Deactivate Ultimate for the local player.</summary>
    public void Deactivate();

    /// <summary>Detect and handle activation input.</summary>
    public void CheckForActivation();

    /// <summary>UpdateInput internal activation state.</summary>
    public void UpdateInput();

    /// <summary>Countdown the charge value.</summary>
    public void Countdown(double elapsed);

    #endregion public methods
}