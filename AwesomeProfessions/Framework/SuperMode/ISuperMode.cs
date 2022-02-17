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
    public bool IsActive { get; }
    public SuperModeGauge Gauge { get; }
    public SuperModeOverlay Overlay { get; }

    public SFX ActivationSfx { get; }
    public Color GlowColor { get; }
    public SuperModeIndex Index { get; }

    #endregion public properties

    #region public methods

    /// <summary>Activate Super Mode for the local player.</summary>
    public void Activate();

    /// <summary>Deactivate Super Mode for the local player.</summary>
    public void Deactivate();

    /// <summary>Detect and handle activation input.</summary>
    public void CheckForActivation();

    /// <summary>Update internal activation state.</summary>
    public void Update();

    /// <summary>Add the Super Stat buff to the player.</summary> />
    public void AddBuff();

    #endregion public methods
}