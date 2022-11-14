namespace DaLion.Shared.Extensions;

/// <summary>Extensions for the <see cref="Random"/> class.</summary>
public static class RandomExtensions
{
    /// <summary>Generates a random boolean value with the the specified probability of success.</summary>
    /// <param name="r">A <see cref="Random"/> number generator.</param>
    /// <param name="p">The p of success (i.e., <see langword="true"/>).</param>
    /// <returns><see langword="true"/> or <see langword="false"/> values with a Binomial distribution and success probability <paramref name="p"/>.</returns>
    public static bool NextBool(this Random r, double p = 0.5)
    {
        return r.NextDouble() < p;
    }
}
