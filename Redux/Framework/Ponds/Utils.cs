namespace DaLion.Redux.Framework.Ponds;

#region using directives

using System.Collections.Generic;

#endregion using directives

internal static class Utils
{
    /// <summary>Lookup of extended family pair by legendary fish id.</summary>
    internal static readonly IReadOnlyDictionary<int, int> ExtendedFamilyPairs = new Dictionary<int, int>
    {
        { Constants.CrimsonfishIndex, Constants.SonOfCrimsonfishIndex },
        { Constants.AnglerIndex, Constants.MsAnglerIndex },
        { Constants.LegendIndex, Constants.Legend2Index },
        { Constants.MutantCarpIndex, Constants.RadioactiveCarpIndex },
        { Constants.GlacierfishIndex, Constants.GlacierfishJrIndex },
        { Constants.SonOfCrimsonfishIndex, Constants.CrimsonfishIndex },
        { Constants.MsAnglerIndex, Constants.AnglerIndex },
        { Constants.Legend2Index, Constants.LegendIndex },
        { Constants.RadioactiveCarpIndex, Constants.MutantCarpIndex },
        { Constants.GlacierfishJrIndex, Constants.GlacierfishIndex },
    };

    /// <summary>Determines whether <paramref name="held"/> and <paramref name="other"/> are an extended family pair.</summary>
    /// <param name="held">The index of the currently held fish.</param>
    /// <param name="other">The index of some other fish.</param>
    /// <returns><see langword="true"/> if <paramref name="held"/> and <paramref name="other"/> are an extended family pair, otherwise <see langword="false"/>.</returns>
    internal static bool IsExtendedFamilyMember(int held, int other)
    {
        return ExtendedFamilyPairs.TryGetValue(other, out var pair) && pair == held;
    }

    /// <summary>Gets the item index of a random algae.</summary>
    /// <param name="bias">A particular type of algae that should be favored.</param>
    /// <param name="r">An optional random number generator.</param>
    /// <returns>The <see cref="int"/> index of an algae <see cref="Item"/>.</returns>
    internal static int ChooseAlgae(int? bias = null, Random? r = null)
    {
        r ??= Game1.random;
        if (bias.HasValue && r.NextDouble() > 2d / 3d)
        {
            return bias.Value;
        }

        return r.NextDouble() switch
        {
            > 2d / 3d => Constants.GreenAlgaeIndex,
            > 1d / 3d => Constants.SeaweedIndex,
            _ => Constants.WhiteAlgaeIndex,
        };
    }

    /// <summary>
    ///     Gets a fish's chance to produce roe given its sale <paramref name="value"/> and number of
    ///     <paramref name="neighbors"/>.
    /// </summary>
    /// <param name="value">The fish's sale value.</param>
    /// <param name="neighbors">How many other fish live in the same pond.</param>
    /// <returns>The percentage chance of a fish with the given <paramref name="value"/> and <paramref name="neighbors"/> to produce roe.</returns>
    internal static double GetRoeChance(int value, int neighbors)
    {
        const int maxValue = 700;
        value = Math.Min(value, maxValue);

        // Mean daily roe value (/w Aquarist profession) by fish value
        // assuming regular-quality roe and fully-populated pond:
        //     30g -> ~324g (~90% roe chance per fish)
        //     700g -> ~1512g (~18% roe chance per fish)
        //     5000g -> ~4050g (~13.5% roe chance per fish)
        const double a = 335d / 4d;
        const double b = 275d / 2d;
        return a / (value + b) * (1d + (neighbors / 11d) - (1d / 11d)) * ModEntry.Config.Ponds.RoeProductionChanceMultiplier;
    }
}
