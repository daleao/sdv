namespace DaLion.Common.Integrations.ImmersiveRings;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Implementation of the mod API.</summary>
public interface IImmersiveRings
{
    /// <summary>The number of steps between two <see cref="IGemstone"/>s in a Diatonic Scale.</summary>
    public enum IntervalNumber
    {
        /// <summary>Zero. Essentially the same <see cref="IGemstone"/>.</summary>
        Unison,

        /// <summary>The second <see cref="IGemstone"/> in the Diatonic Scale.</summary>
        Second,

        /// <summary>The third <see cref="IGemstone"/> in the Diatonic Scale.</summary>
        Third,

        /// <summary>The fourth <see cref="IGemstone"/> in the Diatonic Scale.</summary>
        Fourth,

        /// <summary>The fifth <see cref="IGemstone"/> in the Diatonic Scale, also known as the Dominant.</summary>
        Fifth,

        /// <summary>The sixth <see cref="IGemstone"/> in the Diatonic Scale.</summary>
        Sixth,

        /// <summary>The seventh <see cref="IGemstone"/> in the Diatonic Scale.<summary>
        Seventh,

        /// <summary>A full scale. Essentially the same <see cref="IGemstone"/>.</summary>
        Octave,
    }

    /// <summary>A harmonic set of <see cref="IGemstone"/> wavelengths.</summary>
    /// <remarks>
    ///     The interference of vibration patterns between neighboring <see cref="IGemstone"/>s may amplify, dampen or
    ///     even create new overtones.
    /// </remarks>
    public interface IChord
    {
        /// <summary>Gets the amplitude of the <see cref="IChord"/>.</summary>
        double Amplitude { get; }

        /// <summary>
        ///     Gets the root <see cref="IGemstone"/> of the <see cref="IChord"/>, which determines the
        ///     perceived wavelength.
        /// </summary>
        IGemstone? Root { get; }
    }

    /// <summary>A gemstone which can be applied to an Infinity Band.</summary>
    /// <remarks>
    ///     Each <see cref="IGemstone"/> vibrates with a characteristic wavelength, which allows it to resonate with
    ///     others in the Diatonic Scale of <see cref="IGemstone"/>.
    /// </remarks>
    public interface IGemstone
    {
        /// <summary>Gets the index of the corresponding <see cref="SObject"/>.</summary>
        int ObjectIndex { get; }

        /// <summary>Gets the index of the corresponding <see cref="StardewValley.Objects.Ring"/>.</summary>
        int RingIndex { get; }

        /// <summary>Gets the characteristic frequency with which the <see cref="IGemstone"/> vibrates.</summary>
        /// <remarks>Measured in units of inverse Ruby wavelengths.</remarks>
        float Frequency { get; }

        /// <summary>Gets the characteristic color which results from <see cref="Frequency"/>.</summary>
        Color Color { get; }

        /// <summary>Gets the inverse of <see cref="Color"/>.</summary>
        Color InverseColor { get; }

        /// <summary>Gets the second <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Second { get; }

        /// <summary>Gets the third <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Third { get; }

        /// <summary>Gets the fourth <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Fourth { get; }

        /// <summary>Gets the fifth <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Fifth { get; }

        /// <summary>Gets the sixth <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Sixth { get; }

        /// <summary>Gets the seventh <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Seventh { get; }

        /// <summary>
        ///     Gets the ascending diatonic <see cref="IntervalNumber"/> between this and some other
        ///     <see cref="IGemstone"/>.
        /// </summary>
        /// <param name="other">Some other <see cref="IGemstone"/>.</param>
        /// <returns>The <see cref="IntervalNumber"/> of the between this and <paramref name="other"/>.</returns>
        IntervalNumber IntervalWith(IGemstone other);
    }
}
