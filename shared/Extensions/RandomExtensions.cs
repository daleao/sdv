namespace DaLion.Shared.Extensions;

/// <summary>Extensions for the <see cref="Random"/> class.</summary>
public static class RandomExtensions
{
    /// <summary>Returns a random <see cref="float"/> value that is greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>.</summary>
    /// <param name="r">The <see cref="Random"/> number generator.</param>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
    /// <returns>A <see cref="float"/> that is greater than or equal to minValue and less than maxValue.</returns>
    /// <exception cref="ArgumentException">If <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
    public static float NextFloat(this Random r, float minValue, float maxValue)
    {
        if (minValue > maxValue)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException("Parameter `minValue` cannot be greater than `maxValue`.");
        }

        return (float)((r.NextDouble() * (maxValue - minValue)) + minValue);
    }

    /// <summary>Returns a random <see cref="double"/> value that is greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>.</summary>
    /// <param name="r">The <see cref="Random"/> number generator.</param>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
    /// <returns>A <see cref="double"/> that is greater than or equal to minValue and less than maxValue.</returns>
    /// <exception cref="ArgumentException">If <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
    public static double NextDouble(this Random r, double minValue, double maxValue)
    {
        if (minValue > maxValue)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException("Parameter `minValue` cannot be greater than `maxValue`.");
        }

        return (r.NextDouble() * (maxValue - minValue)) + minValue;
    }

    /// <summary>Generates a random boolean value with the the specified probability of success.</summary>
    /// <param name="r">The <see cref="Random"/> number generator.</param>
    /// <param name="p">The p of success (i.e., <see langword="true"/>).</param>
    /// <returns><see langword="true"/> or <see langword="false"/> values with a Binomial distribution and success probability <paramref name="p"/>.</returns>
    public static bool NextBool(this Random r, double p = 0.5)
    {
        return r.NextDouble() < p;
    }

    /// <summary>Samples a random decimal value from a Gaussian distribution with specified <paramref name="mean"/> and <paramref name="stddev"/> using the Box-Muller Transform.</summary>
    /// <param name="r">The <see cref="Random"/> number generator.</param>
    /// <param name="mean">The mean of the Gaussian distribution.</param>
    /// <param name="stddev">The standard deviation of the Gaussian distribution.</param>
    /// <returns>A sample from the resulting Gaussian distribution.</returns>
    public static double NextGaussian(this Random r, double mean = 0d, double stddev = 1d)
    {
        // The method requires sampling from a uniform random of (0,1]
        // but Random.NextDouble() returns a sample of [0,1).
        var u1 = 1.0 - r.NextDouble();
        var u2 = 1.0 - r.NextDouble();
        return mean + (stddev * Math.Sqrt(-2d * Math.Log(u1)) * Math.Cos(2d * Math.PI * u2));
    }

    /// <summary>Selects a random <typeparamref name="T"/> object from the available <paramref name="choices"/>.</summary>
    /// <typeparam name="T">The type of the objects to choose from.</typeparam>
    /// <param name="r">The <see cref="Random"/> number generator.</param>
    /// <param name="choices">The available choices.</param>
    /// <returns>A <typeparamref name="T"/> value from the available <paramref name="choices"/>, selected at random.</returns>
    public static T Choose<T>(this Random r, params T[] choices)
    {
        return choices[r.Next(choices.Length)];
    }
}
