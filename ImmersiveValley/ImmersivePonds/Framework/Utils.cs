using System;

namespace DaLion.Stardew.Ponds.Framework;

#region using directives

using System.Collections.Generic;

#endregion using directives

internal static class Utils
{
    /// <summary>Dictionary of extended family pair by legendary fish id.</summary>
    internal static readonly Dictionary<int, int> ExtendedFamilyPairs = new()
    {
        { 159, 898 },
        { 160, 899 },
        { 163, 900 },
        { 682, 901 },
        { 775, 902 },
        { 898, 159 },
        { 899, 160 },
        { 900, 163 },
        { 901, 682 },
        { 902, 775 }
    };

    /// <summary>Whether the currently held fish is a family member of another.</summary>
    /// <param name="held">The index of the currently held fish.</param>
    /// <param name="other">The index of some other fish.</param>
    /// <returns></returns>
    internal static bool IsExtendedFamilyMember(int held, int other)
    {
        return ExtendedFamilyPairs.TryGetValue(other, out var pair) && pair == held;
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
        const double a = 335.0 / 4.0;
        const double b = 275.0 / 2.0;
        return a / (value + b) * (1.0 + neighbors / 11.0 - 1.0/11.0) * ModEntry.Config.RoeProductionChanceMultiplier;
    }
}