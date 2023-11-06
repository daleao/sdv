namespace DaLion.Overhaul.Modules.Combat.Resonance;

#region using directives

using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>The number of steps between two <see cref="Gemstone"/>s in a <see cref="DiatonicScale"/>.</summary>
[EnumExtensions]
public enum IntervalNumber
{
    /// <summary>Zero. Both <see cref="Gemstone"/>s are identical.</summary>
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

    /// <summary>One full scale. Effectively the same as <see cref="Unison"/>.</summary>
    Octave,
}

/// <summary>Extensions for the <see cref="IntervalNumber"/> enum.</summary>
public static partial class IntervalNumberExtensions
{
    /// <summary>Gets the complement of the specified <paramref name="intervalNumber"/>.</summary>
    /// <param name="intervalNumber">The <see cref="IntervalNumber"/>.</param>
    /// <returns>The complement of <paramref name="intervalNumber"/>.</returns>
    public static IntervalNumber Complement(this IntervalNumber intervalNumber)
    {
        return 7 - intervalNumber;
    }
}
