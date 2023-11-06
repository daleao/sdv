namespace DaLion.Overhaul.Modules.Combat.Resonance;

#region using directives

using System.Linq;

#endregion using directives

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

/// <summary>Extensions for the <see cref="HarmonicInterval"/> class.</summary>
public static class HarmonicIntervalExtensions
{
    /// <summary>Groups the <see cref="HarmonicInterval"/>s in the specified <paramref name="intervalMatrix"/> by their respective <see cref="IntervalNumber"/>s.</summary>
    /// <param name="intervalMatrix">A 2D matrix of <see cref="HarmonicInterval"/>s in a <see cref="Chord"/>.</param>
    /// <returns>An <see cref="ILookup{TKey,TElement}"/> of all <see cref="HarmonicInterval"/>s in the <paramref name="intervalMatrix"/> by their respective <see cref="IntervalNumber"/>s.</returns>
    public static ILookup<IntervalNumber, HarmonicInterval> GroupByIntervalNumber(this HarmonicInterval[][] intervalMatrix)
    {
        return intervalMatrix
            .SelectMany(intervals => intervals)
            .ToLookup(interval => interval.Number);
    }

    /// <summary>Groups the <see cref="HarmonicInterval"/>s in the specified <paramref name="intervalMatrix"/> by the first <see cref="Gemstone"/> in each interval pair.</summary>
    /// <param name="intervalMatrix">A 2D matrix of <see cref="HarmonicInterval"/>s in a <see cref="Chord"/>.</param>
    /// <returns>An <see cref="ILookup{TKey,TElement}"/> of all <see cref="HarmonicInterval"/>s in the <paramref name="intervalMatrix"/> by their respective <see cref="IntervalNumber"/>s.</returns>
    public static ILookup<Gemstone, HarmonicInterval> GroupByGemstone(this HarmonicInterval[][] intervalMatrix)
    {
        return intervalMatrix
            .SelectMany(intervals => intervals)
            .ToLookup(interval => interval.First);
    }

    /// <summary>Groups the <see cref="HarmonicInterval"/>s in the specified <paramref name="intervalMatrix"/> by the position of <see cref="HarmonicInterval.First"/> in the <see cref="Chord"/>.</summary>
    /// <param name="intervalMatrix">A 2D matrix of <see cref="HarmonicInterval"/>s in a <see cref="Chord"/>.</param>
    /// <returns>An <see cref="ILookup{TKey,TElement}"/> of all <see cref="HarmonicInterval"/>s in the <paramref name="intervalMatrix"/> by their respective <see cref="IntervalNumber"/>s.</returns>
    public static ILookup<int, HarmonicInterval> GroupByNotePosition(this HarmonicInterval[][] intervalMatrix)
    {
        return intervalMatrix
            .SelectMany(intervals => intervals.Select((interval, index) => new { index, interval }))
            .ToLookup(_ => _.index, _ => _.interval);
    }
}
