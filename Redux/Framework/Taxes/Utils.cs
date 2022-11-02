namespace DaLion.Redux.Framework.Taxes;

#region using directives

using System.Collections.Generic;

#endregion using directives

internal static class Utils
{
    internal static float[] Brackets { get; } = { 0.1f, 0.12f, 0.22f, 0.24f, 0.32f, 0.35f, 0.37f };

    internal static IReadOnlyDictionary<float, int> Thresholds { get; } = new Dictionary<float, int>()
    {
        { 0.1f, 9950 },
        { 0.12f, 40525 },
        { 0.22f, 86375 },
        { 0.24f, 164925 },
        { 0.32f, 209425 },
        { 0.35f, 523600 },
        { 0.37f, int.MaxValue },
    };

    /// <summary>Calculates the corresponding income tax percentage based on the specified <paramref name="income"/>.</summary>
    /// <param name="income">The month's income.</param>
    /// <returns>The percentage of tax that should be charged for someone earning this <paramref name="income"/>.</returns>
    internal static float GetTaxBracket(int income)
    {
        return ModEntry.Config.Taxes.IncomeTaxCeiling / 0.37f * income switch
        {
            <= 9950 => 0.1f,
            <= 40525 => 0.12f,
            <= 86375 => 0.22f,
            <= 164925 => 0.24f,
            <= 209425 => 0.32f,
            <= 523600 => 0.35f,
            _ => 0.37f,
        };
    }
}
