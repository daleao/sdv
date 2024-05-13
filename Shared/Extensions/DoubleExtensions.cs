namespace DaLion.Shared.Extensions;

/// <summary>Extensions for the primitive <see cref="double"/> type.</summary>
public static class DoubleExtensions
{
    /// <summary>Determines whether the <paramref name="a"/> and <paramref name="b"/> are approximately equal, with uncertainty <paramref name="eps"/>.</summary>
    /// <param name="a">The first <see cref="double"/> value.</param>
    /// <param name="b">The second <see cref="double"/> value.</param>
    /// <returns><see langword="true"/> if the difference between <paramref name="a"/> and <paramref name="b"/> is less than a factor of <c>1E-15</c>, otherwise <see langword="false"/>.</returns>
    public static bool Approx(this double a, double b)
    {
        var epsilon = Math.Max(Math.Abs(a), Math.Abs(b)) * 1E-15;
        return Math.Abs(a - b) < epsilon;
    }
}
