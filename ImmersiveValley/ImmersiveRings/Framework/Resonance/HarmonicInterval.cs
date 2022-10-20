#pragma warning disable SA1649 // File name should match first type name
namespace DaLion.Stardew.Rings.Framework.Resonance;

#region enum

/// <summary>The number of steps between two <see cref="Gemstone"/>s in a <see cref="DiatonicScale"/>.</summary>
public enum IntervalNumber
{
    /// <summary>Zero. Essentially the same <see cref="Gemstone"/>.</summary>
    Unison,

    /// <summary>The second <see cref="Gemstone"/> in the <see cref="DiatonicScale"/>.</summary>
    Second,

    /// <summary>The third <see cref="Gemstone"/> in the <see cref="DiatonicScale"/>.</summary>
    Third,

    /// <summary>The fourth <see cref="Gemstone"/> in the <see cref="DiatonicScale"/>.</summary>
    Fourth,

    /// <summary>The fifth <see cref="Gemstone"/> in the <see cref="DiatonicScale"/>, also known as the Dominant.</summary>
    Fifth,

    /// <summary>The sixth <see cref="Gemstone"/> in the <see cref="DiatonicScale"/>.</summary>
    Sixth,

    /// <summary>The seventh <see cref="Gemstone"/> in the <see cref="DiatonicScale"/>.</summary>
    Seventh,

    /// <summary>A full scale. Essentially the same <see cref="Gemstone"/>.</summary>
    Octave,
}

/// <summary>Extensions for the <see cref="IntervalNumber"/> enum.</summary>
public static class IntervalNumberExtensions
{
    /// <summary>Gets the complement of the specified <paramref name="intervalNumber"/>.</summary>
    /// <param name="intervalNumber">The <see cref="IntervalNumber"/>.</param>
    /// <returns>The complement of <paramref name="intervalNumber"/>.</returns>
    public static IntervalNumber Inverse(this IntervalNumber intervalNumber)
    {
        return 7 - intervalNumber;
    }
}

#endregion enum

/// <summary>The difference in pitch between a <see cref="Gemstone"/> pair.</summary>
public record HarmonicInterval
{
    /// <summary>Initializes a new instance of the <see cref="HarmonicInterval"/> class.</summary>
    /// <param name="first">The first <see cref="Gemstone"/> in the pair.</param>
    /// <param name="second">The second <see cref="Gemstone"/> in the pair.</param>
    public HarmonicInterval(Gemstone first, Gemstone second)
    {
        this.First = first;
        this.Second = second;
        this.Number = first.IntervalWith(second);
    }

    /// <summary>Gets the first <see cref="Gemstone"/> in the pair.</summary>
    public Gemstone First { get; }

    /// <summary>Gets the second <see cref="Gemstone"/> in the pair.</summary>
    public Gemstone Second { get; }

    /// <summary>Gets the number of steps between <see cref="First"/> and <see cref="Second"/> in a <see cref="DiatonicScale"/>.</summary>
    public IntervalNumber Number { get; }

    /// <summary>Adds two <see cref="HarmonicInterval"/>s.</summary>
    /// <param name="a">The first <see cref="HarmonicInterval"/>.</param>
    /// <param name="b">The second <see cref="HarmonicInterval"/>.</param>
    /// <returns>The sum of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static HarmonicInterval operator +(HarmonicInterval a, HarmonicInterval b)
    {
        if (a.Second != b.First)
        {
            ThrowHelper.ThrowInvalidOperationException("Only sequential intervals can be added.");
        }

        return new HarmonicInterval(a.First, b.Second);
    }
}
#pragma warning restore SA1649 // File name should match first type name
