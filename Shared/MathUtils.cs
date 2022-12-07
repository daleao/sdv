namespace DaLion.Shared;

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
}
