namespace DaLion.Stardew.Rings.Framework;

#region using directives

using System;
using System.Linq;

#endregion using directives

/// <summary>A resonance oscillation phase.</summary>
/// <param name="Peak">The dominant resonance at oscillation peak.</param>
/// <param name="Trough">The dominant resonance at oscillation trough.</param>
/// <param name="Intensity">The resonance intensity.</param>
internal record Phase(Resonance Peak, Resonance Trough, int Intensity)
{
    private static readonly double[] _phaseRange = Enumerable.Range(0, 359).Select(i => i * Math.PI / 180d).ToArray();
    private static int _phaseIndex;

    /// <summary>The current transition state of the phase.</summary>
    internal static double Angle => _phaseRange[_phaseIndex];

    /// <summary>Update the phase state.</summary>
    internal static void Update()
    {
        _phaseIndex = (_phaseIndex + 1) % 359;
    }

    /// <summary>The internal id used to store the generated light source.</summary>
    public int? LightSourceId { get; set; }
};