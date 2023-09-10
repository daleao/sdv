namespace DaLion.Overhaul.Modules.Combat.Resonance;

#region using directives

using System.Linq;

#endregion using directives

/// <summary>Extensions for matrices for <see cref="HarmonicInterval"/>.</summary>
public static class HarmonicIntervalMatrixExtensions
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
