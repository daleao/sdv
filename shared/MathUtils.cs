namespace DaLion.Shared;

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Provides generally useful methods.</summary>
public static class MathUtils
{
    /// <summary>Applies the <paramref name="value"/> to a sigmoid function.</summary>
    /// <param name="value">The desired value.</param>
    /// <returns>The value of the sigmoid at <paramref name="value"/>.</returns>
    public static double Sigmoid(double value)
    {
        var exp = Math.Exp(value);
        return exp / (1d + exp);
    }

    /// <summary>Applies the <paramref name="value"/> to a logit function.</summary>
    /// <param name="value">The desired value.</param>
    /// <returns>The value of the logit at <paramref name="value"/>.</returns>
    /// <remarks>The logit function is the inverse of the sigmoid.</remarks>
    public static double Logit(double value)
    {
        return Math.Log(value / (1d - value));
    }

    /// <summary>Applies the <paramref name="value"/> to an S-curve bounded between 0 and 1.</summary>
    /// <param name="value">The desired value.</param>
    /// <param name="beta">
    ///     Determines the orientation and stretch of the curve.
    ///     Positive values will map 0 -> 1 and 1 -> 0, while negative values will map 0 -> 0 and 1 -> 1.
    ///     Higher absolute values yield a more dramatic transition between bounds.
    /// </param>
    /// <returns>The value of the curve at <paramref name="value"/>.</returns>
    public static double BoundedSCurve(double value, double beta)
    {
        return 1d / (1d + Math.Pow(value / (1d - value), beta));
    }

    /// <summary>Produces a sequence of <paramref name="count"/> double-precision floating point numbers beginning at <paramref name="start"/>.</summary>
    /// <param name="start">The starting value.</param>
    /// <param name="count">The number of elements in the range.</param>
    /// <returns>A range of <paramref name="count"/> double-precision floating point numbers beginning at <paramref name="start"/>.</returns>
    public static IEnumerable<double> Arange(double start, int count)
    {
        return Enumerable.Range((int)start, count).Select(v => (double)v);
    }

    /// <summary>Produces a sequence of numbers spaced evenly on a linear scale (a geometric progression).</summary>
    /// <param name="start">The starting value of the sequence.</param>
    /// <param name="end">The final value of the sequence.</param>
    /// <param name="count">The number of samples to generate.</param>
    /// <returns>Returns numbers spaced evenly on a linear scale.</returns>
    public static IEnumerable<double> LinSpace(double start, double end, int count)
    {
        if (count <= 0)
        {
            return Enumerable.Empty<double>();
        }

        var step = (end - start) / (count - 1);
        return Arange(0, count).Select(v => (v * step) + start);
    }

    /// <summary>Produces a sequence of numbers spaced evenly on a log scale.</summary>
    /// <param name="start">The starting value of the sequence.</param>
    /// <param name="end">The final value of the sequence.</param>
    /// <param name="count">The number of samples to generate.</param>
    /// <param name="base">The base of the log space.</param>
    /// <returns>Returns numbers spaced evenly on a log scale.</returns>
    public static IEnumerable<double> LogSpace(double start, double end, int count, double @base = 10d)
    {
        return LinSpace(start, end, count).Select(e => Math.Pow(@base, e));
    }

    /// <summary>Produces a sequence of numbers spaced evenly on a log scale (a geometric progression).</summary>
    /// <param name="start">The starting value of the sequence.</param>
    /// <param name="end">The final value of the sequence.</param>
    /// <param name="count">The number of samples to generate.</param>
    /// <returns>Returns numbers spaced evenly on a log scale (a geometric progression).</returns>
    public static IEnumerable<double> GeomSpace(double start, double end, int count)
    {
        return LogSpace(Math.Log10(start), Math.Log10(end), count);
    }
}
