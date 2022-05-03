namespace DaLion.Stardew.Professions.Framework.Ultimate;

#region using directives

using System;

#endregion using directives

/// <summary>Interface for Ultimate abilities.</summary>
public interface IUltimate : IDisposable
{
    /// <summary>The index of this Ultimate, which corresponds to the index of the corresponding combat profession.</summary>
    UltimateIndex Index { get; }

    /// <summary>The current charge value.</summary>
    double ChargeValue { get; set; }

    /// <summary>The maximum charge value.</summary>
    int MaxValue { get; }

    /// <summary>The current charge value as a percentage.</summary>
    float PercentCharge { get; }

    /// <summary>Whether the current charge value is at max.</summary>
    bool IsFullyCharged { get; }

    /// <summary>Whether the current charge value is at zero.</summary>
    bool IsEmpty { get; }

    /// <summary>Whether this Ultimate is currently active.</summary>
    bool IsActive { get; }

    /// <summary>Check whether the <see cref="UltimateMeter"/> is currently showing.</summary>
    bool IsMeterVisible { get; }

    /// <summary>Check whether all activation conditions for this Ultimate are currently met.</summary>
    bool CanActivate { get; }
}