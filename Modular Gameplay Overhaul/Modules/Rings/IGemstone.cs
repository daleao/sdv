namespace DaLion.Overhaul.Modules.Rings;

#region using directives

using DaLion.Overhaul.Modules.Rings.Resonance;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>A gemstone which can be applied to an Infinity Band.</summary>
/// <remarks>
///     Each <see cref="IGemstone"/> vibrates with a characteristic wavelength, which allows it to resonate with
///     others in the <see cref="DiatonicScale"/> of <see cref="IGemstone"/>.
/// </remarks>
public interface IGemstone
{
    /// <summary>Gets the index of the corresponding <see cref="SObject"/>.</summary>
    int ObjectIndex { get; }

    /// <summary>Gets the index of the corresponding <see cref="StardewValley.Objects.Ring"/>.</summary>
    int RingIndex { get; }

    /// <summary>Gets the characteristic frequency with which the <see cref="Gemstone"/> vibrates.</summary>
    /// <remarks>Measured in units of inverse <see cref="Gemstone.Ruby"/> wavelengths.</remarks>
    float Frequency { get; }

    /// <summary>Gets the characteristic color which results from <see cref="Frequency"/>.</summary>
    Color Color { get; }

    /// <summary>Gets the inverse of <see cref="Color"/>.</summary>
    Color GlowColor { get; }

    /// <summary>Gets the second <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    Gemstone Second { get; }

    /// <summary>Gets the third <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    Gemstone Third { get; }

    /// <summary>Gets the fourth <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    Gemstone Fourth { get; }

    /// <summary>Gets the fifth <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    Gemstone Fifth { get; }

    /// <summary>Gets the sixth <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    Gemstone Sixth { get; }

    /// <summary>Gets the seventh <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    Gemstone Seventh { get; }

    /// <summary>
    ///     Gets the ascending diatonic <see cref="IntervalNumber"/> between this and some other
    ///     <see cref="Gemstone"/>.
    /// </summary>
    /// <param name="other">Some other <see cref="Gemstone"/>.</param>
    /// <returns>The <see cref="IntervalNumber"/> of the <see cref="HarmonicInterval"/> between this and <paramref name="other"/>.</returns>
    IntervalNumber IntervalWith(Gemstone other);
}
