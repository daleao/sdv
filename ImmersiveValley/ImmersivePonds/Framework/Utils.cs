namespace DaLion.Stardew.Ponds.Framework;

#region using directives

using System;
using System.Collections.Generic;

#endregion using directives

internal static class Utils
{
    /// <summary>Dictionary of extended family pair by legendary fish id.</summary>
    internal static readonly Dictionary<int, int> ExtendedFamilyPairs = new()
    {
        { Constants.CRIMSONFISH_INDEX_I, Constants.SON_OF_CRIMSONFISH_INDEX_I },
        { Constants.ANGLER_INDEX_I, Constants.MS_ANGLER_INDEX_I },
        { Constants.LEGEND_INDEX_I, Constants.LEGEND_II_INDEX_I },
        { Constants.MUTANT_CARP_INDEX_I, Constants.RADIOACTIVE_CARP_INDEX_I },
        { Constants.GLACIERFISH_INDEX_I, Constants.GLACIERFISH_JR_INDEX_I },
        { Constants.SON_OF_CRIMSONFISH_INDEX_I, Constants.CRIMSONFISH_INDEX_I },
        { Constants.MS_ANGLER_INDEX_I, Constants.ANGLER_INDEX_I },
        { Constants.LEGEND_II_INDEX_I, Constants.LEGEND_INDEX_I },
        { Constants.RADIOACTIVE_CARP_INDEX_I, Constants.MUTANT_CARP_INDEX_I },
        { Constants.GLACIERFISH_JR_INDEX_I, Constants.GLACIERFISH_INDEX_I }
    };

    /// <summary>Whether the currently held fish is a family member of another.</summary>
    /// <param name="held">The index of the currently held fish.</param>
    /// <param name="other">The index of some other fish.</param>
    internal static bool IsExtendedFamilyMember(int held, int other) =>
        ExtendedFamilyPairs.TryGetValue(other, out var pair) && pair == held;

    /// <summary>Return the item index of a random algae.</summary>
    /// <param name="bias">A particular type of algae that should be favored.</param>
    /// <param name="r">An optional random number generator.</param>
    internal static int ChooseAlgae(int? bias = null, Random? r = null)
    {
        r ??= Game1.random;
        if (bias.HasValue && r.NextDouble() > 2d / 3d) return bias.Value;

        return r.NextDouble() switch
        {
            > 2d / 3d => Constants.GREEN_ALGAE_INDEX_I,
            > 1d / 3d => Constants.SEAWEED_INDEX_I,
            _ => Constants.WHITE_ALGAE_INDEX_I
        };
    }

    /// <summary>Get the fish's chance to produce roe given its sale value.</summary>
    /// <param name="value">The fish's sale value.</param>
    /// <param name="neighbors">How many other fish live in the same pond.</param>
    internal static double GetRoeChance(int value, int neighbors)
    {
        const int MAX_VALUE_I = 700;
        value = Math.Min(value, MAX_VALUE_I);

        /// Mean daily roe value (/w Aquarist profession) by fish value
        /// assuming regular-quality roe and fully-populated pond:
        ///     30g -> ~324g (~90% roe chance per fish)
        ///     700g -> ~1512g (~18% roe chance per fish)
        ///     5000g -> ~4050g (~13.5% roe chance per fish)
        const double a = 335d / 4d;
        const double b = 275d / 2d;
        return a / (value + b) * (1d + neighbors / 11d - 1d / 11d) * ModEntry.Config.RoeProductionChanceMultiplier;
    }
}